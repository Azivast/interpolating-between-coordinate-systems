using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vectors;

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

    // Using algorithm presented here:
    // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
    public static Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        Quaternion result;

        // Extract each axis and scale from the matrix
        Vector3 x = matrix.GetColumn(0);
        Vector3 y = matrix.GetColumn(1);
        Vector3 z = matrix.GetColumn(2);
        x.Normalize(); y.Normalize(); z.Normalize();
        
        // TODO: Explain
        result.x = Mathf.Sqrt( Mathf.Max( 0, 1 + x.x - y.y - z.z ) ) / 2; 
        result.y = Mathf.Sqrt( Mathf.Max( 0, 1 - x.x + y.y - z.z ) ) / 2; 
        result.z = Mathf.Sqrt( Mathf.Max( 0, 1 - x.x - y.y + z.z ) ) / 2; 
        result.w = Mathf.Sqrt( Mathf.Max( 0, 1 + x.x + y.y + z.z ) ) / 2; 
        
        result.x *= Mathf.Sign( result.x * ( y.z - z.y ) );
        result.y *= Mathf.Sign( result.y * ( z.x - x.z ) );
        result.z *= Mathf.Sign( result.z * ( x.y - y.x ) );

        return result;
    }

    public static void SetRotation(ref Matrix4x4 matrix, Quaternion rotation, Vector3 scale)
    {
        rotation.Normalize(); // TODO illustrate / do manually
        // Technically calculation for conjugate. But inverse == conjugate when using pure rotation quaternions
        Quaternion inverse = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w); 
        
        // quaternion to matrix = q*v*q^-1
        Quaternion x, y, z;
        x = inverse * new Quaternion(1, 0, 0, 0) * rotation;
        y = inverse * new Quaternion(0, 1, 0, 0) * rotation;
        z = inverse * new Quaternion(0, 0, 1, 0) * rotation;
        
        var newX = new Vector4(x.x, y.x, z.x, 0).normalized * scale.x;
        var newY = new Vector4(x.y, y.y, z.y, 0).normalized * scale.y;
        var newZ = new Vector4(x.z, y.z, z.z, 0).normalized * scale.z;
        
        matrix.SetColumn(0, newX);
        matrix.SetColumn(1, newY);
        matrix.SetColumn(2, newZ);
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
}
