using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RaycastExtensions
{
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = PlayerInputController.MouseScreenPosition;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
