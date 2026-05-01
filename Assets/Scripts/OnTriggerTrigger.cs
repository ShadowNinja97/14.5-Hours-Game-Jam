using UnityEngine;
using UnityEngine.Events;

public class OnTriggerTrigger : MonoBehaviour
{
    public UnityEvent eventToTrigger;

    void OnTriggerEnter2D(Collider2D collision)
    {
        eventToTrigger?.Invoke();
    }
}
