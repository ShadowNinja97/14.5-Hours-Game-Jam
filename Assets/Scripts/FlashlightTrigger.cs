using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lightOrigin;

    [Header("LOS")]
    [SerializeField] private LayerMask lightBlockerMask;

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
        if (lightOrigin == null)
            lightOrigin = transform;

        LightLOSOverride losOverride = target.GetComponent<LightLOSOverride>();

        if (losOverride != null && losOverride.ShouldIgnoreAllBlockers())
            return true;

        Vector2 start = lightOrigin.position;
        Vector2 end = target.bounds.center;
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            start,
            direction.normalized,
            distance,
            lightBlockerMask
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D blocker = hits[i].collider;

            if (blocker == null)
                continue;

            if (losOverride != null && losOverride.ShouldIgnoreBlocker(blocker))
                continue;

            return false;
        }

        return true;
    }
}