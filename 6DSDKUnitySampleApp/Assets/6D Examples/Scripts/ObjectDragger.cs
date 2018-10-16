/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

using UnityEngine;

public class ObjectDragger : MonoBehaviour 
{
    public float minDistance = 0.2f;
    public float maxDistance = 7.0f;

    private float distance = 0f;
    private Vector3 offset = Vector3.zero;

    void Update()
    {
        if (Input.touchCount > 0) 
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    Tapped(Input.GetTouch(0).position);
                    break;

                case TouchPhase.Moved:
                    Dragged(Input.GetTouch(0).position);
                    break;

                default:
                    // nothing
                    break;
            }
        }
    }

    void Tapped(Vector2 tapPosition)
    {
        distance = 0f;
        offset = Vector3.zero;
        Camera mainCamera = Camera.main;
        if (!mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(tapPosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, maxDistance, 1 << gameObject.layer)
            && raycastHit.collider.gameObject == gameObject
            && raycastHit.distance >= minDistance)
        {
            distance = raycastHit.distance;
            offset = transform.position - raycastHit.point;
        }
    }

    void Dragged(Vector2 dragPosition)
    {
        Camera mainCamera = Camera.main;
        if (!mainCamera) return;
        if (offset == Vector3.zero) return;
        if (distance <= 0f) return;

        // Cast a ray at a fixed distance from the camera where the finger is
        Ray ray = mainCamera.ScreenPointToRay(dragPosition);
        // Apply the offset obtained from Tapped()
        Vector3 tentativePos = ray.GetPoint(distance) + offset;
        // Cast a new ray there in order to try and intersect the mesh or a plane
        ray = new Ray(mainCamera.transform.position, (tentativePos - mainCamera.transform.position));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, 1 << SixDegrees.SDMesh.MeshLayer))
        {
            // intersect with the mesh, raise by a couple centimeters
            transform.position = hit.point + new Vector3(0f, 0.02f, 0f);
        }
        else
        {
            float dist;
            Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
            if (plane.Raycast(ray, out dist) && dist <= maxDistance)
            {
                // intersect with the Y position plane
                transform.position = ray.GetPoint(dist);
            }
            // else don't move the object
        }
    }
}
