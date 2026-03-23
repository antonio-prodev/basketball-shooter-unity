using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BallController : MonoBehaviour
{
    private GameManager gameManager;
    private Transform trailFocalPoint;
    private Transform trail;
    [SerializeField] private float trailRadius = 0.5f;
    [SerializeField] private float trailRotationSpeed = 200f;
    private Rigidbody ballRb;
    [SerializeField] private float strengthMultiplier = 2f;
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float minDragDistance = 0.01f;
    private bool isAiming = false;
    private Vector3 dragStart;
    private Vector3 dragEnd;
    private Vector3 dragVector;
    [SerializeField] private float resetDelay = 8f;
    [SerializeField] private float spawnDelay = 2f;

    private LineRenderer aimLine;
    private LineRenderer arrowTip;
    [SerializeField] private float arrowTipLength = 0.2f;
    public float aimLineLengthMultiplier = 1f;
    public Slider powerIndicatorSlider;
    public Image powerFill;
    public Color lowPowerColor = Color.green;
    public Color highPowerColor = Color.red;


    [Header("Arc Prediction")]
    public LineRenderer trajectoryLine;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.05f;

    [Tooltip("Fallback radius if no SphereCollider is found.")]
    public float trajectoryRadius = 0.001f;

    [Tooltip("Layers the prediction should collide with.")]
    public LayerMask trajectoryCollisionMask = ~0;
    private SphereCollider sphereCollider;
    public float maxTrajectoryPreviewDistance = 3f;
    public bool useArrow = false;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ballRb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        trailFocalPoint = transform.Find("TrailFocalPoint");
        if (trailFocalPoint != null)
        {
            trail = trailFocalPoint.GetChild(0);

            trailFocalPoint.position = transform.position;
            trail.localPosition = new Vector3(0,0,1) * trailRadius;
        }

        if (aimLine == null && useArrow)
        {
            aimLine = GetComponent<LineRenderer>();
            if (arrowTip == null)
            {
                arrowTip = transform.Find("ArrowTip").GetComponent<LineRenderer>();
            }
        }

        if (trajectoryLine == null && !useArrow)
        {
            trajectoryLine = transform.Find("TrajectoryLine").GetComponent<LineRenderer>();

            if (trajectoryLine != null)
            {
                trajectoryLine.positionCount = trajectoryPoints;
                trajectoryLine.enabled = false;
            }
        }

        ballRb.isKinematic = true; // Start as kinematic to prevent movement until launched

        if (aimLine != null)
        {
            aimLine.positionCount = 3;
            aimLine.enabled = false;
        }


        if (arrowTip != null)
        {
            arrowTip.positionCount = 2;
            arrowTip.enabled = false;
        }
    }

    void Update()
    {
        if (ballRb.isKinematic)
        {
            if (!trailFocalPoint.gameObject.activeInHierarchy)
            {
                trailFocalPoint.gameObject.SetActive(true);
            }
            trailFocalPoint.Rotate(Vector3.right, Time.deltaTime * trailRotationSpeed);
        }
        else
        {
            if (trailFocalPoint.gameObject.activeInHierarchy)
            {
                trailFocalPoint.gameObject.SetActive(false);
            }
        }
    }

    void OnMouseDown()
    {
        if (!gameManager.isGameActive || !ballRb.isKinematic) return;

        isAiming = true;
        dragStart = GetMouseWorldPosition();

        if (aimLine != null)
        {
            aimLine.enabled = true;
        }
        if (arrowTip != null)
        {
            arrowTip.enabled = true;
        }
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = true;
        }


    }

    void OnMouseDrag()
    {
        if (!gameManager.isGameActive || !isAiming || !ballRb.isKinematic) return;

        dragEnd = GetMouseWorldPosition();
        dragEnd.x = transform.position.x;

        dragVector = dragStart - dragEnd;
        dragVector = Vector3.ClampMagnitude(dragVector, maxDragDistance);
        if (useArrow)
        {
            UpdateAimLine();
        }
        else
        {
            UpdateTrajectoryPrediction();
        }

        UpdatePowerIndicator();
    }

    void OnMouseUp()
    {
        if (!gameManager.isGameActive || !isAiming || !ballRb.isKinematic) return;
        if (dragVector.magnitude <= minDragDistance) return;


        if (aimLine != null)
        {
            aimLine.enabled = false;
        }

        if (arrowTip != null)
        {
            arrowTip.enabled = false;
        }

        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }

        LaunchBall(dragVector * strengthMultiplier);
        isAiming = false;

        powerIndicatorSlider.value = 0;
        dragVector = Vector3.zero;
        
        if (useArrow)
        {
            UpdateAimLine();
        }
    }

    void UpdatePowerIndicator()
    {
        powerIndicatorSlider.value = Mathf.Clamp01(dragVector.magnitude / maxDragDistance);
        powerFill.color = Color.Lerp(lowPowerColor, highPowerColor, powerIndicatorSlider.value);
    }

    void UpdateAimLine()
    {
        if (aimLine == null || arrowTip == null) return;

        Vector3 aimArrow = dragVector * aimLineLengthMultiplier;

        Vector3 startPoint = transform.position;
        aimLine.SetPosition(0, startPoint);

        for (int i = 1; i < aimLine.positionCount; i++)
        {
            Vector3 point = aimLine.GetPosition(i - 1) + Vector3.ClampMagnitude(aimArrow, aimArrow.magnitude / (aimLine.positionCount - 1));
            aimLine.SetPosition(i, point);
        }

        Vector3 endPoint = aimLine.GetPosition(aimLine.positionCount - 1);
        Vector3 tipPoint = endPoint + (aimArrow.normalized * arrowTipLength);

        arrowTip.SetPosition(0, endPoint);
        arrowTip.SetPosition(1, tipPoint);
    }

    void UpdateTrajectoryPrediction()
    {
        if (trajectoryLine == null) return;

        // Same shot force you apply on release.
        Vector3 impulse = dragVector * strengthMultiplier;
        Vector3 velocity = impulse / ballRb.mass;

        Vector3 position = transform.position;

        // Use the real ball size if possible.
        float radius = trajectoryRadius;

        List<Vector3> points = new List<Vector3>(trajectoryPoints);
        points.Add(position);

        float damping = ballRb.linearDamping;
        float dt = trajectoryTimeStep;

        float traveledDistance = 0f;

        for (int i = 0; i < trajectoryPoints - 1; i++)
        {
            Vector3 nextVelocity = velocity;
            nextVelocity *= Mathf.Max(0f, 1f - damping * dt);
            nextVelocity += Physics.gravity * dt;

            Vector3 nextPosition = position + nextVelocity * dt;

            float stepDistance = Vector3.Distance(position, nextPosition);
            traveledDistance += stepDistance;


            Vector3 direction = nextPosition - position;
            float distance = direction.magnitude;

            if (distance > 0.0001f)
            {
                direction /= distance; // or direction.Normalize(); works as well

                if (Physics.SphereCast(
                    position,
                    radius,
                    direction,
                    out RaycastHit hit,
                    distance,
                    trajectoryCollisionMask,
                    QueryTriggerInteraction.Ignore))
                {
                    points.Add(hit.point);
                    break;
                }
            }

            points.Add(nextPosition);
            position = nextPosition;
            velocity = nextVelocity;

            if (traveledDistance >= maxTrajectoryPreviewDistance)
                break;
        }

        trajectoryLine.positionCount = points.Count;
        trajectoryLine.SetPositions(points.ToArray());
    }

    /* float GetTrajectoryRadius()
    {
        if (sphereCollider != null)
        {
            // Works best for a uniformly scaled ball.
            return sphereCollider.radius * Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y,
                transform.lossyScale.z);
        }

        return trajectoryRadius;
    } */


    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void LaunchBall(Vector3 force)
    {
        ballRb.useGravity = true;
        ballRb.isKinematic = false;
        GetComponent<SphereCollider>().isTrigger = false;
        ballRb.AddForce(force, ForceMode.Impulse);

        Invoke("SpawnNewball", spawnDelay);
        Invoke("deactivateBall", resetDelay); // Deactivate the ball after a short delay to allow for any final interactions
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            gameManager.AddScore(1);
        }
    }

    public void LinkBallToPowerIndicator()
    {
        if (powerIndicatorSlider == null)
        {
            powerIndicatorSlider = GameObject.FindGameObjectWithTag("Power Indicator").GetComponent<Slider>();
        }

        if (powerFill == null)
        {
            powerFill = GameObject.FindGameObjectWithTag("Power Fill").GetComponent<Image>();
        }
    }

    void deactivateBall()
    {
        gameObject.SetActive(false);
        ballRb.isKinematic = true; // Reset to kinematic for the next launch
    }

    void SpawnNewball()
    {
        gameManager.SpawnBall();
    }
}
