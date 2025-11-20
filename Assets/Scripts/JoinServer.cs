using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    public void JoinAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void JoinAsServer()
    {
        NetworkManager.Singleton.StartHost();
    }
}
