using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI queueTime;
    [SerializeField] TextMeshProUGUI queueStatus;
    [SerializeField] TextMeshProUGUI matchMakingText;
    [SerializeField] Toggle teamToggle;

    [SerializeField] Button findMatchButton;
    public TMP_InputField inputField;

    bool isMatchMaking;
    bool isCancelling;

    private float timer;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueTime.text = string.Empty;
        queueStatus.text = string.Empty;
    }

    public async void FindMatchPressed()
    {
        if (isCancelling) return;
        if (isMatchMaking)
        {
            queueStatus.text = "cancelling";
            isCancelling = true;
            await ClientSingleton.Instance.gameManager.CancelMatchMakeAsync();
            isCancelling = false;
            isMatchMaking = false;
            matchMakingText.text = "Find Match";
            queueStatus.text = string.Empty;
            return;
        }
        matchMakingText.text = "Cancel";
        queueStatus.text = "Searching...";
        isMatchMaking = true;
        ClientSingleton.Instance.gameManager.MatchMakeAsync(teamToggle.isOn,OnMatchMade);
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatus.text = "Connecting";
                break;

            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatus.text = "TicketRetrievalError";
                break;

            case MatchmakerPollingResult.TicketCreationError:
                queueStatus.text = "TicketCreationError";
                break;

            case MatchmakerPollingResult.TicketCancellationError:
                queueStatus.text = "TicketCancellationError";
                break;

            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatus.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.gameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.gameManager.StartClientAsync(inputField.text);
    }

    void Update()
    {
        if (isMatchMaking)
        {
            timer += Time.deltaTime;
            TimeSpan span=TimeSpan.FromSeconds(timer);
            queueTime.text=timer.ToString();
        }
        if (isCancelling)
        {
            timer = 0;
            queueTime.text = timer.ToString();
        }
    }
}
