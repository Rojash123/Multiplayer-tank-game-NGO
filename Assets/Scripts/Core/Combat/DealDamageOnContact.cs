using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private float damage = 5;

    private ulong clientId;
    public void SetOwner(ulong clientId)
    {
        this.clientId = clientId;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            if (collision.TryGetComponent<NetworkObject>(out NetworkObject obj))
            {
                if (obj.OwnerClientId == clientId) return;
            }
            if (collision.attachedRigidbody.GetComponent<Health>() != null)
            {
                collision.attachedRigidbody.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }
}
