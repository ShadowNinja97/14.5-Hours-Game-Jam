using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    private readonly HashSet<ILightInteractable> seenObjects = new();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ILightInteractable lightInteractable = collision.GetComponent<ILightInteractable>();
        if (lightInteractable == null)
            return;

        if (seenObjects.Add(lightInteractable))
        {
            lightInteractable.OnLightSee();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ILightInteractable lightInteractable = collision.GetComponent<ILightInteractable>();
        if (lightInteractable == null)
            return;

        // Safety check in case Enter is missed for some reason
        if (!seenObjects.Contains(lightInteractable))
        {
            seenObjects.Add(lightInteractable);
            lightInteractable.OnLightSee();
        }

        lightInteractable.OnLightCharge();
        Debug.Log("Seeing Object");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ILightInteractable lightInteractable = collision.GetComponent<ILightInteractable>();
        if (lightInteractable == null)
            return;

        if (seenObjects.Remove(lightInteractable))
        {
            lightInteractable.OnLightOff();
        }
    }
}