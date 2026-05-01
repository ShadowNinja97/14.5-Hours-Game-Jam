using UnityEngine;
using UnityEngine.Events;

public class LightToggleObject : MonoBehaviour, ILightInteractable
{
    [Header("Toggle Settings")]
    public bool startsEnabled = true;
    public bool chargeByLight = true;
    public float chargeRate = 1f;
    public float requiredCharge = 1f;
    public bool requireLightOffBeforeRetoggle = true;

    [Header("Visual Settings")]
    [Range(0f, 1f)] public float enabledAlpha = 1f;
    [Range(0f, 1f)] public float disabledAlpha = 0.5f;

    [Header("Objects To Toggle")]
    public Collider2D[] collidersToToggle;
    public SpriteRenderer[] renderersToToggle;
    public GameObject[] gameObjectsToToggle;

    [Header("Optional Events")]
    public UnityEvent toggleOnAction;
    public UnityEvent toggleOffAction;
    public UnityEvent<float> chargePercentAction;

    [Header("Debug")]
    [SerializeField] private float currentCharge = 0f;
    [SerializeField] private bool isEnabledState;
    [SerializeField] private bool canToggleAgain = true;
    [SerializeField] private bool isCurrentlySeen = false;

    private void Awake()
    {
        isEnabledState = startsEnabled;
        ApplyState(isEnabledState);
        SendChargePercent();
    }

    public void OnLightSee()
    {
        if (!chargeByLight) return;

        isCurrentlySeen = true;
    }

    public void OnLightCharge()
    {
        if (!chargeByLight) return;

        if (!canToggleAgain)
            return;

        currentCharge += chargeRate * Time.deltaTime;
        currentCharge = Mathf.Clamp(currentCharge, 0f, requiredCharge);

        SendChargePercent();

        if (currentCharge >= requiredCharge)
        {
            ToggleState();

            currentCharge = 0f;
            SendChargePercent();

            if (requireLightOffBeforeRetoggle)
                canToggleAgain = false;
        }
    }

    public void OnLightOff()
    {
        if (!chargeByLight) return;
        isCurrentlySeen = false;
        currentCharge = 0f;
        SendChargePercent();

        canToggleAgain = true;
    }

    public void ToggleState()
    {
        isEnabledState = !isEnabledState;
        ApplyState(isEnabledState);

        if (isEnabledState)
            toggleOnAction?.Invoke();
        else
            toggleOffAction?.Invoke();
    }

    private void ApplyState(bool enabled)
    {
        for (int i = 0; i < collidersToToggle.Length; i++)
        {
            if (collidersToToggle[i] != null)
                collidersToToggle[i].enabled = enabled;
        }

        float targetAlpha = enabled ? enabledAlpha : disabledAlpha;

        for (int i = 0; i < renderersToToggle.Length; i++)
        {
            if (renderersToToggle[i] == null)
                continue;

            Color color = renderersToToggle[i].color;
            color.a = targetAlpha;
            renderersToToggle[i].color = color;
        }

        // Optional: only use this for extra effects/children, NOT the object holding this script.
        for (int i = 0; i < gameObjectsToToggle.Length; i++)
        {
            if (gameObjectsToToggle[i] != null)
                gameObjectsToToggle[i].SetActive(enabled);
        }
    }

    private void SendChargePercent()
    {
        chargePercentAction?.Invoke(GetChargePercent());
    }

    public float GetChargePercent()
    {
        if (requiredCharge <= 0f)
            return 1f;

        return Mathf.Clamp01(currentCharge / requiredCharge);
    }

    public bool IsEnabledState()
    {
        return isEnabledState;
    }
}