using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PelletSpawner : MonoBehaviour
{
    [Header("Maze Reference")]
    public Tilemap wallTilemap;
    public Tilemap exclusionTilemap;

    [Header("Prefabs")]
    public GameObject normalPelletPrefab;
    public GameObject powerPelletPrefab;

    [Header("Power Pellet Positions")]
    public List<Vector3Int> powerPelletCells = new List<Vector3Int>();

    [Header("Parent")]
    public Transform pelletParent;

    void Awake()
    {
        SpawnPellets();
    }

    public void SpawnPellets()
    {
        if (wallTilemap == null || normalPelletPrefab == null || powerPelletPrefab == null)
        {
            Debug.LogError("[PelletSpawner] Missing a required reference.");
            return;
        }

        if (powerPelletCells.Count == 0)
            Debug.LogWarning("[PelletSpawner] powerPelletCells is empty - no power pellets will spawn.");

        wallTilemap.CompressBounds();
        BoundsInt bounds = wallTilemap.cellBounds;

        HashSet<Vector3Int> matchedPowerCells = new HashSet<Vector3Int>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(cell)) continue;

                Vector3 worldPos = wallTilemap.GetCellCenterWorld(cell);

                if (exclusionTilemap != null)
                {
                    Vector3Int exclCell = exclusionTilemap.WorldToCell(worldPos);
                    if (exclusionTilemap.HasTile(exclCell)) continue;
                }

                bool isPowerPellet = powerPelletCells.Contains(cell);
                if (isPowerPellet) matchedPowerCells.Add(cell);

                GameObject prefab = isPowerPellet ? powerPelletPrefab : normalPelletPrefab;
                Instantiate(prefab, worldPos, Quaternion.identity, pelletParent);
            }
        }

        foreach (Vector3Int powerCell in powerPelletCells)
        {
            if (matchedPowerCells.Contains(powerCell)) continue;

            string reason;
            if (powerCell.x < bounds.xMin || powerCell.x >= bounds.xMax || powerCell.y < bounds.yMin || powerCell.y >= bounds.yMax)
            {
                reason = $"outside wallTilemap.cellBounds {bounds}";
            }
            else if (wallTilemap.HasTile(powerCell))
            {
                reason = "sits on a wall tile";
            }
            else if (exclusionTilemap != null && exclusionTilemap.HasTile(exclusionTilemap.WorldToCell(wallTilemap.GetCellCenterWorld(powerCell))))
            {
                reason = "sits inside the exclusion tilemap";
            }
            else
            {
                reason = "unknown - didn't match despite passing all checks, worth double-checking the coordinate by hand";
            }

            Debug.LogWarning($"[PelletSpawner] powerPelletCells entry {powerCell} was never spawned - {reason}.");
        }
    }
}