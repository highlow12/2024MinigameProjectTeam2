using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Attack : MonoBehaviour

{

    public float damage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public struct AttackData : INetworkStruct
    {
        public float damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            BossMonsterNetworked boss = other.gameObject.GetComponent<BossMonsterNetworked>();
            if (boss)
            {
                AttackData attackData = new()
                {
                    damage = damage
                };
                boss.Rpc_OnBossHit(attackData);
            }
        }
    }

}
