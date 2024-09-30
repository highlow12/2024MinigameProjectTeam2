using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BossHitFeedbackEffect : NetworkBehaviour
{
    [Networked] public NetworkBool IsCallable { get; set; } = true;
    [Networked] public CustomTickTimer AnimationTimer { get; set; }
    [Networked] public float CallTime { get; set; }
    [Networked] public Vector3 CallPositon { get; set; }
    [Networked] public int AttackType { get; set; }
    [Networked] public int EffectType { get; set; }
    [Networked] public int ParentScale { get; set; }
    NetworkMecanimAnimator _mecanim;

    void Awake()
    {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
    }

    public override void Render()
    {
        if (!Equals(AnimationTimer, default(CustomTickTimer)) && AnimationTimer.Expired(Runner))
        {
            IsCallable = true;
            AttackType = 0;
            EffectType = 0;
            AnimationTimer = default;
        }
        if (ParentScale != (transform.parent.localScale.x > 0 ? 1 : -1))
        {
            ParentScale = transform.parent.localScale.x > 0 ? 1 : -1;
            transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
        _mecanim.Animator.SetInteger("AttackType", AttackType);
        _mecanim.Animator.SetInteger("EffectType", EffectType);
    }

    public void PlayEffect()
    {
        if (CallTime != 0)
        {
            IsCallable = false;
            transform.position = CallPositon;
            ParentScale = transform.parent.localScale.x > 0 ? 1 : -1;
            AnimationTimer = CustomTickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }


}
