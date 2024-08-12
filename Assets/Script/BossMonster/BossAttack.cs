using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class BossAttack : MonoBehaviour
{
    public float damage;
    public Collider2D attackCollider;
    public List<PlayerRef> playersHit = new();
    public AudioClip attackClip;
    [System.Serializable]
    public struct AttackData : INetworkStruct
    {
        public float damage;
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
                if (player.CharacterClass == (int)CharacterClassEnum.Tank)
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
                    damage = damage
                };
                player.RPC_OnPlayerHit(attackData);
            }
        }
    }

}
