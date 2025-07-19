using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;
    private bool m_Occupied = false;
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

    // Agaca gittigimizde agacin alt kismini kesecek.
    public Vector3 GetButtomPosition()
    {
        return m_Collider.bounds.min;
    }
}
