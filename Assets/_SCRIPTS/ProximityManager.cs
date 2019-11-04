using UnityEngine;

public class ProximityManager : MonoBehaviour
{
    public static ProximityManager Instance;

    public float Proximity { get; private set; }
    public float Proximity01 { get; private set; }
    public float Proximity10 { get; private set; }

    [SerializeField] Transform object1;
    [SerializeField] Transform object2;

    const float MAX_DISTANCE = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        Proximity = Vector3.Distance(object1.position, object2.position);
        Proximity01 = Mathf.Clamp01(Proximity / MAX_DISTANCE);
        Proximity10 = 1.0f - Proximity01;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
