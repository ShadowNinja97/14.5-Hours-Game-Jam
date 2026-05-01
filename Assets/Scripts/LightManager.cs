using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    private Coroutine fadeRoutine;
    public Node[] nodes;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        RecalculateNodes();
    }

    public void RecalculateNodes()
    {
        nodes = FindObjectsByType<Node>();
    }


    public void FadeAllToDark(float speed = 1, float duration = 1)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeAllToDarkRoutine(speed, duration));
    }

    public void DefaultFadeAll()
    {
        FadeAllToDark();
    }

    public void LongFadeAll()
    {
        FadeAllToDark(1, 2);
    }


    private IEnumerator FadeAllToDarkRoutine(float speed, float duration)
    {
        Light2D[] sceneLights = FindObjectsByType<Light2D>();

        float[] originalIntensities = new float[sceneLights.Length];

        for (int i = 0; i < sceneLights.Length; i++)
        {
            originalIntensities[i] = sceneLights[i].intensity;
        }
        foreach (Node n in nodes) n.HaltUpdate = true;
        yield return StartCoroutine(FadeLights(sceneLights, originalIntensities, 0f, speed));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(FadeLights(sceneLights, originalIntensities, 1f, speed));
        foreach (Node n in nodes) n.HaltUpdate = false;

        fadeRoutine = null;
    }

    private IEnumerator FadeLights(Light2D[] lights, float[] originalIntensities, float targetPercent, float speed)
    {
        float elapsed = 0f;

        float[] startIntensities = new float[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                startIntensities[i] = lights[i].intensity;
        }

        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / speed);

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] == null)
                    continue;

                float targetIntensity = originalIntensities[i] * targetPercent;
                lights[i].intensity = Mathf.Lerp(startIntensities[i], targetIntensity, t);
            }

            yield return null;
        }

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null)
                continue;

            lights[i].intensity = originalIntensities[i] * targetPercent;
        }

    }
}