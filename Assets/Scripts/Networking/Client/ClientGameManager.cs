using Newtonsoft.Json;
using System;
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

public class ClientGameManager:IDisposable
{
    private JoinAllocation joinAllocation;
    private NetworkClient netWorkClient;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        netWorkClient= new NetworkClient(NetworkManager.Singleton);
        AuthenticationState authState=await AuthenticationHandler.DoAuth();
        if (authState == AuthenticationState.authenticated)
        {
            return true;
        }
        return false;
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(2);
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

        UserData data = new UserData()
        {
            userName = PlayerPrefs.GetString("Name", "Name not set"),
            userAuthId = AuthenticationService.Instance.PlayerId

        };
        var payLoad=JsonConvert.SerializeObject(data);
        byte[] byteArray=Encoding.UTF8.GetBytes(payLoad);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = byteArray;
        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        netWorkClient?.Dispose();
    }
}
