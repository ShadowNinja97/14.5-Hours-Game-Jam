using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction JumpAction;
    public bool JumpHeld => JumpAction.IsPressed();


    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }

    private void OnEnable()
    {
        MoveAction.Enable();
        JumpAction.Enable();
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        JumpAction.Disable();
    }

    private void Update()
    {
        MoveInput = MoveAction.ReadValue<Vector2>();
        JumpPressed = JumpAction.triggered;
    }

}
