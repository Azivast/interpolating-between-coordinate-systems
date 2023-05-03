using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vectors;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;

[System.Serializable, ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class MatrixInterpolation : MonoBehaviour
{
    [SerializeField] private Dictionary<string, bool> Toggles = new Dictionary<string, bool>(3);

    private VectorRenderer vectors;
    [Range(0, 1)] public float Time = 0;

    [SerializeField, HideInInspector] internal Matrix4x4 A = Matrix4x4.identity; // Original state
    [SerializeField, HideInInspector] internal Matrix4x4 B = Matrix4x4.identity; // Target 
    [SerializeField, HideInInspector] internal Matrix4x4 C = Matrix4x4.identity; // Current
        

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<VectorRenderer>(out vectors)) //TODO: Creates 2 ??
        {
            vectors = gameObject.AddComponent<VectorRenderer>();
        }
        
        Toggles.Add("Translate", true);
        Toggles.Add("Rotate", true);
        Toggles.Add("Scale", true);
    }

    // Update is called once per frame
    void Update()
    {
        using (vectors.Begin())
        {
            var aPos = MatrixHelper.ExtractTranslation(A);
            
            // Interpolate
            var pos = (1f - Time) * aPos + Time * MatrixHelper.ExtractTranslation(B); // TODO: Base of handles in unity
            
            
            // x
            vectors.Draw(pos, pos + transform.right, Color.red);
            vectors.Draw(pos + Vector3.forward, pos + Vector3.forward + transform.right, Color.red);
            vectors.Draw(pos + Vector3.up, pos + Vector3.up + transform.right, Color.red);
            vectors.Draw(pos + Vector3.up + Vector3.forward, pos + Vector3.up + Vector3.forward + transform.right, Color.red);
            // y
            vectors.Draw(pos, pos + transform.up, Color.green);
            vectors.Draw(pos + Vector3.forward, pos + Vector3.forward + transform.up, Color.green);
            vectors.Draw(pos + Vector3.right, pos + Vector3.right + transform.up, Color.green);
            vectors.Draw(pos + Vector3.right + Vector3.forward, pos + Vector3.right + Vector3.forward + transform.up, Color.green);
            // z
            vectors.Draw(pos, pos + transform.forward, Color.blue);
            vectors.Draw(pos + Vector3.right, pos + Vector3.right + transform.forward, Color.blue);
            vectors.Draw(pos + Vector3.up, pos + Vector3.up  + transform.forward, Color.blue);
            vectors.Draw(pos + Vector3.up  + Vector3.right, pos + Vector3.up  + Vector3.right + transform.forward, Color.blue);
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

        var aPos = new Vector3(matrixInterpolation.A.m03, matrixInterpolation.A.m13, matrixInterpolation.A.m23);
        var newTarget = Handles.PositionHandle(aPos, matrixInterpolation.transform.rotation);
        
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(matrixInterpolation, "Moved target");
            MatrixHelper.SetTranslation(ref matrixInterpolation.A, matrixInterpolation.transform.position); // TODO: Left off here
            MatrixHelper.SetTranslation(ref matrixInterpolation.B, newTarget); 
            //MatrixHelper.SetTranslation(ref matrixInterpolation.C, matrixInterpolation.transform.position);
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
        EditorGUILayout.PrefixLabel("Matrix A");
        EditorGUILayout.BeginVertical();
        Matrix4x4 result = Matrix4x4.identity;;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                result[i, j] = EditorGUILayout.FloatField(matrixInterpolation.A[i, j]);
            }
            EditorGUILayout.EndHorizontal();;
        }
        EditorGUILayout.EndVertical();
        // Matrix B ----------------------------
        EditorGUILayout.PrefixLabel("Matrix B");
        EditorGUILayout.BeginVertical();
        result = Matrix4x4.identity;;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                result[i, j] = EditorGUILayout.FloatField(matrixInterpolation.B[i, j]);
            }
            EditorGUILayout.EndHorizontal();;
        }
        EditorGUILayout.EndVertical();
        // Matrix C ----------------------------
        EditorGUILayout.PrefixLabel("Matrix C");
        EditorGUILayout.BeginVertical();
        result = Matrix4x4.identity;;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                result[i, j] = EditorGUILayout.FloatField(matrixInterpolation.C[i, j]);
            }
            EditorGUILayout.EndHorizontal();;
        }
        EditorGUILayout.EndVertical();
        // ------------------------------------

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(matrixInterpolation, "Change matrix");
            matrixInterpolation.A = result;
            EditorUtility.SetDirty(matrixInterpolation);
        }
    }
}
