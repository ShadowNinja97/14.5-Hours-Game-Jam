using System.Collections;
using UnityEngine;

public class TutorialAnimator : MonoBehaviour
{
    public GameObject[] tutorials;
    public PlayerInputHandler ph;
    public FlashlightAim fa;
    public Animator logo;

    public void EnableAnimation(int tutorial)
    {
        tutorials[tutorial].SetActive(true);
    }

    void Update()
    {
        if (tutorials[0].activeSelf)
        {
            WalkLogic();
        }
        if (tutorials[1].activeSelf)
        {
            FlashLightLogic();
        }
        if (tutorials[2].activeSelf)
        {
            InteractLogic();
        }
    }

    public void WalkLogic()
    {
        if (ph.MoveInput != Vector2.zero)
        {
            tutorials[0].GetComponent<Animator>().SetTrigger("Triggered");
            DelayedDisable(0);
            logo.SetTrigger("Fade");
        }
    }


    public void FlashLightLogic()
    {
        if (fa.aimModifierAction.WasPressedThisFrame())
        {
            tutorials[1].GetComponent<Animator>().SetTrigger("Triggered");
            DelayedDisable(1);
        }
    }

    public void InteractLogic()
    {
        if (ph.InteractPressed)
        {
            tutorials[2].GetComponent<Animator>().SetTrigger("Triggered");
            DelayedDisable(2);
        }
    }

    IEnumerator DelayedDisable(int tut)
    {
        yield return new WaitForSeconds(2);
        tutorials[tut].SetActive(false);
    }

}
