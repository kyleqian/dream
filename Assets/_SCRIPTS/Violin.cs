using UnityEngine;

namespace Dream
{
    [RequireComponent(typeof(ChuckSubInstance))]
    public class Violin : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] Transform bowController;

        const int SYNC_FREQUENCY_FRAMES = 5;
        const float MIN_BOW_VELOCITY = 0.2f;
        readonly Vector2 G_STRING_VEC = new Vector2(Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4));
        readonly Vector2 D_STRING_VEC = new Vector2(Mathf.Cos(Mathf.PI / 12), Mathf.Sin(Mathf.PI / 12));
        readonly Vector2 A_STRING_VEC = new Vector2(-Mathf.Cos(Mathf.PI / 12), Mathf.Sin(Mathf.PI / 12));
        readonly Vector2 E_STRING_VEC = new Vector2(-Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4));

        ChuckIntSyncer syncBowOn;
        ChuckIntSyncer syncNote;
        Vector3 bowPrevPosition;
        int bowOn = 0;
        int note = 2;
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
            syncNote = gameObject.AddComponent<ChuckIntSyncer>();
            syncBowOn.SyncInt(chuck, "bowOn");
            syncNote.SyncInt(chuck, "note");
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

            note = 0;
            float minAngle = G_Angle;
            if (D_Angle < minAngle)
            {
                note = 1;
                minAngle = D_Angle;
            }
            if (A_Angle < minAngle)
            {
                note = 2;
                minAngle = A_Angle;
            }
            if (E_Angle < minAngle)
            {
                note = 3;
                minAngle = E_Angle;
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
            syncNote.SetNewValue(note);
        }
    }
}
