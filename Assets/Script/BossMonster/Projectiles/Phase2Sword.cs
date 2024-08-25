using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2Sword : MonoBehaviour
{
    bool throwing = false;
    float speed = 3;
    Vector3 target = Vector3.zero;
    public void ThrowSword(Vector3 Target)
    {
        throwing = true;
        target = Target;
    }
    public void Gohand()
    {
        throwing = false;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void Update()
    {
        if (throwing) 
        {
            transform.position += (target - transform.position).normalized * speed;
            if (transform.position == target) 
            { 
                throwing = false ;
                Gohand();
            }
        }
    }
}
