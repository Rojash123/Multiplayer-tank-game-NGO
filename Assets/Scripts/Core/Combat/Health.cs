using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<float> currentHealth=new NetworkVariable<float>();
    [field: SerializeField] public float maxHealth { get; private set; } = 100;

    private bool isDead;

    public Action<Health> OnDead;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currentHealth.Value = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        ModifyHealth(-damage);
    }
    public void RegainHealth(float healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(float damage)
    {
        if(isDead) return;

        currentHealth.Value += damage;
        if(currentHealth.Value <= 0)
        {
            isDead = true;
            OnDead?.Invoke(this);
        }
        currentHealth.Value=Mathf.Clamp(currentHealth.Value, 0, maxHealth);
    }
}
