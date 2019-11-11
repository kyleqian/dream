using UnityEngine;

public class StringPosition : MonoBehaviour
{
    public bool IsOn { get; private set; } = false;
    public float Vibrato { get; private set; } = 0;

    const float VIBRATO_MULTIPLIER = 5;
    const float NEARBY_FINGER_STOP_THRESHOLD = 0.01f;

    Transform nearbyFinger;
    float nearbyFingerPrevSpeed;
    Vector3 nearbyFingerPrevPosition;
    Vector3 activeFingerInitialPosition;

    void OnTriggerEnter(Collider finger)
    {
        nearbyFinger = finger.transform;
        nearbyFingerPrevPosition = nearbyFinger.position;
    }

    void OnTriggerExit(Collider finger)
    {
        nearbyFinger = null;
        IsOn = false;
    }

    void Update()
    {
        if (nearbyFinger == null)
        {
            return;
        }

        if (IsOn)
        {
            Vibrato = VIBRATO_MULTIPLIER * (activeFingerInitialPosition.y - nearbyFinger.position.y);
        }

        if (!IsOn && (Vector3.Distance(nearbyFinger.position, nearbyFingerPrevPosition)) < NEARBY_FINGER_STOP_THRESHOLD)
        {
            activeFingerInitialPosition = nearbyFinger.position;
            IsOn = true;
        }

        nearbyFingerPrevPosition = nearbyFinger.position;
    }
}
