using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private Vector2 m_InitialTouchPosition;

    void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            m_InitialTouchPosition = inputPosition;
        }

        // 0 sol mause buttonunu, 1 ise sag mause buttonunu temsil ediyor.
        if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10 )
            {
                DetectClick(inputPosition);
            }
        }
    }

    void DetectClick(Vector2 inputPosition)
    {
        Debug.Log(inputPosition);
    }
}
