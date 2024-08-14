using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolution : MonoBehaviour
{
    public Material mat;
    public Transform targetObj;

    void Update()
    {
        mat.SetVector("_targetpos", targetObj.position);
    }
}
