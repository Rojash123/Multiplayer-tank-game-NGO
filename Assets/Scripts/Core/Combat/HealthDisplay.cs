using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image healthBar;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) return;
        health.currentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.currentHealth.Value);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float oldHealth,float newHealth)
    {
        healthBar.fillAmount =(float)newHealth / health.maxHealth;
    }
}
