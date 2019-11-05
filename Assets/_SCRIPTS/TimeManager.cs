using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public float TimeScale { get { return Time.timeScale; } }
    public float TimeScaleL { get; private set; }
    public float TimeScaleR { get; private set; }

    const float MIN_VELOCITY = 0.1f;

    [SerializeField] GameObject head;
    [SerializeField] GameObject controllerL;
    [SerializeField] GameObject controllerR;

    [Header("Velocity Control")]
    [SerializeField] float velocityMultiplier;
    [SerializeField] float headVelocityWeight;
    [SerializeField] float controllerLVelocityWeight;
    [SerializeField] float controllerRVelocityWeight;

    Vector3 prevHead;
    Vector3 prevControllerL;
    Vector3 prevControllerR;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdatePrevPositions();
    }

    void Update()
    {
        TimeScaleL = Time.unscaledDeltaTime * velocityMultiplier * ((prevControllerL - controllerL.transform.position).magnitude);
        TimeScaleR = Time.unscaledDeltaTime * velocityMultiplier * ((prevControllerR - controllerR.transform.position).magnitude);

        TimeScaleL = Mathf.Clamp(TimeScaleL, 0, 100);
        if (TimeScaleL < MIN_VELOCITY)
        {
            TimeScaleL = 0;
        }

        TimeScaleR = Mathf.Clamp(TimeScaleR, 0, 100);
        if (TimeScaleR < MIN_VELOCITY)
        {
            TimeScaleR = 0;
        }

        float baseVelocity = 0;
        baseVelocity += headVelocityWeight * ((prevHead - head.transform.position).magnitude);
        baseVelocity += controllerLVelocityWeight * ((prevControllerL - controllerL.transform.position).magnitude);
        baseVelocity += controllerRVelocityWeight * ((prevControllerR - controllerR.transform.position).magnitude);

        float newTimeScale = Time.unscaledDeltaTime * velocityMultiplier * baseVelocity;
        newTimeScale = Mathf.Clamp(newTimeScale, 0, 100);
        if (newTimeScale < MIN_VELOCITY)
        {
            newTimeScale = 0;
        }
        Time.timeScale = newTimeScale;

        UpdatePrevPositions();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void UpdatePrevPositions()
    {
        prevHead = head.transform.position;
        prevControllerL = controllerL.transform.position;
        prevControllerR = controllerR.transform.position;
    }
}
