using Newtonsoft.Json;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkServer
{
    NetworkManager networkManager;
    public NetworkServer(NetworkManager manager)
    {
        networkManager = manager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payLoad=System.Text.Encoding.UTF8.GetString(request.Payload);
        JsonConvert.DeserializeObject<UserData>(payLoad);
        response.Approved = true;
    }
}
