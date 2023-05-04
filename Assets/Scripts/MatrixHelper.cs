using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    public static Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        float dotx = Vector3.Dot(((Vector3)matrix.GetColumn(0)).normalized, Vector3.right);
        float doty = Vector3.Dot(((Vector3)matrix.GetColumn(1)).normalized, Vector3.up);
        float dotz = Vector3.Dot(((Vector3)matrix.GetColumn(2)).normalized, Vector3.forward);

        // Smallest dot product = most orthogonal axis
        float minValue = Math.Min(dotx, Math.Min(doty, dotz));
        Vector3 normal, rotationAxis;
        // Calc cross product -> axis around which rotation takes place //TODO: Less repetitive code
        if (minValue == dotx)
        {
            normal = matrix.GetColumn(0).normalized;
            rotationAxis = Vector3.Cross(normal, Vector3.right);
        }
        else if (minValue == doty)
        {
            normal = matrix.GetColumn(1).normalized;
            rotationAxis = Vector3.Cross(normal, Vector3.up);
        }
        else //if (minValue == dotz)
        {
            normal = matrix.GetColumn(2).normalized;
            rotationAxis = Vector3.Cross(normal, Vector3.forward);
        }

        // Create quaternion of rotation
        return new Quaternion(rotationAxis.x, rotationAxis.y, rotationAxis.z, minValue);
    }
    public static void SetRotation(ref Matrix4x4 matrix, Quaternion rotation)
    {
        Quaternion conjugate = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        // Quaternion inverse = conjugate / Mathf.Sqrt(rotation.x * rotation.x +  // TODO: are we using pure rotation quaternions? If so inverse = conjugate
        //                                             rotation.y * rotation.y +
        //                                             rotation.z * rotation.z +
        //                                             rotation.w * rotation.w);
        Quaternion x, y, z;
        x = conjugate * new Quaternion(1, 0 ,0 ,0) * rotation; // inverse = conjugate TODO see above
        y = conjugate * new Quaternion(0, 1 ,0 ,0) * rotation;
        z = conjugate * new Quaternion(0, 1 ,1 ,0) * rotation;
        matrix.SetRow(0, new Vector4(x.x, x.y, x.z, 0));
        matrix.SetRow(1, new Vector4(y.x, y.y, y.z, 0));
        matrix.SetRow(2, new Vector4(z.x, z.y, z.z, 0));
    }
    
    public static Vector3 ExtractScale(Matrix4x4 matrix)
    {
        Vector3 x = new Vector3(matrix.m00, matrix.m10, matrix.m20);
        Vector3 y = new Vector3(matrix.m01, matrix.m11, matrix.m21);
        Vector3 z = new Vector3(matrix.m02, matrix.m12, matrix.m22);

        return new Vector3(x.magnitude, y.magnitude, z.magnitude); // magnitude = sqrt of sum of squared components
    }
    public static void SetScale(ref Matrix4x4 matrix, Vector3 scale)
    {
        Vector3 x = new Vector3(matrix.m00, matrix.m10, matrix.m20);
        Vector3 y = new Vector3(matrix.m01, matrix.m11, matrix.m21);
        Vector3 z = new Vector3(matrix.m02, matrix.m12, matrix.m22);
        
        // Resize axles
        x = x.normalized * scale.x;
        y = y.normalized * scale.y;
        z = z.normalized * scale.z;
        
        // Set axles
        matrix.m00 = x.x;
        matrix.m10 = x.y; 
        matrix.m20 = x.z;
        
        matrix.m01 = y.x;
        matrix.m11 = y.y; 
        matrix.m21 = y.z;
        
        matrix.m02 = z.x;
        matrix.m12 = z.y; 
        matrix.m22 = z.z;
    }

    // static Matrix4x4 Interpolate(Matrix4x4 start, Matrix4x4 end, float t)
    // {
    //     Matrix4x4 result = default;
    //     //TODO: interpolate between start and final with delta t
    //     return result;
    // }
}
