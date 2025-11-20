using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> OnPlayerShoot;
    public event Action<Vector2> OnPlayerMove;

    public Vector2 aimPosition
    {
        get; private set;
    }

    private PlayerMovement playerMovement;
    private void OnEnable()
    {
        if (playerMovement == null)
        {
            playerMovement = new PlayerMovement();
            playerMovement.Player.SetCallbacks(this);
        }
        playerMovement.Enable();
    }
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPlayerMove?.Invoke(context.ReadValue<Vector2>());
        }
        else
        {
            OnPlayerMove?.Invoke(new Vector2(0, 0));
        }
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPlayerShoot?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnPlayerShoot?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimPosition = context.ReadValue<Vector2>();
    }
}
