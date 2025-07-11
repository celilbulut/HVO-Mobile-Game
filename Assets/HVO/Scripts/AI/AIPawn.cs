using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [Tooltip("Birimin hareket hızı (unit/saniye cinsinden).")]
    [SerializeField] private float m_Speed = 5f;

    [Header("Separation")]    
    
    [Tooltip("Komşu birimleri algılamak için yarıçap")]
    [SerializeField] private float m_SeperationRadius = 1f;      // Komşu birimleri algılamak için yarıçap

    [Tooltip("Uzaklaştırma kuvveti (etki gücü). Değer arttıkça daha fazla itme uygulanır.")]
    [SerializeField] private float m_SeperationForce = 0.5f;     // Uzaklaştırma kuvveti (etki gücü)

    [Tooltip("Bu birim için separation algoritması uygulanacak mı?")]
    [SerializeField] private bool m_ApplySeperation = true;      // Bu birim separation kullanacak mı?

    private Vector3? m_CurrentDestination;                       // Hedef pozisyon
    private TilemapManager m_TilemapManager;
    private List<Vector3> m_CurrentPath = new();                // Pathfinding'den gelen yol noktaları
    private int m_CurrentNodeIndex;                             // Şu anki hedef nokta index'i
    private GameManager m_GameManager;
    private Unit m_Unit;

    public UnityAction<Vector3> OnNewPositionSelected = delegate { };  // Yeni pozisyon seçildiğinde tetiklenir
    public UnityAction OnDestinationReached = delegate { };            // Hedefe ulaşıldığında tetiklenir


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

        // Separation vektörü hesapla (aktifse)
        Vector3 seperationVector = m_ApplySeperation ? CalculateSeperation() : Vector3.zero;

        Vector3 targetPosition = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Yön + separation birlikte
        Vector3 combinedDirection = direction + seperationVector;

        // Aşırı uzun vektörleri normalize et
        if (combinedDirection.magnitude > 1f)
        {
            combinedDirection.Normalize();
        }

        // Hareket uygula
        transform.position += combinedDirection * m_Speed * Time.deltaTime;

        // Hedef noktaya yaklaştıysa
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
        m_CurrentPath.Clear();  // Tüm path (hedef noktaları) temizlenir
        m_CurrentNodeIndex = 0; // Mevcut hedef sıfırlanır
    }


    // Bu birim oyuncuya mı ait? (Unit bileşeni üzerinden kontrol)
    protected virtual bool GetPlayerStatus()
    {
        if (m_Unit != null)
        {
            return m_Unit.IsPlayer;
        }

        m_Unit = GetComponent<Unit>();
        return m_Unit.IsPlayer;
    }

    // Separation algoritmasını hesapla (etrafındaki dost birimlere göre)
    Vector3 CalculateSeperation()
    {
        Vector3 separationVector = Vector3.zero;
        float SeparationRadiusSqr = m_SeperationRadius * m_SeperationRadius;

        List<Unit> units = m_GameManager.GetFriendlyUnits(GetPlayerStatus());

        foreach (var unit in units)
        {
            if (unit.gameObject == gameObject) continue;  // Kendimizi hesaba katmayalım

            Vector3 opositeDirection = transform.position - unit.transform.position;
            float sqrDistance = opositeDirection.sqrMagnitude;

            // Yalnızca yakın birimler için uygulansın
            if (sqrDistance < SeparationRadiusSqr && sqrDistance > 0)
            {
                // Yakınsa daha güçlü, uzaksa daha az etki uygula
                separationVector += opositeDirection.normalized / sqrDistance;
            }
        }

        return separationVector * m_SeperationForce;
    }

    // Yol geçerli mi? (yol var mı ve hedef index aşılmamış mı)
    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}