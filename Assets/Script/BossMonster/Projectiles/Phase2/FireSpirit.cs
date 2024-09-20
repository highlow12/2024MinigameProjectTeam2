using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireSpirit : NetworkBehaviour
{
    public float MoveTime = 2;
    public int TurnCount = 3;
    Vector2 EndPoint = Vector2.left;
    Vector2 TargetPos = Vector2.right;
    float currentTime = 0;
    [Networked] private CustomTickTimer life { get; set; }
    public List<PlayerRef> playersHit = new();
    public int damage = 10;
    public float speed = 5;
    public float lifeSeconds = 3;
    private int dir = 1;
    public AudioClip attackClip;
    Vector2 e1;
    Vector2 e2;
    BossMonsterNetworked Boss;

    public void Start()
    {
        Boss = transform.parent.GetComponent<BossMonsterNetworked>();
        currentTime = 0;
        EndPoint = Boss.transform.position;
        life = CustomTickTimer.CreateFromSeconds(Runner, TurnCount * MoveTime);
        updateEndpoint();
    }
    
    public void setTarget(Vector2 vector2)
    {
        TargetPos = vector2;
    }
    public void releseTarget()
    {
        TargetPos = Vector2.right;
    }
    void updateEndpoint(){
        e1 = transform.position;
        e2 = Boss.transform.position;
    }
    Vector2 GetBesierPosition(Vector2 start,Vector2 end,float time) 
    {
        time = Mathf.Clamp01(time);
        
        Vector2 point = new((start.x + end.x)/2, (start.y + end.y)/2 + Mathf.Sign(start.x - end.x) );

        var AB = Vector2.Lerp(start, point, time);
        var BC = Vector2.Lerp(point, end, time);

        return Vector2.Lerp(AB, BC, time);
    }
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        
        

        currentTime += 1 * Time.fixedDeltaTime;
        
        if (currentTime < 1)
        {
            //upward curve
            transform.position = GetBesierPosition(e1, TargetPos, currentTime);
            
        }
        else
        {
            //downward curve
            transform.position = GetBesierPosition(TargetPos, e2, currentTime - 1);
        }

        if (currentTime >= 2)
        {
            currentTime = 0;
            if (!life.Expired(Runner))
            {
                setTarget(Boss.FollowTarget.transform.position);
                updateEndpoint();
                //EndPoint = Boss.transform.position + Vector3.up;
            }
            else
            {
                Runner.Despawn(Object);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("playerHit");
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
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
    void OnDrawGizmos()
    {

        Gizmos.color = Color.red; // 곡선 색상 설정
        Debug.Log(TargetPos);
        for (float t = 0; t <= 1; t += 0.1f)
        {
            Vector2 position = GetBesierPosition(EndPoint, TargetPos, t);
            Gizmos.DrawSphere(position, 0.1f); // 곡선의 각 점에 구체를 그립니다.
        }

        for (float t = 0; t <= 1; t += 0.1f)
        {
            Vector2 position = GetBesierPosition(TargetPos,EndPoint, t);
            Gizmos.DrawSphere(position, 0.1f); // 곡선의 각 점에 구체를 그립니다.
        }
    }
}
