using Unity.Netcode;
using UnityEngine;

public class Aim : NetworkBehaviour
{
    [SerializeField]
    private Transform turretTransform;

    [SerializeField]
    private InputReader inputReader;

    private Vector2 aimPosition;

    private void LateUpdate()
    {
        if (IsOwner)
        {
            HandleAim();
        }
    }

    void HandleAim()
    {
        aimPosition = Camera.main.ScreenToWorldPoint(inputReader.aimPosition);
        turretTransform.up = new Vector2(aimPosition.x-turretTransform.position.x, aimPosition.y - turretTransform.position.y);
    }
}
