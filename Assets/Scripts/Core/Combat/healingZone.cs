using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class healingZone :NetworkBehaviour
{
    [SerializeField] Image healPowerBar;
    [SerializeField] private int maxHealPower=30;
    [SerializeField] private float coolDown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinPerTick = 10;
    [SerializeField] private float healthPerTick = 10;

    private float remainingCoolDown;
    private float tickTimer;


    private List<TankPlayer> playerInZone=new List<TankPlayer>();

    private NetworkVariable<int> healPower=new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            healPower.OnValueChanged += HandlePowerChanged;
            HandlePowerChanged(0, healPower.Value);
        }
        if (IsServer)
        {
            healPower.Value = maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            healPower.OnValueChanged -= HandlePowerChanged;
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        if (remainingCoolDown > 0)
        {
            remainingCoolDown -= Time.deltaTime;
            if (remainingCoolDown <= 0)
            {
                healPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }
        tickTimer += Time.time;
        if (tickTimer > 1/healTickRate)
        {
            foreach (TankPlayer player in playerInZone)
            {
                if (healPower.Value == 0) break;
                if (player.health.currentHealth.Value == player.health.maxHealth) continue;
                if (player.wallet.totalCoins.Value < coinPerTick) continue;

                player.health.RegainHealth(healthPerTick);
            }
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        if(collision.attachedRigidbody.TryGetComponent(out TankPlayer player))
        {
            playerInZone.Add(player);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (collision.attachedRigidbody.TryGetComponent(out TankPlayer player))
        {
            playerInZone.Remove(player);
        }
    }
    private void HandlePowerChanged(int oldValue, int newValue)
    {
        healPowerBar.fillAmount= (float)newValue/maxHealPower;
    }

}
