

using UnityEngine;

public class PlacementProcess
{
    private BuildActionSO m_BuildAction;

    public PlacementProcess(BuildActionSO buildActionSO)
    {
        m_BuildAction = buildActionSO;
    }

    public void Update() 
    {
        Debug.Log("PlacementProcess Update()");        
    }

    public void ShowPlacementOutline()
    {
        Debug.Log("ShowPlacementOutline");
    }
}
