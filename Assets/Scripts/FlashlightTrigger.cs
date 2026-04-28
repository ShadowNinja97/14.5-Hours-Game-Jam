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
        Debug.Log("Trying to connect to " + target.gameObject.name);
        if (target == null)
            return false;

        if (lightOrigin == null)
            lightOrigin = transform;

        Vector2 start = lightOrigin.position;
        Vector2 end = target.bounds.center;

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return true;

        direction.Normalize();

        float startOffset = 0.15f;
        start += direction * startOffset;
        distance -= startOffset;

        if (distance <= 0.001f)
            return true;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            start,
            direction,
            distance,
            lightBlockerMask
        );

        Debug.DrawLine(start, end, Color.red, 0.1f);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i].collider;

            if (hit == null)
                continue;

            // Ignore the target itself
            if (hit == target)
                continue;

            if ((ignoreLOSLayerMask.value & (1 << hit.gameObject.layer)) != 0)
                continue;

            if (hits[i].distance < 0.05f)
                continue;
            Debug.LogWarning("Ran into " + hit.gameObject.name);
            return false;
        }

        return true;
    }

}