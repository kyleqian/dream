using UnityEngine;

namespace Dream
{
    [RequireComponent(typeof(ChuckSubInstance))]
    public class SelfViolin : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] Transform bow;
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

        // Bowing
        ChuckSubInstance chuck;
        ChuckFloatSyncer syncBowIntensity;
        ChuckFloatSyncer syncPitch;
        Vector3 bowPrevPos;
        float integrator = 0f;
        float leakFactor = 0.85f;

        void Start()
        {
            Invoke("Initialize", 1.0f);
        }

        void Initialize()
        {
            //ChuckSubInstance chuck = GetComponent<ChuckSubInstance>();
            //chuck.RunFile("violin.ck");
            //syncBowOn = gameObject.AddComponent<ChuckIntSyncer>();
            //syncFingering = gameObject.AddComponent<ChuckIntSyncer>();
            //syncVibrato = gameObject.AddComponent<ChuckFloatSyncer>();
            //syncBowOn.SyncInt(chuck, "bowOn");
            //syncFingering.SyncInt(chuck, "fingering");
            //syncVibrato.SyncFloat(chuck, "vibrato");

            chuck = GetComponent<ChuckSubInstance>();
            chuck.RunFile("bowing.ck");
            //ChuckSync();
            initialized = true;
        }

        void Update()
        {
            if (!initialized)
            {
                return;
            }

            // Clean up
            float pitch = 0;

            fingering = OPEN_STRINGS[1];

            if (pos4.IsOn)
            {
                pitch = 1 + (pos1.transform.position.y - pos4.transform.position.y);
                fingering += 4;
                vibrato = pos4.Vibrato;
            }
            else if (pos3.IsOn)
            {
                pitch = 1 + (pos1.transform.position.y - pos3.transform.position.y);
                fingering += 3;
                vibrato = pos3.Vibrato;
            }
            else if (pos2.IsOn)
            {
                pitch = 1 + (pos1.transform.position.y - pos2.transform.position.y);
                fingering += 2;
                vibrato = pos2.Vibrato;
            }
            else if (pos1.IsOn)
            {
                pitch = 1;
                fingering++;
                vibrato = pos1.Vibrato;
            }
            else
            {
                pitch = 0.8f;
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

            // Bowing
            Vector3 bowCurrPos = bow.position;
            Vector3 delta = bowCurrPos - bowPrevPos;

            // put magnitude into leaky integrator
            integrator += (delta.magnitude * 5);
            // leak!
            integrator *= leakFactor;

            //Debug.Log("pos:" + bowCurrPos + " prev:" + bowPrevPos + " diff:" + delta + " sum:" + integrator);
            print("Pitch: " + pitch.ToString("F2"));

            // copy into prev
            bowPrevPos = bowCurrPos;

            // send float to chuck
            chuck.SetFloat("bowIntensity", integrator);
            chuck.SetFloat("thePitch", 24 + 36 * pitch);
        }

        void ChuckSync()
        {
            //syncBowOn.SetNewValue(bowOn);
            //syncFingering.SetNewValue(fingering);
            //syncVibrato.SetNewValue(vibrato);
        }
    }
}
