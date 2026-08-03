using Unity.VisualScripting;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance;
    public int level = 1;
    public bool frightened = false;
    public bool scatter = false;
    public int dotCount = 30;
    public float ghostSpeedBase = 5;
    public float ghostSpeed = 0;
    public float ghostFrightenedSpeed = 0;
    public float ghostTunnelSpeed = 0;
    public Transform deadGhostTarget;
    int[,] ghostSpeeds = { { 1, 75, 50, 40 }, { 2, 85, 55, 45 }, { 5, 95, 60, 50 }, { 21, 95, 0, 50 } };
    double[,] ScatterChaseList = { { 1, 7, 20, 7, 20, 5, 20, 5 }, { 2, 7, 20, 7, 20, 5, 1033, 1 / 60f }, { 5, 5, 20, 5, 20, 5, 1037, 1 / 60f } , { 21, 5, 20, 5, 20, 5, 1037, 1/60f } };
    [System.NonSerialized] public double[,] ScatterChase = { { 0, 1 }, { 0, 0 }, { 0, 1 }, { 0, 0 }, { 0, 1 }, { 0, 0 }, { 0, 1 } };
    int[] frightenedTimes = { 6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 1, 0, 1, 0, 0, 0 };
    int[] flashCounts = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 3, 0, 3, 0, 0, 0 };
    int[,] frightenedSpeeds = { { 1, 50 }, { 2, 55 }, { 5, 60 }, { 21, 100 } };
    int timerPosition = 0;
    float time = 0;
    [SerializeField]float friteTime = 0;
    float friteTimer = 0;
    int friteFlashes = 0;
    public bool useGlobleDotCounter = false;
    public int globalDotCount = 0;
    GhostHouseController activeDotCount = null;
    public GameObject[] dotCountGhosts;

    void Awake()
    {
        if(level < 20) {
            for(int i = 0; i < frightenedSpeeds.GetLength(0); i++) {
                if(frightenedSpeeds[i,0] > level) {
                    ghostFrightenedSpeed = (ghostSpeedBase / 100) * frightenedSpeeds[i - 1, 1];
                } else {
                    break;
                }
            }
            friteTime = frightenedTimes[level - 1];
            friteFlashes = flashCounts[level - 1];
        } else {
            friteTime = 0;
            friteFlashes = 0;
            ghostFrightenedSpeed = 0;
        }
        
        if (instance == null) {
            instance = this;
            levelUpdate();
        } else {
            Destroy(gameObject);
        }
    }

    public void levelUpdate(){
        int count = 0; 
        while (ghostSpeeds[count,0] <= level){
            ghostSpeed = (ghostSpeedBase /100) * ghostSpeeds[count,1];
            ghostFrightenedSpeed = (ghostSpeedBase /100) * ghostSpeeds[count,2];
            ghostTunnelSpeed = (ghostSpeedBase / 100) * ghostSpeeds[count, 3];
            for (int i = 1; i <= 7; i++) {
                ScatterChase[i - 1, 0] = ScatterChaseList[count, i];
                //print(ScatterChase[i - 1,0] +","+ScatterChase[i - 1,1]);
            }
            count++;
            if (count >= ghostSpeeds.GetLength(0)){
                break;
            }
        }
        scatter = true;
        timerPosition = 0;
        time = 0;

    }
    private void SetScatter(double state) {
        AllFlip();
        if(state == 0){
            scatter = false;
        } else {
            scatter = true;
        }
    }
    
    private void AllFlip(){
        GhostPathing[] ghosts = FindObjectsByType<GhostPathing>();
        foreach (GhostPathing ghost in ghosts) {
            ghost.flip();
        }
    }
    public void triggerFrightened()
    {
        frightened = true;
        AllFlip();
    }
    public void triggerDotInc(){
        if(activeDotCount != null){
            activeDotCount.personalDotCounter++;
            print("triggerDotInc");
        }
    }
    
    void Update() {
        if(!useGlobleDotCounter){
            int i = 0;
            while (i < dotCountGhosts.Length) {
                GhostHouseController ghostHouse = dotCountGhosts[i].GetComponent<GhostHouseController>();
                if (ghostHouse.shuffling)
                {
                    activeDotCount = ghostHouse;
                    break;
                }
                i++;
            }
            if(1 == dotCountGhosts.Length){
                activeDotCount = null;
            }
        } else {
            
        }
        if(!frightened){
            if (timerPosition < ScatterChase.GetLength(0)-1) {
                if(ScatterChase[timerPosition, 0] > time){
                    time += Time.deltaTime * 1;
                    //print("Time: " + time);
                }else {
                    timerPosition++;
                    time = 0;
                    //print("Scatter: " + ScatterChase[timerPosition, 1]);
                    SetScatter(ScatterChase[timerPosition, 1]);
                }
            }else {
                scatter = false;
            }
        }else{
            if(friteTimer < friteTime){
                friteTimer += Time.deltaTime * 1;
            }else{
                frightened = false;
                friteTimer = 0;
            }
        }
    }
}
