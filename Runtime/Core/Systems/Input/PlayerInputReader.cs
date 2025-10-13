using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Scriptable Objects/PlayerInputReader")]
public class PlayerInputReader : ScriptableObject, PlayerControls.IPlayerActions {
    public event Action<Vector2> MoveEvent;
    public event Action DashEvent;

    private PlayerControls controls;

    private void OnEnable() {
        if (controls == null) {
            controls = new PlayerControls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    private void OnDisable() {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context) {
        if (context.performed) {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        } else if (context.canceled) {
            MoveEvent?.Invoke(Vector2.zero);
        }
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.performed) {
            DashEvent?.Invoke();
        }
    }
}
