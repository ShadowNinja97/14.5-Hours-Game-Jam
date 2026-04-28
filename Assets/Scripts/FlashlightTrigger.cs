using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lightOrigin;

    [Header("LOS")]
    [SerializeField] private LayerMask lightBlockerMask;
    [SerializeField] private LayerMask ignoreLOSLayerMask;

    private readonly HashSet<ILightInteractable> seenObjects = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ILightInteractable interactable = collision.GetComponent<ILightInteractable>();
        if (interactable == null)
            return;

        if (!HasLineOfSight(collision))
            return;

        if (seenObjects.Add(interactable))
        {
            interactable.OnLightSee();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ILightInteractable interactable = collision.GetComponent<ILightInteractable>();
        if (interactable == null)
            return;

        if (!HasLineOfSight(collision))
        {
            if (seenObjects.Remove(interactable))
            {
                interactable.OnLightOff();
            }

            return;
        }

        if (!seenObjects.Contains(interactable))
        {
            seenObjects.Add(interactable);
            interactable.OnLightSee();
        }

        interactable.OnLightCharge();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ILightInteractable interactable = collision.GetComponent<ILightInteractable>();
        if (interactable == null)
            return;

        if (seenObjects.Remove(interactable))
        {
            interactable.OnLightOff();
        }
    }

    private bool HasLineOfSight(Collider2D target)
    {
        if (target == null)
            return false;

        if (lightOrigin == null)
            lightOrigin = transform;

        // Check target or target parent for LOS override.
        LightLOSOverride losOverride = target.GetComponentInParent<LightLOSOverride>();

        if (losOverride != null && losOverride.ShouldIgnoreAllBlockers())
            return true;

        Vector2 start = lightOrigin.position;
        Vector2 end = target.ClosestPoint(start);

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return true;

        direction.Normalize();

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            start,
            direction,
            distance,
            lightBlockerMask
        );

        Debug.DrawLine(start, end, Color.red, 0.1f);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D blocker = hits[i].collider;

            if (blocker == null)
                continue;

            // Ignore the target itself.
            if (blocker == target)
                continue;

            // Ignore globally ignored layers, like Player.
            if ((ignoreLOSLayerMask.value & (1 << blocker.gameObject.layer)) != 0)
                continue;

            // Ignore blockers specifically ignored by this target.
            if (losOverride != null && losOverride.ShouldIgnoreBlocker(blocker))
                continue;

            Debug.LogWarning($"Ran into: {blocker.gameObject.name}", blocker.gameObject);
            return false;
        }

        return true;
    }

}