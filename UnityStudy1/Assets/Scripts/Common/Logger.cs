using System.Diagnostics;

// 추가적인 정보를 넣기 위함 (타임스탬프)
// 출시용 빌드에서 코드의 일괄적 제거
public static class Logger
{
    [Conditional("DEV_VER")]
    public static void Log(string msg)
    {
        UnityEngine.Debug.LogFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), msg);
    }

    [Conditional("DEV_VER")]
    public static void LogWarning(string msg)
    {
        UnityEngine.Debug.LogWarningFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), msg);
    }

    public static void LogError(string msg)
    {
        UnityEngine.Debug.LogErrorFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), msg);
    }
}
