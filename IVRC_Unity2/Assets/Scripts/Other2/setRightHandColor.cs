using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setRightHandColor : MonoBehaviour
{
    // The material to be used for changing colors

    public Material colorChangeMaterial;
    public Material rightHandColor;
    public Material colorHair;

    // Called once when the script instance is being loaded
    void Start()
    {
        // Check if the renderer is found
        if (colorChangeMaterial == null)
        {
            Debug.LogError("colorChangeMaterial is not assigned.");
            return;
        }

        // Assign the color change material to the renderer
        if (colorHair != null)
        {
            rightHandColor.mainTexture = colorHair.mainTexture; 
            rightHandColor.color = colorHair.color; 
        }
        else
        {
            Debug.LogError("colorHair is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (colorChangeMaterial.color != Color.white)
        {
            rightHandColor.mainTexture = colorChangeMaterial.mainTexture; 
            rightHandColor.color = colorChangeMaterial.color; 
        }
    }
}