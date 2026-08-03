using UnityEngine;

public class GhostHouseController : MonoBehaviour
{
    GhostPathing ghostPathing;
    public bool blinky = false;
    public bool pinky = false;
    public bool inky = false;
    public bool clyde = false;
    bool modeChanged = false;
    public bool startUp = true;
    bool movingUp = true;
    private Vector2 gollTile;
    public Vector2 shufflePoint = new Vector2(15.5f,16);
    Vector2[] exitPath = { Vector2.zero, new Vector2(13.5f, 16), new Vector2(13.5f, 19) };
    public int exitCounter = 0;
    public bool shuffling = true;
    private bool leftHome = false;
    public int personalDotCounter = 0;
    [SerializeField] private int dotLimit = 0;
    int[,] inkyDotLimits = { { 1, 30 }, { 2, 0}};
    int[,] clydeDotLimits = { { 1, 60 }, { 2, 50}};
    
    void Start()
    {
        ghostPathing = gameObject.GetComponent<GhostPathing>();
        exitPath[0]= shufflePoint;
        if(!blinky){
            setDirection(startUp);
            gollTile = shufflePoint;

            if(GhostManager.instance.level < 3){
                int[,] dotLimits = new int[2, 2];
                if(inky){
                    dotLimits = inkyDotLimits;
                }else if(clyde){
                    dotLimits = clydeDotLimits;
                }
                dotLimit = dotLimits[GhostManager.instance.level-1, 1];
            }
        }else{
            shuffling = false;
            gollTile = GetFirstMove();
            ghostPathing.SetNextTile(gollTile);
            exitCounter = exitPath.Length;
            
        }

    }

    public Vector2 GetFirstMove(){
        if(!modeChanged){
            ghostPathing.direction = Vector2.left;
            return new Vector2 (13,19);
        }else{
            ghostPathing.direction = Vector2.right;
            return new Vector2 (14,19);
        }
    }
    

    private void setDirection(bool up){
        movingUp = up;
        if(up){
            ghostPathing.direction = Vector2.up;
        }else{
            ghostPathing.direction = Vector2.down;
        }
    }

    private Vector2 getLeaveDirection(){
        return (exitPath[1] - exitPath[0]).normalized;
    }

    private Vector2 getShuffleTile(){
        float pointYMod = 0.5f;
        if(!movingUp){
            pointYMod = -0.5f;
        }
        return shufflePoint + new Vector2(0,pointYMod);
    }

    private void leaveHouse(){
        if(shuffling){
            exitCounter = 0;
            leftHome = false;
            setDirection(!movingUp);
            gollTile = getShuffleTile();
        }else if(exitCounter < exitPath.Length){
            gollTile = exitPath[exitCounter];
            switch (exitCounter)
            {
                case (0):
                    setDirection(!movingUp);
                    break;
                case (1):
                    ghostPathing.direction = getLeaveDirection();
                    break;
                case (2):
                    ghostPathing.direction = Vector2.up;
                    break;
            }
            exitCounter++;
        }else if (!leftHome){
            gollTile = GetFirstMove();
            ghostPathing.SetNextTile(gollTile);
            leftHome = true;
        } else{
            ghostPathing.house = false;

        }
    }
    private void ReturnHouse(){
        if(exitCounter > 0){
            exitCounter--;
            gollTile = exitPath[exitCounter];
            switch (exitCounter)
            {
                case (0):
                    setDirection(!movingUp);
                    break;
                case (1):
                    ghostPathing.direction = getLeaveDirection();
                    break;
                case (2):
                    ghostPathing.direction = Vector2.up;
                    break;
            }
        } else {
            ghostPathing.dead = false;
        } 
    }
    void Update() {
    if(ghostPathing.house){
            if (shuffling && personalDotCounter >= dotLimit){
                shuffling = false;
            }
            if (transform.position == (Vector3)gollTile) {
                if (ghostPathing.dead){
                    ReturnHouse();
                } else {
                    leaveHouse();
                }
            }else{
                transform.position = Vector3.MoveTowards(transform.position, gollTile, GhostManager.instance.ghostSpeed * Time.deltaTime);
            }
        }
    }
}
