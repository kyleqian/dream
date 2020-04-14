using UnityEngine;
using Valve.VR;

public class SkeletonManager : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform clickingController;
    [SerializeField] AudioClip[] clips;
    [SerializeField] GameObject bonePrefab;

    bool clicked;
    Vector3 firstClick;
    Vector3 secondClick;

    void Update()
    {
        if (SteamVR_Actions._default.GrabGrip.GetState(SteamVR_Input_Sources.RightHand))
        {
            firstClick = Vector3.zero;
            secondClick = Vector3.zero;
            return;
        }

        if (clicked)
        {
            if (SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) <= 0.5f)
            {
                clicked = false;
            }
            return;
        }
        else
        {
            clicked = SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) >= 0.9f;
        }

        if (!clicked)
        {
            return;
        }

        if (firstClick == Vector3.zero)
        {
            firstClick = clickingController.position - head.position;
            Debug.Log("First click!!!");
        }
        else if (secondClick == Vector3.zero)
        {
            secondClick = clickingController.position - head.position;
            Debug.Log("Second click!!!");
            SetPositions();
        }
    }

    void SetPositions()
    {
        float maxHeight = firstClick.y;
        float minHeight = secondClick.y;
        float totalHeight = maxHeight - minHeight;
        float eachBoneHeight = totalHeight / 4;
        float forwardDisplace = (new Vector2(secondClick.x, secondClick.z)).magnitude;

        for (int i = 0; i < clips.Length; ++i)
        {
            GameObject newBone = Instantiate(bonePrefab, head, false);
            newBone.GetComponent<Bone>().audioClip = clips[i];
            newBone.transform.localScale = new Vector3(1, eachBoneHeight, 1);

            if (i < 4)
            {
                newBone.transform.Translate(new Vector3(-0.5f, minHeight + (0.5f * eachBoneHeight) + (i * eachBoneHeight), /*-forwardDisplace*/ -0.5f * newBone.transform.localScale.z), newBone.transform.parent);
            }
            else
            {
                newBone.transform.Translate(new Vector3(0.5f, minHeight + (0.5f * eachBoneHeight) + ((i - 4) * eachBoneHeight), /*-forwardDisplace*/ -0.5f * newBone.transform.localScale.z), newBone.transform.parent);
            }
        }
    }
}
