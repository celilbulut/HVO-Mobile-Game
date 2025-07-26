using System.Collections;
using UnityEngine;

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    Chopping,
    Minning,
    Building,
    Dead
}
public enum UnitTask
{
    None,
    Build,
    Chop,
    Mine,
    Attack,
    ReturnResource
}

public enum DestinationSource
{
    CodeTriggered,
    PlayerClick
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private bool m_IsKingUnit = false;
    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_ObjectDetectionRadius = 3f;
    [SerializeField] protected float m_UnitDetectionCheckRate = 0.5f;
    [SerializeField] protected float m_AttackRange = 1f;
    [SerializeField] protected float m_AutoAttackFrequency = 1.5f;
    [SerializeField] protected float m_AutoAttackDamageDelay = 0.5f;
    [SerializeField] protected int m_AutoAttackDamage = 7;
    [SerializeField] protected int m_Health = 100;
    [SerializeField] protected Color m_DamageFlashColor = new Color(1f, 0.63f, 0.63f, 1f);

    [Header("Audio")]
    [SerializeField] protected AudioSettings m_InteractionAudioSettings;
    [SerializeField] protected AudioSettings m_AttackAudioSettings;

    public bool IsTarget;

    protected GameManager m_GameManager;
    protected Animator m_Animator;
    protected AIPawn m_AIPawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightMaterial;
    protected CapsuleCollider2D m_Collider;

    protected float m_NextUnitDetectionTime;
    protected float m_NextAutoAttackTime;
    protected int m_CurrentHealth;
    protected UnitStance m_CurrentStance = UnitStance.Offensive;

    private Coroutine m_FlashCoroutine;

    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;

    public UnitState CurrentState { get; protected set; } = UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
    public Unit Target { get; protected set; }

    public virtual bool IsPlayer => true;
    public virtual bool IsBuilding => false;

    public bool HasTarget => Target != null;
    public int CurrentHealth => m_CurrentHealth;
    public UnitStance CurrentStance => m_CurrentStance;
    public CapsuleCollider2D Collider => m_Collider;
    public bool IsKingUnit => m_IsKingUnit;

    protected virtual void Start()
    {
        RegisterUnit();
    }

    protected void Awake()
    {
        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }

        if (TryGetComponent<AIPawn>(out var aiPawn))
        {
            m_AIPawn = aiPawn;
            m_AIPawn.OnNewPositionSelected += TurnToPosition;
            m_AIPawn.OnDestinationReached += OnDestinationReached;
        }

        m_Collider = GetComponent<CapsuleCollider2D>();
        m_GameManager = GameManager.Get();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/Outline");

