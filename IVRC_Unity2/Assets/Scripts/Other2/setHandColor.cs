using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeColor : MonoBehaviour
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

    // Called once when the script instance is being loaded
    void Start()
    {
        hand = GetComponent<Renderer>();

        // Check if the renderer is found
        if (hand == null)
        {
            Debug.LogError("Renderer component not found on the GameObject.");
            return;
        }

        // Assign the color change material to the renderer
        if (colorChangeMaterial != null)
        {
            hand.material = colorChangeMaterial;
        }
        else
        {
            Debug.LogError("Color change material is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateMaterialColor();
    }

    // void UpdateMaterialColor()
    // {
    //     if (hand != null && hand.material != null)
    //     {
    //         switch (m_ColorNumber)
    //         {
    //             case 0:
    //                 hand.material.color = Color.white;
    //                 break;
    //             case 1:
    //                 hand.material.color = Color.yellow;
    //                 break;
    //             case 2:
    //                 hand.material.color = new Color(191f / 255f, 150f / 255f, 99f / 255f); // Brown
    //                 break;
    //             case 3:
    //                 hand.material.color = new Color(58f / 255f, 117f / 255f, 36f / 255f); // Dark Green
    //                 break;
    //             default:
    //                 hand.material.color = Color.white;
    //                 break;
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("lefthand or its material is not set.");
    //     }
    // }
    void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        // Check if the object the player collided with has the "PickUp" tag.
        if (other.gameObject.CompareTag("Baguette")) 
        {
            hand.material.color = new Color(191f / 255f, 150f / 255f, 99f / 255f); // Brown
        }
        if (other.gameObject.CompareTag("Cheese")) 
        {
            hand.material.color = Color.yellow;
        }
        if (other.gameObject.CompareTag("Wine")) 
        {
            hand.material.color = new Color(58f / 255f, 117f / 255f, 36f / 255f); // Dark Green
        }
    }
}