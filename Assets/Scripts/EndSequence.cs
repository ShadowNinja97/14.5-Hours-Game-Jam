using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class EndSequence : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField] private Light2D spotlight;
    [SerializeField] private FlashlightAim aim;
    [SerializeField] private float targetOuterAngle = 180f;
    [SerializeField] private float targetIntensity = 10f;
    [SerializeField] private float flashlightExpandTime = 3f;

    [Header("White Fade")]
    [SerializeField] private Image whiteFadePanel;
    [SerializeField] private float fadeDelay = 1f;
    [SerializeField] private float fadeTime = 2f;

    private bool hasStarted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasStarted) return;
        hasStarted = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        whiteFadePanel.GetComponent<Animator>().SetTrigger("White");


        StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        aim.enabled = false;
        if (spotlight == null || whiteFadePanel == null)
        {
            Debug.LogWarning("EndSequence is missing spotlight or white fade panel reference.");
            yield break;
        }

        Color panelColor = Color.white;
        panelColor.a = 0f;
        whiteFadePanel.color = panelColor;

        float startAngle = spotlight.pointLightOuterAngle;
        float startIntensity = spotlight.intensity;

        float timer = 0f;
        bool fadeStarted = false;

        while (timer < flashlightExpandTime)
        {
            timer += Time.deltaTime;
            float t = timer / flashlightExpandTime;

            spotlight.pointLightOuterAngle = Mathf.Lerp(startAngle, targetOuterAngle, t);
            spotlight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            if (!fadeStarted && timer >= fadeDelay)
            {
                fadeStarted = true;

            }

            yield return null;
        }

        spotlight.pointLightOuterAngle = targetOuterAngle;
        spotlight.intensity = targetIntensity;
    }
}