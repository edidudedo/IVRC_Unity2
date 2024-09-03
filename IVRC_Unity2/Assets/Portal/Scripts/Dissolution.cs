using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolution : MonoBehaviour
{
    public Material mat;
    public Transform headTransform;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    void Update()
    {
        mat.SetVector("_HeadPos", headTransform.position);
        mat.SetVector("_RightHandPos", rightHandTransform.position);
        mat.SetVector("_LeftHandPos", leftHandTransform.position);
    }
}
