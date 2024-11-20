// CalibrationCalculator.cs
using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

public class CalibrationCalculator
{
    public Matrix4x4 CalculateTransformation(List<Vector3> positionsA, List<Vector3> positionsB)
    {
        var matrixA = Matrix<double>.Build.Dense(positionsA.Count, 3);
        var matrixB = Matrix<double>.Build.Dense(positionsB.Count, 3);

        for (int i = 0; i < positionsA.Count; i++)
        {
            matrixA[i, 0] = positionsA[i].x;
            matrixA[i, 1] = positionsA[i].y;
            matrixA[i, 2] = positionsA[i].z;

            matrixB[i, 0] = positionsB[i].x;
            matrixB[i, 1] = positionsB[i].y;
            matrixB[i, 2] = positionsB[i].z;
        }

        // Calculate centroid
        var centroidA = matrixA.ColumnSums() / positionsA.Count;
        var centroidB = matrixB.ColumnSums() / positionsB.Count;

        var centeredA = matrixA.Clone();
        var centeredB = matrixB.Clone();

        for (int i = 0; i < centeredA.RowCount; i++)
        {
            centeredA[i, 0] -= centroidA[0];
            centeredA[i, 1] -= centroidA[1];
            centeredA[i, 2] -= centroidA[2];
        }

        for (int i = 0; i < centeredB.RowCount; i++)
        {
            centeredB[i, 0] -= centroidB[0];
            centeredB[i, 1] -= centroidB[1];
            centeredB[i, 2] -= centroidB[2];
        }

        // Compute covex metrix
        var H = centeredA.TransposeThisAndMultiply(centeredB);

        // SVD
        var svd = H.Svd();
        var U = svd.U;
        var Vt = svd.VT;

        var R = Vt.TransposeThisAndMultiply(U.Transpose());

        if (R.Determinant() < 0)
        {
            Vt.SetRow(2, Vt.Row(2) * -1);
            R = Vt.TransposeThisAndMultiply(U.Transpose());
        }

        var t = centroidB - R * centroidA;

        Matrix4x4 transformation = Matrix4x4.identity;
        transformation.m00 = (float)R[0, 0];
        transformation.m01 = (float)R[0, 1];
        transformation.m02 = (float)R[0, 2];
        transformation.m10 = (float)R[1, 0];
        transformation.m11 = (float)R[1, 1];
        transformation.m12 = (float)R[1, 2];
        transformation.m20 = (float)R[2, 0];
        transformation.m21 = (float)R[2, 1];
        transformation.m22 = (float)R[2, 2];

        transformation.m03 = (float)t[0];
        transformation.m13 = (float)t[1];
        transformation.m23 = (float)t[2];

        return transformation;
    }
}
