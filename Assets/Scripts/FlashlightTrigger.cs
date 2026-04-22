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

        Vector2 start = lightOrigin.position;
        Vector2 end = target.bounds.center;

        RaycastHit2D hit = Physics2D.Linecast(start, end, lightBlockerMask);

        return hit.collider == null;
    }
}