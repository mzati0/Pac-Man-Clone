using UnityEngine;

public class InkyController : MonoBehaviour
{
    [SerializeField] private Transform pacTarget;
    [SerializeField] private PacMovement pacMan;
    [SerializeField] private Transform blinky;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pacOffset = Vector2.zero;
        if(pacMan.direction == Vector2.up){
            pacOffset = (Vector2)pacMan.transform.position + new Vector2(-2, 2);
            
        } else if(pacMan.direction == Vector2.right){
            pacOffset = (Vector2)pacMan.transform.position + new Vector2(2, 0);
            
        } else if(pacMan.direction == Vector2.left){
            pacOffset = (Vector2)pacMan.transform.position + new Vector2(-2, 0);
            
        } else if(pacMan.direction == Vector2.down){
            pacOffset = (Vector2)pacMan.transform.position + new Vector2(0, -2);
        } 
        pacTarget.position = (Vector2)blinky.position + (pacOffset - (Vector2)blinky.position) * 2;

        
    }
}
