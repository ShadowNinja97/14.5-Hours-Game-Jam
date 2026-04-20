using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Node : MonoBehaviour, ILightInteractable
{
    public enum NodeType
    {
        Toggle,
        Charge,
        Sustain
    }

    [Header("Node Details")]
    public NodeType nodeType;
    public UnityEvent fullChargeAction;
    public UnityEvent toggleOnAction;
    public UnityEvent toggleOffAction;

    [Header("Node Stats")]
    public float chargeRate = 1f;
    public float requiredCharge = 1f;
    public float sustainDecayRate = 1f;
    public float chargedBrightness = 1f;

    [Header("Debug")]
    [SerializeField] private float currentCharge = 0f;
    [SerializeField] private bool isCurrentlySeen = false;
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool toggleState = false;
    [SerializeField] private bool canToggleAgain = true;
    [SerializeField] private Light2D nodeLight;

    void Awake()
    {
        nodeLight = GetComponent<Light2D>();
    }

    public void OnLightSee()
    {
        isCurrentlySeen = true;
    }

    public void OnLightCharge()
    {
        switch (nodeType)
        {
            case NodeType.Toggle:
                HandleToggle();
                break;

            case NodeType.Charge:
                HandleCharge();
                break;

            case NodeType.Sustain:
                HandleSustainCharge();
                break;
        }
    }

    public void OnLightOff()
    {
        isCurrentlySeen = false;

        switch (nodeType)
        {
            case NodeType.Toggle:
                // Reset toggle once the flashlight leaves
                currentCharge = 0f;
                canToggleAgain = true;
                break;

            case NodeType.Sustain:
                // Nothing here, decay happens in Update
                break;

            case NodeType.Charge:
                // Permanent progress, so nothing
                break;
        }
    }

    private void Update()
    {
        if (nodeType == NodeType.Sustain && !isCurrentlySeen)
        {
            if (currentCharge > 0f)
            {
                currentCharge -= sustainDecayRate * Time.deltaTime;
                currentCharge = Mathf.Max(currentCharge, 0f);
            }

            // If sustain drops fully, allow it to be reactivated later
            if (isActivated && currentCharge <= 0f)
            {
                isActivated = false;
            }
        }
        ApplyLightBrightness(nodeLight);
    }

    private void HandleToggle()
    {
        if (!canToggleAgain)
            return;

        currentCharge += chargeRate * Time.deltaTime;
        currentCharge = Mathf.Min(currentCharge, requiredCharge);


        if (currentCharge >= requiredCharge)
        {
            toggleState = !toggleState;
            canToggleAgain = false;
            currentCharge = 0f;

            if (toggleState)
                toggleOnAction?.Invoke();
            else
                toggleOffAction?.Invoke();
        }
    }

    private void HandleCharge()
    {
        if (isActivated)
            return;

        currentCharge += chargeRate * Time.deltaTime;

        if (currentCharge >= requiredCharge)
        {
            currentCharge = requiredCharge;
            isActivated = true;
            fullChargeAction?.Invoke();
        }
    }

    private void HandleSustainCharge()
    {
        currentCharge += chargeRate * Time.deltaTime;
        currentCharge = Mathf.Min(currentCharge, requiredCharge);

        // Trigger once when fully sustained
        if (!isActivated && currentCharge >= requiredCharge)
        {
            isActivated = true;
            fullChargeAction?.Invoke();
        }
    }

    public float GetChargePercent()
    {
        if (requiredCharge <= 0f)
            return 1f;

        return currentCharge / requiredCharge;
    }

    public bool IsActive()
    {
        return isActivated;
    }

    public bool GetToggleState()
    {
        return toggleState;
    }

    public void ApplyLightBrightness(Light2D light)
    {
        if (light == null)
            return;

        float brightnessPercent;

        switch (nodeType)
        {
            case NodeType.Toggle:
                brightnessPercent = GetToggleLightPercent();
                break;

            case NodeType.Charge:
            case NodeType.Sustain:
            default:
                brightnessPercent = GetChargePercent();
                break;
        }

        light.intensity = chargedBrightness * brightnessPercent;
    }

    private float GetToggleLightPercent()
    {
        if (requiredCharge <= 0f)
            return toggleState ? 1f : 0f;

        float holdPercent = Mathf.Clamp01(currentCharge / requiredCharge);

        if (!toggleState)
        {
            // Currently OFF, so shining light builds brightness upward
            return holdPercent;
        }

        // Currently ON, so shining light dims it downward toward OFF
        return 1f - holdPercent;
    }
}