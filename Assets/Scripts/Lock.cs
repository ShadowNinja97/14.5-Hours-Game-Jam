using System.Collections;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private Animator anim;
    public Vector2 LaunchDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }


    public void Unlock()
    {
        StartCoroutine(UnlockAnim());
    }

    IEnumerator UnlockAnim()
    {
        anim.enabled = true;
        anim.SetTrigger("StartShake");
        yield return new WaitForSeconds(3f);
        anim.SetTrigger("StopShake");
        anim.enabled = false;

        rb.simulated = true;
        cc.enabled = true;

        Vector2 randomForce = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        randomForce *= LaunchDirection;
        rb.AddForce(randomForce, ForceMode2D.Impulse);
    }
}
