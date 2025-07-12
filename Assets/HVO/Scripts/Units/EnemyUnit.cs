using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    private float m_AttackCommitmentTime = 1f;
    private float m_CurrentAttackCommitmentTime = 0f;

    public override bool IsPlayer => false;

    protected override void UpdateBehaviour()
    {
        switch (CurrentState)
        {
            case UnitState.Idle:

            case UnitState.Moving:
                // Eğer hedef yoksa: algıla ve hedefe yürü
                // Eğer hedef varsa: menzile girmişse saldır, girmemişse yaklaş
                if (HasTarget)
                {
                    if (IsTargetInRange(Target))
                    {
                        SetState(UnitState.Attacking);
                        StopMovement(); // Hareket anında durur
                    }
                    else
                    {
                        MoveTo(Target.transform.position);
                    }
                }
                else
                {
                    if (TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        MoveTo(foe.transform.position);
                    }
                }
                break;

            case UnitState.Attacking:
                // Eğer hedef hâlâ menzildeyse: saldır
                // Değilse: tekrar hareket et veya hedefi kaybet
                if (HasTarget)
                {
                    if (IsTargetInRange(Target))
                    {
                        m_CurrentAttackCommitmentTime = m_AttackCommitmentTime;
                        TryAttackCurrentTarget(); // Artık cooldown’a göre çalışıyor                        
                    }
                    else
                    {
                        m_CurrentAttackCommitmentTime -= Time.deltaTime;
                        if (m_CurrentAttackCommitmentTime <= 0)
                        {
                            // Hedef kaçtıysa hareket moduna dön
                            SetState(UnitState.Moving);
                        }
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }
                break;
        }        
    }
}
