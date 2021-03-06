﻿using UnityEngine;

namespace Dream
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] ChuckSubInstance chuck;

        const int SYNC_FREQUENCY_FRAMES = 5;
        ChuckFloatSyncer timeScaleSyncerL;
        ChuckFloatSyncer timeScaleSyncerR;
        bool initialized = false;

        void Start()
        {
            Invoke("Initialize", 2.0f);
            OVRInput.SetControllerVibration(1.0f, 1.0f);
        }

        void Initialize()
        {
            initialized = true;
            //chuck.RunFile("test-time.ck");
            //chuck.RunFile("mand-o-matic-time.ck");
            //chuck.RunFile("thx-time.ck");
            chuck.RunFile("canon.ck");
            timeScaleSyncerL = gameObject.AddComponent<ChuckFloatSyncer>();
            timeScaleSyncerR = gameObject.AddComponent<ChuckFloatSyncer>();
            timeScaleSyncerL.SyncFloat(chuck, "timeScaleL");
            timeScaleSyncerR.SyncFloat(chuck, "timeScaleR");
            SyncTimeScale();
        }

        void Update()
        {
            //DebugUI.Instance?.SetText(PhaseManager.Instance.PhaseDifference.ToString("F3"));
            DebugUI.Instance?.SetText(ProximityManager.Instance.Proximity10.ToString("F3"));
            //OVRInput.SetControllerVibration(ProximityManager.Instance.Proximity10, ProximityManager.Instance.Proximity10, OVRInput.Controller.All);

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
