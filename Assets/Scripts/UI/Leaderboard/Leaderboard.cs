using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderBoardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderBoardEntityPrefab;

    private NetworkList<LeaderBoardEntity> leaderBoardEntities;
    private int EntititesToDisplay = 8;
    private List<LeaderboardEntityDisplay> entityDisplay = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderBoardEntities = new NetworkList<LeaderBoardEntity>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged += LeaderBoardEntities_OnListChanged;
            foreach (var entity in leaderBoardEntities)
            {
                LeaderBoardEntities_OnListChanged
                   (
                   new NetworkListEvent<LeaderBoardEntity>
                   {
                       Type = NetworkListEvent<LeaderBoardEntity>.EventType.Add,
                       Value = entity
                   });
            }
        }

        if (!IsServer) return;
        TankPlayer[] tankPlayers = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in tankPlayers)
        {
            HandlePlayerSpawn(player);
        }
        TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
        TankPlayer.OnPlayerDespawn += HandlePlayerDespawn;
    }

    private void LeaderBoardEntities_OnListChanged(NetworkListEvent<LeaderBoardEntity> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardEntity>.EventType.Add:
                if (entityDisplay.Any(x => x.clientId == changeEvent.Value.clientId)) return;
                LeaderboardEntityDisplay display = Instantiate(leaderBoardEntityPrefab, leaderBoardEntityHolder);
                display.Initialise(changeEvent.Value.clientId, changeEvent.Value.playerName, changeEvent.Value.coins);
                entityDisplay.Add(display);
                break;
            case NetworkListEvent<LeaderBoardEntity>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = entityDisplay.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    entityDisplay.Remove(displayToRemove);
                    Destroy(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderBoardEntity>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate = entityDisplay.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.coins);
                }
                break;
        }
        entityDisplay.Sort((x, y) => y.coins.CompareTo(x.coins));
        for (var i = 0; i < entityDisplay.Count; i++)
        {
            entityDisplay[i].transform.SetSiblingIndex(0);
            entityDisplay[i].UpdateText();
            entityDisplay[i].gameObject.SetActive(i <= entityDisplay.Count - 1);
        }
        LeaderboardEntityDisplay selfPlayer= entityDisplay.FirstOrDefault(x => x.clientId == NetworkManager.Singleton.LocalClientId);
        if (selfPlayer != null)
        {
            if(selfPlayer.transform.GetSiblingIndex()>= entityDisplay.Count - 1)
            {
                entityDisplay[entityDisplay.Count - 1].gameObject.SetActive(false);
                selfPlayer.gameObject.SetActive(true);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient) leaderBoardEntities.OnListChanged -= LeaderBoardEntities_OnListChanged;
        if (!IsServer) return;
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
        TankPlayer.OnPlayerDespawn -= HandlePlayerDespawn;
    }

    private void HandlePlayerSpawn(TankPlayer player)
    {
        leaderBoardEntities.Add(new LeaderBoardEntity()
        {
            clientId = player.OwnerClientId,
            playerName = player.playerName.Value,
            coins = 0
        });
        player.wallet.totalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinChanged(player.OwnerClientId, newCoins);
    }
    private void HandlePlayerDespawn(TankPlayer player)
    {
        foreach (var entity in leaderBoardEntities)
        {
            if (entity.clientId != player.OwnerClientId) continue;
            leaderBoardEntities.Remove(entity);
            break;
        }
        player.wallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinChanged(player.OwnerClientId, newCoins);
    }
    public void HandleCoinChanged(ulong clientId, int value)
    {
        for (var i = 0; i < leaderBoardEntities.Count; i++)
        {
            if (leaderBoardEntities[i].clientId != clientId) continue;
            leaderBoardEntities[i] = new LeaderBoardEntity()
            {
                clientId = leaderBoardEntities[i].clientId,
                playerName = leaderBoardEntities[i].playerName,
                coins = value
            };
        }

    }
}
