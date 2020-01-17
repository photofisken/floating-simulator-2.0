using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    // All the converted triangles and vertices in the mesh

    public static List<Triangle> GetTrianglesUnderWater(ref List<Triangle> triangles, ref List<Vector3> myVertices, Transform meshTransform, Vector3 intersectPosition)  //TODO: I don't think the list of vertices is ever used. Might even be able to take it away already at ConvertToTriangles
    {
        List<Triangle> underWaterTriangles = new List<Triangle>();

        Matrix4x4 worldToLocal = meshTransform.worldToLocalMatrix;
        intersectPosition = worldToLocal.MultiplyPoint3x4(intersectPosition);   // Make intersect local

        Vector3[] verticesOver = new Vector3[3];
        int overIndex = 0;
        Vector3[] verticesUnder = new Vector3[3];
        int underIndex = 0;

        List<Vector3> brimVertices = new List<Vector3>();

        // For every triangle in the mesh, check its three vertices and see if average is over or under water
        for (int i = 0; i < triangles.Count; i++)
        {
            for (int j = 0; j < triangles[i].vertices.Length; j++)
            {
                // If over water, add to overWater list, else add to underWater list
                if (triangles[i].vertices[j].y > intersectPosition.y)
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
                    newTriangles = SplitTriangle(triangles[i], verticesUnder[0], verticesOver, intersectPosition.y, ref brimVertices);
                else
                    newTriangles = SplitTriangle(triangles[i], verticesOver[0], verticesUnder, intersectPosition.y, ref brimVertices);

                foreach (Triangle triangle in newTriangles)
                    if (triangle.position.y < intersectPosition.y)
                        underWaterTriangles.Add(triangle);              // Adds the new triangles that are below water level in a separate list

            }
            if (underIndex >= 3)
                underWaterTriangles.Add(triangles[i]);                   // Add the rest of the triangles that have all vertices under the water level in a separate list 

            overIndex = 0;
            underIndex = 0;
        }

        if(brimVertices.Count > 0)
            GetFillingTriangles(brimVertices, ref underWaterTriangles);              // To get correct volume we need to create new triangles where mesh intersects and make it whole

        return underWaterTriangles;
    }

    private static Triangle[] SplitTriangle(Triangle triangle, Vector3 top, Vector3[] floor, float intersectPosition, ref List<Vector3> brimVertices)
    {
        // We will create 3 new triangles by splitting one triangle on a line
        Triangle[] newTriangles = new Triangle[3];

        // Get the intersecting position
        float factor0 = Mathf.InverseLerp(floor[0].y, top.y, intersectPosition);
        float factor1 = Mathf.InverseLerp(floor[1].y, top.y, intersectPosition);

        // Make 2 new vertices (vectors), one between floor0 and top, and another between floor1 and top
        Vector3 mid0 = Vector3.Lerp(floor[0], top, factor0);
        Vector3 mid1 = Vector3.Lerp(floor[1], top, factor1);

        Vector3[] vertices = new Vector3[3];
        vertices[0] = floor[0];
        vertices[1] = mid0;
        vertices[2] = mid1;
        newTriangles[0] = new Triangle(vertices);

        vertices[0] = floor[0];
        vertices[1] = mid1;
        vertices[2] = floor[1];
        newTriangles[1] = new Triangle(vertices);

        vertices[0] = mid0;
        vertices[1] = top;
        vertices[2] = mid1;
        newTriangles[2] = new Triangle(vertices);

        brimVertices.Add(newTriangles[2].vertices[0]);                  // Save mid0  as Vertex in list (because they get translated to Vertex when in a Triangle)
        //brimVertices.Add(newTriangles[2].vertices[2]);

        return newTriangles;
    }
    // Fills the whole where waterlevel intersects
    private static void GetFillingTriangles(List<Vector3> brimVertices, ref List<Triangle> underWaterTriangles)
    {
        Vector3 medianPoint = Vector3.zero;

        // Get median point
        foreach (Vector3 vertex in brimVertices)
            medianPoint += vertex;

        medianPoint /= brimVertices.Count;



        //// Sort the list
        //for (int i = 0; i < brimVertices.Count; i++)
        //{
        //    bool foundNextVertex = false;
        //    for (int j = 0; j < brimVertices[i].triangles.Count; j++)
        //    {
        //        if (foundNextVertex)             
        //            break;

        //        for (int k = 0; k < current.triangles.Count; k++)
        //        {
        //             if (current.triangles[k] == brimVertices[i].triangles[j])
        //            {
        //                sortedBrimVertices[index] = brimVertices[i];
        //                current = brimVertices[i];
        //                index++;
        //                brimVertices.RemoveAt(i);
        //                foundNextVertex = true;
        //                i = 0;
        //                break;                       
        //            }
        //        }
        //    }
        //}

        //Vector3[] vertices = new Vector3[3];
        //Triangle newTriangle;

        //// Connect every pair of vertices in the list to the medianpoint to make a fan-like fill with triangles
        //for (int i = 0; i < sortedBrimVertices.Length; i++)
        //{

        //    if(i < sortedBrimVertices.Length - 2)
        //    {
        //        vertices[0] = sortedBrimVertices[i].position;
        //        vertices[1] = sortedBrimVertices[i + 1].position;
        //        vertices[2] = medianPoint;
        //        newTriangle = new Triangle(vertices);
        //        underWaterTriangles.Add(newTriangle);
        //    }
        //    else
        //    {
        //        vertices[0] = sortedBrimVertices[i].position;
        //        vertices[1] = sortedBrimVertices[0].position;
        //        vertices[2] = medianPoint;
        //        newTriangle = new Triangle(vertices);
        //        underWaterTriangles.Add(newTriangle);
        //    }
        //}


        Vector3[] vertices = new Vector3[3];
        Triangle newTriangle;

        // Connect every pair of vertices in the list to the medianpoint to make a fan-like fill with triangles
        for (int i = 0; i < brimVertices.Count; i++)
        {

            if (i < brimVertices.Count - 2)
            {
                vertices[0] = brimVertices[i];
                vertices[1] = brimVertices[i + 1];
                vertices[2] = medianPoint;
                newTriangle = new Triangle(vertices);
                underWaterTriangles.Add(newTriangle);
            }
            else
            {
                vertices[0] = brimVertices[i];
                vertices[1] = brimVertices[0];
                vertices[2] = medianPoint;
                newTriangle = new Triangle(vertices);
                underWaterTriangles.Add(newTriangle);
            }
        }


    }

    public static void ConvertToTriangles(Mesh mesh, out List<Vector3> listVertices, out List<Triangle> listTriangles)
    {
        listVertices = new List<Vector3>();
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
            foreach(Vector3 vertex in triangle.vertices)
            {
                listVertices.Add(vertex);
            }
        }
    }
}
