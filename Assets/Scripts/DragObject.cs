using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    Vector3 mouseOffset;
    private Camera mainCam;
    private float zCoord;
    private Rigidbody rigidbody;
    [SerializeField] private Rigidbody physicsBody;

    private void Start()
    {
        mainCam = Camera.main;
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zCoord;
        return mainCam.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseDown()
    {
        zCoord = mainCam.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - GetMouseWorldPos();
        rigidbody = transform.GetComponent<Rigidbody>();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mouseOffset;
        if (rigidbody != null)
            rigidbody.velocity = Vector3.zero;
    }
}
