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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateHost()
    {
        gameManager = new HostGameManager();
    }
}
