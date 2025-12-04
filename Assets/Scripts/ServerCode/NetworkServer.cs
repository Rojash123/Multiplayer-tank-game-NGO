using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkServer:IDisposable
{
    NetworkManager networkManager;

    private Dictionary<ulong, string> ClientAuthIdDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> userDataDictionary=new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager manager)
    {
        networkManager = manager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
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
            ClientAuthIdDictionary.Remove(clientId);
            userDataDictionary.Remove(authId);
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
        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
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
