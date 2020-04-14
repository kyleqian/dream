using UnityEngine;

public class Mallet : MonoBehaviour
{
    [SerializeField] GameObject soundingZone;

    const float SPEED_REDUCE_THRESHOLD_M_PER_SEC = 2;

    bool inSoundingZone;
    Vector3 prevPosition;
    float prevInstantaneousSpeed;

    void Update()
    {
        Vector3 currPosition = transform.position;
        float currInstantaneousSpeed = Mathf.Abs(Vector3.Distance(currPosition, prevPosition)) / Time.deltaTime;

        //Debug.Log(string.Format("DECREASE: {0}", prevInstantaneousSpeed - currInstantaneousSpeed));
        if (inSoundingZone && DetectStrike(currInstantaneousSpeed))
        {
            soundingZone.GetComponent<Skeleton>().PlaySound();
        }

        prevPosition = currPosition;
        prevInstantaneousSpeed = currInstantaneousSpeed;
    }

    void Start()
    {
        prevPosition = transform.position;
        prevInstantaneousSpeed = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetInstanceID() == soundingZone.GetComponent<Collider>().GetInstanceID())
        {
            inSoundingZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetInstanceID() == soundingZone.GetComponent<Collider>().GetInstanceID())
        {
            inSoundingZone = false;
        }
    }

    bool DetectStrike(float currInstantaneousSpeed)
    {
        if (prevInstantaneousSpeed == 0)
        {
            return false;
        }

        if (prevInstantaneousSpeed - currInstantaneousSpeed >= SPEED_REDUCE_THRESHOLD_M_PER_SEC)
        {
            return true;
        }

        return false;
    }
}
