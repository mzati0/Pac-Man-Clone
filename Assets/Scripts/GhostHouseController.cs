using UnityEngine;

public class GhostHouseController : MonoBehaviour
{
    GhostPathing ghostPathing;

    bool modeChanged = false;
    public bool startUp = true;
    bool movingUp = true;
    private Vector2 gollTile;
    public Vector2 shufflePoint = new Vector2(15.5f,16);
    Vector2 [] exitPath = {Vector2.zero, new Vector2(13.5f,16),new Vector2(13.5f,19)};
    int exitCounter = 0;
    public bool shuffling = true;
    private bool leftHome = false;
    
    void Start()
    {
        ghostPathing = gameObject.GetComponent<GhostPathing>();
        setDirection(startUp);
        gollTile = getShuffleTile();
        exitPath[0]= shufflePoint;

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
        int pointYMod = 1;
        if(!movingUp){
            pointYMod = -1;
        }
        return shufflePoint + new Vector2(0,pointYMod);
    }

    void Update() {
    if(ghostPathing.house){
            if (transform.position == (Vector3)gollTile) {
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
                    leftHome = true;
                } else{
                    ghostPathing.house = false;
                }
            }else{
                transform.position = Vector3.MoveTowards(transform.position, gollTile, GhostManager.instance.ghostSpeed * Time.deltaTime);
            }
        }
    }
}
