using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] ClientSingleton clientPrefab;
    [SerializeField] HostSingleton hostPrefab;
    [SerializeField] ServerSingleton serverPrefab;
    [SerializeField] NetworkObject networkObject;

    private ApplicationData appData;
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
            Application.targetFrameRate = 60;
            appData = new ApplicationData();
            ServerSingleton server = Instantiate(serverPrefab);
            StartCoroutine(LoadGameAsync(server));
        }
        else
        {
            ClientSingleton client= Instantiate(clientPrefab);
            bool authenticated=await client.CreateClient();
            HostSingleton host= Instantiate(hostPrefab);
            host.CreateHost(networkObject);

            if (authenticated)
            {
                client.gameManager.GoToMenu();
            }
        }
    }

    private IEnumerator LoadGameAsync(ServerSingleton server)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("GameScene");
        while (!async.isDone)
        {
            yield return null;
        }
        Task createServerTask =server.CreateServer(networkObject);
        yield return new WaitUntil(() => createServerTask.IsCompleted);
        Task startServerTask=server.gameManager.StartGameAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }

}
