using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;

    public UnityAction<Vector3> OnNewPositionSelected = delegate { };

    private TilemapManager m_TilemapManager;
    private List<Vector3> m_CurrentPath = new();
    private int m_CurrentNodeIndex;

    void Start() // Ekrana tiklama yaptigimiz yer
    {
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            return;
        }

        //Node currentNode = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 targetPosition = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * m_Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
            {
                Debug.Log("Destination Reached!");
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
        if (m_CurrentPath.Count > 0)
        {
            Node newEndNode = m_TilemapManager.FindNode(destination);
            Vector3 endPosition = new Vector3(newEndNode.centerX, newEndNode.centerY);
            var distance = Vector3.Distance(endPosition, m_CurrentPath[^1]);

            if (distance < 0.1f) // son nodu veriyor. m_CurrentPath.Last() ile ayni
            {
                return;
            }
        }

        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        m_CurrentNodeIndex = 0;

        OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
    }

    public void Stop()
    {
        m_CurrentPath.Clear(); // Tüm path (hedef noktaları) temizlenir
        m_CurrentNodeIndex = 0; // Mevcut hedef sıfırlanır
    }

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}