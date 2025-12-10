using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private TextMeshProUGUI playerName;
    private void Start()
    {
        OnDropdownValueChanged(-1, tankPlayer.teamIndex.Value);
        tankPlayer.teamIndex.OnValueChanged += OnDropdownValueChanged;
    }
    private void OnDestroy()
    {
        tankPlayer.teamIndex.OnValueChanged -= OnDropdownValueChanged;
    }
    void OnDropdownValueChanged(int old, int newName)
    {

    }
}
