using System.Collections;
using UnityEngine;

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    Chopping,
    Minning,
    Building
}
public enum UnitTask
{
    None,
    Build,
    Chop,
    Mine,
    Attack,
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_ObjectDetectionRadius = 3f;
    [SerializeField] protected float m_UnitDetectionCheckRate = 0.5f;
    [SerializeField] protected float m_AttackRange = 1f;
    [SerializeField] protected float m_AutoAttackFrequency = 1.5f;
    [SerializeField] protected float m_AutoAttackDamageDelay = 0.5f;
    [SerializeField] protected int m_AutoAttackDamage = 7;

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

    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;

    public UnitState CurrentState { get; protected set; } = UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
    public Unit Target {get; protected set;}

    public virtual bool IsPlayer => true;
    public virtual bool IsBuilding => false;

    public bool HasTarget => Target != null;

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
        }

        m_Collider = GetComponent<CapsuleCollider2D>();
        m_GameManager = GameManager.Get();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/Outline");
    }

    void OnDestroy()
    {
        if (m_AIPawn != null)
        {
            m_AIPawn.OnNewPositionSelected -= TurnToPosition;
        }

        UnRegisterUnit();
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

    public void MoveTo(Vector3 destination)
    {
        var direction = (destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;

        m_AIPawn.SetDestination(destination);
        OnSetDestination();
    }

    public void Select()
    {
        Highlight();
        IsTarget = true;
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

    protected virtual void OnSetDestination()
    {

    }

    protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
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

    protected virtual bool TryAttackCurrentTarget()
    {
        if (Time.time >= m_NextAutoAttackTime)
        {
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency;
            PerformAttackAnimation();
            StartCoroutine(DelayDamage(m_AutoAttackDamageDelay, m_AutoAttackDamage, Target));
            return true;
        }

        return false;
    }

    protected virtual void PerformAttackAnimation()
    {

    }

    protected virtual void TakeDamage(int damage, Unit damager)
    {
        m_GameManager.ShowTextPopup(
            damage.ToString(),
            GetTopPosition(),
            Color.red
        );
    }

    protected IEnumerator DelayDamage(float delay, int damage, Unit target)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            target.TakeDamage(damage, this);
        }
    }

    // Hedefin saldırı menziline girip girmediğini kontrol eder
    protected bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= m_AttackRange;
    }

    protected Collider2D[] RunProximityObjectDetection()
    {
        return Physics2D.OverlapCircleAll(transform.position, m_ObjectDetectionRadius);
    }

    void TurnToPosition(Vector3 newPosition)
    {
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
