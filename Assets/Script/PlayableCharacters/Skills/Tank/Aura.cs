using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Aura : NetworkBehaviour
{

    [Networked] public NetworkBool IsSkillEnabled { get; set; } = false;
    public AudioClip sfx;

    [SerializeField] PlayerControllerNetworked player;
    NetworkMecanimAnimator _mecanim;

    // This struct is used to send the aura buffs to the player
    [System.Serializable]
    public struct AuraBuffs : INetworkStruct
    {
        public float attackSpeedMultiplier;
        public float moveSpeedMultiplier;
        public float damageMultiplier;
        public float healthRegen;
        public float healthRegenInterval;
    }

    AuraBuffs auraBuffs = new()
    {
        attackSpeedMultiplier = 1.2f,
        moveSpeedMultiplier = 1.15f,
        damageMultiplier = 1.3f,
        healthRegen = 20f,
        healthRegenInterval = 0.5f
    };

    void Awake()
    {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
    }

    public override void Render()
    {
        if (player)
        {
            // If the skill is enabled, play the animation and apply the aura buffs to the caster
            _mecanim.Animator.SetBool("Enabled", IsSkillEnabled);

        }
    }


    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (player)
        {
            if (IsSkillEnabled && HasStateAuthority)
            {
                player.RPC_OnPlayerInSupporterAura(auraBuffs);
            }
        }
    }

    public void EnableSkill(float duration)
    {
        IsSkillEnabled = true;
        StartCoroutine(DisableSkill(duration));
        SFXManager.Instance.playSFX(sfx);
    }

    IEnumerator DisableSkill(float duration)
    {
        CustomTickTimer timer = CustomTickTimer.CreateFromSeconds(Runner, duration);
        while (timer.Expired(Runner) == false)
        {
            yield return new WaitForFixedUpdate();
        }
        IsSkillEnabled = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // If the skill is not enabled, return
        if (IsSkillEnabled == false)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
                player.RPC_OnPlayerInSupporterAura(auraBuffs);
                // Buff auraBuff = new()
                // {
                //     type = (int)BuffTypes.Pray,
                //     stacks = 1,
                //     duration = auraBuffs.healthRegenDuration,
                //     startTime = Time.time
                // };
                // player.buffs.SetBuff(auraBuff);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // If the skill is not enabled, return
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
            }
        }
    }

}
