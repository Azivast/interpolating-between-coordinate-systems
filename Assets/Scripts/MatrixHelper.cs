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
        return new Vector3(matrix.m03, matrix.m13, matrix.m23); // henceforth matrix.GetColumn() will be used
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

        // Extract each axis from the matrix. Normalized since we only deal with rotation and not scale.
        Vector3 x = Calc.Normalize(matrix.GetColumn(0));
        Vector3 y = Calc.Normalize(matrix.GetColumn(1));
        Vector3 z = Calc.Normalize(matrix.GetColumn(2));

        // Extract quaternion from matrix. Done using following formula:
        // q = (w, x, y, z) = (sqrt(1 + tr(M)), (M(2, 3) - M(3, 2))/4s, (M(3, 1) - M(1, 3))/4s, (M(1, 2) - M(2, 1))/4s)
        // where tr(M) is the trace of the matrix (sum of diagonal elements)
        // and s is the scaling factor used to normalize the quaternion.
        // Solving for for each component gives: 
        // w = sqrt(1 + tr(M)) / 2
        // x = (M(2, 3) - M(3, 2)) / (4 * w)
        // y = (M(3, 1) - M(1, 3)) / (4 * w)
        // z = (M(1, 2) - M(2, 1)) / (4 * w)
        //
        // tr(M) is equal to sum of the matrix's eigenvalues => formula for w can be rewritten as:
        // w = sqrt(1 + lambda1 + lambda2 + lambda3) / 2
        //
        // Using matrix values directly the eigenvalues can be calculated implicitly instead of directly, specifically
        // calculating the square root of each of the terms in the w-formula above:
        // w = sqrt(1 + x.x + y.y + z.z) / 2
        // x = sqrt(1 + x.x - y.y - z.z) / 2
        // y = sqrt(1 - x.x + y.y - z.z) / 2
        // z = sqrt(1 - x.x - y.y + z.z) / 2
        //
        // Adapted to code:
        result.x = Mathf.Sqrt( Mathf.Max( 0, 1 + x.x - y.y - z.z ) ) / 2; // rounding errors could cause negative
        result.y = Mathf.Sqrt( Mathf.Max( 0, 1 - x.x + y.y - z.z ) ) / 2; // square roots. Catch using Mathf.Max().
        result.z = Mathf.Sqrt( Mathf.Max( 0, 1 - x.x - y.y + z.z ) ) / 2; // Div by 2 since sum of square of
        result.w = Mathf.Sqrt( Mathf.Max( 0, 1 + x.x + y.y + z.z ) ) / 2; // components = 1.

        // Two quaternions can represent the same rotation, with differing signs.
        // w is conventionally positive so adjust axis to give rotation with positive w.
        result.x *= Mathf.Sign( result.x * ( y.z - z.y ) );
        result.y *= Mathf.Sign( result.y * ( z.x - x.z ) );
        result.z *= Mathf.Sign( result.z * ( x.y - y.x ) );

        return result;
    }

    public static void SetRotation(ref Matrix4x4 matrix, Quaternion rotation, Vector3 scale)
    {
        rotation = Calc.Normalize(rotation);
        
        // Technically calculation for conjugate. But inverse == conjugate when using pure rotation quaternions
        Quaternion inverse = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w); 
        
        // quaternion to vector = q*v*q^-1
        // 
        Quaternion x, y, z;
        x = inverse * new Quaternion(1, 0, 0, 0) * rotation;
        y = inverse * new Quaternion(0, 1, 0, 0) * rotation;
        z = inverse * new Quaternion(0, 0, 1, 0) * rotation;
        
        Vector4 scaledX =  Calc.Normalize(new Vector4(x.x, y.x, z.x, 0)) * scale.x;
        Vector4 scaledY =  Calc.Normalize(new Vector4(x.y, y.y, z.y, 0)) * scale.y;
        Vector4 scaledZ =  Calc.Normalize(new Vector4(x.z, y.z, z.z, 0)) * scale.z;
        
        matrix.SetColumn(0, scaledX);
        matrix.SetColumn(1, scaledY);
        matrix.SetColumn(2, scaledZ);
    }
    
    public static Vector3 ExtractScale(Matrix4x4 matrix)
    {
        Vector3 x = matrix.GetColumn(0);
        Vector3 y = matrix.GetColumn(1);
        Vector3 z = matrix.GetColumn(2);

        return new Vector3(Calc.Magnitude(x), Calc.Magnitude(y), Calc.Magnitude(z));
    }
    public static void SetScale(ref Matrix4x4 matrix, Vector3 scale)
    {
        Vector3 x = matrix.GetColumn(0);
        Vector3 y = matrix.GetColumn(1);
        Vector3 z = matrix.GetColumn(2);
        
        // Resize axles
        x = Calc.Normalize(x) * Mathf.Abs(scale.x); // model can't handle skew + negative scale => Mathf.Abs
        y = Calc.Normalize(y) * Mathf.Abs(scale.y);
        z = Calc.Normalize(z) * Mathf.Abs(scale.z);
        
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
