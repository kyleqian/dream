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
            //chuck.RunCode(@"
            //    global float timeScale;

            //    SinOsc foo => dac;
            //    while( true )
            //    {
            //        if (timeScale > 0)
            //        {
            //            Math.random2f( 300, 1000 ) => foo.freq;
            //            100::ms / timeScale => now;
            //        }
            //        else
            //        {
            //            10::ms => now;
            //        }
            //    }
            //");

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
