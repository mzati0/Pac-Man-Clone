using UnityEngine;
using UnityEngine.Tilemaps; 
using System.Collections.Generic;

public class GhostPathing : MonoBehaviour
{
    public Transform targetPac;
    public Transform targetScatter;
    public Tilemap tilemap;
    public bool dead = false;
    Vector2[] directions = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
    Vector2[] doNotUpdateDirection = {new Vector2(16,7), new Vector2(15,7), new Vector2(13,7), new Vector2(12,7), new Vector2(11,7),
                                    new Vector2(16,19), new Vector2(15,19), new Vector2(13,19), new Vector2(12,19), new Vector2(11,19)};
    public Vector2 direction = Vector2.up;
    [SerializeField] private Vector3 nextTile;
    [SerializeField] private int speed = 5;
    public bool elroy = false;
    public float elroySpeed = 0;
    void Start()
    {
        
        nextTile = transform.position;
    }
    public bool IsTileAtWorldPosition(Vector2 worldPos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
        return tilemap.HasTile(cellPosition);
    }
    void Update()
    {
        if (transform.position == nextTile) {
            if (GhostManager.instance.frightened){
                //random direction
                print(nextTile);

            } else
            if( !System.Array.Exists(doNotUpdateDirection, element => element == (Vector2)transform.position) || IsTileAtWorldPosition(transform.position + (Vector3)direction)){
                    Vector2 target = targetPac.position;
                if(dead){
                    target = GhostManager.instance.deadGhostTarget.position;
                }else
                if(GhostManager.instance.scatter && !elroy){
                    target = targetScatter.position;
                }
                nextTile = GetNextTile(target);
                //print(nextTile);
                direction = nextTile - transform.position;
            } else {
                nextTile = transform.position + (Vector3)direction;
            }
        } else {
            float speed = GhostManager.instance.ghostSpeed;
            if (elroy) {
                speed = elroySpeed;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
        }
    }
    private bool isIntersection()
    {
        int count = 0;
        foreach (Vector2 direct in directions){
            Vector3 tile = transform.position + (Vector3)direct;
            if (!IsTileAtWorldPosition(tile) && direct != -direction)
            {
                count++;
            }
        }
        return count > 1;
    }
    private Vector3 GetNextTile(Vector3 target)
    {
        List<NextTile> nextTiles = new List<NextTile>();

        for (int i = 0; i < directions.Length; i++){
            Vector2 direct = directions[i];
            Vector3 tile = transform.position + (Vector3)direct;
            if (!IsTileAtWorldPosition(tile) && direct != -direction)
            {
                double distance = Vector3.Distance(tile, target);
                nextTiles.Add(new NextTile { position = tile, distance = distance, priority = i });
            }
        }
        if (nextTiles.Count > 1) {
            for (int i = 0; i < nextTiles.Count - 1; i++) {
                for (int j = i+1; j < nextTiles.Count; j++) {
                    if (nextTiles[j].distance < nextTiles[i].distance) {
                        NextTile temp = nextTiles[i];
                        nextTiles[i] = nextTiles[j];
                        nextTiles[j]= temp;
                    } else if (nextTiles[j].distance == nextTiles[i].distance) {
                        if (nextTiles[j].priority < nextTiles[i].priority) {
                            NextTile temp = nextTiles[i];
                            nextTiles[i] = nextTiles[j];
                            nextTiles[j] = temp;
                        }
                    } 
                }
            }
        }
        return nextTiles[0].position;
    }
    public void flip() {
        direction = -direction;
    }

}
[System.Serializable]
public class NextTile{
    public Vector2 position;
    public double distance;
    public int priority;
}
