using System.Collections;
using UnityEngine;

public class RangerUnit : SoldierUnit
{
    [SerializeField] private Projectile m_ProjectilePrefab;

    protected override void OnAttackReady(Unit target)
    {
        OnPlayAttackSound();
        PerformAttackAnimation();
        StartCoroutine(ShootProjectile(0.4f, target));
    }

    private IEnumerator ShootProjectile(float delay, Unit target)
    {
        yield return new WaitForSeconds(delay);

        if (CurrentState == UnitState.Dead) yield return null;

        if (target != null && target.CurrentState != UnitState.Dead)
        {
            var Projectile = Instantiate(m_ProjectilePrefab, transform.position, Quaternion.identity);
            Projectile.Initialize(this, target, m_AutoAttackDamage);
        }
    }
}
