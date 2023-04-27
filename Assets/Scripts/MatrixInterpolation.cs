using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class MatrixInterpolation : MonoBehaviour
{
    [SerializeField] private Dictionary<string, bool> Toggles = new Dictionary<string, bool>(3);

    // Start is called before the first frame update
    void Start()
    {
        Toggles.Add("Translate", true);
        Toggles.Add("Rotate", true);
        Toggles.Add("Scale", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
