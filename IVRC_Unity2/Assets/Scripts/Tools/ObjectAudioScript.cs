using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudioScript : MonoBehaviour
{
    private AudioSource audioSource;
    private VertexPaintTool paintTool;
    public StateManager stateManager;
    public bool isPlaying;
    public SuccessManagerScript successManager;
    private bool _isSuccessPlayed = false;
    private float audioPlayDuration = 15f; // Total duration for playback
    private float fadeOutDuration = 15f; // Duration to fade to 0.2
    private bool isSuccessPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        paintTool = GetComponent<VertexPaintTool>();
        audioSource.volume = 1.0f; // Start with volume at 0.5
        successManager.StopSound();
    }

    void Update()
    {
        Debug.Log(_isSuccessPlayed);
        if (!paintTool.isPainted || stateManager.stateNumber == "6")
        {
            StopAudioLoop();
        }
        else
        {
            if (!_isSuccessPlayed)
            {
                _isSuccessPlayed = true;
                successManager.PlaySoundOnce();
            }

            PlayAudioSource();

            // Start fading to 0.2 after 2 seconds
            if (audioSource.isPlaying && audioSource.volume > 0.0f)
            {
                StartFadeToLowerVolume();
            }
        }
    }

    void PlayAudioSource()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void StartFadeToLowerVolume()
    {
        StartCoroutine(FadeToLowerVolume());
    }

    System.Collections.IEnumerator FadeToLowerVolume()
    {
        float startVolume = audioSource.volume;
        float fadeTime = fadeOutDuration;

        while (fadeTime > 0)
        {
            fadeTime -= Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0.0f, startVolume, fadeTime / fadeOutDuration); 
            yield return null;
        }

        audioSource.volume = 0.0f;
    }

    void StopAudioLoop()
    {
        audioSource.Stop();
        audioSource.volume = 0.5f; // Reset volume to starting level
    }
}
