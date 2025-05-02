
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : SingletonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToClickPrefab;
    [SerializeField] private ActionBar m_ActionBar;

    public Unit ActiveUnit;
    private Vector2 m_InitialTouchPosition;

    public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    public bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    public bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    public bool HasActiveUnit => ActiveUnit != null;

    void Start()
    {
        ClearActionBarUI();
    }

    void Update()
    {
        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(IsLeftClickOrTapDown)
        {
            m_InitialTouchPosition = InputPosition;
        }

        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(IsLeftClickOrTapUp)
        {
            if (Vector2.Distance(m_InitialTouchPosition, InputPosition) < 5 )
            {
                DetectClick(InputPosition);
            }
        }
    }

    void DetectClick(Vector2 inputPosition)
    {
        if(IsPointerOverUIElement())
        {
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if(HasClickedOnUnit(hit, out var unit))
        {
            HandleClickOnUnit(unit);
        }
        else
        {
            HandleClickOnGround(worldPoint);
        }
    }

    bool HasClickedOnUnit(RaycastHit2D hit, out Unit unit)
    {
        if(hit.collider != null && hit.collider.TryGetComponent<Unit>(out var clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }
        unit = null;
        return false;
    }

    void HandleClickOnGround(Vector2 worldPoint)
    {
        if(HasActiveUnit && IsHumanoid(ActiveUnit))
        {
            DisplayClickEffect(worldPoint);
            ActiveUnit.MoveTo(worldPoint);
        }
    }

    void HandleClickOnUnit(Unit unit)
    {
        if(HasActiveUnit)
        {
            if(HasClickedOnActiveUnit(unit))
            {
                CancelActiveUnit();
                return;
            }
        }
        SelectNewUnit(unit);
    }

    void SelectNewUnit(Unit unit)
    {
        if(HasActiveUnit)
        {
            ActiveUnit.DeSelect();
        }

        ActiveUnit = unit;
        ActiveUnit.Select();
        ShowUnitActions(unit);
    }

    bool HasClickedOnActiveUnit(Unit clickedUnit)
    {
        return clickedUnit == ActiveUnit;
    }

    bool IsHumanoid(Unit unit)
    {
        return unit is HumanoidUnit;
    }

    void CancelActiveUnit()
    {
        ActiveUnit.DeSelect();
        ActiveUnit = null;

        ClearActionBarUI();
    }

    void DisplayClickEffect(Vector2 worldPoint)
    {
        Instantiate(m_PointToClickPrefab, (Vector3)worldPoint, Quaternion.identity);
    }

    void ShowUnitActions(Unit unit)
    {
        ClearActionBarUI();

        if(unit.Actions.Length == 0)
        {
            return;
        }

        m_ActionBar.Show();

        foreach (var action in unit.Actions)
        {
            m_ActionBar.RegisterAction(action.Icon);
        }
    }

    void ClearActionBarUI()
    {
        m_ActionBar.ClearActions();
        m_ActionBar.Hide();
    }

    bool IsPointerOverUIElement() //Action bara artik tiklayinca ilerlemiyor unit oraya dogru.
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = InputPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}