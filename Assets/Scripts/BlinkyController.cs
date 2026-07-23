using UnityEngine;

public class BlinkyController : MonoBehaviour
{
    //if the level is above the first int the second int is the threshold for Blinky to ignore scatter mode.
    int[,] dotThreshold = new int[8, 2] { { 1, 20 }, { 2, 30 }, { 3, 40 }, { 6, 50},{9, 60}, {12, 80}, {15, 100}, {19, 120} };

    void Update()
    {
        GhostManager ghostManager = GhostManager.instance;
        if(ghostManager.dotCount > FindThreshold(ghostManager.level)){
            if(ghostManager.scatter){
                Scatter();
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
}
