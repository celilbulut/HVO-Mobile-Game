
using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    private Tree m_AssignedTree;
    protected override void UpdateBehaviour()
    {
        //eger gorevimiz yoksa hic birseyi check etmeyecek.
        if (CurrentTask == UnitTask.Build && HasTarget)
        {
            CheckForConstruction();
        }
        else if (CurrentTask == UnitTask.Chop && m_AssignedTree != null)
        {
            HandleChoppingTask();
        }

        if (CurrentState == UnitState.Chopping)
        {
            StartChopping();
        }
    }

    protected override void OnSetDestination(DestinationSource source)
    {
        ResetState();
    }

    public void OnBuildingFinished()
    {
        ResetState();
    }

    public void SendToBuild(StructureUnit structure)
    {
        MoveTo(structure.transform.position);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    public void SendToChop(Tree tree)
    {
        if (tree.TryToClaim())
        {
            MoveTo(tree.GetButtomPosition());
            SetTask(UnitTask.Chop);
            m_AssignedTree = tree;
        }
    }

    protected override void Die()
    {
        base.Die();

        if (m_AssignedTree != null)
        {
            m_AssignedTree.Release();
        }
    }

    void HandleChoppingTask()
    {
        var treeButtomPosition = m_AssignedTree.GetButtomPosition();
        var workerClosestPoint = Collider.ClosestPoint(treeButtomPosition);

        var Distance = Vector3.Distance(treeButtomPosition, workerClosestPoint);

        if (Distance <= 0.1f)
        {
            StopMovement();
            SetState(UnitState.Chopping);
        }
    }

    void StartChopping()
    {
        m_Animator.SetBool("IsChopping", true); // Isimlendirme onemli.
    }

    void CheckForConstruction()
    {
        var distanceToConstruction = Vector3.Distance(transform.position, Target.transform.position);

        if (distanceToConstruction <= m_ObjectDetectionRadius && CurrentState == UnitState.Idle)
        {
            StartBuilding(Target as StructureUnit);
        }
    }

    void StartBuilding(StructureUnit structure)
    {
        SetState(UnitState.Building);
        //Isim onemli. Animasyondaki ismi tetikliyoruz buradan.
        m_Animator.SetBool("IsBuilding", true);

        structure.AssignWorkerToBuildProcess(this);
    }

    void ResetState()
    {
        SetTask(UnitTask.None);

        if (HasTarget) CleanupTarget();

        //Isim onemli. Animasyondaki ismi tetikliyoruz buradan.
        m_Animator.SetBool("IsBuilding", false);
        m_Animator.SetBool("IsChopping", false);


        if (m_AssignedTree != null)
        {
            m_AssignedTree.Release(); // Ağacı boşalt
            m_AssignedTree = null; // Worker ile ilişkiyi kes
        }
    }

    void CleanupTarget()
    {
        if (Target is StructureUnit structure)
        {
            structure.UnassignWorkerToBuildProcess();
        }
        SetTarget(null);
    }
}
