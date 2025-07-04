using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
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
                    if (IsTargetInRange(Target.transform))
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
                    if (IsTargetInRange(Target.transform))
                    {
                        TryAttackCurrentTarget(); // Artık cooldown’a göre çalışıyor
                    }
                    else
                    {
                        // Hedef kaçtıysa hareket moduna dön
                        SetState(UnitState.Moving);
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
