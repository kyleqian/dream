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
        int bowOn = 0;
        int fingering = 0;

        int leftFingering = 0;
        int rightFingering = 0;
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
            syncBowOn.SyncInt(chuck, "bowOn");
            syncFingering.SyncInt(chuck, "fingering");
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
            }
            else if (pos3.IsOn)
            {
                fingering += 3;
            }
            else if (pos2.IsOn)
            {
                fingering += 2;
            }
            else if (pos1.IsOn)
            {
                fingering++;
            }

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f)
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
        }
    }
}
