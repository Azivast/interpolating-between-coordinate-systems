using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Vectors;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[System.Serializable, ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class MatrixInterpolation : MonoBehaviour
{
    [SerializeField] private bool DoTranslation = true;
    [SerializeField] private bool DoRotation = true;
    [SerializeField] private bool DoScale = true;
    
    public VectorRenderer vectors; // TODO: debug, make private
    [Range(0, 1)] public float Time = 0;

    [SerializeField, HideInInspector] internal Matrix4x4 A = Matrix4x4.identity; // Original state
    [SerializeField, HideInInspector] internal Matrix4x4 B = Matrix4x4.identity; // Target 
    [SerializeField, HideInInspector] internal Matrix4x4 C = Matrix4x4.identity; // Current
        

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<VectorRenderer>(out vectors)) //TODO: Creates 2 ??
        {
            vectors = gameObject.AddComponent<VectorRenderer>();
        }
    }
    
    // Slerp implementation
    public Quaternion InterpolateQuaternions(Quaternion start, Quaternion end, float time)
    {
        // Calc cosine of angle between quaternions (dot product)
        float cos = start.x * end.x + start.y * end.y + start.z * end.z + start.w * end.w;
        
        // If dot product is negative: negate one quaternion to take shorter arc
        if (cos < 0f)
        {
            end = new Quaternion(-end.x, -end.y, -end.z, -end.z);
            cos *= -1;
        }
        // Avoid div by 0 by using lerp
        float kStart, kEnd;
        if (cos > 0.9999f)
        {
            kStart = 1.0f - time;
            kEnd = time;
        }
        else
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

    // Update is called once per frame
    void Update()
    {
        using (vectors.Begin())
        {
            // Reset cube and C matrix
            CubeMesh cube = new CubeMesh();
            C = Matrix4x4.identity;
            
            var aPos = MatrixHelper.ExtractTranslation(A);
            var aScale = MatrixHelper.ExtractScale(A);
            var aRot = MatrixHelper.ExtractRotation(A);
            
            var bPos = MatrixHelper.ExtractTranslation(B);
            var bScale = MatrixHelper.ExtractScale(B);
            var bRot = MatrixHelper.ExtractRotation(B);

            

            // Interpolate position & scale
            var cPos = (1f - Time) * aPos + Time * bPos;
            var cScale = (1f - Time) * aScale + Time * bScale;
            
            // Interpolate rotation
            // Quaternion rotation = aRot * new Quaternion(bRot.x, bRot.y, bRot.z, -bRot.w);
            // var rads = (1f - Time) * Mathf.Acos(aRot.w)*2 + Time * Mathf.Acos(rotation.w)*2;
            Quaternion cRot = InterpolateQuaternions(aRot, bRot, Time);

            // Update C matrix
            if (DoScale)
                MatrixHelper.SetScale(ref C, cScale);
            if (DoRotation)
                MatrixHelper.SetRotation(ref C, cRot, cScale);
            if (DoTranslation)
                MatrixHelper.SetTranslation(ref C, cPos);

            
            // Update mesh using C matrix
            for (int i = 0; i < cube.Vertices.Length; i++)
            {
                cube.Vertices[i] = C.MultiplyPoint(cube.Vertices[i]);
            }
            
            // Draw cube
            // x
            vectors.Draw(cube.Vertices[0], cube.Vertices[2], Color.red);
            vectors.Draw(cube.Vertices[1], cube.Vertices[3], Color.red);
            vectors.Draw(cube.Vertices[4], cube.Vertices[6], Color.red);
            vectors.Draw(cube.Vertices[5], cube.Vertices[7], Color.red);
            // // y
            vectors.Draw(cube.Vertices[0], cube.Vertices[4], Color.green);
            vectors.Draw(cube.Vertices[1], cube.Vertices[5], Color.green);
            vectors.Draw(cube.Vertices[2], cube.Vertices[6], Color.green);
            vectors.Draw(cube.Vertices[3], cube.Vertices[7], Color.green);
            // // z
            vectors.Draw(cube.Vertices[0], cube.Vertices[1], Color.blue);
            vectors.Draw(cube.Vertices[2], cube.Vertices[3], Color.blue);
            vectors.Draw(cube.Vertices[4], cube.Vertices[5], Color.blue);
            vectors.Draw(cube.Vertices[6], cube.Vertices[7], Color.blue);
        }
    }
}

