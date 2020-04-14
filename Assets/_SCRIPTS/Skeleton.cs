using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] AudioClip hitSound;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(hitSound);
    }
}
