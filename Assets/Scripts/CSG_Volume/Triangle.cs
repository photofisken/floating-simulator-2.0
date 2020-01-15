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
            vertex.AddTriangle(this);           //Keeps track on what triangles a particular vertex is in
            vertices[i] = vertex;
            position += vertex.position;
        }

        position /= vertices.Length;

        //int index = 1;                                                // Adding the other vertices in the triangle to a vertices list of neighbours
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    if (!vertices[i].connections.Contains(vertices[index]))
        //    {
        //        vertices[i].connections.Add(vertices[index]);
        //    }

        //    if (index == 2)
        //        index = 0;
        //    else
        //        index++;
        //}


        /*
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].UpdateConnections();
        }
        */
    }

}
