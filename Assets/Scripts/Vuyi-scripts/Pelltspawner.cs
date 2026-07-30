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

    public int PelletCount { get; private set; }

    void Awake()
    {
        SpawnPellets();
    }

    public void ClearPellets()
    {
        if (pelletParent != null)
        {
            for (int i = pelletParent.childCount - 1; i >= 0; i--)
                Destroy(pelletParent.GetChild(i).gameObject);
        }
        else
        {
            foreach (Pellet p in FindObjectsOfType<Pellet>(true))
                Destroy(p.gameObject);
        }
        PelletCount = 0;
    }

    public void SpawnPellets()
    {
        if (wallTilemap == null || normalPelletPrefab == null || powerPelletPrefab == null)
            return;

        wallTilemap.CompressBounds();
        BoundsInt bounds = wallTilemap.cellBounds;
        HashSet<Vector3Int> matchedPowerCells = new HashSet<Vector3Int>();
        int spawnedCount = 0;

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
                spawnedCount++;
            }
        }

        PelletCount = spawnedCount;
    }
}