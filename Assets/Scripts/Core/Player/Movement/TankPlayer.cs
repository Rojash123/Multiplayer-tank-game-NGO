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
    [SerializeField] private Texture2D crossHair;
    [SerializeField] Teamcolor color;

    public NetworkVariable<int> teamIndex { get; private set; }


    private const int priority = 15;
    public NetworkVariable<FixedString32Bytes> playerName=new NetworkVariable<FixedString32Bytes>();
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData data = new();
            if (IsHost)
            {
                data = HostSingleton.Instance.gameManager.networkServer.GetUserName(OwnerClientId);
                playerName.Value = data.userName;
                teamIndex.Value = data.teamIndex;
                OnPlayerSpawned?.Invoke(this);
            }
            else
            {
                data = ServerSingleton.Instance.gameManager.NetworkServer.GetUserName(OwnerClientId);
            }
        }
        if (IsOwner)
        {
            camera.Priority= priority;
            Cursor.SetCursor(crossHair, new Vector2(crossHair.width / 2, crossHair.height / 2), CursorMode.Auto);
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
