using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolutionOnly : MonoBehaviour
{
    public Material mat;
    public Transform headTransform;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    private void Start()
    {
        
    }

    void Update()
    {
        mat.SetVector("_HeadPos", headTransform.position);
        mat.SetVector("_RightHandPos", rightHandTransform.position);
        mat.SetVector("_LeftHandPos", leftHandTransform.position);
    }

}
