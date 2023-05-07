using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectors;

/// <summary>
/// Collection of vertices representing a cube
/// </summary>
public class CubeMesh
{
    public readonly Vector3[] Vertices =
    {
        new Vector3(-0.5f, -0.5f, 0-.5f),
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.forward,
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.right,
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.forward + Vector3.right,

        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.up + Vector3.zero,
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.up + Vector3.forward,
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.up + Vector3.right,
        new Vector3(-0.5f, -0.5f, 0-.5f) + Vector3.up + Vector3.forward + Vector3.right
    };

    public void Draw(VectorRenderer vectors)
    {
        // x
        vectors.Draw(Vertices[0], Vertices[2], Color.red);
        vectors.Draw(Vertices[1], Vertices[3], Color.red);
        vectors.Draw(Vertices[4], Vertices[6], Color.red);
        vectors.Draw(Vertices[5], Vertices[7], Color.red);
        // // y
        vectors.Draw(Vertices[0], Vertices[4], Color.green);
        vectors.Draw(Vertices[1], Vertices[5], Color.green);
        vectors.Draw(Vertices[2], Vertices[6], Color.green);
        vectors.Draw(Vertices[3], Vertices[7], Color.green);
        // // z
        vectors.Draw(Vertices[0], Vertices[1], Color.blue);
        vectors.Draw(Vertices[2], Vertices[3], Color.blue);
        vectors.Draw(Vertices[4], Vertices[5], Color.blue);
        vectors.Draw(Vertices[6], Vertices[7], Color.blue);
    }
    
    public void Draw(VectorRenderer vectors, Color color)
    {
        // x
        vectors.Draw(Vertices[0], Vertices[2], color);
        vectors.Draw(Vertices[1], Vertices[3], color);
        vectors.Draw(Vertices[4], Vertices[6], color);
        vectors.Draw(Vertices[5], Vertices[7], color);
        // // y
        vectors.Draw(Vertices[0], Vertices[4], color);
        vectors.Draw(Vertices[1], Vertices[5], color);
        vectors.Draw(Vertices[2], Vertices[6], color);
        vectors.Draw(Vertices[3], Vertices[7], color);
        // // z
        vectors.Draw(Vertices[0], Vertices[1], color);
        vectors.Draw(Vertices[2], Vertices[3], color);
        vectors.Draw(Vertices[4], Vertices[5], color);
        vectors.Draw(Vertices[6], Vertices[7], color);
    }
}
