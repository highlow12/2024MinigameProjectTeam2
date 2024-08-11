using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject followTarget;
    public float cameraSpeed;
    public float groundWidth;
    public bool isBossJumping = false;
    public bool isPlayerDead = false;
    public bool isFollowBoss = false;
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPlayerDead && followTarget)
        {
            var controller = followTarget.GetComponent<PlayerControllerNetworked>();
            if (controller && controller.CurrentHealth <= 0)
            {
                isPlayerDead = true;
            }
        }
        if (isPlayerDead && !isFollowBoss)
        {
            followTarget = GameObject.FindGameObjectWithTag("Boss");
            isFollowBoss = true;
        }
        if (!followTarget)
        {
            return;
        }

        // use Lerp to make smooth camera movement
        Vector3 targetPos = new(followTarget.transform.position.x, 0, transform.position.z);
        Camera.main.orthographicSize = isBossJumping ? 8f : 5f;
        // if (isBossJumping)
        // {
        //     targetPos.y += 10f;

        // }
        if (Math.Abs(targetPos.x) < groundWidth / 2 - Camera.main.orthographicSize * Camera.main.aspect)
        {
            targetPos.x = followTarget.transform.position.x;
        }
        else
        {
            targetPos.x = Mathf.Clamp(targetPos.x, -groundWidth / 2 + Camera.main.orthographicSize * Camera.main.aspect, groundWidth / 2 - Camera.main.orthographicSize * Camera.main.aspect);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed);
    }
}
