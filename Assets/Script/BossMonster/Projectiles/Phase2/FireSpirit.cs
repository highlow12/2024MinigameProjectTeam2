using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireSpirit : NetworkBehaviour
{
    public float MoveTime = 2;
    Vector2 EndPoint = Vector2.left;
    Vector2 TargetPos = new Vector2(0, 0);
    float currentTime = 0;
    [Networked] private TickTimer life { get; set; }
    public List<PlayerRef> playersHit = new();
    public int damage = 10;
    public float speed = 5;
    public float lifeSeconds = 3;
    private int dir = 1;
    public AudioClip attackClip;

    public void Awake()
    {
        currentTime = 0;
        releseTarget();
    }
    public void setTarget(Vector2 vector2)
    {
        TargetPos = vector2;
    }
    public void releseTarget()
    {
        TargetPos = Vector2.right;
    }
    Vector2 GetBesierPosition(Vector2 start,Vector2 end, Vector2 point,float time) 
    {
        time = Mathf.Clamp01(time);

        var AB = Vector2.Lerp(start, point, time);
        var BC = Vector2.Lerp(point, end, time);

        return Vector2.Lerp(AB, BC, time);
    }
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        currentTime += 1 / Runner.TickRate;
        currentTime %= MoveTime;
        if (currentTime < 1)
        {
            //upward curve
            transform.position = GetBesierPosition(TargetPos, EndPoint, Vector2.up, currentTime);
        }
        else
        {
            //downward curve
            transform.position = GetBesierPosition(EndPoint, TargetPos, Vector2.down, currentTime - 1);
        }
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
                    damage = damage
                };
                player.RPC_OnPlayerHit(attackData);

            }
        }
    }
}
