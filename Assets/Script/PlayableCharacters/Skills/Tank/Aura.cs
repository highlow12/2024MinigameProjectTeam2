using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Aura : NetworkBehaviour
{

    [Networked] public NetworkBool IsSkillEnabled { get; set; } = false;

    [SerializeField] PlayerControllerNetworked player;

    [System.Serializable]
    public struct AuraBuffs : INetworkStruct
    {
        public float attackSpeedMultiplier;
        public float moveSpeedMultiplier;
        public float damageMultiplier;
        public float healthRegen;
        public float healthRegenDuration;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            GetComponent<Animator>().SetBool("Enabled", IsSkillEnabled);
        }
    }

    public void ToggleSkill()
    {
        IsSkillEnabled = !IsSkillEnabled;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (IsSkillEnabled == false)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
                AuraBuffs auraBuffs = new()
                {
                    attackSpeedMultiplier = 1.2f,
                    moveSpeedMultiplier = 1.15f,
                    damageMultiplier = 1.3f,
                    healthRegen = 1f,
                    healthRegenDuration = 0.5f
                };
                player.RPC_OnPlayerInSupporterAura(auraBuffs);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNetworked player = other.gameObject.GetComponent<PlayerControllerNetworked>();
            if (player)
            {
                AuraBuffs auraBuffs = new()
                {
                    attackSpeedMultiplier = 1.0f,
                    moveSpeedMultiplier = 1.0f,
                    damageMultiplier = 1.0f,
                    healthRegen = 0f
                };
                player.RPC_OnPlayerInSupporterAura(auraBuffs);
            }
        }
    }

}
