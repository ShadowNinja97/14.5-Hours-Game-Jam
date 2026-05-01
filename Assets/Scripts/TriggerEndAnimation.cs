using UnityEngine;

public class TriggerEndAnimation : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Animator>().SetTrigger("DO IT");
    }
}
