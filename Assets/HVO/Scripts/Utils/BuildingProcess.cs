using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO m_BuildAction;
    private WorkerUnit m_Worker;
    private float m_ProgressTimer;
    private bool m_IsFinished;
    private bool InProgress => HasActiveWorker && m_Worker.CurrentState == UnitState.Building;
    public bool HasActiveWorker => m_Worker != null;

    public BuildingProcess(
                            BuildActionSO buildAction,
                            Vector3 placementPosition,
                            WorkerUnit worker
                            )
    {
        m_BuildAction = buildAction;
        var structure = Object.Instantiate(buildAction.StructurePrefab);
        structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        structure.transform.position = placementPosition;
        structure.RegisterProcess(this);

        worker.SendToBuild(structure);
    }

    public void Update() 
    {
        if (HasActiveWorker)
        {
            m_ProgressTimer += Time.deltaTime;            
            Debug.Log(m_ProgressTimer);
        }
    }

    public void AddWorker(WorkerUnit worker)
    {
        if(HasActiveWorker) return;
        Debug.Log("Adding Worker");
        m_Worker = worker;
    }
    public void RemoveWorker()
    {
        if(!HasActiveWorker) return;
        Debug.Log("Removing Worker");
        m_Worker = null;
    }
}
