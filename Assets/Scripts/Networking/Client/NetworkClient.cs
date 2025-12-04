using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    NetworkManager networkManager;
    private Dictionary<ulong, string> ClientAuthIdDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> userDataDictionary = new Dictionary<string, UserData>();

    public NetworkClient(NetworkManager manager)
    {
        networkManager = manager;
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId != 0 && clientId != networkManager.LocalClientId) return;
        if (SceneManager.GetActiveScene().name != "MenuScene")
        {
            SceneManager.LoadScene("MenuScene");
        }
        if(networkManager.IsConnectedClient)networkManager.Shutdown();
    }
}
