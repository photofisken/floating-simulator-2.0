using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeCalculator
{

    public static float GetTotalVolume(Mesh mesh)
    {
        float totalVolume = 0f;

        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            Vector3 p1 = mesh.vertices[mesh.triangles[i + 0]];
            Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];

            totalVolume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return totalVolume;
    }

    public static float GetVolume(ref List<MyTriangle> myTriangles, ref List<MyVertex> myVertices, Transform meshTransform, Vector3 intersectPosition)
    {

        List<MyTriangle> underWaterTriangles = Intersection.GetTriangleList(ref myTriangles, ref myVertices, meshTransform, intersectPosition);

        float underWaterVolume = 0f;

        for (int i = 0; i < underWaterTriangles.Count; i++)
        {
            Vector3 p1 = underWaterTriangles[i].vertices[0].position;
            Vector3 p2 = underWaterTriangles[i].vertices[1].position;
            Vector3 p3 = underWaterTriangles[i].vertices[2].position;

            underWaterVolume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return underWaterVolume;
    }

    public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }
}
