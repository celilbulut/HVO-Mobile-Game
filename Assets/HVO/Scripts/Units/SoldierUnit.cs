using UnityEngine;

public class SoldierUnit : HumanoidUnit
{
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
