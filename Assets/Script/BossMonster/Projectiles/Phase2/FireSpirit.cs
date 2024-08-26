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
}
