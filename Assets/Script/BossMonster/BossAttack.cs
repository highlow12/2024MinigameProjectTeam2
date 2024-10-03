using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BossAttack : MonoBehaviour
{
    public float damage;
    public bool isApplyKnockback;
    public bool isParryable = true;
    public Collider2D attackCollider;
    public List<PlayerRef> playersHit = new();
    public AudioClip attackClip;
    [System.Serializable]
    public struct AttackData : INetworkStruct
    {
        public float damage;
        public Vector2 knockbackDirection;
        public bool isApplyKnockback;
    }


    void Start()
    {
    }

    void Update()
    {

    }

    public void Attack()
    {
    }

    public void StopAttack()
    {
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
                if (playersHit.Contains(player.Player))
                {
                    return;
                }
                playersHit.Add(player.Player);
                if (player.CharacterClass == (int)CharacterClassEnum.Tank && isParryable)
                {
                    if (player.weapon.isDraw)
                    {
                        player.Skill();
                        SFXManager.Instance.playSFX(attackClip);
                        return;
                    }
                }
                AttackData attackData = new()
                {
                    damage = damage,
                    knockbackDirection = (player.transform.position - transform.position).normalized,
                    isApplyKnockback = isApplyKnockback
                };
                player.RPC_OnPlayerHit(attackData);
            }
        }
    }

}
