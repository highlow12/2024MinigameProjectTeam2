using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    private BossMonsterNetworked bossScript;
    [SerializeField]
    private TextMeshProUGUI panelText;


    // Start is called before the first frame update
    void Start()
    {
        bossScript = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossMonsterNetworked>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bossScript)
        {
            return;
        }
        panelText.text = "Boss State: " + bossScript.CurrentState + "\n" +
                        "Condition: " + bossScript.bossCondition + "\n" +
                        "Condition Duration: " + bossScript.conditionDuration + "\n" +
                        "Current Distance: " + bossScript.currentDistance + "\n" +
                        "Attack Cooldown: " + bossScript.attackCooldown + "\n" +
                        "Is Attacking: " + bossScript.isAttacking + "\n" +
                        "Is Moving: " + bossScript.isMoving + "\n" +
                        "Max Health: " + bossScript.maxHealth + "\n" +
                        "Current Health: " + bossScript.CurrentHealth + "\n" +
                        "Boss Scale: " + bossScript.BossScale + "\n" +
                        "Follow Target: " + bossScript.FollowTarget + "\n" +
                        "Attack Type: " + bossScript.attackType;
    }
}
