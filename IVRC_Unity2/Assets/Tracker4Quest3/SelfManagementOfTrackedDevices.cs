using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Text;

public class SelfManagementOfTrackedDevices : MonoBehaviour
{
    [System.Serializable]
    public class TrackerBinding
    {
        public GameObject targetObject;
        public string serialNumber;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        [HideInInspector] public int deviceId = -1;
    }

    public List<TrackerBinding> trackerBindings = new List<TrackerBinding>();
    public ETrackedDeviceClass targetClass = ETrackedDeviceClass.GenericTracker;
    public KeyCode resetDeviceIds = KeyCode.Tab;

    private CVRSystem _vrSystem;

    void Start()
    {
        var error = EVRInitError.None;
        _vrSystem = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);
        if (error != EVRInitError.None)
        {
            Debug.LogWarning("Init error: " + error);
        }
        else
        {
            Debug.Log("OpenVR initialized successfully");
            SetDeviceIds();
        }
    }

    void SetDeviceIds()
    {
        foreach (var binding in trackerBindings)
        {
            binding.deviceId = -1;
            binding.targetObject.SetActive(false);
        }

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
                    binding.targetObject.SetActive(true);
                    Debug.Log($"Bound device {i} to object: {binding.targetObject.name}");
                }
            }
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

    void UpdateTrackedObjects()
    {
        TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

        foreach (var binding in trackerBindings)
        {
            if (binding.deviceId != -1)
            {
                var pose = allPoses[binding.deviceId];
                var absTracking = pose.mDeviceToAbsoluteTracking;
                var mat = new SteamVR_Utils.RigidTransform(absTracking);

                Quaternion trackerRotation = mat.rot;

                Quaternion offsetRotation = Quaternion.Euler(binding.rotationOffset);
                Quaternion finalRotation = trackerRotation * offsetRotation;

                Vector3 offsetPositionInWorldSpace = trackerRotation * binding.positionOffset;

                Vector3 finalPosition = mat.pos + offsetPositionInWorldSpace;

                binding.targetObject.transform.SetPositionAndRotation(finalPosition, finalRotation);
            }
        }
    }

    void Update()
    {
        UpdateTrackedObjects();
        if (Input.GetKeyDown(resetDeviceIds))
        {
            SetDeviceIds();
        }
    }

    void OnDestroy()
    {
        OpenVR.Shutdown();
    }
}