using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Scriptable Objects/PlayerInputReader")]
public class PlayerInputReader : ScriptableObject, PlayerControls.IPlayerActions {
    public event Action<Vector2> MoveEvent;
    public event Action DashEvent;
    public event Action InventoryEvent;
    public event Action AttackEvent;
    public event Action CursorToggleEvent;
    public event Action InventoryOpenedEvent;
    public event Action InventoryClosedEvent;

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

    public void OnInventory(InputAction.CallbackContext context) {
        if (context.performed) {
            InventoryEvent?.Invoke();
        }
    }
    public void OnCursorToggle(InputAction.CallbackContext context) {
        if (context.performed) {
            CursorToggleEvent?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (context.performed) {
            AttackEvent?.Invoke();
        }
    }

    public void NotifyInventoryOpened() {
        Debug.Log("[PlayerInputReader] NotifyInventoryOpened - Broadcasting to subscribers");
        InventoryOpenedEvent?.Invoke();
    }

    public void NotifyInventoryClosed() {
        Debug.Log("[PlayerInputReader] NotifyInventoryClosed - Broadcasting to subscribers");
        InventoryClosedEvent?.Invoke();
    }
}
