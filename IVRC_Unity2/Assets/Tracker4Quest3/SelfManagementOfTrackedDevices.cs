using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

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
    public string calibrationTrackerSerialNumber;

    private CVRSystem _vrSystem;
    private Matrix4x4 calibrationTransformation = Matrix4x4.identity;
    private bool isCalibrated = false;

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

                Vector3 trackerPosition = mat.pos;
                Quaternion trackerRotation = mat.rot;

                if (isCalibrated)
                {
                    CalibrationCalculator calculator = new CalibrationCalculator();
                    trackerPosition = calculator.TransformPosition(trackerPosition, calibrationTransformation);
                    trackerRotation = calculator.TransformRotation(trackerRotation, calibrationTransformation);
                }

                Vector3 finalPosition = trackerPosition + binding.positionOffset;
                Quaternion finalRotation = trackerRotation * Quaternion.Euler(binding.rotationOffset);

                binding.targetObject.transform.SetPositionAndRotation(finalPosition, finalRotation);
            }
        }
    }

    public Vector3 GetTrackerPosition()
    {
        foreach (var binding in trackerBindings)
        {
            if (binding.deviceId != -1 && binding.serialNumber == calibrationTrackerSerialNumber)
            {
                TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
                _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

                var pose = allPoses[binding.deviceId];
                var absTracking = pose.mDeviceToAbsoluteTracking;
                var mat = new SteamVR_Utils.RigidTransform(absTracking);

                Vector3 trackerPosition = mat.pos;

                Debug.Log($"Tracker {binding.serialNumber} position: {trackerPosition}");

                return trackerPosition;
            }
        }
        Debug.LogWarning("Calibration tracker not found or device ID invalid.");
        return Vector3.zero;
    }

    public Quaternion GetTrackerRotation()
    {
        foreach (var binding in trackerBindings)
        {
            if (binding.deviceId != -1 && binding.serialNumber == calibrationTrackerSerialNumber)
            {
                TrackedDevicePose_t[] allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
                _vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses);

                var pose = allPoses[binding.deviceId];
                var absTracking = pose.mDeviceToAbsoluteTracking;
                var mat = new SteamVR_Utils.RigidTransform(absTracking);

                return mat.rot;
            }
        }
        Debug.LogWarning("Calibration tracker not found or device ID invalid.");
        return Quaternion.identity;
    }

    public void ApplyCalibrationTransformation(Matrix4x4 transformation)
    {
        calibrationTransformation = transformation;
        isCalibrated = true;
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