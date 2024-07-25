using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : PoolAble
{
    private bool isInit = false;

    void Start()
    {
        if (!isInit)
        {
            isInit = true;
            gameObject.SetActive(false);
        }
    }

    public void Release()
    {
        ReleaseObject();
    }
}
