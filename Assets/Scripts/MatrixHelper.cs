using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatrixHelper
{
    public static Vector3 ExtractTranslation(Matrix4x4 matrix)
    {
        return new Vector3(matrix.m03, matrix.m13, matrix.m23);
    }
    public static void SetTranslation(ref Matrix4x4 matrix, Vector3 translation)
    {
        matrix.m03 = translation.x;
        matrix.m13 = translation.y; 
        matrix.m23 = translation.z;
    }
    
    public static Matrix4x4  ExtractRotation(Matrix4x4 matrix)
    {
        Matrix4x4 rotation = default;
        //TODO: Extract and return rotation of input matrix
        return rotation;
    }
    
    public static Matrix4x4  ExtractScale(Matrix4x4 matrix)
    {
        Matrix4x4 scale = default;
        //TODO: Extract and return scale of input matrix
        return scale;
    }

    // static Matrix4x4 Interpolate(Matrix4x4 start, Matrix4x4 end, float t)
    // {
    //     Matrix4x4 result = default;
    //     //TODO: interpolate between start and final with delta t
    //     return result;
    // }
}
