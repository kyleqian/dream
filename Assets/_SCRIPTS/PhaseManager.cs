using UnityEngine;

namespace Dream
{
    public class PhaseManager : MonoBehaviour
    {
        public static PhaseManager Instance;

        // Positive means L is ahead. Negative means R is ahead.
        public float PhaseDifference { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            PhaseDifference += (TimeManager.Instance.TimeScaleL - TimeManager.Instance.TimeScaleR);
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
