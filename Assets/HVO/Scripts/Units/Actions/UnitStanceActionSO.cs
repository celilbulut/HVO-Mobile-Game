using UnityEngine;

public enum UnitStance
{
    Defensive,
    Offensive
}

[CreateAssetMenu(fileName = "StanceAction", menuName = "HVO/Actions/UnitStanceActions")]
public class UnitStanceActionSO : ActionSO
{
    [SerializeField] private UnitStance m_UnitStace;

    public UnitStance UnitStance => m_UnitStace;

    public override void Execute(GameManager manager)
    {
        if (manager.ActiveUnit != null)
        {
            Debug.Log("Change state to" + m_UnitStace.ToString());
        }
    }
}
