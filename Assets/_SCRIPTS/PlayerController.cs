using UnityEngine;

namespace Dream
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] ChuckSubInstance chuck;

        const int SYNC_FREQUENCY_FRAMES = 1;
        ChuckFloatSyncer timeScaleSyncer;

        void Start()
        {
            //chuck.RunFile("test-time.ck");
            chuck.RunFile("mand-o-matic-time.ck");
            timeScaleSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
            timeScaleSyncer.SyncFloat(chuck, "timeScale");
            SyncTimeScale();
        }

        void Update()
        {
            if (Time.frameCount % SYNC_FREQUENCY_FRAMES == 0)
            {
                SyncTimeScale();
            }
        }

        void SyncTimeScale()
        {
            //print(TimeManager.Instance.TimeScale.ToString());
            timeScaleSyncer.SetNewValue(TimeManager.Instance.TimeScale);
        }
    }
}
