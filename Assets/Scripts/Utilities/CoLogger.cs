using Assets.Scripts.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public static class CoLogger 
{
    public static void Log(string str)
    {
        Log(LogLevel.Info, str);
    }
    
    public static void LogWarning(string str)
    {
        Log(LogLevel.Warning, str);
    }
    
    public static void LogError(string str)
    {
        Log(LogLevel.Error, str);
    }

    public static void Log(LogLevel level, string log )
    {
#if UNITY_EDITOR
        var methodInfo = new StackTrace().GetFrame(2).GetMethod();
        var className = methodInfo.DeclaringType.FullName.Split(".").Last();
        if(className.IndexOf("+") > 0){
            className = className.Substring(0, className.IndexOf("+"));
        }
        var hex = StringToColor.GenerateColorString(className);

        switch (level)
        {
            case LogLevel.Info:
                Debug.Log($"[<color={hex}>{className}</color>] {log}");
                break;
            case LogLevel.Warning:
                Debug.LogWarning($" ⚠️⚠️⚠️ [<color={hex}>{className}</color>] {log}");
                break;
            case LogLevel.Error:
                Debug.LogError($" ❌❌❌ [<color={hex}>{className}</color>] {log}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
#endif
    }
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}