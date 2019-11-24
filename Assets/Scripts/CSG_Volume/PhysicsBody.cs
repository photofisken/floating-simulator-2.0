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
    [SerializeField] private Vector3 intersectPosition;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        water = GameObject.Find("Water");
        intersectPosition = water.transform.position;
        totalVolume = VolumeCalculator.GetTotalVolume(mf.mesh);
    }
    private void FixedUpdate()
    {
        if (!dragging)
        {
            velocity += Gravity.Acceleration() * Time.fixedDeltaTime;
            velocity += Lift.CalculateLift(mf.mesh, transform, intersectPosition, totalVolume) / mass * Time.fixedDeltaTime;
            ApplyVelocity(velocity);
        }
    }

    public void ApplyVelocity(Vector3 addVelocity)
    {
        transform.position += addVelocity * Time.fixedDeltaTime;
    }
}
