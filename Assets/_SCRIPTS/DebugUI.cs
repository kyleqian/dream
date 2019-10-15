using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] GameObject hmdToAttachTo;
    [SerializeField] TextMeshPro text;

    void Start()
    {
        transform.parent = hmdToAttachTo.transform;
    }

    void Update()
    {
        text.text = TimeManager.Instance.TimeScale.ToString("F3");
    }
}
