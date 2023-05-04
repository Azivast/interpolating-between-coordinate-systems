using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMesh
{
    public Vector3[] Vertices =
    {
        Vector3.zero,
        Vector3.forward,
        Vector3.right,
        Vector3.forward + Vector3.right,

        Vector3.up + Vector3.zero,
        Vector3.up + Vector3.forward,
        Vector3.up + Vector3.right,
        Vector3.up + Vector3.forward + Vector3.right
    };
}
