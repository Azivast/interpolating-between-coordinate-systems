using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
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
    
    private VectorRenderer vectors;
    [SerializeField][Range(0, 1)] private float time = 0;

    [SerializeField, HideInInspector] internal Matrix4x4 A = Matrix4x4.identity; // Original state
    [SerializeField, HideInInspector] internal Matrix4x4 B = Matrix4x4.identity; // Target state
    [SerializeField, HideInInspector] internal Matrix4x4 C = Matrix4x4.identity; // Current state

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<VectorRenderer>(out vectors)) //TODO: Creates 2 ??
        {
            vectors = gameObject.AddComponent<VectorRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        using (vectors.Begin())
        {
            // Reset cubes and C matrix
            CubeMesh cubeA = new CubeMesh();
            CubeMesh cubeB = new CubeMesh();
            CubeMesh cubeC = new CubeMesh();
            C = Matrix4x4.identity;
            
            var aPos = MatrixHelper.ExtractTranslation(A);
            var aScale = MatrixHelper.ExtractScale(A);
            var aRot = MatrixHelper.ExtractRotation(A);
            
            var bPos = MatrixHelper.ExtractTranslation(B);
            var bScale = MatrixHelper.ExtractScale(B);
            var bRot = MatrixHelper.ExtractRotation(B);
            
            // Interpolate position, scale and rotation
            var cPos = (1f - time) * aPos + time * bPos; // lerp
            var cScale = (1f - time) * aScale + time * bScale; // lerp
            Quaternion cRot = Calc.InterpolateQuaternions(aRot, bRot, time); // slerp

            // Update C matrix based on interpolated values. If toggle is unchecked values from A are used.
            if (DoScale)
                MatrixHelper.SetScale(ref C, cScale);
            else
                cScale = aScale;
            if (DoRotation)
                MatrixHelper.SetRotation(ref C, cRot, cScale);
            else
                MatrixHelper.SetRotation(ref C, aRot, cScale);
            if (DoTranslation)
                MatrixHelper.SetTranslation(ref C, cPos);
            else
                MatrixHelper.SetTranslation(ref C, aPos);
            
            // Transform cubes using the matrices
            for (int i = 0; i < cubeC.Vertices.Length; i++)
            {
                cubeC.Vertices[i] = C.MultiplyPoint(cubeC.Vertices[i]);
                cubeA.Vertices[i] = A.MultiplyPoint(cubeA.Vertices[i]);
                cubeB.Vertices[i] = B.MultiplyPoint(cubeB.Vertices[i]);
            }
            
            // Illustrate model
            cubeA.Draw(vectors, Color.yellow);
            cubeB.Draw(vectors, Color.yellow);
            cubeC.Draw(vectors);
            vectors.Draw(aPos, bPos, Color.cyan);
        }
    }
}

[CustomEditor(typeof(MatrixInterpolation))]
public class MatrixInterpolationEditor : Editor
{
    // Render and update tool handles
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
        
        
        if(EditorGUI.EndChangeCheck()) // values changed
        {
            Undo.RecordObject(matrixInterpolation, "Transformed target");
            
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
            
            EditorUtility.SetDirty(matrixInterpolation); // update editor
        }
    }
    
    // Custom inspector UI
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
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Determinant: " +  Calc.Determinant(matrixInterpolation.A));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
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
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Determinant: " + Calc.Determinant(matrixInterpolation.B));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
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
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Determinant: " +  Calc.Determinant(matrixInterpolation.C));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        // ------------------------------------
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
