using UnityEngine;
using UnityEngine.Assertions;

public class StringPosition : MonoBehaviour
{
    public bool IsOn { get; private set; } = false;

    int numTriggerEntrances = 0;

    void OnTriggerEnter(Collider finger)
    {
        numTriggerEntrances++;
        IsOn = true;
    }

    void OnTriggerExit(Collider finger)
    {
        if (--numTriggerEntrances == 0)
        {
            IsOn = false;
        }
        Assert.IsTrue(numTriggerEntrances >= 0);
    }
}
