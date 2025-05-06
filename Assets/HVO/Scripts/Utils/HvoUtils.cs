using UnityEngine;
using UnityEngine.EventSystems;

public static class HvoUtils
{
    //GameManager.cs ile baglantili.
    public static Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    public static bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    public static bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    //GameManager.cs ile baglantili.
    private static Vector2 m_InitialTouchPosition;
    public static bool TryGetShortClickPosition(out Vector2 inputPos, float maxDistance = 5f)
    {
        inputPos = InputPosition;
        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(IsLeftClickOrTapDown)
        {
            m_InitialTouchPosition = inputPos;
        }

        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(IsLeftClickOrTapUp)
        {
            if (Vector2.Distance(m_InitialTouchPosition, inputPos) < maxDistance )
            {
                return true;
            }
        }
        return false;
    }

    public static bool TryGetHoldPosition(out Vector3 worldPosition)
    {
        if(Input.touchCount > 0)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            return true;
        }
        else if (Input.GetMouseButton(0))
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return true;
        }
        worldPosition = Vector3.zero;
        return false;
    }

    public static bool IsPointerOverUIElement() //Action bara artik tiklayinca ilerlemiyor unit oraya dogru.
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
