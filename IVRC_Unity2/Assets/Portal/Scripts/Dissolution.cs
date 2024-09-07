using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolution : MonoBehaviour
{
    public Material mat;
    public Transform headTransform;
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Texture2D initialPainting;
    public Texture2D finishedPainting;
    private StateManager stateManager;

    private void Start()
    {
        GameObject stateManagerObject = GameObject.Find("StateManager");

        if (stateManagerObject != null)
        {
            // Get the StateManager component
            stateManager = stateManagerObject.GetComponent<StateManager>();

            // Check if the stateManager and lightAnimation components are found
            if (stateManager != null)
            {
                // Check the stateNumber and enable/disable lightAnimation accordingly
                UpdatePaintingTexture();
            }
            else
            {
                Debug.LogError("StateManager component not found!");
            }
        }
        else
        {
            Debug.LogError("StateManager GameObject not found!");
        }
    }

    void Update()
    {
        mat.SetVector("_HeadPos", headTransform.position);
        mat.SetVector("_RightHandPos", rightHandTransform.position);
        mat.SetVector("_LeftHandPos", leftHandTransform.position);

        UpdatePaintingTexture();
    }

    void UpdatePaintingTexture()
    {
        if (stateManager.stateNumber == "6")
        {
            mat.SetTexture("_Painting", finishedPainting);
        }
        else
        {
            mat.SetTexture("_Painting", initialPainting);
        }
    }
}
