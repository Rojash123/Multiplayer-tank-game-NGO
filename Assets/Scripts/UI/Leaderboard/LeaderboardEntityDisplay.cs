using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    public ulong clientId { get; private set; }
    private FixedString32Bytes name;
    public int coins { get; private set; }
    public void Initialise(ulong clientID, FixedString32Bytes name, int coins)
    {
        this.clientId = clientID;
        this.name = name;
        this.coins = coins;
        UpdateCoins(coins);
    }
    public void UpdateCoins(int coins)
    {
        this.coins = coins;
        UpdateText();
    }
    public void UpdateText()
    {
        textMeshProUGUI.text = $"{transform.GetSiblingIndex() + 1}-{name} {coins}";
    }
}
