using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance;
    public int level = 1;
    public bool frightened = false;
    public bool scatter = false;
    public int dotCount = 30;
    public double ghostSpeedBase = 5;
    public double ghostSpeed = 0;
    public double ghostFrightenedSpeed = 0;
    public double ghostTunnelSpeed= 0;
    int[,] ghostSpeeds = {{1,75,50,40},{2,85,55,45},{5,95,60,50},{21,95,0,50}};

    void Awake()
    {
        if (instance == null) {
            instance = this;
            levelUpdate();
        } else {
            Destroy(gameObject);
        }
    }

    public void levelUpdate(){
        foreach (int[] item in ghostSpeeds){
            if (level >= item[0]){
                ghostSpeed = (ghostSpeedBase /100) * item[1];
                ghostFrightenedSpeed = (ghostSpeedBase /100) * item[2];
                ghostTunnelSpeed = (ghostSpeedBase /100) * item[3];
                brake();
            }
        }
    }
    void Update()
    {

    }
    
}
