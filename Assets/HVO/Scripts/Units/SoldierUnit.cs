using UnityEngine;

public class SoldierUnit : HumanoidUnit
{
    private bool m_IsRetreating = false;

    protected override void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        // Eğer yeni görev "Attack" ise ve geçerli bir hedef varsa
        if (newTask == UnitTask.Attack && HasTarget)
        {
            // Hedefin pozisyonuna doğru hareket başlat
            MoveTo(Target.transform.position);
        }

        base.OnSetTask(oldTask, newTask);
    }

    protected override void OnSetDestination()
    {
        if (HasTarget && (CurrentTask == UnitTask.Attack || CurrentState == UnitState.Attacking))
        {
            m_IsRetreating = true;
        }

        if (CurrentTask == UnitTask.Attack)
            {
                SetTask(UnitTask.None);
                SetTarget(null);
            }
    }

    protected override void OnDestinationReached()
    {
        if (m_IsRetreating)
        {
            m_IsRetreating = false;
        }
    }

    // Birimin davranışlarını her frame güncelleyen metot.
    // Basit bir state machine (durum makinesi) yapısı kullanır.
    protected override void UpdateBehaviour()
    {
        // Eğer birim boştaysa (Idle) ya da hareket halindeyse (Moving)
        if (CurrentState == UnitState.Idle || CurrentState == UnitState.Moving)
        {
            // Hedef varsa
            if (HasTarget)
            {
                // Ve hedef menzildeyse
                if (IsTargetInRange(Target.transform))
                {
                    // Hareketi durdur ve saldırı durumuna geç
                    StopMovement();
                    SetState(UnitState.Attacking);
                }
            }
            else
            {
                if (!m_IsRetreating && TryFindClosestFoe(out var foe))
                {
                    SetTarget(foe);
                    SetTask(UnitTask.Attack);
                }
            }
        }
        // Eğer birim saldırı halindeyse
        else if (CurrentState == UnitState.Attacking)
        {
            // Hedef varsa
            if (HasTarget)
            {
                // Ve hedef hâlâ menzildeyse saldır
                if (IsTargetInRange(Target.transform))
                {
                    TryAttackCurrentTarget();
                }
                // Hedef menzilden çıktıysa, idle durumuna dön (takip için)
                else
                {
                    SetState(UnitState.Idle);
                }
            }
            // Hedef yoksa, idle durumuna geç
            else
            {
                SetState(UnitState.Idle);
            }
        }
    }
}
