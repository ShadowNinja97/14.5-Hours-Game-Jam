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
        if (playerInRange != null)
        {
            Debug.Log("[Door] - Player entered range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInputHandler pI = collision.GetComponent<PlayerInputHandler>();
        if (pI != null && pI == playerInRange)
        {
            playerInRange = null;
            Debug.Log("[Door] - Player left range");
        }
    }

    private void Update()
    {
        if (playerInRange != null && playerInRange.InteractPressed && !Locked)
        {
            Debug.Log("[Door] - Interact Called");
            DoorOpenEvent?.Invoke();
            GetComponent<AudioSource>().Play();
            Debug.Log("[Door] - Event Invoked");
        }
    }

    public void ChangeLockState(bool locked)
    {
        Locked = locked;
    }
}
