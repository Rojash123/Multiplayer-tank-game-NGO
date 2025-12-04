using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private TextMeshProUGUI playerName;

    private void Start()
    {
        OnDropdownValueChanged(string.Empty, tankPlayer.playerName.Value);
        tankPlayer.playerName.OnValueChanged+= OnDropdownValueChanged;
    }
    private void OnDestroy()
    {
        tankPlayer.playerName.OnValueChanged-= OnDropdownValueChanged;
    }
    void OnDropdownValueChanged(FixedString32Bytes old, FixedString32Bytes newName)
    {
        playerName.text = newName.ToSafeString();
    }
}
