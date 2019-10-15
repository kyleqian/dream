using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public float TimeScale { get { return Time.timeScale; } }

    const float MIN_VELOCITY = 0.01f;

    [SerializeField] GameObject head;
    [SerializeField] GameObject controllerL;
    [SerializeField] GameObject controllerR;

    [Header("Velocity Control")]
    [SerializeField] float maxBaseVelocity;
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
        float baseVelocity = 0;
        baseVelocity += headVelocityWeight * ((prevHead - head.transform.position).magnitude);
        baseVelocity += controllerLVelocityWeight * ((prevControllerL - controllerL.transform.position).magnitude);
        baseVelocity += controllerRVelocityWeight * ((prevControllerR - controllerR.transform.position).magnitude);
        //print(delta);

        float newTimeScale = velocityMultiplier * (baseVelocity / maxBaseVelocity);
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
