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
                if (playersHit.Contains(player.player))
                {
                    return;
                }
                AttackData attackData = new()
                {
                    damage = damage
                };
                player.RPC_OnPlayerHit(attackData);
                playersHit.Add(player.player);
            }
        }
    }

}
