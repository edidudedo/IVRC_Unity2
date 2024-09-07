using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStateManager : MonoBehaviour
{
    // Reference to the lightAnimation script
    private LightAnimation lightAnimation;

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
            lightAnimation = GetComponent<LightAnimation>();

            // Check if the stateManager and lightAnimation components are found
            if (stateManager != null && lightAnimation != null)
            {
                // Check the stateNumber and enable/disable lightAnimation accordingly
                UpdateLightAnimationState();
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
        // Optionally, you can update the light animation in every frame if needed
        UpdateLightAnimationState();
    }

    void UpdateLightAnimationState()
    {
        if (stateManager.stateNumber == "2" || stateManager.stateNumber == "3" || stateManager.stateNumber == "4" || stateManager.stateNumber == "5")
        {
            lightAnimation.enabled = true;
        }
        else if (stateManager.stateNumber == "1" || stateManager.stateNumber == "6")
        {
            lightAnimation.enabled = false;
        }
    }
}
