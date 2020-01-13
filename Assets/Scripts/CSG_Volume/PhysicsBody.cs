using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public float mass;
    public float totalVolume;

    public Vector3 velocity;

    public Vector3 gravity;
    public Vector3 lift;

    //Not adjustable factors (for now)
    [SerializeField] private MeshFilter mf;
    [HideInInspector] public bool dragging = false;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject ground;
    [SerializeField] private Vector3 intersectPosition;

    public List<Vertex> vertices;
    public List<Triangle> triangles;
    public List<Triangle> underWaterTriangles;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        water = GameObject.Find("Water");
        ground = GameObject.Find("Ground");
        intersectPosition = water.transform.position;
        totalVolume = VolumeCalculator.GetTotalVolume(mf.mesh);

        Intersection.ConvertToTriangles(mf.mesh, out vertices, out triangles);
    }
    private void FixedUpdate()
    {
        if (!dragging)
        {
            velocity += Gravity.Acceleration() * Time.fixedDeltaTime;

            float time = Time.realtimeSinceStartup;

            // Get Triangle List
            List<Triangle> triangleList = Intersection.GetTriangleList(ref triangles, ref vertices, transform, intersectPosition);
            //Debug.Log("GetTriangleList(): " + (Time.realtimeSinceStartup - time) * 1000f + "ms");
            //time = Time.realtimeSinceStartup;

            // Get Volume
            float volumeUnderWater = VolumeCalculator.GetVolume(ref triangleList, transform, intersectPosition);
           // Debug.Log("GetVolume(): " + (Time.realtimeSinceStartup - time) * 1000f + "ms");
            //time = Time.realtimeSinceStartup;

            // Calculate Lift
            Vector3 lift = Lift.CalculateLift(transform, intersectPosition, volumeUnderWater, totalVolume);
            //Debug.Log("CalculateLift(): " + (Time.realtimeSinceStartup - time) * 1000f + "ms");
            //time = Time.realtimeSinceStartup;

            // velocity += lift;
            velocity += lift / mass * Time.fixedDeltaTime;

            //velocity += Lift.CalculateLift(ref myTriangles, ref myVertices, transform, intersectPosition, totalVolume) / mass * Time.fixedDeltaTime;
            ApplyVelocity(velocity);
        }
    }

    public void ApplyVelocity(Vector3 addVelocity)
    {
        transform.position += addVelocity * Time.fixedDeltaTime;
    }
}
