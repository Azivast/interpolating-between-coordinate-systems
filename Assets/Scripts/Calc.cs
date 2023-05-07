using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Own implementation of various mathematical concepts to demonstrate understanding of how to calculate.
/// </summary>
public static class Calc
{
    public static float Magnitude(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
    }
    public static float Magnitude(Vector4 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
    }
    
    public static float Magnitude(Quaternion rot)
    {
        return Mathf.Sqrt(rot.x * rot.x + rot.y * rot.y + rot.z * rot.z + rot.w * rot.w);
    }
    
    public static Vector3 Normalize(Vector3 vec)
    {
        var len = Magnitude(vec);
        return new Vector3(vec.x / len, vec.y / len, vec.z / len);
    }
    
    public static Vector4 Normalize(Vector4 vec)
    {
        var len = Magnitude(vec);
        return new Vector4(vec.x / len, vec.y / len, vec.z / len, vec.w / len);
    }
    
    public static Quaternion Normalize(Quaternion rot)
    {
        var len = Magnitude(rot);
        return len == 0 ? Quaternion.identity : new Quaternion(rot.x / len, rot.y / len, rot.z / len, rot.w / len);
    }
    
    public static float Determinant(Matrix4x4 matrix)
    {
        // Take 4x4 matrix and cross out first row & column.
        // Take first value of first row [0,0] and multiply it with the determinant of the remaining (uncrossed) 3x3 matrix.
        // Add result to the final sum.
        
        // Take 4x4 matrix and cross out first row & second column.
        // Take second value of first row [0,1] and multiply it with the determinant of the remaining (uncrossed) 3x3 matrix.
        // Subtract result from the final sum.
        
        // Repeat for [0,2] and [0,3] and remember alternate between addition and subtraction.
        // Then do the same for each 3x3 matrix. Once left with only 2x2 matrices calculate their determinant
        // using [0,0]*[1,1] - [0,1]*[1,0].
        // Finally sum it all together
        // Below is a hardcoded version of this for 4x4 matrices.
        
         return
         matrix[0,3] * matrix[1,2] * matrix[2,1] * matrix[3,0] - matrix[0,2] * matrix[1,3] * matrix[2,1] * matrix[3,0] -
         matrix[0,3] * matrix[1,1] * matrix[2,2] * matrix[3,0] + matrix[0,1] * matrix[1,3] * matrix[2,2] * matrix[3,0] +
         matrix[0,2] * matrix[1,1] * matrix[2,3] * matrix[3,0] - matrix[0,1] * matrix[1,2] * matrix[2,3] * matrix[3,0] -
         matrix[0,3] * matrix[1,2] * matrix[2,0] * matrix[3,1] + matrix[0,2] * matrix[1,3] * matrix[2,0] * matrix[3,1] +
         matrix[0,3] * matrix[1,0] * matrix[2,2] * matrix[3,1] - matrix[0,0] * matrix[1,3] * matrix[2,2] * matrix[3,1] -
         matrix[0,2] * matrix[1,0] * matrix[2,3] * matrix[3,1] + matrix[0,0] * matrix[1,2] * matrix[2,3] * matrix[3,1] +
         matrix[0,3] * matrix[1,1] * matrix[2,0] * matrix[3,2] - matrix[0,1] * matrix[1,3] * matrix[2,0] * matrix[3,2] -
         matrix[0,3] * matrix[1,0] * matrix[2,1] * matrix[3,2] + matrix[0,0] * matrix[1,3] * matrix[2,1] * matrix[3,2] +
         matrix[0,1] * matrix[1,0] * matrix[2,3] * matrix[3,2] - matrix[0,0] * matrix[1,1] * matrix[2,3] * matrix[3,2] -
         matrix[0,2] * matrix[1,1] * matrix[2,0] * matrix[3,3] + matrix[0,1] * matrix[1,2] * matrix[2,0] * matrix[3,3] +
         matrix[0,2] * matrix[1,0] * matrix[2,1] * matrix[3,3] - matrix[0,0] * matrix[1,2] * matrix[2,1] * matrix[3,3] -
         matrix[0,1] * matrix[1,0] * matrix[2,2] * matrix[3,3] + matrix[0,0] * matrix[1,1] * matrix[2,2] * matrix[3,3];
    }
    
    // Linear interpolation of vectors
    public static Vector3 Lerp(Vector3 start, Vector3 end, float time)
    {
        return (1f - time) * start + time * end;
    }
    
    // Spherical linear interpolation of quaternions
    public static Quaternion InterpolateQuaternions(Quaternion start, Quaternion end, float time)
    {
        // Calc cosine of angle between quaternions (dot product)
        float cos = start.x * end.x + start.y * end.y + start.z * end.z + start.w * end.w;
        
        // If dot product is negative: negate one quaternion to take shorter arc
        if (cos < 0f)
        {
            end = new Quaternion(-end.x, -end.y, -end.z, -end.z);
            cos *= -1;
        }
        // Avoid div by 0 by using lerp when sin would be 0
        float kStart, kEnd;
        if (cos > 0.9999f)
        {
            kStart = 1.0f - time;
            kEnd = time;
        }
        else // else slerp
        {
            float sin = Mathf.Sqrt(1f - cos * cos);

            float angle = Mathf.Atan2(sin, cos);

            float oneOverSin = 1 / sin;

            kStart = Mathf.Sin(((1f - time) * angle) * oneOverSin);
            kEnd = Mathf.Sin((time * angle) * oneOverSin);
        }

        return new Quaternion(
            start.x * kStart + end.x * kEnd,
            start.y * kStart + end.y * kEnd,
            start.z * kStart + end.z * kEnd,
            start.w * kStart + end.w * kEnd);
    }
}
