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
    public bool elroyDisabled = false;
    int[,] ghostSpeeds = { { 1, 75, 50, 40 }, { 2, 85, 55, 45 }, { 5, 95, 60, 50 }, { 21, 95, 100, 50 } };
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
    private float idolTimeLimit;
    private float idolTimer = 0;

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

        if(level < 20) {
            /*for(int i = 0; i < frightenedSpeeds.GetLength(0); i++) {
                if(frightenedSpeeds[i,0] > level) {
                    ghostFrightenedSpeed = (ghostSpeedBase / 100) * frightenedSpeeds[i - 1, 1];
                } else {
                    break;
                }
            }*/
            friteTime = frightenedTimes[level - 1];
            friteFlashes = flashCounts[level - 1];
        } else {
            friteTime = 0;
            friteFlashes = 0;
            //ghostFrightenedSpeed = 0;
        }

        if (level < 5){
            idolTimeLimit = 4;
        } else{
            idolTimeLimit = 3;
        }
        useGlobleDotCounter = false;
        scatter = false;
        timerPosition = 0;
        time = 0;
        frightened = false;
        activeDotCount = null;

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
        if(!useGlobleDotCounter){
            if(activeDotCount != null){
                activeDotCount.personalDotCounter++;
                print("triggerDotInc");
            }
        } else {
            globalDotCount++;
            idolTimer = 0;
        }
    }
    public void PacManDeath(){
        globalDotCount = 0;
        GhostPathing [] ghosts = FindObjectsByType<GhostPathing>();
        elroyDisabled = true;
        foreach (GhostPathing ghost in ghosts){
            ghost.reset();
        }
    }

    public void NewLevel(int levelIn){
        level = levelIn;
        useGlobleDotCounter = false;
        GhostPathing [] ghosts = FindObjectsByType<GhostPathing>();
        foreach (GhostPathing ghost in ghosts){
            ghost.reset();
            ghost.gameObject.GetComponent<GhostHouseController>().personalDotCounter = 0;
        }
        levelUpdate();
    }
    void Update() {
        if(!useGlobleDotCounter){
            int i = 0;
            while (i < dotCountGhosts.Length) {
                GhostHouseController ghostHouse = dotCountGhosts[i].GetComponent<GhostHouseController>();
                if (ghostHouse.shuffling && dotCountGhosts[i].GetComponent<GhostPathing>().house)
                {
                    activeDotCount = ghostHouse;
                    break;
                }
                i++;
            }
            if(i == dotCountGhosts.Length){
                activeDotCount = null;
            }
        } else {
            switch(globalDotCount){
                case 7:
                    dotCountGhosts[0].GetComponent<GhostHouseController>().shuffling = false;
                    break;
                case 17:
                    dotCountGhosts[1].GetComponent<GhostHouseController>().shuffling = false;
                    break;
                case 32:
                    if(dotCountGhosts[2].GetComponent<GhostHouseController>().shuffling){
                        dotCountGhosts[2].GetComponent<GhostHouseController>().shuffling = false;
                        elroyDisabled = false;
                        useGlobleDotCounter = false;
                    }
                    break;
            }
            idolTimer += Time.deltaTime * 1;
            if(idolTimer >= idolTimeLimit){
                int j = 0;
                while (j < dotCountGhosts.Length) {
                    GhostHouseController ghostHouse = dotCountGhosts[j].GetComponent<GhostHouseController>();
                    if (ghostHouse.shuffling && dotCountGhosts[j].GetComponent<GhostPathing>().house)
                    {
                        ghostHouse.shuffling = false;
                        idolTimer = 0;
                        break;
                    }
                    j++;
                }
            }
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
