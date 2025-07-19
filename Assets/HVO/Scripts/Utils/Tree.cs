using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;
    public bool m_Occupied = false;
    public bool Occupied => m_Occupied;

    // 1 tane workerin agaci secmesini saglamak icin    
    public bool TryOccupy()
    {
        if (!m_Occupied)
        {
            m_Occupied = true;
            return true;
        }

        return false;
    }

    public void UnOccupy()
    {
        m_Occupied = false;
    }

    // Agaca gittigimizde agacin alt kismini kesecek.
    public Vector3 GetButtomPosition()
    {
        return m_Collider.bounds.min;
    }
}
