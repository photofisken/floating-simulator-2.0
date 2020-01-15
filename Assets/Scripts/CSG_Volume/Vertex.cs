using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vertex
{
    public Vector3 position;
    [System.NonSerialized] public List<Triangle> triangles = new List<Triangle>();

    public Vertex(Vector3 position)
    {
        this.position = position;
    }

    public void AddTriangle(Triangle triangle)
    {
        triangles.Add(triangle);
    }

    /*
    public void UpdateConnections()
    {
        connections.Clear();

        // For every triangle the vertex appears in (typically 8?)
        foreach (MyTriangle triangle in triangles)
        {
            // For each vertex in the triangle the vertex appears in
            foreach (MyVertex vertex in triangle.vertices)
            {
                // Add the other teo as a connection to the vertex (neighbours)
                if (vertex != this && !connections.Contains(vertex))
                    connections.Add(vertex);
            }
        }
    }
    */
}
