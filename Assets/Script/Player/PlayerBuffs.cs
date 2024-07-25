using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Linq;
using Unity.VisualScripting;

public class PlayerBuffs : NetworkBehaviour
{
    public GameObject buffPrefab;
    public TestBuffIndicator buffIndicator;
    
    public TestBuff[] buffObjects = new TestBuff[6];
    [Networked, Capacity(6)] public NetworkArray<Buff> buffs { get; }
        = MakeInitializer(new Buff[] {});

    public float iconHalfSize;

    public List<int> emptyIndexes
    {
        get
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < buffs.Length; i++)
            {
                if (buffs[i].type == 0) temp.Add(i);
            }
            return temp;
        }
    }

    public int Length
    {
        get
        {
            return buffs.Length - emptyIndexes.Count();
        }
    }

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
        return GetIndex(type);
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
        if (!buffs[0].Equals(default))
        {
            if (HasInputAuthority)
            {
                
            }
        }
    }

    public void Test()
    {
        Buff a = new Buff
        {
            type = (int)BuffTypes.Burn,
            duration = 10f,
            stacks = 100,
            startTime = Time.time
        };
        Buff b = new Buff
        {
            type = (int)BuffTypes.Blind,
            duration = 5f,
            stacks = 100,
            startTime = Time.time
        };
        
        AddBuff(a);
        AddBuff(b);
    }
    
    public void AddBuff(Buff buff)
    {
        int index = GetIndex(buff.type);
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
            buffs.Set(index, cur);
            buffIndicator.reqUpdated = true;
        }
        else
        {
            int _index = emptyIndexes.First();
            GameObject obj = Instantiate(buffPrefab, buffIndicator.transform);
            TestBuff nD = obj.GetComponent<TestBuff>();
            nD.indicator = buffIndicator;
            nD.buff = buff;
            buffObjects[_index] = nD;
            buffs.Set(_index, buff);
            buffIndicator.reqUpdated = true;
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
        if (index >= 0) 
        {
            buffs.Set(index, buff);
            buffObjects[index].buff = buff;
        }
        else 
        {
            int _index = emptyIndexes.First();
            GameObject obj = Instantiate(buffPrefab, buffIndicator.transform);
            TestBuff nD = obj.GetComponent<TestBuff>();
            nD.indicator = buffIndicator;
            nD.buff = buff;
            buffObjects[_index] = nD;
            buffs.Set(_index, buff);
        }
    }

    public void RemoveBuff(int buffType)
    {
        int index = GetIndex(buffType);
        if (index == -1) return;
        Destroy(buffObjects[index]);
        buffs.Set(index, default);
    }

    public void RemoveBuff(BuffTypes buffType)
    {
        RemoveBuff(buffType);
    }
}
