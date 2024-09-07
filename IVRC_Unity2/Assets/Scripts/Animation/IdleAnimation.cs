using UnityEngine;

public class IdleAnimation : MonoBehaviour
{
    public float amplitude = 0.05f; // How much the object moves up and down
    public float frequency = 3f;    // Speed of the up-and-down movement
    private Vector3 startPosition;  // The starting position of the object

    void Start()
    {
        // Save the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Create a new position for the object based on its original position
        Vector3 newPosition = startPosition;

        // Add vertical movement using a sine wave for smooth motion
        newPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the new position
        transform.position = newPosition;
    }

    // When the script is disabled, reset the object's position to the original
    void OnDisable()
    {
        transform.position = startPosition; // Return to the original position
    }
}
