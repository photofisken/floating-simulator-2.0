using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    Vector3 mouseOffset;
    private Camera mainCam;
    private float zCoord;
    [SerializeField] private PhysicsBody physicsBody;

    private void Start()
    {
        mainCam = Camera.main;
        physicsBody = GetComponent<PhysicsBody>();
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
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mouseOffset;
        physicsBody.velocity = Vector3.zero;
        physicsBody.start = false;
    }
}
