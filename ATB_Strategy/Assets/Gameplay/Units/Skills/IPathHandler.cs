using System;

public interface IPathHandler
{
    event Action<PathData> OnPathChanged;
}
