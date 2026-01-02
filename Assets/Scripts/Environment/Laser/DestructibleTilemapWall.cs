using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class DestructibleTilemapWall : MonoBehaviour
{
    [Header("Hole size")]
    [SerializeField] private int defaultRadiusCells = 1;

    [Header("Optional")]
    [SerializeField] private CompositeCollider2D compositeToRebuild;
    [SerializeField] private bool debugLogs = true;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        if (!compositeToRebuild)
            compositeToRebuild = GetComponent<CompositeCollider2D>();

        if (debugLogs)
        {
            Debug.Log($"[DestructibleTilemapWall] Ready on '{gameObject.name}'. " +
                      $"Tilemap='{tilemap.name}', HasComposite={(compositeToRebuild != null)}");
        }
    }

    public void CarveHoleAtHit(Vector2 worldPos, int radiusCells = -1)
    {
        if (radiusCells < 0) radiusCells = defaultRadiusCells;

        Vector3Int center = tilemap.WorldToCell(worldPos);

        int removed = 0;

        for (int y = -radiusCells; y <= radiusCells; y++)
        {
            for (int x = -radiusCells; x <= radiusCells; x++)
            {
                if (x * x + y * y > radiusCells * radiusCells) continue;

                Vector3Int cell = new Vector3Int(center.x + x, center.y + y, center.z);

                if (!tilemap.HasTile(cell)) continue;

                tilemap.SetTile(cell, null);
                removed++;
            }
        }

        if (removed > 0)
        {
            tilemap.RefreshAllTiles();

            if (compositeToRebuild != null)
                compositeToRebuild.GenerateGeometry();
        }

        if (debugLogs)
        {
            Debug.Log($"[DestructibleTilemapWall] Carve at {worldPos} -> HitCell={center} " +
                      $"radiusCells={radiusCells} removedTiles={removed} Tilemap='{tilemap.name}'");
        }
    }

    public void CarveHoleAtHit(Vector2 worldPos)
    {
        CarveHoleAtHit(worldPos, defaultRadiusCells);
    }

}
