// CalibrationManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CalibrationManager : MonoBehaviour
{
    private SelfManagementOfTrackedDevices trackerManager;

    public Transform HMD;

    private List<Vector3> positionsA = new List<Vector3>();
    private List<Vector3> positionsB = new List<Vector3>();
    private List<Quaternion> rotationsA = new List<Quaternion>();
    private List<Quaternion> rotationsB = new List<Quaternion>();
    private Matrix4x4 currentTransformation = Matrix4x4.identity;
    private bool isCalibrated = false;
    private int minimumDataPoints = 200;

    void Start()
    {
        trackerManager = GetComponent<SelfManagementOfTrackedDevices>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CalibrationRoutine());
        }

        if (isCalibrated)
        {
            trackerManager.ApplyCalibrationTransformation(currentTransformation);
        }
    }

    private IEnumerator CalibrationRoutine()
    {
        Debug.Log("Starting calibration, please move the device in all directions");

        yield return new WaitForSeconds(0.5f);

        positionsA.Clear();
        positionsB.Clear();
        rotationsA.Clear();
        rotationsB.Clear();

        float totalTime = 0f;
        float calibrationDuration = 5f;

        while (totalTime < calibrationDuration)
        {
            CollectCalibrationData();
            totalTime += Time.deltaTime;
            yield return null;
        }

        if (positionsA.Count >= minimumDataPoints)
        {
            ComputeCalibration();
            isCalibrated = true;
            Debug.Log("Calibration completed successfully");
        }
        else
        {
            Debug.LogWarning("Not enough valid samples collected. Please try again with more movement.");
        }
    }

    private void CollectCalibrationData()
    {
        if (trackerManager == null)
        {
            Debug.LogError("trackerManager is null in CollectCalibrationData.");
            return;
        }

        Vector3 positionA = trackerManager.GetTrackerPosition();
        Quaternion rotationA = trackerManager.GetTrackerRotation();
        if (positionA != null)
        {
            positionsA.Add(positionA);
            rotationsA.Add(rotationA);
        }
        else
        {
            Debug.LogWarning("positionA is null.");
        }

        if (HMD != null)
        {
            Vector3 positionB = HMD.position;
            Quaternion rotationB = HMD.rotation;
            positionsB.Add(positionB);
            rotationsB.Add(rotationB);
        }
        else
        {
            Debug.LogError("HMD is null in CollectCalibrationData.");
        }
    }

    private void ComputeCalibration()
    {
        if (positionsA.Count == 0 || positionsB.Count == 0)
        {
            Debug.LogError("No data collected for calibration.");
            return;
        }

        if (positionsA.Count != positionsB.Count || rotationsA.Count != rotationsB.Count)
        {
            Debug.LogError("positions or rotations have different counts.");
            return;
        }

        CalibrationCalculator calculator = new CalibrationCalculator();
        currentTransformation = calculator.CalculateTransformation(positionsA, positionsB, rotationsA, rotationsB);
    }
}
