/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject : MonoBehaviour
{
    [SerializeField] float height;
    [SerializeField] Transform water;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 floatForce;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < water.position.y + (height / 2f))
        {
            float heightUnderWater = water.position.y + (height / 2f) - transform.position.y;
            float percentageUnderWater = Mathf.Clamp01(heightUnderWater / height);

            Debug.Log("Percentage under water: " + percentageUnderWater);

            //floatForce = Lift.CalculateLiftNaive(Mathf.Pow(height, 3f) * percentageUnderWater);

            rb.AddForce(floatForce);
            //rb.AddForce(new Vector3(0f, Gravity.Force, 0f));
        }
    }
}
*/
