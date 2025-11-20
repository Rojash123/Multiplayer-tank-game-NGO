using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] RespawningCoin spawningCoinPrefab;
    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int value = 10;

    [SerializeField]
    private Vector2 spawnRangeX, spawnRangeY;

    public LayerMask layerMask;

    private float coinRadius;

    private Collider2D[] buffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        coinRadius = spawningCoinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(spawningCoinPrefab,GetSpawnPoint(),Quaternion.identity);
        coinInstance.OnCoinCollected += CoinInstance_OnCoinCollected;
        coinInstance.SetValue(value);
        coinInstance.GetComponent<NetworkObject>().Spawn();
    }

    private void CoinInstance_OnCoinCollected(RespawningCoin obj)
    {
        obj.transform.position = GetSpawnPoint();
        obj.Reset();
    }

    Vector2 GetSpawnPoint()
    {
        float spawnX = 0;
        float spawnY = 0;
        while (true)
        {
            spawnX = Random.Range(spawnRangeX.x, spawnRangeX.y);
            spawnY = Random.Range(spawnRangeY.x, spawnRangeY.y);
            Vector2 spawnPoint = new Vector2(spawnX, spawnY);
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, buffer,layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }


}
