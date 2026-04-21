using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformCollider : MonoBehaviour
{
    public int DisabledAlpha = 50;
    private TilemapCollider2D col;
    private Tilemap tile;

    void Awake()
    {
        col = GetComponent<TilemapCollider2D>();
        tile = GetComponent<Tilemap>();
    }

    public void SetColliders(bool enabled)
    {

        col.enabled = enabled;

        if (enabled) tile.color = new Color(tile.color.r, tile.color.g, tile.color.b, 1);
        else tile.color = new Color(tile.color.r, tile.color.g, tile.color.b, DisabledAlpha);
    }
}