        m_CurrentHealth = m_Health;
    }

    void OnDestroy()
    {
        if (m_AIPawn != null)
        {
            m_AIPawn.OnNewPositionSelected -= TurnToPosition;
            m_AIPawn.OnDestinationReached -= OnDestinationReached;
        }
    }

    public void SetTask(UnitTask task)
    {
        OnSetTask(CurrentTask, task);
    }

    public void SetState(UnitState state)
    {
        OnSetState(CurrentState, state);
    }

    public void SetTarget(Unit target)
    {
        Target = target;
    }

    public virtual void SetStance(UnitStanceActionSO stanceActionSO)
    {
        m_CurrentStance = stanceActionSO.UnitStance;

        for (int i = 0; i < m_Actions.Length; i++)
        {
            if (m_Actions[i] == stanceActionSO)
            {
                m_GameManager.FocusActionUI(i);
                return;
            }
        }
    }

    // Calisanlar mine a girince kaybolacak.
    public void Hide()
    {
        m_SpriteRenderer.enabled = false;
        m_Collider.enabled = false;
    }

    // Calisanlar mine dan cikinca tekrar gozukecek.
    public void Show()
    {
        m_SpriteRenderer.enabled = true;
        m_Collider.enabled = true;
    }

    public void MoveTo(Vector3 destination, DestinationSource source = DestinationSource.CodeTriggered)
    {
        var direction = (destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;

        m_AIPawn.SetDestination(destination);
        OnSetDestination(source);
    }

    public void Select()
    {
        OnPlayInteractionSound();

        Highlight();
        IsTarget = true;

        for (int i = 0; i < m_Actions.Length; i++)
        {
            if (m_Actions[i] is UnitStanceActionSO stanceActionSO && stanceActionSO.UnitStance == m_CurrentStance)
            {
                m_GameManager.FocusActionUI(i);
                return;
            }
        }
    }

    public void DeSelect()
    {
        UnHighlight();
        IsTarget = false;
    }

    public void StopMovement()
    {
        m_AIPawn.Stop(); // IPawn üzerindeki stop fonksiyonu çağrılır
    }

    public Vector3 GetTopPosition()
    {
        if (m_Collider == null) return transform.position;

        return transform.position + Vector3.up * m_Collider.size.y / 2;
    }

    protected virtual void OnSetDestination(DestinationSource source)
    {
        if (source == DestinationSource.PlayerClick)
        {
            OnPlayInteractionSound();
        }
    }

    protected virtual void OnPlayAttackSound()
    {
        AudioManager.Get().PlaySound(m_AttackAudioSettings, transform.position);
    }

    protected virtual void OnPlayInteractionSound()
    {
        AudioManager.Get().PlaySound(m_InteractionAudioSettings, transform.position);
    }

    protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    protected virtual void OnDestinationReached()
    {

    }

    public virtual void RegisterUnit()
    {
        m_GameManager.RegisterUnit(this);
    }

    public virtual void UnRegisterUnit()
    {
        m_GameManager.UnRegisterUnit(this);
    }

    // En yakın düşmanı (veya oyuncuyu) belli aralıklarla bulur
    // Gereksiz yere her frame’de çalışmaz, zamanlıdır
    protected virtual bool TryFindClosestFoe(out Unit foe)
    {
        if (Time.time >= m_NextUnitDetectionTime)
        {
            // Hedef tespiti yapılır
            // En yakın düşman GameManager.FindClosestUnit ile bulunur
            // Eğer bulunduysa out parametresine atanır
            // m_NextUnitDetectionTime güncellenir (örneğin şimdi + 0.5 saniye)
            m_NextUnitDetectionTime = Time.time + m_UnitDetectionCheckRate;
            foe = m_GameManager.FindClosestUnit(transform.position, m_ObjectDetectionRadius, !IsPlayer);
            return foe != null;
        }
        else
        {
            // Süre dolmadığı için tekrar arama yapılmaz
            // Hedef null döndürülür
            foe = null;
            return false;
        }
    }

    protected virtual void OnAttackReady(Unit target)
    {
        OnPlayAttackSound();
        PerformAttackAnimation();
        StartCoroutine(DelayDamage(m_AutoAttackDamageDelay, m_AutoAttackDamage, Target));
    }

    protected virtual bool TryAttackCurrentTarget()
    {
        if (Target.CurrentState == UnitState.Dead) return false;

        if (Time.time >= m_NextAutoAttackTime)
        {
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency;
            OnAttackReady(Target);
            return true;
        }

        return false;
    }

    protected virtual void PerformAttackAnimation()
    {

    }

    protected virtual void RunDeadEffect()
    {

    }

    protected virtual void Die()
    {
        SetState(UnitState.Dead);

        if (m_AIPawn != null)
        {
            StopMovement();            
        }
        
        RunDeadEffect();
        UnRegisterUnit();
    }


    public virtual void TakeDamage(int damage, Unit damager)
    {
        if (CurrentState == UnitState.Dead) return;

        m_CurrentHealth -= damage;

        if (!HasTarget)
        {
            SetTarget(damager);
        }

        // Cikan damage yazisinin ayari
        m_GameManager.ShowTextPopup(
            damage.ToString(),
            GetTopPosition(),
            Color.red
        );

        if (m_FlashCoroutine == null)
        {
            // Hasar alinca kirmizi yanip sonmesi icin olusturdugumuz flash effect
            m_FlashCoroutine = StartCoroutine(FlashEffect(0.2f, 2, m_DamageFlashColor));
        }        

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    // Hasar alinca kirmizi yanip sonmesi icin olusturdugumuz flash effect
    protected IEnumerator FlashEffect(float duration, int flashCount, Color color)
    {
        Color originalColor = m_SpriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            m_SpriteRenderer.color = color;
            yield return new WaitForSeconds(duration / 2f);

            m_SpriteRenderer.color = originalColor;
            yield return new WaitForSeconds(duration / 2f);
        }

        m_SpriteRenderer.color = originalColor;
        m_FlashCoroutine = null;
    }

    protected IEnumerator DelayDamage(float delay, int damage, Unit target)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            if (target.CurrentState == UnitState.Dead)
            {
                SetTarget(null); // hedefi sifirla
            }
            else
            {
                target.TakeDamage(damage, this);
            }
        }
    }

    // Hedefin saldırı menziline girip girmediğini kontrol eder
    protected bool IsTargetInRange(Unit target)
    {
        var targetCollider = target.Collider;
        var targetClosestPoint = targetCollider.ClosestPoint(transform.position);

        return Vector3.Distance(targetClosestPoint, transform.position) <= m_AttackRange;
    }

    protected Collider2D[] RunProximityObjectDetection()
    {
        return Physics2D.OverlapCircleAll(transform.position, m_ObjectDetectionRadius);
    }

    void TurnToPosition(Vector3 newPosition)
    {
        if (HasTarget && !IsPlayer) return;

        var direction = (newPosition - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;
    }

    void Highlight()
    {
        m_SpriteRenderer.material = m_HighlightMaterial;
    }

    void UnHighlight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, m_ObjectDetectionRadius);

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawSphere(transform.position, m_AttackRange);
    }
}
