using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : Singleton<ServerSingleton>
{
    public ServerManager gameManager { get; private set; }
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        gameManager?.Dispose();
    }
    public async Task CreateServer(NetworkObject playerPrefab)
    {
        await UnityServices.InitializeAsync();
        gameManager = new(ApplicationData.IP(), ApplicationData.Port(),ApplicationData.QPort(),NetworkManager.Singleton, playerPrefab);
    }
}
