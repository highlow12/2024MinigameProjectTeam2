using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    private BossMonsterNetworked bossScript;
    private Rigidbody2D rb;
    [SerializeField]
    private TextMeshProUGUI panelText;


    // Start is called before the first frame update
    void Start()
    {
        bossScript = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossMonsterNetworked>();
        rb = bossScript.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bossScript)
        {
            return;
        }
        panelText.text = "Boss State: " + bossScript.CurrentState + "\n" +
                        "Boss Speed: " + bossScript.BossSpeed + "\n" +
                        "Boss x Velocity: " + Math.Abs(rb.velocity.x) + "\n" +
                        "Attack Timer: " + bossScript.BossAttackTimer.NormalizedValue(NetworkRunner.Instances.First()) + "\n" +
                        "Condition: " + bossScript.bossCondition + "\n" +
                        "Condition Duration: " + bossScript.conditionDuration + "\n" +
                        "Current Distance: " + bossScript.currentDistance + "\n" +
                        "Is Attacking: " + bossScript.isAttacking + "\n" +
                        "Is Moving: " + bossScript.isMoving + "\n" +
                        "Max Health: " + bossScript.maxHealth + "\n" +
                        "Current Health: " + bossScript.CurrentHealth + "\n" +
                        "Follow Target: " + bossScript.FollowTarget + "\n" +
                        "Attack Type: " + bossScript.attackType;
    }
}
