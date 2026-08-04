using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PacMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Collision")]
    public Tilemap wallTilemap;

    [Header("Tunnel Wrap")]
    public bool enableTunnelWrap = true;

    [Header("Cornering")]
    public float cornerWindow = 0.15f;

    [Header("Ghost Collision")]
    public float ghostCollisionDistance = 0.5f;
    private GameObject[] ghosts;
    private bool isDead;

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

    void Awake()
    {
        controls = new InputSystem_Actions();
        anim = GetComponent<Animator>();
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
        anim.Rebind();
        
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
        CheckGhostCollision();
        if (isDead) return; // freeze the instant a collision is registered this frame

        if (transform.position == nextTile)
        {
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

                if (enableTunnelWrap && direction.x != 0 && IsPastHorizontalEdge(candidate))
                {
                    transform.position = WrapToOppositeSide(transform.position);
                    candidate = transform.position + (Vector3)direction;
                }

                nextTile = candidate;
            }
        }
        else
        {
            TryCorner();
            transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
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
                if (ghostP.frightened)
                {
                    ghostP.TriggerDead();
                    continue;
                } else 
                    if (ghostP.dead) {
                        continue;
                }

                isDead = true;
                GameManager.Instance.PacManDied();
                return;
            }
        }
    }

    void TryCorner()
    {
        if (direction == Vector2.zero) return;
        if (queuedDirection == Vector2.zero || queuedDirection == direction) return;
        if (Vector2.Dot(queuedDirection, direction) != 0f) return;

        float distToNext = Vector3.Distance(transform.position, nextTile);
        if (distToNext <= cornerWindow && !IsTileAtWorldPosition(nextTile + (Vector3)queuedDirection))
        {
            Corner(nextTile, queuedDirection);
            return;
        }

        float distFromLast = Vector3.Distance(transform.position, lastIntersection);
        if (distFromLast <= cornerWindow && !IsTileAtWorldPosition(lastIntersection + (Vector3)queuedDirection))
        {
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

    Vector3 WrapToOppositeSide(Vector3 pos)
    {
        Vector3Int cell = wallTilemap.WorldToCell(pos);
        int oppositeX = direction.x > 0
            ? wallTilemap.cellBounds.xMin
            : wallTilemap.cellBounds.xMax - 1;

        Vector3 wrapped = wallTilemap.GetCellCenterWorld(new Vector3Int(oppositeX, cell.y, cell.z));
        return new Vector3(wrapped.x, pos.y, pos.z);
    }

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