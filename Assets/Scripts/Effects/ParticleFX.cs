using UnityEngine;

public class ParticleFX : MonoBehaviour
{
    public ParticleSystem primary;
    public ParticleSystem secondary;
    public float maxMultiplier = 100f;

    public bool display = true;

    public void UpdateParticleEmissions(float percentage, bool sustain = false)
    {
        if (display == false) return;

        var emissions = primary.emission;
        emissions.rateOverTime = percentage * maxMultiplier;

        if (percentage == 1 && !sustain)
        {
            primary.Stop();
            display = false;

            if (secondary != null)
            {
                secondary.Play();
            }
        }
    }

    public void UpdateDisplay(bool state)
    {
        display = state;
    }
}
