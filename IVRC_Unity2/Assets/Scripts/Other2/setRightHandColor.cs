using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeRightHandColor : MonoBehaviour
{
    /*
    0 = nothing
    1 = cheese
    2 = bread
    3 = wine bottle
    */
    //public int m_ColorNumber = 0;
    private Renderer hand;

    // The material to be used for changing colors

    public Material colorChangeMaterial;
    public Material vrObjectMaterial;
    public Material originalColor;

    // Called once when the script instance is being loaded
    void Start()
    {
        if (colorChangeMaterial != null)
        {
            colorChangeMaterial.mainTexture = originalColor.mainTexture;

            colorChangeMaterial.color = originalColor.color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("RIGHT Collision detected with " + other.gameObject.name);
        // Check if the object the player collided with has the "PickUp" tag.
        if (other.gameObject.CompareTag("Baguette"))
        {
            colorChangeMaterial.color = new Color(191f / 255f, 150f / 255f, 99f / 255f); // Brown
            colorChangeMaterial.mainTexture = vrObjectMaterial.mainTexture;
            TouchedObjectController.BaguetteIsTouched = true;
            TouchedObjectController.CheeseIsTouched = false;
            TouchedObjectController.WineIsTouched = false;
        }
        if (other.gameObject.CompareTag("Cheese"))
        {
            colorChangeMaterial.color = Color.yellow;
            colorChangeMaterial.mainTexture = vrObjectMaterial.mainTexture;
            TouchedObjectController.BaguetteIsTouched = false;
            TouchedObjectController.CheeseIsTouched = true;
            TouchedObjectController.WineIsTouched = false;
        }
        if (other.gameObject.CompareTag("Wine"))
        {
            colorChangeMaterial.color = new Color(58f / 255f, 117f / 255f, 36f / 255f); // Dark Green
            colorChangeMaterial.mainTexture = vrObjectMaterial.mainTexture;
            TouchedObjectController.BaguetteIsTouched = false;
            TouchedObjectController.CheeseIsTouched = false;
            TouchedObjectController.WineIsTouched = true;
        }
    }
}