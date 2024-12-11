using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;

public class CalibrationCalculator
{
    private const float MIN_MOVEMENT_THRESHOLD = 0.01f;      // 1cm
    private const int MIN_SAMPLES = 200;
    private const float OUTLIER_THRESHOLD = 3.0f;            // 3 standard deviations

    private void LogMatrix(string title, Matrix4x4 matrix)
    {
        Debug.Log($"=== {title} ===");
        for (int i = 0; i < 4; i++)
        {
            string row = $"Row {i}: ";
            for (int j = 0; j < 4; j++)
            {
                row += $"{matrix[i, j]:F4} ";
            }
            Debug.Log(row);
        }
    }

    public Matrix4x4 CalculateTransformation(
        List<Vector3> trackerPositions,
        List<Vector3> hmdPositions,
        List<Quaternion> trackerRotations,
        List<Quaternion> hmdRotations
    )
    {
        if (trackerPositions.Count < MIN_SAMPLES)
        {
            Debug.LogError($"Not enough samples. Need at least {MIN_SAMPLES}, got {trackerPositions.Count}");
            return Matrix4x4.identity;
        }

        // Remove outliers and filter samples
        List<(Vector3, Vector3)> filteredPositions = FilterSamples(trackerPositions, hmdPositions);

        if (filteredPositions.Count < MIN_SAMPLES)
        {
            Debug.LogError($"Not enough valid samples after filtering. Need at least {MIN_SAMPLES}, got {filteredPositions.Count}");
            return Matrix4x4.identity;
        }

        // Split filtered positions back into separate lists
        List<Vector3> filteredTrackerPos = new List<Vector3>();
        List<Vector3> filteredHmdPos = new List<Vector3>();
        foreach (var pair in filteredPositions)
        {
            filteredTrackerPos.Add(pair.Item1);
            filteredHmdPos.Add(pair.Item2);
        }

        // Build matrices for computation
        var matrixA = Matrix<double>.Build.Dense(filteredTrackerPos.Count, 3);
        var matrixB = Matrix<double>.Build.Dense(filteredHmdPos.Count, 3);

        // Fill matrices
        for (int i = 0; i < filteredTrackerPos.Count; i++)
        {
            Vector3 trackerPos = filteredTrackerPos[i];
            Vector3 hmdPos = filteredHmdPos[i];

            matrixA[i, 0] = trackerPos.x;
            matrixA[i, 1] = trackerPos.y;
            matrixA[i, 2] = trackerPos.z;

            matrixB[i, 0] = hmdPos.x;
            matrixB[i, 1] = hmdPos.y;
            matrixB[i, 2] = hmdPos.z;
        }

        // Calculate and subtract centroids
        var centroidA = CalculateCentroid(matrixA);
        var centroidB = CalculateCentroid(matrixB);

        Debug.Log($"Tracker Centroid: ({centroidA[0]:F4}, {centroidA[1]:F4}, {centroidA[2]:F4})");
        Debug.Log($"HMD Centroid: ({centroidB[0]:F4}, {centroidB[1]:F4}, {centroidB[2]:F4})");

        // Center the points
        var centeredA = CenterPoints(matrixA, centroidA);
        var centeredB = CenterPoints(matrixB, centroidB);

        // Calculate optimal rotation
        var covariance = centeredA.TransposeThisAndMultiply(centeredB);
        var svd = covariance.Svd();

        var rotation = svd.VT.Transpose().Multiply(svd.U.Transpose());

        // Ensure proper rotation matrix (determinant = 1)
        if (rotation.Determinant() < 0)
        {
            var V = svd.VT.Transpose();
            V.SetColumn(2, V.Column(2).Multiply(-1));
            rotation = V.Multiply(svd.U.Transpose());
        }

        // Calculate translation
        var translation = centroidB - rotation.Multiply(centroidA);

        // Build transformation matrix
        Matrix4x4 transformation = Matrix4x4.identity;

        // Set rotation
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                transformation[i, j] = (float)rotation[i, j];
            }
        }

        // Set translation
        transformation.m03 = (float)translation[0];
        transformation.m13 = (float)translation[1];
        transformation.m23 = (float)translation[2];

        LogMatrix("Transformation", transformation);

        return transformation;
    }

    private List<(Vector3, Vector3)> FilterSamples(List<Vector3> trackerPositions, List<Vector3> hmdPositions)
    {
        List<(Vector3, Vector3)> filteredPairs = new List<(Vector3, Vector3)>();
        Vector3 lastTrackerPos = trackerPositions[0];

        // First pass: movement threshold filter
        for (int i = 0; i < trackerPositions.Count; i++)
        {
            float movement = Vector3.Distance(trackerPositions[i], lastTrackerPos);

            if (movement > MIN_MOVEMENT_THRESHOLD)
            {
                filteredPairs.Add((trackerPositions[i], hmdPositions[i]));
                lastTrackerPos = trackerPositions[i];
            }
        }

        // Second pass: statistical outlier removal
        if (filteredPairs.Count > 10)
        {
            float meanDistance = 0;
            List<float> distances = new List<float>();

            foreach (var pair in filteredPairs)
            {
                float dist = Vector3.Distance(pair.Item1, pair.Item2);
                distances.Add(dist);
                meanDistance += dist;
            }
            meanDistance /= filteredPairs.Count;

            float variance = 0;
            foreach (float dist in distances)
            {
                variance += (dist - meanDistance) * (dist - meanDistance);
            }
            variance /= distances.Count;
            float stdDev = Mathf.Sqrt((float)variance);

            filteredPairs = filteredPairs.FindAll(pair =>
                Mathf.Abs(Vector3.Distance(pair.Item1, pair.Item2) - meanDistance) < OUTLIER_THRESHOLD * stdDev);
        }

        Debug.Log($"Filtered samples: {filteredPairs.Count} out of {trackerPositions.Count}");
        return filteredPairs;
    }

    private Vector<double> CalculateCentroid(Matrix<double> points)
    {
        var centroid = Vector<double>.Build.Dense(3);
        for (int j = 0; j < 3; j++)
        {
            centroid[j] = points.Column(j).Average();
        }
        return centroid;
    }

    private Matrix<double> CenterPoints(Matrix<double> points, Vector<double> centroid)
    {
        var centered = points.Clone();
        for (int i = 0; i < points.RowCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                centered[i, j] -= centroid[j];
            }
        }
        return centered;
    }

    public Vector3 TransformPosition(Vector3 position, Matrix4x4 transformation)
    {
        return transformation.MultiplyPoint3x4(position);
    }

    public Quaternion TransformRotation(Quaternion rotation, Matrix4x4 transformation)
    {
        // Simply return the input rotation without any transformation
        return rotation;
    }
}