using Unity.Netcode;
using UnityEngine;

public class LeaveHUD : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.gameManager.Shutdown();
        }
        ClientSingleton.Instance.gameManager.Disconnect();
    }
}
