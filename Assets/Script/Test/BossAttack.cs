using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class BossAttack : MonoBehaviour
{
    public float damage;
    public Collider2D attackCollider;
    public List<PlayerRef> playersHit = new();
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
                if (player.CharacterClass == (int)CharacterClassEnum.Tank)
                {
                    if (player.weapon.isdraw)
                    {
                        return;
                    }
                }
                if (playersHit.Contains(player.Player))
                {
                    return;
                }
                AttackData attackData = new()
                {
                    damage = damage
                };
                player.RPC_OnPlayerHit(attackData);
                playersHit.Add(player.Player);
            }
        }
    }

}
