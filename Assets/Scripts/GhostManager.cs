using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance;
    public int level = 1;
    public bool frightened = false;
    public bool scatter = false;
    public int dotCount = 30;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

    }
    
}
