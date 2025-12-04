using NUnit.Framework;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] TMP_Text LobbyName;
    [SerializeField] TMP_Text playerData;

    private LobbiesList Lobbies;
    private Lobby lobby;

    public void Initialise(LobbiesList lobbiesList, Lobby lobby)
    {
        this.Lobbies = lobbiesList;
        this.lobby = lobby;
        LobbyName.text= lobby.Name;
        playerData.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void JoinLobby()
    {
        Lobbies.JoinMatchAsync(lobby);
    }
}
