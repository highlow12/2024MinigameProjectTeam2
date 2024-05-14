using System.Collections;
using UnityEngine;
using Items;

public class Katana : Weapon
{
    private bool isAttackCooldown = false;
    public Katana()
    {
        attackSpeed = 2.0f;
        range = 2.0f;
        damage = 60;
        isRangeObjectSpawned = false;
        rangeObject = Resources.Load<GameObject>("KatanaRange");
    }

    public override IEnumerator Attack(Animator anim, Transform character)
    {
        if (!isRangeObjectSpawned)
        {
            rangeObject = GameObject.Instantiate(rangeObject, character);
            rangeObject.GetComponent<Attack>().damage = damage;
            isRangeObjectSpawned = true;
        }
        if (!isAttackCooldown)
        {
            var randomAttackAnim = Random.Range(1, 4);
            anim.SetTrigger($"Attack{randomAttackAnim}");
            rangeObject.GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(0.1f);
            rangeObject.GetComponent<Collider2D>().enabled = false;
            isAttackCooldown = true;
            yield return new WaitForSeconds(1.0f / attackSpeed - 0.1f);
            isAttackCooldown = false;
        }
        else
        {
            yield return null;
        }
    }


}
