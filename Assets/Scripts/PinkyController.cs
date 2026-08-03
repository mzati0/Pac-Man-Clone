using UnityEngine;

public class PinkyController : MonoBehaviour
{
    [SerializeField] private Transform targetPac;
    [SerializeField] private PacMovement pacMan;
    void Update()
    {
        if(pacMan.direction == Vector2.up){
            targetPac.position = (Vector2)pacMan.transform.position + new Vector2(-4, 4);
            
        } else if(pacMan.direction == Vector2.right){
            targetPac.position = (Vector2)pacMan.transform.position + new Vector2(4, 0);
            
        } else if(pacMan.direction == Vector2.left){
            targetPac.position = (Vector2)pacMan.transform.position + new Vector2(-4, 0);
            
        } else if(pacMan.direction == Vector2.down){
            targetPac.position = (Vector2)pacMan.transform.position + new Vector2(0, -4);
        } 
    }
}
