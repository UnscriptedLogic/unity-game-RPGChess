using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public static class GameManager
{
    private static bool doLogging = false;

    private static List<Unit> playerTeam;
    private static List<List<Unit>> enemyWaves;

    public static void SetPlayerTeam(List<Unit> playerTeam)
    {
        GameManager.playerTeam = playerTeam;
        LogSender();
    }

    private static void LogSender(string message = "")
    {
        if (!doLogging) return;

        StackTrace stackTrace = new StackTrace();
        MethodBase method = stackTrace.GetFrame(1).GetMethod();
        MethodBase sender = stackTrace.GetFrame(2).GetMethod();

        string logStatement = $"{sender.Name}() in {sender.ReflectedType.Name} called {method.Name}()";
        if (message != "")
        {
            logStatement += $"with the message:\n{message}";
        }

        UnityEngine.Debug.Log(logStatement);
    }
}
