using UnityEngine;

[CreateAssetMenu(fileName = "BuildAction", menuName = "HVO/Actions/BuildActions")]
public class BuildActionSO : ActionSO
{
    [SerializeField] private StructureUnit m_StructurePrefab;
    [SerializeField] private Sprite m_PlacementSprite;
    [SerializeField] private Sprite m_FoundationSprite;
    [SerializeField] private Sprite m_CompletionSprite;
    
    [SerializeField] private Vector3Int m_BuildingSize;
    [SerializeField] private Vector3Int m_OriginOffset;

    [SerializeField] private int m_GoldCost;
    [SerializeField] private int m_WoodCost;

    [SerializeField] private float m_ConstructionTime;

    public StructureUnit StructurePrefab =>m_StructurePrefab;

    public Sprite PlacementSprite => m_PlacementSprite;
    public Sprite FoundationSprite => m_FoundationSprite;
    public Sprite CompletionSprite => m_CompletionSprite;

    public Vector3Int BuildingSize => m_BuildingSize;
    public Vector3Int OriginOffset => m_OriginOffset;

    public int GoldCost => m_GoldCost;
    public int WoodCost => m_WoodCost;

    public float ConstructionTime => m_ConstructionTime;

    public override void Execute(GameManager manager)
    {
        manager.StartBuildProcess(this);
    }
}
