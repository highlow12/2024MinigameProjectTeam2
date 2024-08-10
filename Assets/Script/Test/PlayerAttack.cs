using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerAttack : MonoBehaviour
{
    // Enum for attack types
    public enum AttackType
    {
        Katana = 1,
        ProjectileOrShield = 2,
        
    }
    public AttackType attackType;
    public float damage;
    public bool isHit;
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
        public AttackType attackType;
        public Vector3 hitPosition;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            BossMonsterNetworked boss = other.gameObject.GetComponent<BossMonsterNetworked>();
            if (boss && !isHit)
            {
                AttackData attackData = new()
                {
                    attackType = attackType,
                    damage = damage,
                    hitPosition = other.ClosestPoint(transform.position)
                };
                boss.Rpc_OnBossHit(attackData);
                isHit = true;
            }
        }
    }

}
