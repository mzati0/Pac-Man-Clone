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

    public Vector2 direction = Vector2.zero;
    private Vector2 queuedDirection = Vector2.zero;
    [SerializeField] private Vector3 nextTile;
    private Vector3 lastIntersection;

    private InputSystem_Actions controls;

    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();
    void OnDestroy() => controls.Dispose();

    void Start()
    {
        nextTile = transform.position;
        lastIntersection = transform.position;

        if (wallTilemap != null)
            wallTilemap.CompressBounds();
    }

    public bool IsTileAtWorldPosition(Vector2 worldPos)
    {
        if (wallTilemap == null) return false;
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPos);
        return wallTilemap.HasTile(cellPosition);
    }

    void Update()
    {
        ReadQueuedDirection();

        if (transform.position == nextTile)
        {
            lastIntersection = transform.position;

            if (queuedDirection != Vector2.zero && !IsTileAtWorldPosition(transform.position + (Vector3)queuedDirection))
            {
                direction = queuedDirection;
            }
            else if (direction != Vector2.zero && IsTileAtWorldPosition(transform.position + (Vector3)direction))
            {
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