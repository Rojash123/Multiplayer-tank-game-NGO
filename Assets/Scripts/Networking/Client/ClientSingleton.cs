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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async Task<bool> CreateClient()
    {
        gameManager = new ClientGameManager();
        return await gameManager.InitAsync();
    }
}
