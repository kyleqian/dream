using UnityEngine;

namespace Dream
{
    [RequireComponent(typeof(ChuckSubInstance))]
    public class SelfViolin : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] StringPosition pos1;
        [SerializeField] StringPosition pos2;
        [SerializeField] StringPosition pos3;
        [SerializeField] StringPosition pos4;

        const int SYNC_FREQUENCY_FRAMES = 5;

        readonly int[] OPEN_STRINGS = { 0, 4, 8, 12 };

        ChuckIntSyncer syncBowOn;
        ChuckIntSyncer syncFingering;
        ChuckFloatSyncer syncVibrato;
        int bowOn = 0;
        int fingering = 0;
        float vibrato = 0;
        bool initialized = false;

        void Start()
        {
            Invoke("Initialize", 1.0f);
        }

        void Initialize()
        {
            ChuckSubInstance chuck = GetComponent<ChuckSubInstance>();
            chuck.RunFile("violin.ck");
            syncBowOn = gameObject.AddComponent<ChuckIntSyncer>();
            syncFingering = gameObject.AddComponent<ChuckIntSyncer>();
            syncVibrato = gameObject.AddComponent<ChuckFloatSyncer>();
            syncBowOn.SyncInt(chuck, "bowOn");
            syncFingering.SyncInt(chuck, "fingering");
            syncVibrato.SyncFloat(chuck, "vibrato");
            ChuckSync();
            initialized = true;
        }

        void Update()
        {
            if (!initialized)
            {
                return;
            }

            fingering = OPEN_STRINGS[1];

            if (pos4.IsOn)
            {
                fingering += 4;
                vibrato = pos4.Vibrato;
            }
            else if (pos3.IsOn)
            {
                fingering += 3;
                vibrato = pos3.Vibrato;
            }
            else if (pos2.IsOn)
            {
                fingering += 2;
                vibrato = pos2.Vibrato;
            }
            else if (pos1.IsOn)
            {
                fingering++;
                vibrato = pos1.Vibrato;
            }
            else
            {
                vibrato = 0;
            }

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f)
            {
                bowOn = 1;
            }
            else
            {
                bowOn = 0;
            }

            if (Time.frameCount % SYNC_FREQUENCY_FRAMES == 0)
            {
                ChuckSync();
            }
        }

        void ChuckSync()
        {
            syncBowOn.SetNewValue(bowOn);
            syncFingering.SetNewValue(fingering);
            syncVibrato.SetNewValue(vibrato);
        }
    }
}
