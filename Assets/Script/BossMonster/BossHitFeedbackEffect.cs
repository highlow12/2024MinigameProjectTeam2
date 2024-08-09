using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BossHitFeedbackEffect : NetworkBehaviour
{
    [Networked] public NetworkBool IsCallable { get; set; } = true;
    [Networked] public CustomTickTimer AnimationTimer { get; set; }
    [Networked, OnChangedRender(nameof(PlayEffect))] public float CallTime { get; set; }
    public Vector3 callPositon;
    public int attackType;
    public int effectType;
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (AnimationTimer.Expired(Runner))
        {
            IsCallable = true;
            _animator.SetInteger("AttackType", 0);
            _animator.SetInteger("EffectType", 0);
        }
    }

    public void PlayEffect()
    {
        if (HasStateAuthority)
        {
            StartCoroutine(PlayEffectCoroutine());
        }
    }

    public IEnumerator PlayEffectCoroutine()
    {
        if (CallTime != 0)
        {
            IsCallable = false;
            _animator.SetInteger("AttackType", attackType);
            yield return new WaitForFixedUpdate();
            _animator.SetInteger("EffectType", effectType);
            transform.position = callPositon;
            AnimationTimer = CustomTickTimer.CreateFromSeconds(Runner, 0.5f);
        }
        yield return new WaitForFixedUpdate();

    }
}
