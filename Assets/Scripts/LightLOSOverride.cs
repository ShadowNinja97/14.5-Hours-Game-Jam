using UnityEngine;

public class LightLOSOverride : MonoBehaviour
{
    public enum LOSMode
    {
        UseDefault,
        IgnoreAllBlockers,
        IgnoreSpecificBlockers
    }

    [Header("LOS Override")]
    public LOSMode losMode = LOSMode.UseDefault;

    [Tooltip("Only used if LOS Mode is IgnoreSpecificBlockers.")]
    public Collider2D[] blockersToIgnore;

    public bool ShouldIgnoreAllBlockers()
    {
        return losMode == LOSMode.IgnoreAllBlockers;
    }

    public bool ShouldIgnoreBlocker(Collider2D blocker)
    {
        if (losMode != LOSMode.IgnoreSpecificBlockers)
            return false;

        if (blocker == null || blockersToIgnore == null)
            return false;

        for (int i = 0; i < blockersToIgnore.Length; i++)
        {
            if (blockersToIgnore[i] == blocker)
                return true;
        }

        return false;
    }
}