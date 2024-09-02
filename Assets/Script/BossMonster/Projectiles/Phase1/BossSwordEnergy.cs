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
    private int dir = 1;
    public AudioClip attackClip;
    public void Init(int _dir)
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeSeconds);

        dir = _dir / Mathf.Abs(_dir);

        transform.localScale = new Vector3(dir, 1);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += speed * transform.right * Runner.DeltaTime * dir;
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
                playersHit.Add(player.Player);
                if (player.CharacterClass == (int)CharacterClassEnum.Tank)
                {
                    if (player.weapon.isDraw)
                    {
                        player.Skill();
                        Runner.Despawn(Object);
                        SFXManager.Instance.playSFX(attackClip);
                        return;
                    }
                }
                BossAttack.AttackData attackData = new()
                {
                    damage = damage,
                    knockbackDirection = Vector2.zero,
                    isApplyKnockback = false
                };
                player.RPC_OnPlayerHit(attackData);

            }
        }
    }
}
