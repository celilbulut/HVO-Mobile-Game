using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Unit : MonoBehaviour
{
    public bool IsMoving;

    protected Animator m_Animator;
    protected void Awake()
    {
        m_Animator = GetComponent<Animator>();

        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }
        var manager = GameManager.Get();
        manager.Test();
    }
}
