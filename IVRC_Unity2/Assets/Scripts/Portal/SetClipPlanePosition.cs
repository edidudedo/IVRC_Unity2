using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetClipPlanePosition : MonoBehaviour
{
    public Material mat;
    public Transform planeTransform;

    void Start()
    {
        Vector3 planePosition = planeTransform.position;
        Vector3 planeNormal = planeTransform.forward;

        mat.SetVector("_PlanePos", planePosition);
        mat.SetVector("_PlaneNormal", planeNormal);
    }
}
