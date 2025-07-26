using UnityEngine;

public class SoldierUnit : HumanoidUnit
{
    private bool m_IsRetreating = false;

    public override void SetStance(UnitStanceActionSO stanceActionSO)
    {
        base.SetStance(stanceActionSO);

        if (CurrentStance == UnitStance.Defensive)
        {
            SetState(UnitState.Idle);
            StopMovement();
            m_IsRetreating = false;
        }
    }

    protected override void OnSetState(UnitState oldState, UnitState newState)
    {
        if (newState == UnitState.Attacking)
        {
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency / 2f;
        }

        base.OnSetState(oldState, newState);
    }

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

    protected override void OnSetDestination(DestinationSource source)
    {
        base.OnSetDestination(source);

        if (
            HasTarget
            && source == DestinationSource.PlayerClick
            && (CurrentTask == UnitTask.Attack || CurrentState == UnitState.Attacking))
        {
            m_IsRetreating = true;
            SetTarget(null);
            SetTask(UnitTask.None);
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
                if (IsTargetInRange(Target))
                {
                    StopMovement(); // Menzildeyse dur
                    SetState(UnitState.Attacking); // Saldırıya geç
                }
                else if (CurrentStance == UnitStance.Offensive)
                {
                    // Menzilde değilse ama "offensive" ise kovala
                    MoveTo(Target.transform.position);
                }
            }
            else
            {
                if (CurrentStance == UnitStance.Offensive)
                {
                    // Geri çekilmiyorsak en yakın düşmanı bul, hedef olarak ata ve saldır
                    if (!m_IsRetreating && TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        SetTask(UnitTask.Attack);
                    }
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
                if (IsTargetInRange(Target))
                {
                    TryAttackCurrentTarget();
                    StopMovement(); // Menzildeyse dur
                }
                // Hedef menzilden çıktıysa, idle durumuna dön (takip için)
                else
                {
                    if (CurrentStance == UnitStance.Defensive)
                    {
                        SetTarget(null);
                        SetState(UnitState.Idle);
                    }
                    else
                    {
                        MoveTo(Target.transform.position);
                    }
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
