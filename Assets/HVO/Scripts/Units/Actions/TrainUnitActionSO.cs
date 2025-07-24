using UnityEngine;

[CreateAssetMenu(fileName = "TrainUnitAction", menuName = "HVO/Actions/TrainUnitActions")]
public class TrainUnitActionSO : ActionSO
{
    [SerializeField] private Unit m_UnitPrefab;
    [SerializeField] private int m_GoldCost;
    [SerializeField] private int m_WoodCost;

    public Unit UnitPrefab => m_UnitPrefab;
    public int GoldCost => m_GoldCost;
    public int WoodCost => m_WoodCost;


    public override void Execute(GameManager manager)
    {
        manager.StartUnitTrainProcess(this);
    }
}
