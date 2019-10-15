using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float degreesPerSecond;
    [SerializeField] bool reverseDirection;

    void Update()
    {
        float degrees = Time.deltaTime * degreesPerSecond;
        if (reverseDirection)
        {
            degrees *= -1;
        }
        transform.Rotate(Vector3.up, degrees);
    }
}
