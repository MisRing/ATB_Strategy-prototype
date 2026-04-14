using System.Collections.Generic;
using UnityEngine;

public class AbilityViewRenderer : MonoBehaviour
{
    [SerializeField] private CursorCostDisplay _costDisplay;
    [SerializeField] private PathLineRenderer _pathRenderer;
    public static readonly List<IPathHandler> PathHandlers = new List<IPathHandler>();

    private void Awake()
    {
        _pathRenderer.Init();
        _costDisplay.Init();
    }

    private void OnEnable()
    {
        foreach(IPathHandler pathHandler in PathHandlers)
        {
            if(pathHandler == null) continue;
            
            pathHandler.OnPathChanged += DrawPath;
        }
    }

    private void OnDisable()
    {
        foreach (IPathHandler pathHandler in PathHandlers)
        {
            if (pathHandler == null) continue;
            
            pathHandler.OnPathChanged -= DrawPath;
        }
    }

    private void DrawPath(PathData data)
    {
        if (data == null)
        {
            _pathRenderer.UnsetPathLine();
            _costDisplay.UnsetCost();
            return;
        }

        _pathRenderer.SetPathLine(data.Points);
        _costDisplay.SetCost(data.TurnsCost);
    }
}
