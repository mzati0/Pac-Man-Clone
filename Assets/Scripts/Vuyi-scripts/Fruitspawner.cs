using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FruitSpawn
    {
        public GameObject fruitPrefab;
        public Transform spawnPoint;
        public int pelletThreshold = 70;
    }

  
    public FruitSpawn[] spawns;

    private bool[] triggered;

    void Awake()
    {
        triggered = new bool[spawns.Length];
    }

    public void NotifyPelletEaten(int eatenCount)
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            if (triggered[i] || eatenCount < spawns[i].pelletThreshold) continue;

            triggered[i] = true;
            var s = spawns[i];
            if (s.fruitPrefab != null && s.spawnPoint != null)
                Instantiate(s.fruitPrefab, s.spawnPoint.position, Quaternion.identity);
        }
    }
}