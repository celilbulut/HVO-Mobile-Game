using UnityEngine;

public class TowerUnit : StructureUnit
{
    [SerializeField] private Projectile m_ProjectilePrefab;

    public override void OnConstructionFinished()
    {
        base.OnConstructionFinished();

        var Projectile = Instantiate(m_ProjectilePrefab, transform.position, Quaternion.identity);
    }

    protected override void AfterConstructionUpdate()
    {
        Debug.Log("AfterConstructionUpdate!");
    }
}
