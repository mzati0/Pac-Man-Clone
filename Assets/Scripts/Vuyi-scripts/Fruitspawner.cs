using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FruitDefinition
    {
        public string label;
        public int points;
        public GameObject fruitPrefab;

        [Tooltip("Score popup sprite for this fruit (e.g. '100', '300', '500'...).")]
        public Sprite scorePopupSprite;
    }

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Fruit Progression")]
    [Tooltip("Order must match FruitLevelCounterUI: 0=Cherry(100) 1=Strawberry(300) 2=Orange(500) 3=Apple(700) 4=Melon(1000) 5=Galaxian(2000) 6=Bell(3000) 7=Key(5000)")]
    public FruitDefinition[] fruitsByLevel;

    [Header("Spawn Triggers")]
    public int[] pelletThresholds = { 70, 170 };

    [Header("Lifetime")]
    public float minLifetimeSeconds = 9f;
    public float maxLifetimeSeconds = 10f;

    private bool[] triggered;
    private int currentLevel = 1;
    private GameObject activeFruit;

    void Awake()
    {
        triggered = new bool[pelletThresholds.Length];
    }

    public void NotifyPelletEaten(int eatenCountThisLevel)
    {
        for (int i = 0; i < pelletThresholds.Length; i++)
        {
            if (triggered[i]) continue;
            if (eatenCountThisLevel < pelletThresholds[i]) continue;
            triggered[i] = true;
            SpawnFruit();
        }
    }

    public void ResetForNewLevel(int newLevel)
    {
        currentLevel = newLevel;
        if (triggered == null || triggered.Length != pelletThresholds.Length)
            triggered = new bool[pelletThresholds.Length];
        else
            System.Array.Clear(triggered, 0, triggered.Length);

        if (activeFruit != null)
        {
            Destroy(activeFruit);
            activeFruit = null;
        }
    }

   
    public void DespawnActiveFruit()
    {
        if (activeFruit != null)
        {
            Destroy(activeFruit);
            activeFruit = null;
        }
    }

    void SpawnFruit()
    {
        if (fruitsByLevel == null || fruitsByLevel.Length == 0)
            return;

        if (spawnPoint == null)
            return;

        if (activeFruit != null)
            Destroy(activeFruit);

        int index = FruitLevelCounterUI.GetFruitIndexForLevel(currentLevel);
        index = Mathf.Clamp(index, 0, fruitsByLevel.Length - 1);
        FruitDefinition def = fruitsByLevel[index];

        if (def.fruitPrefab == null)
            return;

        activeFruit = Instantiate(def.fruitPrefab, spawnPoint.position, Quaternion.identity);
        Fruit fruit = activeFruit.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.lifetime = Random.Range(minLifetimeSeconds, maxLifetimeSeconds);
            fruit.points = def.points;
            fruit.scorePopupSprite = def.scorePopupSprite;
        }
    }
}