using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityViewRenderer : MonoBehaviour
{
    [SerializeField] private PathLineRenderer _pathRenderer;
    public static List<IPathHandler> PathHandlers = new List<IPathHandler>();

    private void Awake()
    {
        _pathRenderer.Init();
    }

    private void OnEnable()
    {
        foreach(IPathHandler pathHandler in PathHandlers)
        {
            if(pathHandler == null)
            {
                continue;
            }
            pathHandler.OnPathChanged += DrawPath;
        }
    }

    private void OnDisable()
    {
        foreach (IPathHandler pathHandler in PathHandlers)
        {
            if (pathHandler == null)
            {
                continue;
            }
            pathHandler.OnPathChanged -= DrawPath;
        }
    }

    private void DrawPath(PathData data)
    {
        if (!data.IsReacheble)
        {
            _pathRenderer.UnsetPathLine();
            return;
        }

        _pathRenderer.SetPathLine(data.Points);
    }
}
