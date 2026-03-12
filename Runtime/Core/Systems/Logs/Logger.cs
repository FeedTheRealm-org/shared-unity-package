using UnityEngine;

namespace Logging
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
    }

    /// <summary>
    /// Handles logging modularly and usable for different logger game objects.
    /// </summary>
    [CreateAssetMenu(fileName = "Logger", menuName = "Scriptable Objects/Logger")]
    public class Logger : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField]
        private bool showLogs;

        [Header("Log Prefix")]
        [SerializeField]
        private string loggerPrefix;

        [Header("Log Colors")]
        [SerializeField]
        private Color loggerColor;

        [SerializeField]
        private bool showColor;

        private string _stringColor = "";
        private string _resetColor = "";

        private void OnEnable()
        {
#if SERVER_BUILD
            showColor = false;
#endif
            if (showColor)
            {
                _stringColor = $"<color=#{ColorUtility.ToHtmlStringRGB(loggerColor)}>";
                _resetColor = "</color>";
            }
        }

        public void Log(object msg, LogType type = LogType.Info)
        {
            Log(msg, null, type);
        }

        public void Log(object msg, Object sender, LogType type = LogType.Info)
        {
            if (!showLogs)
                return;

            var time = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string formattedMsg =
                $"{_stringColor}{time} | {type} | {loggerPrefix} {msg}{_resetColor}";

            switch (type)
            {
                case LogType.Warning:
                    Debug.LogWarning(formattedMsg, sender);
                    break;
                case LogType.Error:
                    Debug.LogError(formattedMsg, sender);
                    break;
                case LogType.Info:
                default:
                    Debug.Log(formattedMsg, sender);
                    break;
            }
        }
    }
}
