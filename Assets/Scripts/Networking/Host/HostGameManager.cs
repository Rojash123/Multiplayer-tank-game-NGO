using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;

    private string joinCode;
    private string lobbyId;
    private const int maxConnection = 20;

    private NetworkServer networkServer;
    public async Task StartHostAsync()
    {
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        }
        catch (Exception e)
        {
            MyDebug.Log(e.Message);
            return;
        }
        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            MyDebug.Log(joinCode);
        }
        catch (Exception e)
        {
            MyDebug.Log(e.Message);
            return;
        }
       

        UnityTransport transPort = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData serverData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        transPort.SetRelayServerData(serverData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                { "joincode",new DataObject
                (visibility:DataObject.VisibilityOptions.Member,
                    value:joinCode
                    )}
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("MyLobby", maxConnection, lobbyOptions);
            lobbyId = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15f));
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
            return;
        }
        networkServer = new NetworkServer(NetworkManager.Singleton);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    private IEnumerator HeartBeatLobby(float waitTimeSec)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSec);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}