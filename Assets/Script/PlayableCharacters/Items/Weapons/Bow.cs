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

    public NetworkObject arrowPrefab;
    public NetworkObject arrowEffectPrefab;

    private bool isAttackCooldown = false;
    private CustomTickTimer attackTimer;
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
            NetworkRunner runner = NetworkRunner.Instances.First();
            if (controller.AttackState == 3)
            {
                controller.AttackState = 0;
            }
            controller.AttackState++;
            // 애니메이션 배속
            controller.AttackAnimSpeed = 0.5f * attackSpeed;
            controller.AttackState = controller.AttackState;
            prevAttack = (int)runner.Tick;
            controller.PrevAttack = prevAttack;
            controller.Attack = true;
            controller.Combo = true;
            isAttackCooldown = true;
            attackTimer = CustomTickTimer.CreateFromSeconds(runner, 1.0f / attackSpeed);
            while (attackTimer.Expired(runner) == false)
            {
                yield return new WaitForFixedUpdate();
            }
            isAttackCooldown = false;
            if (attackState < 3)
            {
                attackTimer = CustomTickTimer.CreateFromSeconds(runner, 0.3f);
                while (attackTimer.Expired(runner) == false)
                {
                    yield return new WaitForFixedUpdate();
                }
                if (((int)runner.Tick - prevAttack) / runner.TickRate >= 1.0f / attackSpeed + (0.3f - 3.0f / runner.TickRate))
                {
                    controller.AttackState = 0;
                    controller.Combo = false;
                    attackState = 0;
                }
            }
        }
        else
        {
            yield return null;
        }
    }

    public override void FireProjectileAlt(int state, Transform character)
    {
        if (arrowPrefab == null || arrowEffectPrefab == null)
        {
            // arrow prefab id: 9
            // arrow effect prefab id: 10
            arrowPrefab = NetworkRunner.Instances.First().Prefabs.Load(NetworkPrefabId.FromIndex(9), false);
            arrowEffectPrefab = NetworkRunner.Instances.First().Prefabs.Load(NetworkPrefabId.FromIndex(10), false);

        }
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
        if (controller.AttackState == 2)
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
        // Check if this is the client
        if (runner.IsClient)
        {
            return;
        }

        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        NetworkObject projectile = runner.Spawn(arrowPrefab, pos + new Vector3(scale.x * _pos.x, _pos.y), Quaternion.identity, null);
        projectile.transform.localScale = scale;

        Base arrow = projectile.GetComponent<Base>();
        arrow.projectileSpeed = projectileSpeed;
        arrow.damage = damage * damageMultiplier;
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
        // Check if this is the client
        if (runner.IsClient)
        {
            return;
        }
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        NetworkObject projectileEffect = runner.Spawn(arrowEffectPrefab, pos + new Vector3(scale.x * _pos.x, _pos.y, 0), Quaternion.Euler(rotation), null);
        projectileEffect.transform.localScale = scale;
        projectileEffect.GetComponent<ArrowEffect>().ShotType = shotType;
    }

}
