using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : Singleton<ClientSingleton>
{
    public ClientGameManager gameManager { get; private set;}
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        gameManager?.Dispose();
    }

    public async Task<bool> CreateClient()
    {
        gameManager = new ClientGameManager();
        return await gameManager.InitAsync();
    }
}
