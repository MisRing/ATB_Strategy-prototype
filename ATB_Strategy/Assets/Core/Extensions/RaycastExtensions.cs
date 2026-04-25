using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RaycastExtensions
{
    public static bool IsPointerOverUIObject()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
