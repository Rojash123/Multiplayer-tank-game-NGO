using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins=new NetworkVariable<int>();

    private int temporaryCoin;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        temporaryCoin = 0;
        totalCoins.Value = 0;
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
}
