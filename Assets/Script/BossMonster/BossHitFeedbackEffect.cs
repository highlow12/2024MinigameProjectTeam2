using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BossHitFeedbackEffect : NetworkBehaviour
{
    [Networked] public NetworkBool IsCallable { get; set; } = true;
    [Networked] public CustomTickTimer AnimationTimer { get; set; }
    [Networked, OnChangedRender(nameof(PlayEffect))] public float CallTime { get; set; }
    [Networked] public Vector3 CallPositon { get; set; }
    public int attackType;
    public int effectType;
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
            attackType = 0;
            effectType = 0;
            Debug.Log("SetAsDefault");
            AnimationTimer = default;
        }
        _mecanim.Animator.SetInteger("AttackType", attackType);
        _mecanim.Animator.SetInteger("EffectType", effectType);
    }

    public void PlayEffect()
    {
        if (CallTime != 0)
        {
            IsCallable = false;
            transform.position = CallPositon;
            AnimationTimer = CustomTickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }


}
