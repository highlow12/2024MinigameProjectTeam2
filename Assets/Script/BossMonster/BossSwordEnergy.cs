using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class BossSwordEnergy : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    public List<PlayerRef> playersHit = new();
    public int damage = 10;
    public float speed = 5;
    public float lifeSeconds = 3;
    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeSeconds);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += speed * transform.right * Runner.DeltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
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
                if (player.CharacterClass == (int)CharacterClassEnum.Tank)
                {
                    if (player.weapon.isDraw)
                    {
                        player.Skill();
                        Runner.Despawn(Object);
                        return;
                    }
                }
                BossAttack.AttackData attackData = new()
                {
                    damage = damage
                };
                player.RPC_OnPlayerHit(attackData);
                playersHit.Add(player.Player);

            }
        }
    }
}
