using System.Collections;
using UnityEngine;
using Valve.VR;

namespace Dream
{
    [RequireComponent(typeof(ChuckSubInstance))]
    public class SelfViolin : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] Transform bowHand;
        [SerializeField] Transform pitchHand;
        [SerializeField] StringPosition pos1;
        [SerializeField] StringPosition pos2;
        [SerializeField] StringPosition pos3;
        [SerializeField] StringPosition pos4;
        [SerializeField] bool wow;

        [SerializeField] SteamVR_Action_Single bowSqueezeAction;
        [SerializeField] SteamVR_Action_Boolean bowGripAction;
        [SerializeField] SteamVR_Action_Single pitchSqueezeAction;

        const int SYNC_FREQUENCY_FRAMES = 5;

        readonly int[] OPEN_STRINGS = { 0, 4, 8, 12 };

        ChuckIntSyncer syncBowOn;
        ChuckIntSyncer syncFingering;
        ChuckFloatSyncer syncVibrato;
        int bowOn = 0;
        int fingering = 0;
        float vibrato = 0;

        // Bowing
        const float vibratoIntensity = 5;
        const float vibratoMaxMidi = 0.2f;
        const float vibratoMinMidi = -0.2f;
        ChuckSubInstance chuck;
        ChuckFloatSyncer syncBowIntensity;
        ChuckFloatSyncer syncPitch;
        Vector3 bowPrevPos;
        float integrator = 0f;
        float leakFactor = 0.85f;
        bool vibratoActive = false;
        Vector3 vibratoInitialPosition;
        bool prevPizz = false;

        void Start()
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
            float originalVolume = AudioListener.volume;
            AudioListener.volume = 0;
            chuck.RunFile("bowing.ck");
            StartCoroutine(Initialize(originalVolume));
            //ChuckSync();
        }

        IEnumerator Initialize(float originalVolume)
        {
            yield return new WaitForSeconds(5);
            AudioListener.volume = originalVolume;
        }

        void Update()
        {
            // Clean up
            float pitch = 0;

            fingering = OPEN_STRINGS[1];

            if (pos4.IsOn)
            {
                pitch = 7;
                fingering += 4;
                vibrato = pos4.Vibrato;
            }
            else if (pos3.IsOn)
            {
                pitch = 5;
                fingering += 3;
                vibrato = pos3.Vibrato;
            }
            else if (pos2.IsOn)
            {
                pitch = 4;
                fingering += 2;
                vibrato = pos2.Vibrato;
            }
            else if (pos1.IsOn)
            {
                pitch = 2;
                fingering++;
                vibrato = pos1.Vibrato;
            }
            else
            {
                pitch = 0;
                vibrato = 0;
            }

            //if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f)
            //{
            //    bowOn = 1;
            //}
            //else
            //{
            //    bowOn = 0;
            //}

            if (bowGripAction.active)
            {
                print("HIHIHIHIHIHI@@@@@@@@");
            }

            if (Time.frameCount % SYNC_FREQUENCY_FRAMES == 0)
            {
                ChuckSync();
            }

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f)
            {
                if (!vibratoActive)
                {
                    vibratoInitialPosition = pitchHand.position;
                    vibratoActive = true;
                }
                vibrato = vibratoInitialPosition.y - pitchHand.position.y;
            }
            else
            {
                if (vibratoActive)
                {
                    vibratoActive = false;
                    vibrato = 0;
                }
            }

            // TODO: Make smoother, more like actual vibrato.
            vibrato = Mathf.Clamp(vibrato * vibratoIntensity, vibratoMinMidi, vibratoMaxMidi);

            // Bowing/pizz
            bool pizz = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) >= 0.1f;

            Vector3 bowCurrPos = pizz ? bowPrevPos : bowHand.position;
            Vector3 delta = bowCurrPos - bowPrevPos;

            // put magnitude into leaky integrator
            if ((prevPizz && !pizz) || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= 0.1f)
            {
                integrator += (delta.magnitude * 5);
            }
            // leak!
            integrator *= leakFactor;

            //Debug.Log("pos:" + bowCurrPos + " prev:" + bowPrevPos + " diff:" + delta + " sum:" + integrator);
            print("Pitch: " + pitch.ToString("F2"));

            // copy into prev
            bowPrevPos = bowCurrPos;

            // send float to chuck
            chuck.SetFloat("bowIntensity", integrator);
            chuck.SetFloat("thePitch", wow ? 24 + 36 * bowCurrPos.y : 69 + pitch + vibrato);

            prevPizz = pizz;
        }

        void ChuckSync()
        {
            //syncBowOn.SetNewValue(bowOn);
            //syncFingering.SetNewValue(fingering);
            //syncVibrato.SetNewValue(vibrato);
        }
    }
}
