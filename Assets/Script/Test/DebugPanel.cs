using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField]
    private BossBehaviourTree bossBehaviourTree;
    [SerializeField]
    private TestBossMonsterSingle testBossMonsterSingle;
    [SerializeField]
    private TextMeshProUGUI panelText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        panelText.text = "Boss State: " + bossBehaviourTree.currentState + "\n" +
                        "Condition: " + bossBehaviourTree.bossCondition + "\n" +
                        "Condition Duration: " + bossBehaviourTree.conditionDuration + "\n" +
                        "Current Distance: " + bossBehaviourTree.currentDistance + "\n" +
                        "Attack Cooldown: " + bossBehaviourTree.attackCooldown + "\n" +
                        "Is Attacking: " + bossBehaviourTree.isAttacking + "\n" +
                        "Is Moving: " + bossBehaviourTree.isMoving + "\n" +
                        "Max Health: " + testBossMonsterSingle.maxHealth + "\n" +
                        "Current Health: " + testBossMonsterSingle.currentHealth + "\n" +
                        "Boss Scale: " + testBossMonsterSingle.bossScale + "\n" +
                        "Follow Target: " + testBossMonsterSingle.followTarget + "\n" +
                        "Attack Type: " + bossBehaviourTree.bossAttribute["attackType"];
    }
}
