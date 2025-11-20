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

    private Vector2 previousMovement;

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
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float zRotation = previousMovement.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0, 0, zRotation);
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        rb.linearVelocity=(Vector2)bodyTransform.up* previousMovement.y * movementSpeed;
    }
}
