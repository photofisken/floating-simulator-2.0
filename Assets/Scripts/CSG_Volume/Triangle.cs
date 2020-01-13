using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Triangle
{
    public Vertex[] vertices = new Vertex[3];
    public Vector3 position;
    public Triangle(Vector3[] points)
    {
        position = Vector3.zero;

        // For all the vertices in the triangle (3) create a MyVertex with the triangle(s) it is in
        for (int i = 0; i < Mathf.Min(points.Length, 3); i++)
        {
            Vertex vertex = new Vertex(points[i]);
            vertex.AddTriangle(this);
            vertices[i] = vertex;
            position += vertex.position;
        }

        position /= vertices.Length;

        /*
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].UpdateConnections();
        }
        */
    }

}
