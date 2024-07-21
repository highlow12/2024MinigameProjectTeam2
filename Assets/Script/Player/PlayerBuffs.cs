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
    
    public GameObject[] buffObjects = new GameObject[6];
    [Networked, Capacity(6)] public NetworkArray<Buff> buffs { get; }
        = MakeInitializer(new Buff[] {});

    public float iconHalfSize;

    public List<int> emptyIndexes {
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

    public int Length {
        get
        {
            return buffs.Length - emptyIndexes.Count();
        }
    }

    void Awake()
    {
        iconHalfSize = buffPrefab.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    public override void Spawned()
    {
        base.Spawned();
        buffIndicator = GameObject.FindGameObjectWithTag("BuffIndicator").GetComponent<TestBuffIndicator>();
        buffIndicator.playerBuffs = this;
        Test();
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
            duration = 5f,
            stacks = 10,
            startTime = Time.time
        };
        
        AddBuff(a);
    }
    
    public void AddBuff(Buff buff)
    {
        try
        {
            int index = buffs
                .Select((x, i) => new { buff = x, index = i })
                .Where(x => x.buff.type == buff.type)
                .First().index;
            
            Debug.Log("Exists");

            Buff cur = buffs[index];
            if (cur.stacks == 0)
            {
                cur.startTime = Time.time;
            }
            else
            {
                cur.stacks += 100;
            }

            buffs.Set(index, cur);
            buffIndicator.reqUpdated = true;
        }
        catch (InvalidOperationException)
        {
            Debug.Log("ADD");
            GameObject obj = Instantiate(buffPrefab, buffIndicator.transform);
            TestBuff nD = obj.GetComponent<TestBuff>();
            nD.indicator = buffIndicator;
            nD.buff = buff;
            buffObjects[emptyIndexes.First()] = obj;
            buffs.Set(emptyIndexes.First(), buff);
            buffIndicator.reqUpdated = true;
        }
    }

    public void SetBuff(Buff buff)
    {
        try
        {
            int index = buffs
                .Select((x, i) => new { buff = x, index = i })
                .Where(x => x.buff.type == buff.type)
                .First().index;

            buffs.Set(index, buff);
        }
        catch (InvalidOperationException)
        {
            buffs.Set(emptyIndexes.First(), buff);
        }
    }

    public void RemoveBuff(int buffType)
    {
        try
        {
            int index = buffs
                .Select((x, i) => new { buff = x, index = i })
                .Where(x => x.buff.type == buffType)
                .First().index;

            Destroy(buffObjects[index]);
            buffs.Set(index, default);
        }
        catch (InvalidOperationException) {}
    }

    public void RemoveBuff(BuffTypes buffType)
    {
        RemoveBuff((int)buffType);
    }
}
