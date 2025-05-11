
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
}
