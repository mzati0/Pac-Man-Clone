using UnityEngine;

public class BlinkyController : MonoBehaviour
{
    //if the level is above the first int the second int is the threshold for Blinky to ignore scatter mode.
    //the thinrd is the speed blinky moves at in this mode.
    int[,] dotThreshold = new int[8, 2] { { 1, 20 }, { 2, 30 }, { 3, 40 }, { 6, 50 }, { 9, 60 }, { 12, 80 }, { 15, 100 }, { 19, 120 } };
    int[,] elroySpeed = new int[3, 2] { { 1, 80 }, { 2, 90 }, { 5, 100 } };
    bool elroyAlowed = true;

    void Update()
    {
        GhostManager ghostManager = GhostManager.instance;
        if (elroyAlowed){
            int speedmod;
            if(ghostManager.dotCount <= FindThreshold(ghostManager.level)){
                gameObject.GetComponent<GhostPathing>().elroy = true;
                speedmod = FindSpeed(ghostManager.level);
                
                if(ghostManager.dotCount <= FindThreshold(ghostManager.level)/2) {
                    speedmod += 5;
                }
                
                gameObject.GetComponent<GhostPathing>().elroySpeed = GameManager.Instance.BaseSpeed * speedmod / 100;
            }else{
                gameObject.GetComponent<GhostPathing>().elroy = false;
            }
        }
    }
    public void Scatter()
    {
        // Implement scatter behavior for Blinky here
    }
    private int FindThreshold(int level) {
        for (int i = 0; i < dotThreshold.GetLength(0); i++)
        {
            if (level >= dotThreshold[i, 0])
            {
                return dotThreshold[i, 1];
            }
        }
        return 120;
    }
    private int FindSpeed(int level) {
        for (int i = 0; i < elroySpeed.GetLength(0); i++)
        {
            if (level >= elroySpeed[i, 0])
            {
                return elroySpeed[i, 1];
            }
        }
        return 100;
    }
}
