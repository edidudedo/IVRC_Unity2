using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerStateScript : MonoBehaviour
{
    public AudioSource audioSource;

    // Reference to the StateManager
    private StateManager stateManager;

    void Start()
    {
        // Find the StateManager object
        GameObject stateManagerObject = GameObject.Find("StateManager");

        if (stateManagerObject != null)
        {
            // Get the StateManager component
            stateManager = stateManagerObject.GetComponent<StateManager>();

            // Get the LightAnimation script from this GameObject (assuming it's attached to the same object)
            audioSource = GetComponent<AudioSource>();

            // Check if the stateManager and lightAnimation components are found
            if (stateManager != null && audioSource != null)
            {
                // Check the stateNumber and enable/disable lightAnimation accordingly
                UpdateSoundAnimationState();
            }
            else
            {
                Debug.LogError("StateManager or LightAnimation component not found!");
            }
        }
        else
        {
            Debug.LogError("StateManager GameObject not found!");
        }
    }

    void Update()
    {
        UpdateSoundAnimationState();
    }

    void UpdateSoundAnimationState()
    {
        Debug.Log("StateNumber : " + stateManager.stateNumber);
        if (stateManager.stateNumber == "2" || stateManager.stateNumber == "3" || stateManager.stateNumber == "4" || stateManager.stateNumber == "5")
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (stateManager.stateNumber == "0" || stateManager.stateNumber == "1" || stateManager.stateNumber == "6")
        {
            audioSource.Stop();
        }
    }


}
