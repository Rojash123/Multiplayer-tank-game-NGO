using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    private bool isJoining;
    private bool isRefreshing;

    [SerializeField] LobbyItem lobbyItemPrefab;
    [SerializeField] Transform parentLobbyItem;

    private void OnEnable()
    {
        RefreshList();
    }
    private async void RefreshList()
    {
        if (isRefreshing) return;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field:QueryFilter.FieldOptions.AvailableSlots,
                    op:QueryFilter.OpOptions.GT,
                    value:"0"),
                new QueryFilter(
                    field:QueryFilter.FieldOptions.IsLocked,
                    op:QueryFilter.OpOptions.EQ,
                    value:"0")
            };
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
            foreach (Transform child in parentLobbyItem)
            {
                Destroy(child.gameObject);
            }
            foreach (var lobby in lobbies.Results)
            {
                LobbyItem lobbyItem=Instantiate(lobbyItemPrefab, parentLobbyItem);
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch (LobbyServiceException ex)
        {

        }
        isRefreshing = true;
    }
    public async void JoinMatchAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["joincode"].Value;
            await ClientSingleton.Instance.gameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
        isJoining = false;
    }
}
