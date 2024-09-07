using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineChildComponent : MonoBehaviour
{
    public WineManager parentObject; // Reference to the parent ObjectManager

    void OnTriggerEnter(Collider other)
    {
        if (parentObject != null)
        {
            parentObject.OnChildTriggerEnter(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (parentObject != null)
        {
            parentObject.OnChildTriggerExit(other);
        }
    }
}
