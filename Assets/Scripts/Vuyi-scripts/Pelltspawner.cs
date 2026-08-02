using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PelletSpawner : MonoBehaviour
{
    private const int MaxNormalPellets = 240;

    [Header("Maze Reference")]
    public Tilemap wallTilemap;
    public Tilemap exclusionTilemap;

    [Header("Prefabs")]
    public GameObject normalPelletPrefab;
    public GameObject powerPelletPrefab;

    [Header("Power Pellet Positions")]
    public List<Vector3Int> powerPelletCells = new List<Vector3Int>();

    [Header("Excluded Pellet Cells")]
    public List<Vector3Int> excludedPelletCells = new List<Vector3Int>();

    [Header("Pac-Man Start Exclusion")]
    public Transform pacManStartPosition;
    public float pacManStartExclusionRadius = 0.6f;

    [Header("Parent")]
    public Transform pelletParent;

    public int PelletCount { get; private set; }
    public int NormalPelletCount { get; private set; }
    public int PowerPelletCount { get; private set; }
    public int TotalPelletPoints { get; private set; }

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
        NormalPelletCount = 0;
        PowerPelletCount = 0;
        TotalPelletPoints = 0;
    }

    public void SpawnPellets()
    {
        if (wallTilemap == null || normalPelletPrefab == null || powerPelletPrefab == null)
            return;

        Pellet normalPelletData = normalPelletPrefab.GetComponent<Pellet>();
        Pellet powerPelletData = powerPelletPrefab.GetComponent<Pellet>();
        int normalScoreValue = normalPelletData != null ? normalPelletData.scoreValue : 0;
        int powerScoreValue = powerPelletData != null ? powerPelletData.scoreValue : 0;

        BoundsInt bounds = wallTilemap.cellBounds;

        int normalCount = 0;
        int powerCount = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(cell)) continue;

                if (excludedPelletCells.Contains(cell)) continue;

                Vector3 worldPos = wallTilemap.GetCellCenterWorld(cell);

                if (exclusionTilemap != null)
                {
                    Vector3Int exclCell = exclusionTilemap.WorldToCell(worldPos);
                    if (exclusionTilemap.HasTile(exclCell)) continue;
                }

                if (pacManStartPosition != null &&
                    Vector3.Distance(worldPos, pacManStartPosition.position) <= pacManStartExclusionRadius)
                    continue;

                bool isPowerPellet = powerPelletCells.Contains(cell);

                // Arcade-accurate cap: never place more than 240 normal pellets.
                // Power pellets are unaffected and always spawn at their assigned cells.
                if (!isPowerPellet && normalCount >= MaxNormalPellets)
                    continue;

                GameObject prefab = isPowerPellet ? powerPelletPrefab : normalPelletPrefab;
                Instantiate(prefab, worldPos, Quaternion.identity, pelletParent);

                if (isPowerPellet)
                    powerCount++;
                else
                    normalCount++;
            }
        }

        NormalPelletCount = normalCount;
        PowerPelletCount = powerCount;
        PelletCount = normalCount + powerCount;
        TotalPelletPoints = normalCount * normalScoreValue + powerCount * powerScoreValue;
    }
}