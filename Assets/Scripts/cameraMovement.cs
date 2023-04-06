using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    private float baseMoveSpeed = 20f;
    private float zoomSpeed = 50f;
    private float minZoom = 2f;
    private float maxZoom = 120f;

    private Camera cam;

    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        float moveSpeed = baseMoveSpeed * (cam.fieldOfView / maxZoom);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0f);
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }
}
