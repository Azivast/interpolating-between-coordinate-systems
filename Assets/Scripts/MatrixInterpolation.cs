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
    public Vector3 Target = Vector3.forward;
    [Range(0, 1)] public float Time = 0;

    [SerializeField, HideInInspector] internal Matrix4x4 A;
        

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<VectorRenderer>(out vectors))
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
            var aPos = new Vector3(A.m03, A.m13, A.m23);
            var pos = (1f - Time) * aPos + Time * Target;
            vectors.Draw(pos, pos + transform.up, Color.green);
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
            var copy = matrixInterpolation.A;
            copy.m03 = newTarget.x;
            copy.m13 = newTarget.y; 
            copy.m23 = newTarget.z;
            matrixInterpolation.A = copy;
            EditorUtility.SetDirty(matrixInterpolation);
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var matrixInterpolation = target as MatrixInterpolation;
        if (!matrixInterpolation) return;
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
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
        EditorGUILayout.EndHorizontal();
        
        
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(matrixInterpolation, "Change matrix");
            matrixInterpolation.A = result;
            EditorUtility.SetDirty(matrixInterpolation);
        }
    }
}
