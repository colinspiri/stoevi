using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class TorbalanVision : MonoBehaviour {
    // components
    public static TorbalanVision Instance;
    
    // synced with behavior tree
    public bool PlayerWithinVision { get; set; }
    public float Awareness => torbalanAwareness.Value;

    // constants
    public bool blind;
    public Vector3 eyesOffset;
    [Header("Normal Vision")]
    public float normalVisionDistance;
    [Range(0, 360)] public float normalVisionAngle;
    [Header("Peripheral Vision")]
    public float peripheralVisionDistance;
    [Range(0, 360)] public float peripheralVisionAngle;
    [Header("Close Vision")]
    public float closeVisionDistance;
    [Range(0, 360)] public float closeVisionAngle;

    [Header("Awareness")] 
    public float baseAwarenessTime;
    public float awarenessDecayTime;
    [Space]
    public float closeThreshold;
    public float farThreshold;
    public float closeFactor;
    public float mediumFactor;
    public float farFactor;
    [Space] 
    public float sideThreshold;
    public float frontFactor;
    public float sideFactor;
    [Space] 
    public float sparseCoverFactor;
    [Space] 
    public float standingFactor;
    public float crouchedFactor;
    [Space] 
    public float stillFactor;
    public float crouchWalkingFactor;
    public float walkingFactor;
    public float runningFactor;
    
    // state
    public CoverSet allCover;
    public FloatVariable torbalanAwareness;
    private bool playerBehindSparseCover;
    private bool playerInNormalVision;
    private bool playerInPeripheralVision;
    private bool playerInCloseVision;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        torbalanAwareness.SetValue(0);
    }

    private void Update() {
        if (FirstPersonMovement.Instance == null) return;
        
        LookForPlayer();

        if (PlayerWithinVision) {
            IncreaseAwareness();
        }
        else {
            if (torbalanAwareness.Value > 0) torbalanAwareness.ApplyChange(-Time.deltaTime / awarenessDecayTime);
            else torbalanAwareness.SetValue(0);
        }
    }

    private void IncreaseAwareness() {
        float multiplier = 1f;
        Vector3 eyesPosition = transform.position + eyesOffset;
        Vector3 targetPosition = FirstPersonMovement.Instance.GetRaycastTarget();
        
        // check distance
        float distance = Vector3.Distance(eyesPosition, targetPosition);
        if (distance < closeThreshold) multiplier *= closeFactor;
        else if (distance > farThreshold) multiplier *= farFactor;
        else multiplier *= mediumFactor;

        // check angle
        Vector3 directionToTarget = (targetPosition - eyesPosition).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > sideThreshold) multiplier *= sideFactor;
        else multiplier *= frontFactor;
        
        // check cover
        if (playerBehindSparseCover) {
            multiplier *= sparseCoverFactor;
        }
        // OR check player stance
        else {
            if (FirstPersonMovement.Instance.crouching) multiplier *= crouchedFactor;
            else multiplier *= standingFactor;
        }
        
        // check player motion
        switch (FirstPersonMovement.Instance.moveState) {
            case FirstPersonMovement.MoveState.Still:
                multiplier *= stillFactor;
                break;
            case FirstPersonMovement.MoveState.CrouchWalking:
                multiplier *= crouchWalkingFactor;
                break;
            case FirstPersonMovement.MoveState.Walking:
                multiplier *= walkingFactor;
                break;
            case FirstPersonMovement.MoveState.Running:
                multiplier *= runningFactor;
                break;
        }

        // calculate awareness time
        float awarenessTime = baseAwarenessTime * multiplier;
        
        // calculate speed
        float speed = 1 / awarenessTime;

        // increment awareness
        torbalanAwareness.ApplyChange(speed * Time.deltaTime);
        if (torbalanAwareness.Value > 1) torbalanAwareness.SetValue(1);
        
        //Debug.Log("awarenessTime = " + awarenessTime + " (" + multiplier + ")");
    }

    private void LookForPlayer() {
        playerBehindSparseCover = false;
        if (blind) {
            playerInNormalVision = false;
            playerInPeripheralVision = false;
            playerInCloseVision = false;
            return;
        }

        playerInNormalVision = CanSeePlayerInCone(normalVisionDistance, normalVisionAngle);
        playerInPeripheralVision = CanSeePlayerInCone(peripheralVisionDistance, peripheralVisionAngle);
        playerInCloseVision = CanSeePlayerInCone(closeVisionDistance, closeVisionAngle);

        PlayerWithinVision = playerInNormalVision || playerInPeripheralVision || playerInCloseVision;
    }

    public bool CanSeePoint(Vector3 point) {
        bool pointInNormalVision = CheckIfPointWithinCone(point, normalVisionDistance, normalVisionAngle);
        bool pointInPeripheralVision = CheckIfPointWithinCone(point, peripheralVisionDistance, peripheralVisionAngle);
        bool pointInCloseVision = CheckIfPointWithinCone(point, closeVisionDistance, closeVisionAngle);

        return pointInNormalVision || pointInPeripheralVision || pointInCloseVision;
    }

    private bool CanSeePlayerInCone(float distance, float angle) {
        // check if player in complete cover
        if (allCover.PlayerInCompleteCover()) {
            return false;
        }
        
        Vector3 targetPosition = FirstPersonMovement.Instance.GetRaycastTarget();
        
        // distance
        bool withinDistance = PointWithinDistance(targetPosition, distance);
        if (!withinDistance) return false;

        // view angle
        bool withinAngle = PointWithinAngle(targetPosition, angle);
        if (!withinAngle) return false;
        
        // cover
        bool behindCompleteCover = PointBehindCover(targetPosition);
        if (behindCompleteCover) return false;
        
        // check if player in sparse cover
        bool behindSparseCover = allCover.PlayerInSparseCover() || PointBehindCover(targetPosition, true);
        if (behindSparseCover) {
            playerBehindSparseCover = true;
            return true;
        }

        return true;
    }
    
    private bool CheckIfPointWithinCone(Vector3 point, float distance, float angle) {
        // distance
        bool withinDistance = PointWithinDistance(point, distance);
        if (!withinDistance) return false;

        // view angle
        bool withinAngle = PointWithinAngle(point, angle);
        if (!withinAngle) return false;
        
        // cover
        bool behindCover = PointBehindCover(point);
        if (behindCover) return false;

        return true;
    }

    private bool PointWithinDistance(Vector3 point, float distance) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        float distanceToTarget = Vector3.Distance(eyesPosition, point);

        return distanceToTarget < distance;
    }

    private bool PointWithinAngle(Vector3 point, float angle) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        Vector3 directionToTarget = (point - eyesPosition).normalized;
        
        return Vector3.Angle(transform.forward, directionToTarget) < angle / 2;
    }

    private bool PointBehindCover(Vector3 point, bool countSparseCover = false) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        Vector3 directionToTarget = (point - eyesPosition).normalized;
        float distanceToTarget = Vector3.Distance(eyesPosition, point);

        RaycastHit[] hits = Physics.RaycastAll(eyesPosition, directionToTarget, distanceToTarget,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        
        foreach (var hit in hits) {
            Cover cover = hit.transform.GetComponent<Cover>();
            if (cover != null) {
                if (cover.type == Cover.CoverType.Complete) {
                    return true;
                }

                if (cover.type == Cover.CoverType.Sparse && countSparseCover) {
                    return true;
                }
            }
        }

        // no cover found, point is not behind cover
        return false;
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void SetBlind(bool value) {
        blind = value;
    }

    private void OnDrawGizmos() {
        DrawVisionCone(normalVisionDistance, normalVisionAngle, Color.white);
        DrawVisionCone(peripheralVisionDistance, peripheralVisionAngle, Color.blue);
        DrawVisionCone(closeVisionDistance, closeVisionAngle, Color.magenta);

        // if seen, line to player
        Gizmos.color = Color.red;
        if (playerInNormalVision) {
            Vector3 eyesPosition = transform.position + eyesOffset;
            Gizmos.DrawLine(eyesPosition, FirstPersonMovement.Instance.GetRaycastTarget());
        }
    }

    private void DrawVisionCone(float distance, float angle, Color color) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        
        Vector3 viewAngleA = DirectionFromAngle(-angle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(angle / 2, false);
        
        Gizmos.color = color;
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleA * distance);
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleB * distance);
    }
}
