using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        CanCommitToTransform = IsOwner;
        if (NetworkManager != null)
        {
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    
                }
            }
        }
    }
}
