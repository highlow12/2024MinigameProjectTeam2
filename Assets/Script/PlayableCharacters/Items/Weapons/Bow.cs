using System.Collections;
using UnityEngine;
using Items;
using Fusion;
using System.Linq;

public class Bow : Weapon
{
    [System.Serializable]
    private class MultiShotArrowProperties
    {
        public Vector3 position;
        public Vector3 rotation;
        public float range;
        public float damage;
    }

    private bool isAttackCooldown = false;
    private MultiShotArrowProperties[] multiShotArrows = new MultiShotArrowProperties[3];

    // initialize bow properties
    public Bow(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
        projectileSpeed = 30.0f;
        range = 20.0f;
        damage = 50.0f;
    }

    // initialize multi shot arrow properties
    private void InitMultiShotArrows()
    {
        multiShotArrows[0] = new MultiShotArrowProperties
        {
            position = new Vector3(1.4f, 0.57f, 0),
            rotation = new Vector3(0, 0, -10.0f),
            damage = 25.0f,
            range = 10.0f
        };

        multiShotArrows[1] = new MultiShotArrowProperties
        {
            position = new Vector3(1.3f, 0.1f, 0),
            rotation = new Vector3(0, 0, -20.0f),
            damage = 25.0f,
            range = 10.0f
        };

        multiShotArrows[2] = new MultiShotArrowProperties
        {
            position = new Vector3(0.9f, -0.25f, 0),
            rotation = new Vector3(0, 0, -26.0f),
            damage = 25.0f,
            range = 10.0f
        };
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        if (multiShotArrows[0] == null)
        {
            InitMultiShotArrows();
        }
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay

            if (attackState == 3)
            {
                attackState = 0;
            }
            attackState++;
            // 애니메이션 배속
            anim.SetFloat("AttackAnimSpeed", 0.5f * attackSpeed);
            anim.SetInteger("AttackState", attackState);
            prevAttack = Time.time;
            anim.SetFloat("PrevAttack", prevAttack);
            mecanim.SetTrigger("Attack", true);
            anim.SetBool("Combo", true);
            isAttackCooldown = true;
            yield return new WaitForSeconds(1.0f / attackSpeed);
            isAttackCooldown = false;
            yield return new WaitForSeconds(0.3f);
            if (Time.time - prevAttack > 1.0f / attackSpeed + 0.3f)
            {
                anim.SetInteger("AttackState", 0);
                anim.SetBool("Combo", false);
                attackState = 0;
            }
        }
        else
        {
            yield return null;
        }
    }

    public override void FireProjectileAlt(int state, Transform character)
    {
        if (state == 2)
        {
            MultiShot(character);
        }
        else
        {
            RPC_SpawnProjectile(character, new Vector3(0.3f, -0.1f), Vector3.zero, damage, range);
            RPC_SpawnEffect(1, character, new Vector3(0.3f, -0.1f, 0), Vector3.zero);
        }
    }

    public override IEnumerator FireProjectile(Animator anim, Transform character)
    {
        if (anim.GetInteger("AttackState") == 2)
        {
            MultiShot(character);
            yield return null;
        }
        else
        {
            RPC_SpawnProjectile(character, new Vector3(0, 1.5f), Vector3.zero, damage, range);
            RPC_SpawnEffect(1, character, new Vector3(0.3f, -0.1f, 0), Vector3.zero);
            yield return null;
        }
    }

    public void MultiShot(Transform character)
    {
        if (multiShotArrows[0] == null)
        {
            InitMultiShotArrows();
        }
        for (int i = 0; i < 3; i++)
        {
            RPC_SpawnProjectile(
                character,
                multiShotArrows[i].position,
                multiShotArrows[i].rotation,
                multiShotArrows[i].damage,
                multiShotArrows[i].range
            );

            if (i == 0)
            {
                RPC_SpawnEffect(2, character, new Vector3(0.3f, -0.1f, 0), Vector3.zero);
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SpawnProjectile(Transform transform, Vector3 _pos, Vector3 rotation, float damage, float range)
    {
        NetworkRunner runner = NetworkRunner.Instances.First();

        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        NetworkObject projectile = runner.Spawn(dynamicObjectProvider.arrowPrefab, Vector3.zero, Quaternion.identity, null);
        // projectile.transform.position = pos + new Vector3(scale.x * 1.5f, 0, 0);
        projectile.transform.position = pos + new Vector3(scale.x * _pos.x, _pos.y);
        projectile.transform.localScale = scale;

        Base arrow = projectile.GetComponent<Base>();
        arrow.projectileSpeed = projectileSpeed;
        arrow.damage = damage;
        arrow.range = range;

        GameObject projectileObject = arrow.projectile;
        projectileObject.transform.localPosition = new Vector3(0, 0, 0);
        rotation.z *= scale.x;
        projectileObject.transform.rotation = Quaternion.Euler(rotation);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SpawnEffect(int shotType, Transform transform, Vector3 _pos, Vector3 rotation)
    {
        NetworkRunner runner = NetworkRunner.Instances.First();

        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        NetworkObject projectileEffect = runner.Spawn(dynamicObjectProvider.arrowEffectPrefab, Vector3.zero, Quaternion.identity, null);
        projectileEffect.transform.localPosition = new Vector3(0, 0, 0);
        projectileEffect.transform.position = pos + new Vector3(scale.x * _pos.x, _pos.y, 0);
        projectileEffect.transform.localScale = scale;
        projectileEffect.transform.rotation = Quaternion.Euler(rotation);
        projectileEffect.GetComponent<ArrowEffect>().ShotType = shotType;
    }

}
