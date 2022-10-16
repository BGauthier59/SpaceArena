using UnityEngine;

public class NewConduit : MonoBehaviour
{
    public NewVent[] vents;
    public PlayerController player;

    public ConduitPoint currentPoint;
    public ConduitPoint nextPoint;
    private float distanceBetweenPoints;

    private bool isMoving;

    [Tooltip("Move duration for 1 meter distance")]
    public float moveDurationUnit;

    private float moveTimer;

    private bool isOnIntersection;
    [SerializeField] private float intersectionDuration;
    private float intersectionTimer;

    private void Start()
    {
        foreach (var vent in vents)
        {
            vent.Initialization(this);
        }
    }

    private void Update()
    {
        if (isMoving) Moving();
        if (isOnIntersection) SetIntersectionTimer();
    }

    public void EntersConduit(NewVent initVent, PlayerController pl)
    {
        player = pl;
        currentPoint = initVent.firstReachedPoint;
        player.currentConduit = this;
        if (currentPoint.conduitPointType == ConduitPoint.ConduitPointType.IntersectionPoint)
        {
            isOnIntersection = true;
        }
    }

    public void SetNextPoint(ConduitDirection direction)
    {
        ConduitPoint next = null;
        switch (direction)
        {
            case ConduitDirection.Up:
                next = currentPoint.upPoint;
                break;
            case ConduitDirection.Bottom:
                next = currentPoint.bottomPoint;
                break;
            case ConduitDirection.Left:
                next = currentPoint.leftPoint;
                break;
            case ConduitDirection.Right:
                next = currentPoint.rightPoint;
                break;
            default:
                Debug.LogError("Direction not available.");
                break;
        }

        if (next == null) return;

        player.detectInputConduit = false;
        nextPoint = next;
        distanceBetweenPoints = Vector3.Distance(currentPoint.pointPos.position, nextPoint.pointPos.position);
        isMoving = true;
    }

    public void Moving()
    {
        if (moveTimer > (moveDurationUnit * distanceBetweenPoints))
        {
            ReachPoint();
        }
        else
        {
            moveTimer += Time.deltaTime;
            player.transform.position = Vector3.Lerp(currentPoint.pointPos.position, nextPoint.pointPos.position,
                moveTimer / (moveDurationUnit * distanceBetweenPoints));
        }
    }

    public void SetIntersectionTimer()
    {
        if (intersectionTimer > intersectionDuration)
        {
            intersectionTimer = 0f;
            isOnIntersection = false;
            player.detectInputConduit = true;
        }
        else intersectionTimer += Time.deltaTime;
    }

    public void ReachPoint()
    {
        moveTimer = 0f;
        currentPoint = nextPoint;
        isMoving = false;
        player.transform.position = nextPoint.pointPos.position;

        switch (currentPoint.conduitPointType)
        {
            case ConduitPoint.ConduitPointType.VentPoint:
                // Sortie automatique du conduit depuis la vent
                currentPoint.linkedVent.ExitsVent(player);
                player.currentConduit = null;
                player = null;
                currentPoint = null;
                nextPoint = null;
                break;

            case ConduitPoint.ConduitPointType.IntersectionPoint:
                // Peut rebouger, ne fait rien ?
                isOnIntersection = true;
                break;

            default:
                Debug.LogError("Conduit Point Type is not valid.");
                break;
        }
    }


    public enum ConduitDirection
    {
        NA,
        Up,
        Bottom,
        Left,
        Right
    }
}