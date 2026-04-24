using System.Collections;
using UnityEngine;

public class Warp : MonoBehaviour
{
    [Header("Warp")]
    public Vector2 WarpPosition;

    [Header("Pan Settings")]
    public bool Pan = false;
    public Vector2 PanPosition;
    public float PanDuration = 5f;
    public float PanPause = 0f;

    [Header("Vanish Settings")]
    public bool Vanish = true;
    public float VanishDelay = 0.2f;
    public float VanishReturn = 2f;

    private GameObject player;
    private Camera mainCamera;
    private bool warpInProgress = false;

    private void Awake()
    {
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (playerMovement != null)
            player = playerMovement.gameObject;

        mainCamera = Camera.main;
    }

    public void WarpPlayer()
    {
        if (warpInProgress || player == null)
            return;

        StartCoroutine(PlayerWarping());
    }

    private IEnumerator PlayerWarping()
    {
        warpInProgress = true;

        if (Vanish)
            yield return StartCoroutine(HandleVanish(false));

        if (Pan && mainCamera != null)
            yield return StartCoroutine(HandlePan());

        player.transform.position = WarpPosition;

        if (Vanish)
            yield return StartCoroutine(HandleVanish(true));

        warpInProgress = false;
    }

    private IEnumerator HandleVanish(bool reappear)
    {
        if (!reappear)
        {
            yield return new WaitForSeconds(VanishDelay);
            player.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(VanishReturn);
            player.SetActive(true);
        }
    }

    private IEnumerator HandlePan()
    {
        yield return new WaitForSeconds(PanPause);

        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = new Vector3(PanPosition.x, PanPosition.y, startPos.z);

        float elapsed = 0f;

        while (elapsed < PanDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / PanDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        mainCamera.transform.position = targetPos;

    }
}