using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation joinAllocation;
    private NetworkClient netWorkClient;
    private MatchplayMatchmaker matchmaker;
    private UserData userData;

    public async void MatchMakeAsync(bool isTeamQueue,Action<MatchmakerPollingResult> OnMatchMakeResponse)
    {
        if (matchmaker.IsMatchmaking) return;
        userData.userGamePreferences.gameQueue = isTeamQueue ? GameQueue.team : GameQueue.solo;
        MatchmakerPollingResult results = await GetMatchAsync();
        OnMatchMakeResponse?.Invoke(results);
    }
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult result = await matchmaker.Matchmake(userData);
        if (result.result == MatchmakerPollingResult.Success)
        {
            StartClient(result.ip, result.port);
        }
        return result.result;
    }
    public async Task CancelMatchMakeAsync()
    {
        await matchmaker.CancelMatchmaking();
    }
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        netWorkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new();
        AuthenticationState authState = await AuthenticationHandler.DoAuth();
        if (authState == AuthenticationState.authenticated)
        {
            userData = new UserData()
            {
                userName = PlayerPrefs.GetString("Name", "Name not set"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }
        return false;
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(2);
    }
    public void StartClient(string ip, int port)
    {
        UnityTransport transPort = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transPort.SetConnectionData(ip, (ushort)port);
        ConnectClient();
    }
    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            MyDebug.Log(e.Message);
            return;
        }
        UnityTransport transPort = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData serverData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        transPort.SetRelayServerData(serverData);
        ConnectClient();
    }
    public void ConnectClient()
    {
        var payLoad = JsonConvert.SerializeObject(userData);
        byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = byteArray;
        NetworkManager.Singleton.StartClient();
    }
    public void Dispose()
    {
        netWorkClient?.Dispose();
    }
    public void Disconnect()
    {
        netWorkClient.DisConnect();
    }

    
}
