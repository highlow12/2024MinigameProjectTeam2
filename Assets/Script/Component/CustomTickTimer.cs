using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct CustomTickTimer : INetworkStruct
{
    private int _target;
    private int _initialTick;
    public bool Expired(NetworkRunner runner) => runner.IsRunning && _target > 0
      && (Tick)_target <= runner.Tick;

    public bool IsRunning => _target > 0;

    public static CustomTickTimer CreateFromTicks(NetworkRunner runner, int ticks)
    {
        if (runner == false || runner.IsRunning == false)
            return new CustomTickTimer();

        CustomTickTimer fromTicks = new()
        {
            _target = (int)runner.Tick + ticks,
            _initialTick = runner.Tick
        };
        return fromTicks;
    }

    public static CustomTickTimer CreateFromSeconds(NetworkRunner runner, float seconds)
    {
        if (runner == false || runner.IsRunning == false)
            return new CustomTickTimer();

        CustomTickTimer fromSeconds = new()
        {
            _target = (int)(runner.Tick + (seconds * runner.TickRate)),
            _initialTick = runner.Tick
        };
        return fromSeconds;
    }

    public float NormalizedValue(NetworkRunner runner)
    {
        if (runner == null || runner.IsRunning == false || IsRunning == false)
            return 0;

        if (Expired(runner))
            return 1;

        return ElapsedTicks(runner) / (_target - (float)_initialTick);
    }

    public int ElapsedTicks(NetworkRunner runner)
    {
        if (runner == false || runner.IsRunning == false)
            return 0;

        if (IsRunning == false || Expired(runner))
            return 0;

        return runner.Tick - _initialTick;
    }
}