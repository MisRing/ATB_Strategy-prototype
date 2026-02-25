using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityViewRenderer : MonoBehaviour
{
    [SerializeField] private PathLineRenderer _pathRenderer;
    [SerializeField] private List<AbilityBasic> _abilities = new List<AbilityBasic>(); //vremenno
    private List<IPathHandler> _pathHandlers = new List<IPathHandler>();

    private void Awake()
    {
        _pathRenderer.Init();

        //vremenno

        foreach (AbilityBasic ability in _abilities)
        {
            if(ability is IPathHandler)
            {
                _pathHandlers.Add((IPathHandler)ability);
            }
        }

        //vremenno
    }

    private void OnEnable()
    {
        foreach(IPathHandler pathHandler in _pathHandlers)
        {
            pathHandler.OnPathChanged += DrawPath;
        }
    }

    private void OnDisable()
    {
        foreach (IPathHandler pathHandler in _pathHandlers)
        {
            pathHandler.OnPathChanged -= DrawPath;
        }
    }

    private void DrawPath(PathData data)
    {
        if (!data.IsReacheble) return;

        _pathRenderer.SetPathLine(data.Points);
    }
}
