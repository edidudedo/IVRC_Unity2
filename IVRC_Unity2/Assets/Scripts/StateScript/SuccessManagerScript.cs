using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessManagerScript : MonoBehaviour
{
    public AudioSource audioSource;


    void Start()
    {
        audioSource.volume = 0.2f;
    }

    void Update()
    {
     
    }

    public void PlaySoundOnce()
    {
        if (!audioSource.isPlaying) // Check if the AudioSource is not currently playing
        {
            audioSource.Play(); // Play the sound
        }
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
}
