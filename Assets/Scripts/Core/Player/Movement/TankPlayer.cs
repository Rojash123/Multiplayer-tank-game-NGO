using System;
using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] CinemachineCamera camera;
    [SerializeField] TextMeshProUGUI nameHolder;
    [field:SerializeField] public Health health { get; private set; }
    [field: SerializeField] public CoinWallet wallet { get; private set; }


    private const int priority = 15;

    public NetworkVariable<FixedString32Bytes> playerName=new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData data = HostSingleton.Instance.gameManager.networkServer.GetUserName(OwnerClientId);
            playerName.Value = data.userName;
            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            camera.Priority= priority;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }
}
