using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudioScript : MonoBehaviour
{
    private AudioSource audioSource;
    private VertexPaintTool paintTool;
    public StateManager stateManager;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        paintTool = GetComponent<VertexPaintTool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paintTool.isPainted || stateManager.stateNumber == "6")
        {
            StopAudioLoop();
        }    
        else
        {
            PlayAudioLoop();
        }
    }

    void PlayAudioLoop()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopAudioLoop()
    {
        audioSource.Stop();
    }
}
