using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    // All the converted triangles and vertices in the mesh

    public static List<Triangle> GetTrianglesUnderWater(ref List<Triangle> triangles, ref List<Vertex> myVertices, Transform meshTransform, Vector3 intersectPosition)
    {
        List<Triangle> underWaterTriangles = new List<Triangle>();

        Matrix4x4 worldToLocal = meshTransform.worldToLocalMatrix;
        intersectPosition = worldToLocal.MultiplyPoint3x4(intersectPosition);   // Make intersect local

        Vertex[] verticesOver = new Vertex[3];
        int overIndex = 0;
        Vertex[] verticesUnder = new Vertex[3];
        int underIndex = 0;

        // For every triangle in the mesh, check its three vertices and see if average is over or under water
        for (int i = 0; i < triangles.Count; i++)
        {
            for (int j = 0; j < triangles[i].vertices.Length; j++)
            {
                // If over water, add to overWater list, else add to underWater list
                if (triangles[i].vertices[j].position.y > intersectPosition.y)
                {
                    verticesOver[overIndex] = triangles[i].vertices[j];
                    overIndex++;
                }
                else
                {
                    verticesUnder[underIndex] = triangles[i].vertices[j];
                    underIndex++;
                }
            }

            if (overIndex > 0 && overIndex < 3)   // If not all vertices in a triangle are over or under water!
            {
                Triangle[] newTriangles = new Triangle[3];

                if (overIndex == 2)
                    newTriangles = SplitTriangle(triangles[i], verticesUnder[0], verticesOver, intersectPosition.y);
                else
                    newTriangles = SplitTriangle(triangles[i], verticesOver[0], verticesUnder, intersectPosition.y);

                foreach (Triangle triangle in newTriangles)
                    if (triangle.position.y < intersectPosition.y)
                        underWaterTriangles.Add(triangle);              // Adds the new triangles that are below water level in a separate list

            }
            if (underIndex >= 3)
                underWaterTriangles.Add(triangles[i]);      // Add the rest of the triangles that have all vertices under the water level in a separate list 

            overIndex = 0;
            underIndex = 0;
        }

        return underWaterTriangles;
    }

    public static Triangle[] SplitTriangle(Triangle triangle, Vertex top, Vertex[] floor, float intersectPosition)
    {
        Triangle[] newTriangles = new Triangle[3];

        float factor0 = Mathf.InverseLerp(floor[0].position.y, top.position.y, intersectPosition);
        float factor1 = Mathf.InverseLerp(floor[1].position.y, top.position.y, intersectPosition);

        Vector3 mid0 = Vector3.Lerp(floor[0].position, top.position, factor0);
        Vector3 mid1 = Vector3.Lerp(floor[1].position, top.position, factor1);

        Vector3[] vertices = new Vector3[3];
        vertices[0] = floor[0].position;
        vertices[1] = mid0;
        vertices[2] = mid1 ;
        newTriangles[0] = new Triangle(vertices);

        vertices[0] = floor[0].position;
        vertices[1] = mid1;
        vertices[2] = floor[1].position;
        newTriangles[1] = new Triangle(vertices);

        vertices[0] = mid0;
        vertices[1] = top.position;
        vertices[2] = mid1;
        newTriangles[2] = new Triangle(vertices);

        return newTriangles;
    }
    
    public static void ConvertToTriangles(Mesh mesh, out List<Vertex> listVertices, out List<Triangle> listTriangles)
    {
        listVertices = new List<Vertex>();
        listTriangles = new List<Triangle>();

        // Original vertices and triangles from the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Go through the vertices in the triangles and add them to a list with converted Triangles
        for(int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3[] points = new Vector3[3];
            points[0] = vertices[triangles[i + 0]];
            points[1] = vertices[triangles[i + 1]];
            points[2] = vertices[triangles[i + 2]];

            Triangle triangle = new Triangle(points);
            listTriangles.Add(triangle);

            // After convertion in Triangle (from point to vertex) add vertices in list
            foreach(Vertex vertex in triangle.vertices)
            {
                listVertices.Add(vertex);
            }
        }
    }
}
