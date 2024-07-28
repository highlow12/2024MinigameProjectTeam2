using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Linq;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerBuffs : NetworkBehaviour
{
    public GameObject buffPrefab;
    public TestBuffIndicator buffIndicator;
    
    public TestBuff[] buffObjects = new TestBuff[6];
    [Networked] bool reqUpdate { get; set; } = false;
    [Networked, Capacity(6)] public NetworkArray<Buff> buffs { get; }
        = MakeInitializer(new Buff[] {});

    public float iconHalfSize;

    // public List<int> emptyIndexes
    // {
    //     get
    //     {
    //         List<int> temp = new List<int>();
    //         for (int i = 0; i < buffs.Length; i++)
    //         {
    //             if (buffs[i].type == 0) temp.Add(i);
    //         }
    //         return temp;
    //     }
    // }

    private int _index = 0;
    public int index {
        get 
        {
            return _index++ % buffs.Length;
        }
    }

    // public int Length
    // {
    //     get
    //     {
    //         return buffs.Length - emptyIndexes.Count();
    //     }
    // }

    public int GetIndex(int type)
    {
        try
        {
            int index = buffs
                .Select((x, i) => new { buff = x, index = i })
                .Where(x => x.buff.type == type)
                .First().index;
            return index;
        }
        catch (InvalidOperationException)
        {
            return -1;
        }
    }

    public int GetIndex(BuffTypes type)
    {
        return GetIndex((int)type);
    }

    void Awake()
    {
        iconHalfSize = buffPrefab.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        if (!reqUpdate) return;
        for (int i = 0; i < buffs.Length; i++)
        {
            // Debug.Log($"[{i}] {buffs[i].type} {buffObjects[i]} {buffs[i].type == 0}");
            if (buffs[i].type == 0)
            {
                if (buffObjects[i] != null) {
                    Destroy(buffObjects[i].gameObject);
                }
            }
            else if (buffObjects[i] == null)
            {
                GameObject obj = Instantiate(buffPrefab, buffIndicator.transform);
                TestBuff nD = obj.GetComponent<TestBuff>();
                nD.indicator = buffIndicator;
                buffObjects[i] = nD;
                buffObjects[i].buff = buffs[i];
                buffObjects[i].gameObject.SetActive(true);
            }
            else
            {
                buffObjects[i].buff = buffs[i];
                
            }
        }
        buffIndicator.reqUpdated = true;
        reqUpdate = false;
        RPC_UpdateDone();
    }

    public void Test()
    {
        Buff a = new Buff
        {
            type = (int)BuffTypes.Burn,
            duration = 10f,
            stacks = 3,
            startTime = Time.time
        };
        Buff b = new Buff
        {
            type = (int)BuffTypes.Blind,
            duration = 5f,
            stacks = 3,
            startTime = Time.time
        };
        
        // RPC_SetBuff(0, a);
        // RPC_SetBuff(1, b);
        SetBuff(a);
        SetBuff(b);
    }

    public void Test2()
    {
        AddBuff(BuffTypes.Burn);
    }
    
    public void AddBuff(BuffTypes type)
    {
        int index = GetIndex((int)type);
        if (index >= 0)
        {
            Buff cur = buffs[index];
            if (cur.stacks == 0)
            {
                cur.startTime = Time.time;
            }
            else
            {
                cur.stacks += 1;
            }

            buffObjects[index].buff = cur;
            RPC_SetBuff(index, cur);
        }
    }

    public Buff GetBuff(BuffTypes type)
    {
        int index = GetIndex(type);
        if (index == -1) return default;
        else return buffs[index];
    }

    public void SetBuff(Buff buff)
    {
        int index = GetIndex(buff.type);
        if (index == -1) index = this.index;
        Debug.Log($"SetBuff [{index}] = {buff.type}");
        RPC_SetBuff(index, buff);
    }

    public void RemoveBuff(int buffType)
    {
        int index = GetIndex(buffType);
        if (index == -1) return;
        RPC_SetBuff(index, default);
    }

    public void RemoveBuff(BuffTypes buffType)
    {
        RemoveBuff((int)buffType);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, TickAligned = false)]
    public void RPC_SetBuff(int index, Buff buff)
    {
        buffs.Set(index, buff);
        reqUpdate = true;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_UpdateDone()
    {
        reqUpdate = false;
    }
}
