using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;

    [Header("Separation")]
    [SerializeField] private float m_SeperationRadius = 1f;
    [SerializeField] private float m_SeperationForce = 0.5f;
    [SerializeField] private bool m_ApplySeperation = true;


    private Vector3? m_CurrentDestination;
    private TilemapManager m_TilemapManager;
    private List<Vector3> m_CurrentPath = new();
    private int m_CurrentNodeIndex;

    private GameManager m_GameManager;
    
    public UnityAction<Vector3> OnNewPositionSelected = delegate { };
    public UnityAction OnDestinationReached = delegate { };

    void Start() // Ekrana tiklama yaptigimiz yer
    {
        m_GameManager = GameManager.Get();
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            m_CurrentDestination = null;
            return;
        }

        if (m_ApplySeperation)
        {
            ApplySeperation();
        }

        //Node currentNode = m_CurrentPath[m_CurrentNodeIndex];
            Vector3 targetPosition = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * m_Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
            {
                OnDestinationReached.Invoke();
                m_CurrentPath = new();
            }
            else
            {
                m_CurrentNodeIndex++;
                
                OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
            }
        }
    }

    public void SetDestination(Vector3 destination) // Ekrana tiklama yaptigimiz yer
    {
        // Hedef daha önceyle aynıysa yeni yol çizme
        if (m_CurrentDestination.HasValue && Vector3.Distance(m_CurrentDestination.Value, destination) < 0.1f)
        {
            return;
        }

        m_CurrentDestination = destination;
        
        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        m_CurrentNodeIndex = 0;
        OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
    }

    public void Stop()
    {
        m_CurrentPath.Clear(); // Tüm path (hedef noktaları) temizlenir
        m_CurrentNodeIndex = 0; // Mevcut hedef sıfırlanır
    }

    private Unit m_Unit;

    protected virtual bool GetPlayerStatus()
    {
        if (m_Unit != null)
        {
            return m_Unit.IsPlayer;
        }

        m_Unit = GetComponent<Unit>();
        return m_Unit.IsPlayer;
    }

    void ApplySeperation()
    {
        List<Unit> units = m_GameManager.GetFriendlyUnits(GetPlayerStatus());

        Debug.Log(units.Count);
    }

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}