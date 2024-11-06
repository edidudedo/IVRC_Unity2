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

        // New fields to store the previous offsets
        public Vector3 previousPositionOffset;
        public float previousRotationOffset;
    }

    public List<TrackerBinding> trackerBindings = new List<TrackerBinding>();
    public ETrackedDeviceClass targetClass = ETrackedDeviceClass.GenericTracker;
    public KeyCode resetDeviceIds = KeyCode.Tab;

    public KeyCode checkDistanceKey = KeyCode.Space;
    public float positionThreshold = 0.25f; // Position Threshold
    public float pitchThreshold = 5.0f; // Yaw Threshold
    public string hmdTrackerSerialNumber = "LHR-A5A8A1BC";

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

    //void UpdateTrackedObjects()
    //{
    //    TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    //    _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

    //    foreach (var binding in trackerBindings)
    //    {
    //        if (binding.deviceId != -1)
    //        {
    //            var pose = allPoses[binding.deviceId];
    //            var absTracking = pose.mDeviceToAbsoluteTracking;
    //            var mat = new SteamVR_Utils.RigidTransform(absTracking);

    //            Vector3 trackerPosition = mat.pos;
    //            Quaternion trackerRotation = mat.rot;

    //            Quaternion offsetRotation = Quaternion.Euler(binding.rotationOffset);
    //            Quaternion finalRotation = trackerRotation * offsetRotation;

    //            Vector3 finalPosition = trackerPosition + binding.positionOffset;

    //            binding.targetObject.transform.SetPositionAndRotation(finalPosition, finalRotation);
    //        }
    //    }
    //}
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

                Vector3 trackerPosition = mat.pos;
                Quaternion trackerRotation = mat.rot;

                Quaternion offsetRotation = Quaternion.Euler(binding.rotationOffset);
                Quaternion finalRotation = trackerRotation * offsetRotation;

                Vector3 finalPosition = trackerPosition + binding.positionOffset;

                binding.targetObject.transform.SetPositionAndRotation(finalPosition, finalRotation);
            }
        }

        for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
        {
            var deviceClass = _vrSystem.GetTrackedDeviceClass(i);
            if (deviceClass == targetClass && trackerBindings.Find(b => b.deviceId == (int)i) == null) 
            {
                var pose = allPoses[i];
                if (pose.bDeviceIsConnected && pose.bPoseIsValid)
                {
                    var absTracking = pose.mDeviceToAbsoluteTracking;
                    var mat = new SteamVR_Utils.RigidTransform(absTracking);

                    Vector3 trackerPosition = mat.pos;
                    Vector3 trackerRotation = mat.rot.eulerAngles;

                    Debug.Log($"Unbound Tracker {i} Position: {trackerPosition}, Rotation (X, Y, Z): {trackerRotation.x}, {trackerRotation.y}, {trackerRotation.z}");
                }
            }
        }
    }


    //void Update()
    //{
    //    UpdateTrackedObjects();

    //    GameObject hmd = GameObject.Find("CenterEyeAnchor");
    //    if (hmd != null)
    //    {
    //        Vector3 hmdPosition = hmd.transform.position;
    //        Vector3 hmdRotation = hmd.transform.rotation.eulerAngles;

    //        Debug.Log($"HMD Position: {hmdPosition}, Rotation (X, Y, Z): {hmdRotation.x}, {hmdRotation.y}, {hmdRotation.z}");
    //    }

    //    foreach (var binding in trackerBindings)
    //    {
    //        if (binding.deviceId != -1)
    //        {
    //            Vector3 trackerPosition = binding.targetObject.transform.position;
    //            Vector3 trackerRotation = binding.targetObject.transform.rotation.eulerAngles;

    //            Debug.Log($"Tracker {binding.deviceId} Position: {trackerPosition}, Rotation (X, Y, Z): {trackerRotation.x}, {trackerRotation.y}, {trackerRotation.z}");
    //        }
    //    }

    //    if (Input.GetKeyDown(resetDeviceIds))
    //    {
    //        SetDeviceIds();
    //    }
    //}

    void Update()
    {
        UpdateTrackedObjects();

        GameObject hmd = GameObject.Find("CenterEyeAnchor");
        if (hmd != null)
        {
            Vector3 hmdPosition = hmd.transform.position;
            Vector3 hmdRotation = hmd.transform.rotation.eulerAngles;

            Debug.Log($"HMD Position: {hmdPosition}, Rotation (X, Y, Z): {hmdRotation.x}, {hmdRotation.y}, {hmdRotation.z}");

            if (Input.GetKeyDown(checkDistanceKey))
            {
                TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
                _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

                for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
                {
                    var deviceClass = _vrSystem.GetTrackedDeviceClass(i);
                    if (deviceClass == ETrackedDeviceClass.GenericTracker)
                    {
                        string serialNumber = GetDeviceSerialNumber((int)i);
                        if (serialNumber == hmdTrackerSerialNumber)
                        {
                            var pose = allPoses[i];
                            if (pose.bDeviceIsConnected && pose.bPoseIsValid)
                            {
                                var mat = new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking);

                                Vector3 trackerPosition = mat.pos;
                                Vector3 trackerRotation = mat.rot.eulerAngles;

                                Vector3 positionOffset = hmdPosition - trackerPosition + new Vector3(-0.02f, 0.05f, 0.1f);
                                float yawOffset = hmdRotation.y - trackerRotation.y;

                                if (positionOffset.magnitude > positionThreshold)
                                {
                                    Debug.Log("Position exceeded threshold, applying offset...");

                                    foreach (var tBinding in trackerBindings)
                                    {
                                        if (tBinding.deviceId != -1)
                                        {
                                            // Remove previous offset
                                            tBinding.positionOffset -= tBinding.previousPositionOffset;

                                            // Apply the new offset
                                            tBinding.positionOffset += positionOffset;

                                            // Store the current offset as previous for the next cycle
                                            tBinding.previousPositionOffset = positionOffset;

                                            // Update the position and rotation of the bound object
                                            tBinding.targetObject.transform.position += positionOffset;
                                        }
                                    }
                                }
                                if (Mathf.Abs(yawOffset) > pitchThreshold)
                                {
                                    Debug.Log("Pitch offset exceeded threshold, applying offset...");

                                    foreach (var tBinding in trackerBindings)
                                    {
                                        if (tBinding.deviceId != -1)
                                        {
                                            // Remove previous offset
                                            tBinding.rotationOffset.y -= tBinding.previousRotationOffset;

                                            // Apply the new offset
                                            tBinding.rotationOffset.y += yawOffset;

                                            // Store the current offset as previous for the next cycle
                                            tBinding.previousRotationOffset = yawOffset;

                                            // Update the position and rotation of the bound object
                                            Quaternion currentRotation = tBinding.targetObject.transform.rotation;
                                            tBinding.targetObject.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y + yawOffset, currentRotation.eulerAngles.z);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Output the position and rotation of trackers
        foreach (var binding in trackerBindings)
        {
            if (binding.deviceId != -1)
            {
                Vector3 trackerPosition = binding.targetObject.transform.position;
                Vector3 trackerRotation = binding.targetObject.transform.rotation.eulerAngles;

                Debug.Log($"Tracker {binding.deviceId} Position: {trackerPosition}, Rotation (X, Y, Z): {trackerRotation.x}, {trackerRotation.y}, {trackerRotation.z}");
            }
        }

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