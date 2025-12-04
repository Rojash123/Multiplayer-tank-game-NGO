using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton :Singleton<HostSingleton>
{
    public HostGameManager gameManager { get; private set; }
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        gameManager?.Dispose();
    }
    public void CreateHost()
    {
        gameManager = new HostGameManager();
    }
}
