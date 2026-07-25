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

    [Header("Debug")]
    public bool debugMode = true;

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

        wallTilemap.CompressBounds();
        BoundsInt bounds = wallTilemap.cellBounds;
        int spawned = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(cell)) continue;
                if (exclusionTilemap != null && exclusionTilemap.HasTile(cell)) continue;

                Vector3 worldPos = wallTilemap.GetCellCenterWorld(cell);
                GameObject prefab = powerPelletCells.Contains(cell) ? powerPelletPrefab : normalPelletPrefab;
                Instantiate(prefab, worldPos, Quaternion.identity, pelletParent);
                spawned++;
            }
        }

        if (debugMode)
            Debug.Log($"[PelletSpawner] spawned {spawned} pellets.");
    }
}