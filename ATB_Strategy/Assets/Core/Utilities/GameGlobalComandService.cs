using System;
using UnityEngine;

public static class GameGlobalComandService
{
    public static event Action OnPlayerCommandReset;
    public static event Action OnUICommandReset;

    public static void ResetPlayerCommands()
    {
        OnPlayerCommandReset?.Invoke();
    }
    
    public static void ResetUI()
    {
        OnUICommandReset?.Invoke();
    }
}