[CustomEditor(typeof(MatrixInterpolation))]
public class MatrixInterpolationEditor : Editor
{
    private void OnSceneGUI()
    {
        var matrixInterpolation = target as MatrixInterpolation;
        if (!matrixInterpolation) return;
        
        EditorGUI.BeginChangeCheck();

        var aPos = MatrixHelper.ExtractTranslation(matrixInterpolation.A);
        var aScale = MatrixHelper.ExtractScale(matrixInterpolation.A);
        Quaternion aRotation = MatrixHelper.ExtractRotation(matrixInterpolation.A);
        
        var bPos = MatrixHelper.ExtractTranslation(matrixInterpolation.B);
        var bScale = MatrixHelper.ExtractScale(matrixInterpolation.B);
        Quaternion bRotation = MatrixHelper.ExtractRotation(matrixInterpolation.B);
        
        
        if (Tools.current == Tool.Rotate)
        {
            aRotation = Handles.RotationHandle(aRotation, aPos);
            bRotation = Handles.RotationHandle(bRotation, bPos);
        }
        if (Tools.current == Tool.Move)
        {
            aPos = Handles.PositionHandle(aPos, aRotation);
            bPos = Handles.PositionHandle(bPos, bRotation);
        }
        if (Tools.current == Tool.Scale)
        {
            aScale = Handles.ScaleHandle(aScale, aPos, aRotation);
            bScale = Handles.ScaleHandle(bScale, bPos, bRotation);
        }

        
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(matrixInterpolation, "Moved target");
            
            if (Tools.current == Tool.Move)
            {
                MatrixHelper.SetTranslation(ref matrixInterpolation.A, aPos); 
                MatrixHelper.SetTranslation(ref matrixInterpolation.B, bPos);
            }
            if (Tools.current == Tool.Rotate)
            {
                MatrixHelper.SetRotation(ref matrixInterpolation.A, aRotation, aScale); 
                MatrixHelper.SetRotation(ref matrixInterpolation.B, bRotation, bScale); 
            }
            if (Tools.current == Tool.Scale)
            {
                MatrixHelper.SetScale(ref matrixInterpolation.A, aScale);
                MatrixHelper.SetScale(ref matrixInterpolation.B, bScale);
            }
            
            EditorUtility.SetDirty(matrixInterpolation); // Update editor
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var matrixInterpolation = target as MatrixInterpolation;
        if (!matrixInterpolation) return;
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        
        // Matrix A ----------------------------
        EditorGUILayout.PrefixLabel("Matrix A (Start)");
        EditorGUILayout.BeginVertical();
        Matrix4x4 aResult = Matrix4x4.identity;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                aResult[i, j] = EditorGUILayout.FloatField(matrixInterpolation.A[i, j]);
            }
            EditorGUILayout.EndHorizontal();;
        }
        EditorGUILayout.EndVertical();
        // Matrix B ----------------------------
        EditorGUILayout.PrefixLabel("Matrix B (End)");
        EditorGUILayout.BeginVertical();
        Matrix4x4 bResult = Matrix4x4.identity;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                bResult[i, j] = EditorGUILayout.FloatField(matrixInterpolation.B[i, j]);
            }
            EditorGUILayout.EndHorizontal();;
        }
        EditorGUILayout.EndVertical();
        // Matrix C ----------------------------
        EditorGUILayout.PrefixLabel("Matrix C (Interpolated)");
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                EditorGUILayout.FloatField(matrixInterpolation.C[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        // ------------------------------------

        // TODO: Replace with own code
        EditorGUILayout.PrefixLabel("Determinants, A, B, and C");
        EditorGUILayout.FloatField(matrixInterpolation.A.determinant);
        EditorGUILayout.FloatField(matrixInterpolation.B.determinant);
        EditorGUILayout.FloatField(matrixInterpolation.C.determinant);
        
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(matrixInterpolation, "Change matrix");
            matrixInterpolation.A = aResult;
            matrixInterpolation.B = bResult;
            EditorUtility.SetDirty(matrixInterpolation);
        }
    }
}
