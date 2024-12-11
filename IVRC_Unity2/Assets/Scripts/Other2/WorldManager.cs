using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Text;

public class InitialPositionBinder : MonoBehaviour
{
    [System.Serializable]
    public class TrackerBinding
    {
        public GameObject targetObject;
        public string serialNumber;
        public Vector3 positionOffset;
        [HideInInspector] public int deviceId = -1;
        [HideInInspector] public Vector3 initialPosition;
    }

    public List<TrackerBinding> trackerBindings = new List<TrackerBinding>();
    public ETrackedDeviceClass targetClass = ETrackedDeviceClass.GenericTracker;
    public KeyCode rebindKey = KeyCode.R;

    private CVRSystem _vrSystem;

    void Start()
    {
        Debug.Log("test");
        var error = EVRInitError.None;
        _vrSystem = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);
        if (error != EVRInitError.None)
        {
            Debug.LogWarning("Init error: " + error);
            Debug.Log("test2");
        }
        else
        {
            Debug.Log("OpenVR initialized successfully");
            BindInitialPositions();
        }
    }

    void BindInitialPositions()
    {
        foreach (var binding in trackerBindings)
        {
            binding.deviceId = -1;
        }

        TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

        for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
        {
            var deviceClass = _vrSystem.GetTrackedDeviceClass(i);
            if (deviceClass != ETrackedDeviceClass.Invalid && deviceClass == targetClass)
            {
                string serialNumber = GetDeviceSerialNumber((int)i);
                Debug.Log($"Found device {i} with serial number: {serialNumber}");

                var binding = trackerBindings.Find(b => b.serialNumber == serialNumber);
                if (binding != null)
                {
                    binding.deviceId = (int)i;
                    Debug.Log($"Bound device {i} to object: {binding.targetObject.name}");

                    var pose = allPoses[i];
                    var absTracking = pose.mDeviceToAbsoluteTracking;
                    var mat = new SteamVR_Utils.RigidTransform(absTracking);
                    Vector3 trackerPosition = mat.pos;

                    binding.initialPosition = trackerPosition;
                    UpdateObjectPosition(binding);

                    Debug.Log($"Set initial position for {binding.targetObject.name}: {binding.initialPosition}");
                }
            }
        }
    }

    void UpdateObjectPosition(TrackerBinding binding)
    {
        if (binding.targetObject != null)
        {
            binding.targetObject.transform.position = binding.initialPosition + binding.positionOffset;
        }
    }

    string GetDeviceSerialNumber(int deviceId)
    {
        StringBuilder serialNumber = new StringBuilder(64);
        ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
        _vrSystem.GetStringTrackedDeviceProperty((uint)deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String, serialNumber, 64, ref error);
        if (error != ETrackedPropertyError.TrackedProp_Success)
        {
            Debug.LogError($"Error getting serial number for device {deviceId}: {error}");
            return "Unknown";
        }
        return serialNumber.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(rebindKey))
        {
            BindInitialPositions();
        }

        foreach (var binding in trackerBindings)
        {
            UpdateObjectPosition(binding);
        }
    }

    void OnDestroy()
    {
        OpenVR.Shutdown();
    }
}