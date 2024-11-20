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
        positionsA.Clear();
        positionsB.Clear();
        isCalibrated = false;

        Debug.Log("Calibration started. Please move the device to collect data.");

        int dataPointsCollected = 0;

        while (dataPointsCollected < minimumDataPoints)
        {
            CollectCalibrationData();
            dataPointsCollected++;
            yield return null;
        }

        ComputeCalibration();
        isCalibrated = true;

        Debug.Log("Calibration completed.");
    }

    private void CollectCalibrationData()
    {
        if (trackerManager == null)
        {
            Debug.LogError("trackerManager is null in CollectCalibrationData.");
            return;
        }

        Vector3 positionA = trackerManager.GetTrackerPosition();
        if (positionA != null)
        {
            positionsA.Add(positionA);
            Debug.Log($"Collected positionA: {positionA}");
        }
        else
        {
            Debug.LogWarning("positionA is null.");
        }

        if (HMD != null)
        {
            Vector3 positionB = HMD.position;
            positionsB.Add(positionB);
            Debug.Log($"Collected positionB: {positionB}");
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

        if (positionsA.Count != positionsB.Count)
        {
            Debug.LogError("positionsA and positionsB have different counts.");
            return;
        }

        CalibrationCalculator calculator = new CalibrationCalculator();
        currentTransformation = calculator.CalculateTransformation(positionsA, positionsB);
    }
}
