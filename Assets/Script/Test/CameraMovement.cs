using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Vector3 followTarget;
    public float cameraSpeed;
    public float groundWidth;
    public bool isBossJumping = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget == Vector3.zero)
        {
            return;
        }
        // use Lerp to make smooth camera movement
        Vector3 targetPos = followTarget;
        Camera.main.orthographicSize = isBossJumping ? 8f : 5f;
        if (isBossJumping)
        {
            targetPos.y += 10f;

        }
        if (Math.Abs(targetPos.x) < groundWidth / 2 - Camera.main.orthographicSize * Camera.main.aspect)
        {
            targetPos.x = followTarget.x;
        }
        else
        {
            targetPos.x = Mathf.Clamp(targetPos.x, -groundWidth / 2 + Camera.main.orthographicSize * Camera.main.aspect, groundWidth / 2 - Camera.main.orthographicSize * Camera.main.aspect);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed);
    }
}
