using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] ClientSingleton clientPrefab;
    [SerializeField] HostSingleton hostPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await IsDedicatedServer(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task IsDedicatedServer(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            Debug.Log("Hello");
            ClientSingleton client= Instantiate(clientPrefab);
            bool authenticated=await client.CreateClient();
            HostSingleton host= Instantiate(hostPrefab);
            host.CreateHost();

            if (authenticated)
            {
                client.gameManager.GoToMenu();
            }
        }
    }

}
