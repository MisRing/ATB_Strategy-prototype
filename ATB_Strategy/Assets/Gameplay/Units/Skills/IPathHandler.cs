using System;
using UnityEngine;

public interface IPathHandler
{
    event Action<PathData> OnPathChanged;
}
