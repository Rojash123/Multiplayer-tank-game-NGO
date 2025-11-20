using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Attributes")]

    [SerializeField]
    GameObject serverProjectilePrefab, clientProjectilePrefab;

    [SerializeField]
    private Transform projectileSpawnPoint;

    [SerializeField] InputReader reader;

    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private Collider2D collider;

    [Header("Settings")]

    private bool shouldFire;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;

    private float lastFireTime = 0;
    private float muzzleFlashTimer;

    private void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlashTimer = 0;
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;
        if (!shouldFire) return;

        if (Time.time - lastFireTime < (1 / fireRate)) return;
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        lastFireTime = Time.time;
    }

    void SpawnDummyProjectile(Vector3 projectileSpawnPoint, Vector3 dirn)
    {
        muzzleFlash.SetActive(true);

        var gameObjectProjectileInstance = Instantiate(clientProjectilePrefab, projectileSpawnPoint, Quaternion.identity);
        gameObjectProjectileInstance.transform.up = dirn;

        Physics2D.IgnoreCollision(collider, gameObjectProjectileInstance.GetComponent<Collider2D>());

        gameObjectProjectileInstance.SetActive(true);

        if (gameObjectProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
        muzzleFlashTimer = muzzleFlashDuration;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        reader.OnPlayerShoot += Reader_OnPlayerShoot;
    }

    private void Reader_OnPlayerShoot(bool obj)
    {
        this.shouldFire = obj;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPoint, Vector3 dirn)
    {
        var gameObjectProjectileInstance = Instantiate(serverProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        gameObjectProjectileInstance.transform.up = dirn;

        lastFireTime = Time.time;
        Physics2D.IgnoreCollision(collider, gameObjectProjectileInstance.GetComponent<Collider2D>());
        gameObjectProjectileInstance.SetActive(true);

        if (gameObjectProjectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact obj))
        {
            obj.SetOwner(OwnerClientId);
        }

        if (gameObjectProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
        PrimaryFireClientRpc(spawnPoint, dirn);
    }

    [ClientRpc]
    private void PrimaryFireClientRpc(Vector3 spawnPoint, Vector3 dirn)
    {
        if (IsOwner) return;
        SpawnDummyProjectile(spawnPoint, dirn);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        reader.OnPlayerShoot -= Reader_OnPlayerShoot;
    }
}
