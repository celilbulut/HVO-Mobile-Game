using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private Sprite m_ActiveSprite;
    [SerializeField] private Sprite m_DefaultSprite;
    [SerializeField] private CapsuleCollider2D m_Collider;
    [SerializeField] private SpriteRenderer m_Renderer;
    [SerializeField] private float m_EnterMineFreq = 2f;
    [SerializeField] private float m_MinningDuration = 2f;

    private int m_MaxAllowedMiners = 2; // Mine a 2 kisi en fazla girebilecek.
    private Queue<WorkerUnit> m_ActiveMinersQueue = new();
    private float m_NextPossibleEnterTime;

    void Update()
    {
        if (m_ActiveMinersQueue.Count > 0)
        {
            m_Renderer.sprite = m_ActiveSprite;
        }
        else
        {
            m_Renderer.sprite = m_DefaultSprite;
        }
    }

    public bool TryToEnterMine(WorkerUnit worker)
    {
        if (m_ActiveMinersQueue.Count < m_MaxAllowedMiners
           && Time.time >= m_NextPossibleEnterTime
          )
        {
            worker.OnEnterMine();
            m_ActiveMinersQueue.Enqueue(worker);
            m_NextPossibleEnterTime = Time.time + m_EnterMineFreq;
            StartCoroutine(ReleaseWorkerAfterDelay(worker, m_MinningDuration));
            return true;
        }

        Debug.Log("Can not enter yet");
        return false;
    }

    public Vector3 GetBottomPosition()
    {
        return m_Collider.bounds.min;
    }

    IEnumerator ReleaseWorkerAfterDelay(WorkerUnit worker, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (m_ActiveMinersQueue.Contains(worker))
        {
            m_ActiveMinersQueue.Dequeue();
            worker.OnLeaveMine();
        }
    }
}
