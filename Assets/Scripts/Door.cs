using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    private PlayerInputHandler playerInRange;
    public UnityEvent DoorOpenEvent;

    public bool Locked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = collision.GetComponent<PlayerInputHandler>();
        collision.GetComponent<PlayerMovement>().doorBlocked = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInputHandler pI = collision.GetComponent<PlayerInputHandler>();
        if (pI != null && pI == playerInRange)
        {
            playerInRange = null;
        }
        collision.GetComponent<PlayerMovement>().doorBlocked = false;

    }

    private void Update()
    {
        if (playerInRange != null && playerInRange.InteractPressed && !Locked)
        {
            DoorOpenEvent?.Invoke();
            AudioManager.Instance.PlayClip(2);
        }
    }

    public void ChangeLockState(bool locked)
    {
        Locked = locked;
    }
}
