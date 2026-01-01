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
            if (showColor)
            {
                _stringColor = $"<color=#{ColorUtility.ToHtmlStringRGB(loggerColor)}>";
                _resetColor = "</color>";
            }
        }

        public void Log(object msg, Object sender, LogType type = LogType.Info)
        {
            if (showLogs)
            {
                var time = System.DateTime.Now.ToString("HH:mm:ss.fff");
                string formattedMsg =
                    $"{_stringColor}{time} | {type.ToString()} | {loggerPrefix} {msg}{_resetColor}";
                switch (type)
                {
                    case LogType.Info:
                        Debug.Log(formattedMsg, sender);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(formattedMsg, sender);
                        break;
                    case LogType.Error:
                        Debug.LogError(formattedMsg, sender);
                        break;
                }
            }
        }
    }
}
