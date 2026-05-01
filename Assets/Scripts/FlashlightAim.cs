using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera;
    public Light2D flashLight;

    [Tooltip("The trigger hitbox for the flashlight in its default state.")]
    public PolygonCollider2D NearHitbox;
    [Tooltip("The trigger hitbox for the flashlight in its aiming state.")]
    public PolygonCollider2D AimHitbox;

    [Header("Input Actions")]
    public InputAction mousePositionAction;
    public InputAction aimModifierAction;

    [Header("Settings")]
    public bool facingRight = true;
    public float defaultRadius = 4f;
    public float defaultAngle = 40f;
    public float aimRadius = 6f;
    public float aimAngle = 20f;
    public float aimAngleOffset = 45f;
    public float returnRotationSpeed = 360f; // degrees per second


    private bool freeAimHeld = false;
    private bool previousFacingRight;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        previousFacingRight = facingRight;

    }

    private void OnEnable()
    {
        mousePositionAction.Enable();
        aimModifierAction.Enable();
    }

    private void OnDisable()
    {
        mousePositionAction.Disable();
        aimModifierAction.Disable();
    }

    private void Update()
    {
        if (aimModifierAction.WasPressedThisFrame())
        {
            freeAimHeld = !freeAimHeld;
        }

        if (freeAimHeld)
        {
            AimTowardsMouse();
            ApplyAimLightSettings();
        }
        else
        {
            AimForward();
            ApplyDefaultLightSettings();
        }
    }

    private void AimTowardsMouse()
    {
        Vector2 mouseScreenPos = mousePositionAction.ReadValue<Vector2>();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - player.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + aimAngleOffset);
    }

    private void AimForward()
    {
        float finalAngle = facingRight ? 0f : 180f;
        finalAngle += aimAngleOffset;

        bool justFlippedDirection = previousFacingRight != facingRight;

        float targetAngle = finalAngle;

        if (justFlippedDirection)
        {
            float currentAngle = transform.eulerAngles.z;

            // Nudge upward first so Unity avoids the downward 180-degree snap path.
            targetAngle = currentAngle + (facingRight ? -20f : 20f);
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            returnRotationSpeed * Time.deltaTime
        );

        Quaternion finalRotation = Quaternion.Euler(0f, 0f, finalAngle);

        if (Quaternion.Angle(transform.rotation, finalRotation) < 1f)
        {
            previousFacingRight = facingRight;
        }
    }

    private void ApplyDefaultLightSettings()
    {
        flashLight.pointLightOuterRadius = defaultRadius;
        flashLight.pointLightOuterAngle = defaultAngle;

        if (NearHitbox != null)
            NearHitbox.enabled = true;

        if (AimHitbox != null)
            AimHitbox.enabled = false;
    }

    private void ApplyAimLightSettings()
    {
        flashLight.pointLightOuterRadius = aimRadius;
        flashLight.pointLightOuterAngle = aimAngle;

        if (NearHitbox != null)
            NearHitbox.enabled = false;

        if (AimHitbox != null)
            AimHitbox.enabled = true;
    }

    public void SetFacingDirection(float moveInputX)
    {
        if (moveInputX > 0.01f)
            facingRight = true;
        else if (moveInputX < -0.01f)
            facingRight = false;
    }
}