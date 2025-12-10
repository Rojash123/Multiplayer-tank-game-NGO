using Unity.Netcode;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [SerializeField]InputReader _reader;

    [SerializeField]
    private Transform bodyTransform;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField] float movementSpeed=4f;
    [SerializeField] float turningRate = 30f;

    [SerializeField] ParticleSystem dustCLoud;
    [SerializeField] float particleEmissionValue = 10;
    private ParticleSystem.EmissionModule emissionModule;

    private Vector2 previousMovement;
    private Vector3 previousPos;

    private void Awake()
    {
        emissionModule = dustCLoud.emission;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _reader.OnPlayerMove += HandleMovement;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _reader.OnPlayerMove -= HandleMovement;

    }
    void HandleMovement(Vector2 movement)
    {
        previousMovement = movement;
    }
    void Update()
    {
        if (!IsOwner) return;

        float zRotation = previousMovement.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0, 0, zRotation);
    }
    private void FixedUpdate()
    {
        if ((transform.position - previousPos).sqrMagnitude > 0.005f)
        {
            emissionModule.rateOverTime = particleEmissionValue;
        }
        else
        {
            emissionModule.rateOverTime = 0;
        }
        previousPos = transform.position;
        if (!IsOwner) return;
        rb.linearVelocity=(Vector2)bodyTransform.up* previousMovement.y * movementSpeed;
    }
}
