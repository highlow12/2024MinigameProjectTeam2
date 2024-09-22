using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ScheduledBehaviors : NetworkBehaviour
{
    public enum RunBy
    {
        Default,
        Tick,
        Health
    }
    [Serializable]
    public struct Behavior : INetworkStruct
    {
        public int tick;
        public int renewTick;
        public int phase;
        public int pendedTick;
        public float healthRatio;
        public float renewHealthRatio;
        public NetworkString<_128> skillName;
        public NetworkBool canPend;
        public NetworkBool canRenew;
        public NetworkBool isPending;
        public RunBy runBy;
    }

    public static ScheduledBehaviors instance;
    public int pendingTickThreshold = 5; // 5 seconds

    [Networked]
    [Capacity(20)]
    public NetworkArray<Behavior> Behaviors { get; }
        = MakeInitializer(new Behavior[] { });

    void Start()
    {
        instance = this;
        pendingTickThreshold *= Runner.TickRate;
        AddBehavior(new Behavior
        {
            tick = 0,
            phase = 1,
            healthRatio = 0.99f,
            skillName = "JumpAttack",
            canPend = true,
            canRenew = true,
            renewHealthRatio = 0.1f,
            runBy = RunBy.Health
        });
    }

    public override void FixedUpdateNetwork()
    {

    }


    public void AddBehavior(Behavior behavior)
    {
        int index = GetIndex();
        if (index == -1)
        {
            return;
        }
        Behaviors.Set(index, behavior);
    }

    int GetIndex(Behavior behaviour = default)
    {
        if (!Equals(behaviour, default))
        {
            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Equals(Behaviors[i], behaviour))
                {
                    return i;
                }
            }
        }
        for (int i = 0; i < Behaviors.Length; i++)
        {
            if (Equals(Behaviors[i].runBy, RunBy.Default))
            {
                return i;
            }
        }
        return -1;
    }

    public Behavior GetBehavior(float maxHealth = -1, float currentHealth = -1, bool isAttacking = false, int phase = 1)
    {
        int currentServerTick = (int)Runner.Tick;
        // if pending behavior exists, execute it first
        List<Behavior> pendingBehaviors = Behaviors.Select(behavior => behavior).Where(behavior => behavior.isPending).ToList();
        if (pendingBehaviors.Count > 0)
        {
            pendingBehaviors.Sort((a, b) => a.pendedTick.CompareTo(b.pendedTick));
            return pendingBehaviors[0];
        }
        foreach (var behavior in Behaviors)
        {
            if (behavior.runBy == RunBy.Tick)
            {
                if (behavior.tick == currentServerTick)
                {
                    return CheckBehavior(behavior, isAttacking, phase);
                }
            }
            else if (behavior.runBy == RunBy.Health)
            {
                if (maxHealth != -1 && currentHealth != -1)
                {
                    if ((currentHealth / maxHealth <= behavior.healthRatio) || (phase != behavior.phase) /* for remove behavior */)
                    {
                        return CheckBehavior(behavior, isAttacking, phase);
                    }
                }
            }
        }
        return default;
    }

    public Behavior CheckBehavior(Behavior behavior, bool isAttacking, int phase)
    {
        int behaviorIndex = GetIndex(behavior);
        if (phase != behavior.phase)
        {
            RemoveBehavior(behaviorIndex);
            return default;
        }
        if (isAttacking && behavior.canPend)
        {
            // if already pending, not update the pending tick
            if (behavior.isPending)
            {
                // if pending for too long, force execute the behavior
                if ((int)Runner.Tick - behavior.pendedTick > pendingTickThreshold)
                {
                    behavior.isPending = false;
                    behavior.pendedTick = 0;
                }
                return default;
            }
            else
            {
                behavior.isPending = true;
                behavior.pendedTick = (int)Runner.Tick + 1;
                Behaviors.Set(behaviorIndex, behavior);
                return default;
            }
        }
        if (behavior.canRenew)
        {
            switch (behavior.runBy)
            {
                case RunBy.Health:
                    behavior.healthRatio -= behavior.renewHealthRatio;
                    break;
                case RunBy.Tick:
                    behavior.tick = (int)Runner.Tick + behavior.renewTick;
                    break;
            }
            Behaviors.Set(behaviorIndex, behavior);
        }
        else
        {
            RemoveBehavior(behaviorIndex);
        }
        return behavior;
    }

    public void RemoveBehavior(int index)
    {
        Behaviors.Set(index, default);
    }

}
