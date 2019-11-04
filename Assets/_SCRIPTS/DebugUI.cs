using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public static DebugUI Instance;

    [SerializeField] GameObject hmdToAttachTo;
    [SerializeField] TextMeshPro text;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        transform.parent = hmdToAttachTo.transform;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
