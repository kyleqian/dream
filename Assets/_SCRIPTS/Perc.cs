using UnityEngine;

namespace Dream
{
    public class Perc : MonoBehaviour
    {
        [SerializeField] ChuckSubInstance chuck;

        const int SYNC_FREQUENCY_FRAMES = 5;
        ChuckFloatSyncer timeScaleSyncerL;
        ChuckFloatSyncer timeScaleSyncerR;
        bool initialized = false;

        void Start()
        {
            Invoke("Initialize", 2.0f);
        }

        void Initialize()
        {
            initialized = true;
            chuck.RunFile("perc-time.ck");
            timeScaleSyncerL = gameObject.AddComponent<ChuckFloatSyncer>();
            timeScaleSyncerR = gameObject.AddComponent<ChuckFloatSyncer>();
            timeScaleSyncerL.SyncFloat(chuck, "timeScaleL");
            timeScaleSyncerR.SyncFloat(chuck, "timeScaleR");
            SyncTimeScale();
        }

        void Update()
        {
            DebugUI.Instance?.SetText(TimeManager.Instance.TimeScale.ToString("F3"));

            if (!initialized)
            {
                return;
            }

            if (Time.frameCount % SYNC_FREQUENCY_FRAMES == 0)
            {
                SyncTimeScale();
            }
        }

        void SyncTimeScale()
        {
            //print(TimeManager.Instance.TimeScale.ToString());
            timeScaleSyncerL.SetNewValue(TimeManager.Instance.TimeScaleL);
            timeScaleSyncerR.SetNewValue(TimeManager.Instance.TimeScaleR);
        }
    }
}
