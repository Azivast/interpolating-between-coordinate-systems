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
    
    public static Quaternion ExtractRotation(Matrix4x4 matrix, VectorRenderer vectors)
    {
        Vector3 scale = ExtractScale(matrix);
        
        Vector3 x = ((Vector3)matrix.GetColumn(0)).normalized;
        Vector3 y = ((Vector3)matrix.GetColumn(1)).normalized;
        Vector3 z = ((Vector3)matrix.GetColumn(2)).normalized;
        
        // Get cosine value & normal of each axle pair
        float cosX = Vector3.Dot(x, Vector3.right);
        float cosY = Vector3.Dot(y, Vector3.up);
        float cosZ = Vector3.Dot(z, Vector3.forward);
        Vector3 crossX = Vector3.Cross(Vector3.right, x);
        Vector3 crossY = Vector3.Cross(Vector3.up, y);
        Vector3 crossZ = Vector3.Cross(Vector3.forward, z);

        Vector3 rotationAxis = (x + y + z);
        Vector3 normal = rotationAxis.normalized;
        if (rotationAxis == Vector3.one) return Quaternion.identity;
        
        
        float smallestCos = Math.Min(cosX, Math.Min(cosY, cosZ));
        
        float angle, halfSin, halfCos;
        
        if (Math.Abs(smallestCos - cosX) < 0.0001f) // x
        {
            var projPlane = crossX - (Vector3.Dot(crossX, normal)) / (matrix.GetColumn(0).magnitude * matrix.GetColumn(0).magnitude) * normal; // vector, plane, and normal makes triangle, solve for plane      // x - ((x*n)/n.magnetude^)*n
            projPlane.Normalize();
            angle =  Mathf.Acos(Vector3.Dot(crossX, projPlane));
        }
        else if (Math.Abs(smallestCos - cosY) < 0.0001f) // y
        {
            var projPlane = crossY - (Vector3.Dot(crossY, normal)) / (matrix.GetColumn(1).magnitude * matrix.GetColumn(1).magnitude) * normal;
            projPlane.Normalize();
            angle =  Mathf.Acos(Vector3.Dot(crossY, projPlane));
        }
        else if (Math.Abs(smallestCos - cosZ) < 0.0001f) // z
        {
            var projPlane = crossZ - (Vector3.Dot(crossZ, normal)) /
                (matrix.GetColumn(2).magnitude * matrix.GetColumn(2).magnitude) * normal;
            projPlane.Normalize();
            angle = Mathf.Acos(Vector3.Dot(crossZ, projPlane));
        }
        else throw (new Exception("Invalid matrix rotation"));

        // halfSin = Mathf.Sqrt((1 - angle)/2);
        // halfCos = Mathf.Sqrt((1 + angle)/2);
        halfSin = Mathf.Sin(angle / 2);
        halfCos = Mathf.Cos(angle / 2);
        
        //DEBUG: TODO REMOVE
        vectors.Draw(Vector3.zero, Vector3.zero + rotationAxis, Color.cyan);

        return new Quaternion(
            normal.x*scale.x*halfSin,
            normal.y*scale.y*halfSin, 
            normal.z*scale.z*halfSin, 
            scale.x*halfCos);
    }
    public static void SetRotation(ref Matrix4x4 matrix, Quaternion rotation)
    {
        // Technically calculation for conjugate. But inverse == conjugate when using pure rotation quaternions
        Quaternion inverse = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w); 

        Quaternion x, y, z;
        x = inverse * new Quaternion(1, 0 ,0 ,0) * rotation;
        y = inverse * new Quaternion(0, 1 ,0 ,0) * rotation;
        z = inverse * new Quaternion(0, 0 ,1 ,0) * rotation;
        matrix.SetColumn(0, new Vector4(x.x, x.y, x.z, 0));
        matrix.SetColumn(1, new Vector4(y.x, y.y, y.z, 0));
        matrix.SetColumn(2, new Vector4(z.x, z.y, z.z, 0));
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
