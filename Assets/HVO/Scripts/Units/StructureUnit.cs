
public class StructureUnit : Unit
{
    private BuildingProcess m_BuildingProcess;
    public bool IsUnderConstruction => m_BuildingProcess != null;
    
    void Update()
    {
        if (IsUnderConstruction)
        {
            m_BuildingProcess.Update();
        }
    }

    public void RegisterProcess(BuildingProcess process)
    {
        m_BuildingProcess = process;
    }

    public void AssignWorkerToBuildProcess(WorkerUnit worker)
    {
        m_BuildingProcess?.AddWorker(worker);
    }
    public void UnassignWorkerToBuildProcess()
    {
        m_BuildingProcess?.RemoveWorker();        
    }
}
