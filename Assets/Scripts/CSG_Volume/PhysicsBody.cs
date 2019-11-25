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

    List<MyVertex> myVertices;
    List<MyTriangle> myTriangles;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        water = GameObject.Find("Water");
        ground = GameObject.Find("Ground");
        intersectPosition = water.transform.position;
        totalVolume = VolumeCalculator.GetTotalVolume(mf.mesh);

        Intersection.ConvertToTriangles(mf.mesh, out myVertices, out myTriangles);
    }
    private void FixedUpdate()
    {
        if (!dragging)
        {
            velocity += Gravity.Acceleration() * Time.fixedDeltaTime;

            // Get Triangle List
            // Get Volume
            // Calculate Lift
            // velocity += lift;

            //velocity += Lift.CalculateLift(ref myTriangles, ref myVertices, transform, intersectPosition, totalVolume) / mass * Time.fixedDeltaTime;
            ApplyVelocity(velocity);
        }
    }

    public void ApplyVelocity(Vector3 addVelocity)
    {
        transform.position += addVelocity * Time.fixedDeltaTime;
    }
}
