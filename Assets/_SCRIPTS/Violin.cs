using UnityEngine;

namespace Dream
{
    [RequireComponent(typeof(ChuckSubInstance))]
    public class Violin : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] Transform bowController;

        const int SYNC_FREQUENCY_FRAMES = 5;
        const float MIN_BOW_VELOCITY = 0.1f;
        readonly Vector2 G_STRING_VEC = new Vector2(Mathf.Cos(67.5f * Mathf.Deg2Rad), Mathf.Sin(67.5f * Mathf.Deg2Rad));
        readonly Vector2 D_STRING_VEC = new Vector2(Mathf.Cos(22.5f * Mathf.Deg2Rad), Mathf.Sin(22.5f * Mathf.Deg2Rad));
        readonly Vector2 A_STRING_VEC = new Vector2(-Mathf.Cos(22.5f * Mathf.Deg2Rad), Mathf.Sin(22.5f * Mathf.Deg2Rad));
        readonly Vector2 E_STRING_VEC = new Vector2(-Mathf.Cos(67.5f * Mathf.Deg2Rad), Mathf.Sin(67.5f * Mathf.Deg2Rad));

        readonly int[] OPEN_STRINGS = { 0, 4, 8, 12 };

        ChuckIntSyncer syncBowOn;
        ChuckIntSyncer syncFingering;
        Vector3 bowPrevPosition;
        int bowOn = 0;
        int fingering = 0;
        bool initialized = false;

        void Start()
        {
            bowPrevPosition = bowController.position;
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

            Vector3 bowMvtVector = bowController.position - bowPrevPosition;
            float bowVelocity = Time.deltaTime == 0 ? 0 : Mathf.Abs(bowMvtVector.magnitude) / Time.deltaTime;

            // Determine note.
            Vector3 projection = Vector3.ProjectOnPlane(bowMvtVector, head.forward).normalized;
            if (projection.y < 0)
            {
                projection *= -1;
            }
            projection = head.InverseTransformDirection(projection);

            float G_Angle = Vector2.Angle(projection, G_STRING_VEC);
            float D_Angle = Vector2.Angle(projection, D_STRING_VEC);
            float A_Angle = Vector2.Angle(projection, A_STRING_VEC);
            float E_Angle = Vector2.Angle(projection, E_STRING_VEC);

            fingering = OPEN_STRINGS[0];
            float minAngle = G_Angle;
            if (D_Angle < minAngle)
            {
                fingering = OPEN_STRINGS[1];
                minAngle = D_Angle;
            }
            if (A_Angle < minAngle)
            {
                fingering = OPEN_STRINGS[2];
                minAngle = A_Angle;
            }
            if (E_Angle < minAngle)
            {
                fingering = OPEN_STRINGS[3];
                minAngle = E_Angle;
            }

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.3f)
            {
                fingering += 4;
            }
            else if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) > 0.3f)
            {
                fingering += 3;
            }
            else if (OVRInput.Get(OVRInput.Button.Four, OVRInput.Controller.Touch))
            {
                fingering += 2;
            }
            else if (OVRInput.Get(OVRInput.Button.Three, OVRInput.Controller.Touch))
            {
                fingering++;
            }

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= 0.9f && bowVelocity >= MIN_BOW_VELOCITY)
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

            bowPrevPosition = bowController.position;
        }

        void ChuckSync()
        {
            syncBowOn.SetNewValue(bowOn);
            syncFingering.SetNewValue(fingering);
        }
    }
}
