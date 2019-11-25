using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyTriangle
{
    public MyVertex[] vertices = new MyVertex[3];
    public Vector3 position;
    public MyTriangle(Vector3[] points)
    {
        // For all the vertices in the triangle(3) create a MyVertex with the triangle(s) it is in
        for (int i = 0; i < Mathf.Min(points.Length, 3); i++)
        {
            MyVertex vertex = new MyVertex(points[i]);
            vertex.AddTriangle(this);
            vertices[i] = vertex;
            position += vertex.position;
        }
        position /= vertices.Length;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].UpdateConnections();
        }
    }

}

[System.Serializable]
public class MyVertex
{
    public Vector3 position;
    public List<MyVertex> connections = new List<MyVertex>();
    public List<MyTriangle> triangles = new List<MyTriangle>();

    public MyVertex(Vector3 position)
    {
        this.position = position;
    }

    public void AddTriangle(MyTriangle triangle)
    {
        triangles.Add(triangle);
    }

    public void UpdateConnections()
    {
        connections.Clear();

        // For every triangle the vertex appears in (typically 8?)
        foreach(MyTriangle triangle in triangles)
        {
            // For each vertex in the triangle the vertex appears in
            foreach(MyVertex vertex in triangle.vertices)
            {
                // Add the other teo as a connection to the vertex (neighbours)
                if (vertex != this && !connections.Contains(vertex))
                    connections.Add(vertex);
            }
        }
    }
}

public class Intersection
{
    // All the converted triangles and vertices in the mesh

    public static List<MyTriangle> GetTriangleList(ref List<MyTriangle> myTriangles, ref List<MyVertex> myVertices, Transform meshTransform, Vector3 intersectPosition)
    {
        // Convert the mesh vertexes into myVertexes with connections and stuff

        Matrix4x4 worldToLocal = meshTransform.worldToLocalMatrix;
        intersectPosition = worldToLocal.MultiplyPoint3x4(intersectPosition);   // Make intersect local
        List<MyTriangle> underWaterTriangles = new List<MyTriangle>();

        // For every triangle in the mesh, check its three vertices and see if average is over or under water
        for (int i = 0; i < myTriangles.Count; i++)
        {
            List<MyVertex> verticesOver = new List<MyVertex>();
            List<MyVertex> verticesUnder = new List<MyVertex>();

            for (int j = 0; j < myTriangles[i].vertices.Length; j++)
            {
                // If over water, add to overWater list, else add to underWater list
                if (myTriangles[i].vertices[j].position.y > intersectPosition.y)
                    verticesOver.Add(myTriangles[i].vertices[j]);
                else
                    verticesUnder.Add(myTriangles[i].vertices[j]);
            }

            if (verticesOver.Count > 0 && verticesOver.Count < 3)
            {
                MyTriangle[] newTriangles = new MyTriangle[3];

                if (verticesOver.Count >= 2)
                    newTriangles = SplitTriangle(myTriangles[i], verticesUnder[0], verticesOver.ToArray(), intersectPosition.y);
                else
                    newTriangles = SplitTriangle(myTriangles[i], verticesOver[0], verticesUnder.ToArray(), intersectPosition.y);

                foreach (MyTriangle triangle in newTriangles)
                {
                    if (triangle.position.y < intersectPosition.y)
                        underWaterTriangles.Add(triangle);
                }
                // Add these to the list, avoid duplicates, then do more stuff
            }
            if (verticesUnder.Count >= 3)
            {
                underWaterTriangles.Add(myTriangles[i]);
            }
        }

        return underWaterTriangles;
    }

    public static MyTriangle[] SplitTriangle(MyTriangle triangle, MyVertex top, MyVertex[] floor, float intersectPosition)
    {
        MyTriangle[] newTriangles = new MyTriangle[3];

        float factor0 = Mathf.InverseLerp(floor[0].position.y, top.position.y, intersectPosition);
        float factor1 = Mathf.InverseLerp(floor[1].position.y, top.position.y, intersectPosition);

        Vector3 mid0 = Vector3.Lerp(floor[0].position, top.position, factor0);
        Vector3 mid1 = Vector3.Lerp(floor[1].position, top.position, factor1);

        Vector3[] vertices = new Vector3[3];
        vertices[0] = floor[0].position;
        vertices[1] = mid0;
        vertices[2] = mid1 ;
        newTriangles[0] = new MyTriangle(vertices);

        vertices[0] = floor[0].position;
        vertices[1] = mid1;
        vertices[2] = floor[1].position;
        newTriangles[1] = new MyTriangle(vertices);

        vertices[0] = mid0;
        vertices[1] = top.position;
        vertices[2] = mid1;
        newTriangles[2] = new MyTriangle(vertices);

        return newTriangles;
    }
    
    public static void ConvertToTriangles(Mesh mesh, out List<MyVertex> myVertices, out List<MyTriangle> myTriangles)
    {
        myVertices = new List<MyVertex>();
        myTriangles = new List<MyTriangle>();

        // Original vertices and triangles from the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Add the triangle vertices to a MyTriangle then add to list
        for(int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3[] points = new Vector3[3];
            points[0] = vertices[triangles[i + 0]];
            points[1] = vertices[triangles[i + 1]];
            points[2] = vertices[triangles[i + 2]];

            MyTriangle triangle = new MyTriangle(points);
            myTriangles.Add(triangle);

            // After convertion in MyTriangle (from point to vertex) add vertex in list
            foreach(MyVertex vertex in triangle.vertices)
            {
                myVertices.Add(vertex);
            }
        }
    }
}
