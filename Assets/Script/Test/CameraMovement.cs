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
<<<<<<< HEAD
=======
    public bool isPlayerDead = false;
    public bool isFollowBoss = false;
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
    void Start()
    {
    }

    // Update is called once per frame
<<<<<<< HEAD
    void Update()
    {
=======
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
            followTarget.GetComponent<BossMonsterNetworked>().RPC_ForceRetarget();
            isFollowBoss = true;
        }
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
        if (!followTarget)
        {
            return;
        }
<<<<<<< HEAD
        // use Lerp to make smooth camera movement
        Vector3 targetPos = new(followTarget.transform.position.x, followTarget.transform.position.y + 3.25f, transform.position.z);
        Camera.main.orthographicSize = isBossJumping ? 8f : 5f;
        if (isBossJumping)
        {
            targetPos.y += 10f;

        }
=======

        // use Lerp to make smooth camera movement
        Vector3 targetPos = new(followTarget.transform.position.x, 0, transform.position.z);
        Camera.main.orthographicSize = isBossJumping ? 8f : 5f;
        // if (isBossJumping)
        // {
        //     targetPos.y += 10f;

        // }
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
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
