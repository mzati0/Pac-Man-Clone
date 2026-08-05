using UnityEngine;
using UnityEngine.Tilemaps; 
using System.Collections.Generic;

public class GhostPathing : MonoBehaviour
{
    public Transform targetPac;
    public Transform targetScatter;
    public Tilemap tilemap;
    public bool dead = false;
    public bool frightened = false;
    public bool house = false;
    Vector2[] directions = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
    Vector2[] doNotUpdateDirection = {new Vector2(16,7), new Vector2(15,7), new Vector2(13,7), new Vector2(12,7), new Vector2(11,7),
                                    new Vector2(16,19), new Vector2(15,19), new Vector2(13,19), new Vector2(12,19), new Vector2(11,19)};
    Vector2[] TunnelTiles = {new Vector2(22,16), new Vector2(23,16), new Vector2(24,16), new Vector2(25,16), new Vector2(26,16), new Vector2(27,16),
                            new Vector2(0,16), new Vector2(1,16), new Vector2(2,16), new Vector2(3,16), new Vector2(4,16), new Vector2(5,16)};
    Vector2[] TunnelWarpTiles = {new Vector2(28f,16), new Vector2(-1,16)};
    public Vector2 direction = Vector2.up;
    public Vector3 nextTile;
    [SerializeField] private int speed = 5;
    public bool elroy = false;
    public float elroySpeed = 0;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        nextTile = transform.position;
    }
    public bool IsTileAtWorldPosition(Vector2 worldPos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
        return tilemap.HasTile(cellPosition);
    }
    void Update()
    {
        anim.SetFloat("X", (int)direction.x);
        anim.SetFloat("Y", (int)direction.y);
        anim.SetBool("frightened", frightened);
        anim.SetBool("Dead", dead);
        if(!house){
            if(dead && transform.position == GhostManager.instance.deadGhostTarget.position){
                house = true;
                return;
            }
            if((Vector2)transform.position == TunnelWarpTiles[0]){
                transform.position = TunnelWarpTiles[1];
                direction = Vector2.right;
                nextTile = transform.position + (Vector3)direction;
            } else if((Vector2)transform.position == TunnelWarpTiles[1]){
                transform.position = TunnelWarpTiles[0];
                direction = Vector2.left;
                nextTile = transform.position + (Vector3)direction;
            }
            if (transform.position == nextTile) {
                if( !System.Array.Exists(doNotUpdateDirection, element => element == (Vector2)transform.position) || !dead || IsTileAtWorldPosition(transform.position + (Vector3)direction)){
                    if(dead){
                        nextTile = GetNextTile(GhostManager.instance.deadGhostTarget.position);
                    } else if (frightened){
                        nextTile = GetRandomTile();
                    } else
                    if(GhostManager.instance.scatter && !elroy){
                        nextTile = GetNextTile(targetScatter.position);
                    } else{
                        nextTile = GetNextTile(targetPac.position);
                    }
                    //print(nextTile);
                    direction = nextTile - transform.position;
                } else {
                    nextTile = transform.position + (Vector3)direction;
                }
            } else {
                float speed = GhostManager.instance.ghostSpeed;
                if(dead){
                    speed = GameManager.Instance.BaseSpeed;
                } else if (System.Array.Exists(TunnelTiles, element => element == (Vector2)transform.position)) {
                    speed = GhostManager.instance.ghostTunnelSpeed;
                } else if (frightened) {
                    speed = (int)GhostManager.instance.ghostFrightenedSpeed;
                } else if (elroy && !GhostManager.instance.elroyDisabled) {
                    speed = elroySpeed;
                } 
                transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
            }
        }
        
    }
    public void TriggerDead(){
        dead = true;
        SoundManager.Instance.PlayGhostEaten();
        // TODO: Freeze the game, show the points
        SoundManager.Instance.PlayRunHome();
        direction = Vector2.zero;
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
    public void Flash()
    {
        anim.SetBool("Flash", !anim.GetBool("Flash"));
    }
    public void SetFlash(bool flash)
    {
        anim.SetBool("Flash", flash);
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
    private Vector3 GetRandomTile()
    {
        List<NextTile> nextTiles = new List<NextTile>();
        Vector3 randDirection = (Vector3)Vector2.zero;
        float roundedRandom = UnityEngine.Random.Range(0, 1000) / 10.0f; 
        //print(roundedRandom);
        if(roundedRandom >= 83.7 && -direction != Vector2.up && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.up)){
            randDirection = (Vector3)Vector2.up;
        } else if(roundedRandom >= 58.5 && -direction != Vector2.right && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.right)){
            randDirection = (Vector3)Vector2.right;
        } else if(roundedRandom >= 30 && -direction != Vector2.down && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.down)){
             randDirection = (Vector3)Vector2.down;

        }else if(-direction != Vector2.left && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.left)){
            randDirection = (Vector3)Vector2.left;
        }else if(-direction != Vector2.up && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.up)){
            randDirection = (Vector3)Vector2.up;
        } else if(-direction != Vector2.right && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.right)){
            randDirection = (Vector3)Vector2.right;
        } else if( -direction != Vector2.down && !IsTileAtWorldPosition(transform.position + (Vector3)Vector2.down)){
             randDirection = (Vector3)Vector2.down;
        }
        return transform.position + randDirection;
    }
    public void flip()
    {
        if (!dead) {
            direction = -direction;
        }
    }
    public void SetNextTile(Vector3 nextTile) {
        this.nextTile = nextTile;
    }
    public void reset(){
        dead = false;
        house = true;
        elroy = false;
        GhostHouseController houseCont = GetComponent<GhostHouseController>();
        houseCont.Configure();
        
    }

}
[System.Serializable]
public class NextTile{
    public Vector2 position;
    public double distance;
    public int priority;
}
