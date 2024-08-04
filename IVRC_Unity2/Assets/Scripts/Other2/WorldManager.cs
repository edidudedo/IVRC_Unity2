using UnityEngine;
using UnityEngine.XR.Management;
using Oculus;

public class WorldManager : MonoBehaviour
{
    private bool isPassthroughEnabled = false;
    public GameObject cameraRig;
    public GameObject vrWorld;
    public GameObject HandVisualLeft;
    public GameObject HandVisualRight;

    private OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        // Assuming the OVRPassthroughLayer is attached to the cameraRig
        if (cameraRig != null)
        {
            passthroughLayer = cameraRig.GetComponent<OVRPassthroughLayer>();
            if (passthroughLayer == null)
            {
                Debug.LogError("OVRPassthroughLayer component not found on cameraRig.");
            }
        }
        else
        {
            Debug.LogError("cameraRig GameObject is not assigned.");
        }
        TogglePassthrough();
    }

    void Update()
    {
        // Check for a key press to toggle passthrough
        if (Input.GetKeyDown(KeyCode.P)) // Change KeyCode.P to whatever key you prefer
        {
            TogglePassthrough();
        }
    }

    void TogglePassthrough()
    {
        if (passthroughLayer != null)
        {
            if (isPassthroughEnabled)
            {
                // Disable passthrough
                passthroughLayer.enabled = false;
                vrWorld.SetActive(true); // Assuming you want to show the VR world when passthrough is off
                HandVisualLeft.SetActive(true);
                HandVisualRight.SetActive(true);
            }
            else
            {
                // Enable passthrough
                passthroughLayer.enabled = true;
                vrWorld.SetActive(false); // Assuming you want to hide the VR world when passthrough is on
                HandVisualLeft.SetActive(false);
                HandVisualRight.SetActive(false);
            }

            // Toggle the state
            isPassthroughEnabled = !isPassthroughEnabled;
        }
    }
}

