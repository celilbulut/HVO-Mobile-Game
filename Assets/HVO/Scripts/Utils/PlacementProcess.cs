
using UnityEngine;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;

    public PlacementProcess(BuildActionSO buildActionSO)
    {
        m_BuildAction = buildActionSO;
    }

    public void Update() 
    {
        if (HvoUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutline.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        }
    }

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999; // Order in the layer
        renderer.color = new Color(1, 1, 1, 0.5f); // Yari transparent yapiyoruz bu sayede.
        renderer.sprite = m_BuildAction.PlacementSprite; // Towerin seklini cikartmak icin.
    }
}
