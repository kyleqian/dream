using UnityEngine;

public class Bone : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //void Update()
    //{
    //    Vector3 eulerAngles = transform.eulerAngles;
    //    eulerAngles.x = 0;
    //    eulerAngles.z = 0;
    //    transform.eulerAngles = eulerAngles;
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Mallet")
        {
            return;
        }
        //Debug.Log("COLLIDING WITH: " + other.name);
        audioSource.PlayOneShot(audioClip);
    }
}
