using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer:IDisposable
{
    NetworkManager networkManager;

    private Dictionary<ulong, string> ClientAuthIdDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> userDataDictionary=new Dictionary<string, UserData>();

    public Action<string> OnClientLeft;

    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;

    private NetworkObject playerPrefab;


    public NetworkServer(NetworkManager manager, NetworkObject playerPrefab)
    {
        networkManager = manager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
        this.playerPrefab = playerPrefab;
    }

    public bool OpenConnection(string ip, int port)
    {
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        return networkManager.StartServer();
    }
    public UserData GetUserName(ulong clientId)
    {
        if(ClientAuthIdDictionary.TryGetValue(clientId,out string authData))
        {
            return userDataDictionary[authData];
        }
        else { return null; }
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(ClientAuthIdDictionary.TryGetValue(clientId,out string authId))
        {
            OnUserLeft?.Invoke(userDataDictionary[authId]);
            ClientAuthIdDictionary.Remove(clientId);
            userDataDictionary.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }

    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payLoad=System.Text.Encoding.UTF8.GetString(request.Payload);
        var data=JsonConvert.DeserializeObject<UserData>(payLoad);

        //Alternative way to store data
        //ClientAuthIdDictionary[request.ClientNetworkId] = data.userAuthId;

        ClientAuthIdDictionary.Add(request.ClientNetworkId, data.userAuthId);
        userDataDictionary.Add(data.userAuthId, data);
        OnUserJoined?.Invoke(data);

        _=SpawnPlayerDelayed(request.ClientNetworkId);

        response.Approved = true;
        response.CreatePlayerObject = false;
    }
    private async Task SpawnPlayerDelayed(ulong clientID)
    {
        await Task.Delay(1000);
        NetworkObject playerInstance = GameObject.Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.SpawnAsPlayerObject(clientID);
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= NetworkManager_OnServerStarted;
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            if (networkManager.IsListening) networkManager.Shutdown();
        }
    }
}
