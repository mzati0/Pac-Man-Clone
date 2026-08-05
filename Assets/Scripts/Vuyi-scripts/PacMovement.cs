using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PacMovement : MonoBehaviour
{
    [Header("Speed")]
    int[,] pacSpeeds = { { 1, 80, 71, 90, 79 }, { 2, 90, 79, 95, 83 }, { 5, 100, 87, 100, 87 }, { 21, 90, 79, 0, 0 } };
    Vector2[] TunnelWarpTiles = {new Vector2(28f,16), new Vector2(-1,16)};
    [SerializeField] private float currentSpeed;
    public float normSpeed;
    public float normDotSpeed;
    public float FrightSpeed;
    public float FrightDotSpeed;

    [Header("Collision")]
    public Tilemap wallTilemap;

    [Header("Tunnel Wrap")]
    public bool enableTunnelWrap = true;

    [Header("Cornering")]
    public float cornerWindow = 0.15f;

    [Header("Ghost Collision")]
    public float ghostCollisionDistance = 0.5f;
    private GameObject[] ghosts;
    private Dictionary<GameObject, SpriteRenderer> ghostRenderers;
    private bool isDead;

    [Header("Ghost Eaten Score Popup")]
    [Tooltip("Assign in order: 200, 400, 800, 1600 - matches classic Pac-Man's ghost combo scoring.")]
    public Sprite[] ghostScoreSprites;
    public float scorePopupDuration = 1f;
    public string scorePopupSortingLayer = "Default";
    public int scorePopupOrderInLayer = 10;

    private int ghostsEatenThisFright = 0;
    private bool wasFrightened = false;

    [Header("Ghost Eaten Pause")]
    [Tooltip("Real-time seconds the game freezes - the arcade holds for ~1s.")]
    public float ghostEatenPauseDuration = 1f;
    private SpriteRenderer pacSpriteRenderer;
    private List<SpriteRenderer> pendingRevealRenderers = new List<SpriteRenderer>();

   
    private enum PacState { Active, GhostEatenFreeze }
    private PacState pacState = PacState.Active;

    public Vector2 direction = Vector2.zero;
    private Vector2 queuedDirection = Vector2.zero;
    [SerializeField] private Vector3 nextTile;
    private Vector3 lastIntersection;

    [Header("Spawn")]
    public float spawnX = 14.003f;
    public float spawnY = 7f;

    private Vector3 startPosition;
    private Vector2 startDirection;

    private InputSystem_Actions controls;
    private Animator anim;
    private bool isCorner = false;

    void Awake()
    {
        controls = new InputSystem_Actions();
        anim = GetComponent<Animator>();
        pacSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        GameManager.OnPacManDied += HandleDied;
    }

    void OnDisable()
    {
        controls.Player.Disable();
        GameManager.OnPacManDied -= HandleDied;

       
        if (pacState == PacState.GhostEatenFreeze)
        {
            Time.timeScale = 1f;
            pacState = PacState.Active;
            RevealPendingGhosts();
        }
    }

    void OnDestroy() => controls.Dispose();

    void Start()
    {
        startPosition = new Vector3(spawnX, spawnY, transform.position.z);
        startDirection = direction;

        transform.position = startPosition;
        nextTile = startPosition + new Vector3(-0.5f, 0, 0f); ;
        direction = Vector2.left;
        lastIntersection = startPosition;

        if (wallTilemap != null)
            wallTilemap.CompressBounds();

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        ghostRenderers = new Dictionary<GameObject, SpriteRenderer>();
        foreach (var ghost in ghosts)
        {
            if (ghost == null) continue;
            ghostRenderers[ghost] = ghost.GetComponentInChildren<SpriteRenderer>();
        }

        UpdateSpeeds();
    }

    public void ResetToStart()
    {
        transform.position = startPosition;
        direction = startDirection;
        queuedDirection = Vector2.left;
        nextTile = startPosition + new Vector3(-0.5f, 0, 0f);
        direction = Vector2.left;
        lastIntersection = startPosition;
        isDead = false;
        ghostsEatenThisFright = 0;
        wasFrightened = false;

        pacState = PacState.Active;
        Time.timeScale = 1f;
        if (pacSpriteRenderer != null)
            pacSpriteRenderer.enabled = true;
        RevealPendingGhosts();

        anim.Rebind();
        UpdateSpeeds();

    }
    private void UpdateSpeeds(){
        int level = GameManager.Instance.level;
        int count = 0; 
        while (pacSpeeds[count,0] <= level){
            normSpeed = (GameManager.Instance.BaseSpeed /100) * pacSpeeds[count,1];
            normDotSpeed = (GameManager.Instance.BaseSpeed /100) * pacSpeeds[count,2];
            FrightSpeed = (GameManager.Instance.BaseSpeed /100) * pacSpeeds[count,3];
            FrightDotSpeed = (GameManager.Instance.BaseSpeed / 100) * pacSpeeds[count, 4];
            count++;
        }
    }
    public void PacStart(){
        
    }
    public void triggerPacDeathAnm(){
        anim.SetFloat("Speed", 1f);
        anim.SetTrigger("Death");
    }
    public void StopAnm(){
        anim.SetFloat("Speed", 0f);
    }
    public void PlayAnm(){
        anim.SetFloat("Speed", 1f);
    }
    private void HandleDied()
    {
        isDead = true;
    }

    public bool IsTileAtWorldPosition(Vector2 worldPos)
    {
        if (wallTilemap == null) return false;
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPos);
        return wallTilemap.HasTile(cellPosition);
    }

    void Update()
    {
        if(direction != Vector2.zero){
            anim.SetFloat("X", (int)direction.x);
            anim.SetFloat("Y", (int)direction.y);
        }
        
        if (isDead) return;

        ReadQueuedDirection();

       
        if (pacState != PacState.Active) return;

        bool frightenedNow = GhostManager.instance != null && GhostManager.instance.globalFrightened;
        if (frightenedNow && !wasFrightened)
            ghostsEatenThisFright = 0; 
        wasFrightened = frightenedNow;

        CheckGhostCollision();
        if (isDead) return; 
        if((Vector2)transform.position == TunnelWarpTiles[0]){
                transform.position = TunnelWarpTiles[1];
                direction = Vector2.right;
                nextTile = transform.position + (Vector3)direction;
            } else if((Vector2)transform.position == TunnelWarpTiles[1]){
                transform.position = TunnelWarpTiles[0];
                direction = Vector2.left;
                nextTile = transform.position + (Vector3)direction;
            }
        if (transform.position == nextTile)
        {
            isCorner = false;
            lastIntersection = transform.position;

            if (queuedDirection != Vector2.zero && !IsTileAtWorldPosition(transform.position + (Vector3)queuedDirection))
            {
                direction = queuedDirection;
                anim.SetFloat("Speed", 1f);
            }
            else if (direction != Vector2.zero && IsTileAtWorldPosition(transform.position + (Vector3)direction))
            {
                anim.SetFloat("Speed", 0f);
                direction = Vector2.zero;
            }

            if (direction != Vector2.zero)
            {
                Vector3 candidate = transform.position + (Vector3)direction;

                /*if (enableTunnelWrap && direction.x != 0 && IsPastHorizontalEdge(candidate))
                {
                    //transform.position = WrapToOppositeSide(transform.position);
                    candidate = transform.position + (Vector3)direction;
                }*/

                nextTile = candidate;
            }
        }
        else
        {
            TryCorner();
            Vector2 box = new Vector2(1f, 1f);
            bool hasDot = false;
            Collider2D[] hit = Physics2D.OverlapBoxAll(nextTile, box, 0);
            for (int i = 0; i < hit.Length; i++)
            {
                if(hit[i].TryGetComponent<Pellet>(out Pellet pellet))
                {
                    hasDot = true;
                    break;
                }
            }
            if(hasDot){
                if(GhostManager.instance.globalFrightened){
                currentSpeed = FrightDotSpeed;
                } else {
                    currentSpeed = normDotSpeed;
                }
                print("Pellet Speed: " + currentSpeed);
            }else{
                if(GhostManager.instance.globalFrightened){
                currentSpeed = FrightSpeed;
                } else {
                    currentSpeed = normSpeed;
                }
            }
            if(isCorner){
                currentSpeed *= 1.5f;
                
            }
            transform.position = Vector3.MoveTowards(transform.position, nextTile, currentSpeed * Time.deltaTime);
        }
    }

    void CheckGhostCollision()
    {
        if (ghosts == null || GameManager.Instance == null) return;

        foreach (var ghost in ghosts)
        {
            if (ghost == null) continue;

            if (Vector3.Distance(transform.position, ghost.transform.position) <= ghostCollisionDistance)
            {
                GhostPathing ghostP = ghost.GetComponent<GhostPathing>();

                if (ghostP.dead)
                {
                    continue;
                }
                else if (ghostP.frightened)
                {
                    ghostP.TriggerDead();

                   
                    if (ghostRenderers.TryGetValue(ghost, out SpriteRenderer sr) && sr != null)
                    {
                        sr.enabled = false;
                        pendingRevealRenderers.Add(sr);
                    }

                    SpawnGhostScorePopup(ghost.transform.position);
                    GameManager.Instance.GhostEaten();

                    if (pacState != PacState.GhostEatenFreeze)
                        StartCoroutine(GhostEatenPause());

                    continue;
                }

                isDead = true;
                GameManager.Instance.PacManDied();
                return;
            }
        }
    }

    private IEnumerator GhostEatenPause()
    {
        pacState = PacState.GhostEatenFreeze;

        if (pacSpriteRenderer != null)
            pacSpriteRenderer.enabled = false;

        Time.timeScale = 0f; 

        yield return new WaitForSecondsRealtime(ghostEatenPauseDuration);

        Time.timeScale = 1f;

        if (pacSpriteRenderer != null)
            pacSpriteRenderer.enabled = true;

        RevealPendingGhosts(); 

        pacState = PacState.Active;
    }

    private void RevealPendingGhosts()
    {
        foreach (var sr in pendingRevealRenderers)
        {
            if (sr != null) sr.enabled = true;
        }
        pendingRevealRenderers.Clear();
    }

    void SpawnGhostScorePopup(Vector3 position)
    {
        if (ghostScoreSprites == null || ghostScoreSprites.Length == 0) return;

        int index = Mathf.Min(ghostsEatenThisFright, ghostScoreSprites.Length - 1);
        ghostsEatenThisFright++;

        GameObject popup = new GameObject("GhostScorePopup");
        popup.transform.position = position;

        SpriteRenderer sr = popup.AddComponent<SpriteRenderer>();
        sr.sprite = ghostScoreSprites[index];
        sr.sortingLayerName = scorePopupSortingLayer;
        sr.sortingOrder = scorePopupOrderInLayer;

        StartCoroutine(DestroyAfterRealtimeDelay(popup, scorePopupDuration));
    }

    private IEnumerator DestroyAfterRealtimeDelay(GameObject obj, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (obj != null) Destroy(obj);
    }

    void TryCorner()
    {
        if (direction == Vector2.zero) return;
        if (queuedDirection == Vector2.zero || queuedDirection == direction) return;
        if (Vector2.Dot(queuedDirection, direction) != 0f) return;

        float distToNext = Vector3.Distance(transform.position, nextTile);
        if (distToNext <= cornerWindow && !IsTileAtWorldPosition(nextTile + (Vector3)queuedDirection))
        {
            isCorner = true;
            Corner(nextTile, queuedDirection);
            return;
        }

        float distFromLast = Vector3.Distance(transform.position, lastIntersection);
        if (distFromLast <= cornerWindow && !IsTileAtWorldPosition(lastIntersection + (Vector3)queuedDirection))
        {
            isCorner = true;
            Corner(lastIntersection, queuedDirection);
        }
    }

    void Corner(Vector3 intersection, Vector2 newDirection)
    {
        direction = newDirection;
        nextTile = intersection + (Vector3)newDirection;
    }

    void ReadQueuedDirection()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();

        if (input == Vector2.zero) return;

        bool xDominant = Mathf.Abs(input.x) > Mathf.Abs(input.y);
        bool yDominant = Mathf.Abs(input.y) > Mathf.Abs(input.x);

        if (!xDominant && !yDominant)
        {
            bool movingHorizontally = direction == Vector2.left || direction == Vector2.right;
            xDominant = !movingHorizontally;
            yDominant = movingHorizontally;
        }

        Vector2 newQueued = queuedDirection;
        if (yDominant)
            newQueued = input.y > 0 ? Vector2.up : Vector2.down;
        else if (xDominant)
            newQueued = input.x > 0 ? Vector2.right : Vector2.left;

        queuedDirection = newQueued;
    }

    bool IsPastHorizontalEdge(Vector3 worldPos)
    {
        Vector3Int cell = wallTilemap.WorldToCell(worldPos);
        return cell.x < wallTilemap.cellBounds.xMin || cell.x >= wallTilemap.cellBounds.xMax;
    }

    /*Vector3 WrapToOppositeSide(Vector3 pos)
    {
        Vector3Int cell = wallTilemap.WorldToCell(pos);
        int oppositeX = direction.x > 0
            ? wallTilemap.cellBounds.xMin
            : wallTilemap.cellBounds.xMax - 1;

        Vector3 wrapped = wallTilemap.GetCellCenterWorld(new Vector3Int(oppositeX, cell.y, cell.z));
        return new Vector3(wrapped.x, pos.y, pos.z);
    }*/

    void OnDrawGizmos()
    {
        if (wallTilemap == null) return;

        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var d in dirs)
        {
            Vector2 center = (Vector2)transform.position + d;
            bool clear = !IsTileAtWorldPosition(center);
            Gizmos.color = clear ? Color.green : Color.red;
            Gizmos.DrawWireCube(center, Vector2.one * 0.8f);
        }

        Gizmos.color = Color.cyan;
        if (direction != Vector2.zero)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * 0.4f);

        Gizmos.color = Color.yellow;
        if (queuedDirection != Vector2.zero)
            Gizmos.DrawLine(transform.position + Vector3.up * 0.05f, transform.position + Vector3.up * 0.05f + (Vector3)queuedDirection * 0.3f);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(nextTile, cornerWindow);
    }
}