using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public static class MatrixUtils
{
    public static Matrix4x4 ConvertToUnityMatrix(Matrix<double> matrix)
    {
        Matrix4x4 unityMatrix = new Matrix4x4();
        unityMatrix.m00 = (float)matrix[0, 0];
        unityMatrix.m01 = (float)matrix[0, 1];
        unityMatrix.m02 = (float)matrix[0, 2];
        unityMatrix.m10 = (float)matrix[1, 0];
        unityMatrix.m11 = (float)matrix[1, 1];
        unityMatrix.m12 = (float)matrix[1, 2];
        unityMatrix.m20 = (float)matrix[2, 0];
        unityMatrix.m21 = (float)matrix[2, 1];
        unityMatrix.m22 = (float)matrix[2, 2];
        unityMatrix.m33 = 1.0f;
        return unityMatrix;
    }

    public static Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
    }
}