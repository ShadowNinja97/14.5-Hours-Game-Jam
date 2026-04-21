using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public GameObject Player;
    public UnityEvent DoorOpenEvent;

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("[Door] - Seeing Player");
        PlayerInputHandler pI = collision.GetComponent<PlayerInputHandler>();
        if (pI == null) return;
        
        if (pI.InteractPressed)
        {
            Debug.Log("[Door] - Interact Called");
            DoorOpenEvent?.Invoke();
            Debug.Log("[Door] - Event Invoked");
        }

    }

}
