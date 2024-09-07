using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimation : MonoBehaviour
{
    public float rotationDurationMinutes = 1.0f; 
    private float rotationDurationSeconds; 
    private float rotationSpeed; 

    void Start()
    {
        rotationDurationSeconds = rotationDurationMinutes * 60;
        rotationSpeed = 360f / rotationDurationSeconds;
    }

    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}