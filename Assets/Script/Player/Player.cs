using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    readonly int MaxHP = 100;
    [Networked] public int PlayerHP { get; set; }
    public override void Spawned()
    {
        base.Spawned();
        PlayerHP = MaxHP;

        //GetDamage();
    }
    
    
    public void GetDamage(int damage = 10)
    {
        PlayerHP -= damage;
       //Debug.Log("Damaged");
    }
}
