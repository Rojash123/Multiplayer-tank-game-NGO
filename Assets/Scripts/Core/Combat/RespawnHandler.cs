using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] TankPlayer playerPrefab;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        TankPlayer[] tankPlayers = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in tankPlayers)
        {
            TankPlayer_OnPlayerSpawned(player);
        }
        TankPlayer.OnPlayerSpawned += TankPlayer_OnPlayerSpawned;
        TankPlayer.OnPlayerDespawn += TankPlayer_OnPlayerDespawn; 
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        TankPlayer.OnPlayerSpawned -= TankPlayer_OnPlayerSpawned;
        TankPlayer.OnPlayerDespawn -= TankPlayer_OnPlayerDespawn;
    }

    private void TankPlayer_OnPlayerSpawned(TankPlayer player)
    {
        player.health.OnDead +=(health)=> HandlePlayerDead(player);
    }

    private void HandlePlayerDead(TankPlayer player)
    {
        Destroy(player.gameObject);
        int coinValue = (int)player.wallet.totalCoins.Value * 40/100;
        StartCoroutine(SpawnPlayer(player.OwnerClientId,coinValue));
    }

    private void TankPlayer_OnPlayerDespawn(TankPlayer player)
    {
        player.health.OnDead -= (health) => HandlePlayerDead(player);
    }

    IEnumerator SpawnPlayer(ulong ownerClientId,int coinValue)
    {
        yield return new WaitForEndOfFrame();
        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(),Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        playerInstance.wallet.totalCoins.Value +=coinValue;
    }


}
