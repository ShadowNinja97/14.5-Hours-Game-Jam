using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject endScreen;


    public void WinLevel(int level)
    {
        endScreen.SetActive(true);
    }
}
