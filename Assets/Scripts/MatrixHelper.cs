using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixHelper : MonoBehaviour
{
    Matrix4x4 RetrieveTranslation(Matrix4x4 transformation)
    {
        Matrix4x4 translation = default;
        //TODO: Extract and return translation of input matrix
        return translation;
    }
    
    Matrix4x4 RetrieveRotation(Matrix4x4 transformation)
    {
        Matrix4x4 rotation = default;
        //TODO: Extract and return rotation of input matrix
        return rotation;
    }
    
    Matrix4x4 RetrieveScale(Matrix4x4 transformation)
    {
        Matrix4x4 scale = default;
        //TODO: Extract and return scale of input matrix
        return scale;
    }

    Matrix4x4 Interpolate(Matrix4x4 start, Matrix4x4 end, float t)
    {
        Matrix4x4 result = default;
        //TODO: interpolate between start and final with delta t
        return result;
    }
}
