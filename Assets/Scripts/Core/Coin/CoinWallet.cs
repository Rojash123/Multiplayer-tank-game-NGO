using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [SerializeField] BountyCoin spawningbountyCoinPrefab;
    [SerializeField] private Health health;
    public NetworkVariable<int> totalCoins=new NetworkVariable<int>();

    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private float coinSpread = 5;

    public LayerMask layerMask;

    private float coinRadius;

    private Collider2D[] buffer = new Collider2D[1];

    private int temporaryCoin;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        coinRadius = spawningbountyCoinPrefab.GetComponent<CircleCollider2D>().radius;
        health.OnDead += HandleDead;
        temporaryCoin = 0;
        totalCoins.Value = 0;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        health.OnDead -= HandleDead;
    }

    private void HandleDead(Health health)
    {
        int bountyValue =(int) totalCoins.Value * 40 / 100;
        int bountyCoinValue = bountyValue / bountyCoinCount;
        if (bountyCoinValue < minBountyCoinValue)
        {
            return;
        }
        for (int i = 0; i < bountyCoinCount; i++) 
        {
            BountyCoin bountyCoinInstance = Instantiate(spawningbountyCoinPrefab, GetSpawnPoint(), Quaternion.identity);
            bountyCoinInstance.SetValue(bountyCoinValue);
            bountyCoinInstance.NetworkObject.Spawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Coin>(out Coin coins))
        {
            temporaryCoin += coins.Collect();
            HandleCoinCollect();
        }
    }

    void HandleCoinCollect()
    {
        if(!IsServer)return;
            totalCoins.Value = temporaryCoin;
    }
    Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint =(Vector2)transform.position+UnityEngine.Random.insideUnitCircle*coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, buffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
