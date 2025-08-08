using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public AudioClip[] footStepSounds;
    AudioSource audioSource;
    int currentFootstepSound = 0;
    public float footStepInterval = 0.7f;
    float footStepTimer = 0;
    [Range(0, 1)] public float footStepVolume = 0.5f;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = footStepVolume;
    }

    public void PlayFootStepSound()
    {
        if (audioSource == null)
        {
            Debug.Log("AudioSource component is null");
            return;
        }

        int num = Random.Range(0, footStepSounds.Length);
        AudioClip sound = footStepSounds[num];
        if (sound == null)
        {
            Debug.Log("Unable to play footstep sound at index: " + num);
            return;
        }
        
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void UpdateFootsteps()
    {
        footStepTimer += Time.fixedDeltaTime;

        if (footStepTimer >= footStepInterval)
        {
            PlayFootStepSound();
            footStepTimer = 0;
        }
    }
}
