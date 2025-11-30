using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField inputField;
    public async void StartHost()
    {
        await HostSingleton.Instance.gameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.gameManager.StartClientAsync(inputField.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
