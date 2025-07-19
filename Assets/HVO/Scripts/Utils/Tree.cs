using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;
    public bool m_Claimed = false;
    public bool Claimed => m_Claimed;

    // 1 tane workerin agaci secmesini saglamak icin    
    public bool TryToClaim()
    {
        if (!m_Claimed)
        {
            m_Claimed = true;
            return true;
        }

        return false;
    }

    public void Release()
    {
        m_Claimed = false;
    }

    // Agaca gittigimizde agacin alt kismini kesecek.
    public Vector3 GetButtomPosition()
    {
        return m_Collider.bounds.min;
    }
}
