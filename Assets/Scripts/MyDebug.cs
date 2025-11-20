using UnityEngine;

public static class MyDebug
{
    static bool canPrintLog=true;
    public static void Log(string message)
    {
        if (!canPrintLog) return;
        Debug.Log(message);
    }
    public static void LogError(string message)
    {
        if (!canPrintLog) return;
        Debug.LogError(message);
    }
    public static void LogWarning(string message)
    {
        if (!canPrintLog) return;
        Debug.LogWarning(message);
    }
}
