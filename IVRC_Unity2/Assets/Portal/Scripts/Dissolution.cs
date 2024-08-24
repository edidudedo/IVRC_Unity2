using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolution : MonoBehaviour
{
    public Material mat;
    public Transform targetObj;
    public Transform targetObj1;

    void Update()
    {
        mat.SetVector("_targetpos", targetObj.position);
        mat.SetVector("_targetpos1", targetObj1.position);
    }
}
