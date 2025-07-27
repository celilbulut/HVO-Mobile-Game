using System.Collections;
using UnityEngine;

public class HumanoidUnit : Unit
{
    [SerializeField] private AudioSettings m_FootstepAudioSettings;
    [SerializeField] private float m_FootstepFrequency = 0.3f;

    protected Vector2 m_Velocity;
    protected Vector3 m_LastPosition;
    protected float m_SmoothFactor = 50;
    protected float m_SmoothedSpeed;

    public float CurrentSpeed => m_Velocity.magnitude;

    private float m_LastFootStepTime;

    protected override void Start()
    {
        base.Start();
        m_LastPosition = transform.position;
    }

    protected void Update()
    {
        if (CurrentState == UnitState.Dead || CurrentState == UnitState.Minning) return;

        UpdateVelocity();
        UpdateBehaviour();
        UpdateMovementAnimation();
    }

    protected virtual void UpdateBehaviour() { }

    protected virtual void UpdateVelocity()
    {
        m_Velocity = new Vector2
        (
            (transform.position.x - m_LastPosition.x),
            (transform.position.y - m_LastPosition.y)
        ) / Time.deltaTime;

        m_LastPosition = transform.position;
        m_SmoothedSpeed = Mathf.Lerp(m_SmoothedSpeed, CurrentSpeed, Time.deltaTime * m_SmoothFactor);

        if (CurrentState != UnitState.Attacking)
        {
            var state = m_SmoothedSpeed > 0.1f ? UnitState.Moving : UnitState.Idle;
            SetState(state);
        }

        if (IsTarget
            && CurrentState == UnitState.Moving
            && Time.time >= m_LastFootStepTime + m_FootstepFrequency
           )
        {
            m_AudioManager.PlaySound(m_FootstepAudioSettings, transform.position);
            m_LastFootStepTime = Time.time;
        }
    }

    protected virtual void UpdateMovementAnimation()
    {
        m_Animator?.SetFloat("Speed", Mathf.Clamp01(m_SmoothedSpeed));
    }

    protected override void PerformAttackAnimation()
    {
        Vector3 direction = (Target.transform.position - transform.position).normalized;

        // Eger hedefin solunda yada sagindaysa oraya bakarak saldiri yapmasi
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            m_SpriteRenderer.flipX = direction.x < 0;
            m_Animator.SetTrigger("AttackHorizontal"); // Yazim onemli animatorle ayni olmali.
        }
        else // Eger hedefin yukarisinda yada asagisindaysa
        {
            m_Animator.SetTrigger(direction.y > 0 ? "AttackUp" : "AttackDown"); // Yazim onemli animatorle ayni olmali.
        }
    }

    protected override void RunDeadEffect()
    {
        m_Animator.SetTrigger("Dead");
        StartCoroutine(LateObjectDestroy(1.2f)); // 1.2f saniye sonra olecek.
    }

    // Olen bir Objenin Animasyondan sonra olmesi icin delay ekliyoruz
    private IEnumerator LateObjectDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
