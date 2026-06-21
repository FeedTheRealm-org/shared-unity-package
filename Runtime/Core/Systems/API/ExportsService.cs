using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(
        fileName = "ExportsService",
        menuName = "Scriptable Objects/API/ExportsService"
    )]
    public class ExportsService : BaseApiService
    {
        [SerializeField]
        private ApiConfig apiConfig;

        private string VersionsUrl => $"{apiConfig.Hostname}:{apiConfig.Port}/exports/zip/versions";

        /// <summary>
        /// Returns the latest version string for the given app on the current OS
        /// (Windows or Linux). On failure, version is "" and error contains the reason.
        /// </summary>
        public async Task<(string version, string error)> GetLatestVersion()
        {
            string targetOs = GetCurrentOs();
            string appName = GetCurrentApp();
            if (targetOs == null || appName == null)
            {
                string platformError =
                    $"[ExportsService] Unsupported platform '{Application.platform}' or application name for version lookup.";
                logger?.Log(platformError, this, Logging.LogType.Error);
                return ("", platformError);
            }

            var (responseText, result, statusCode) = await SendRequestAsync(
                VersionsUrl,
                "GET",
                session.AccessToken,
                null,
                "GetLatestVersion"
            );

            string requestError = ParseError(result, responseText, statusCode, "GetLatestVersion");
            if (requestError != null)
                return ("", requestError);

            ExportsListResponse envelope;
            try
            {
                envelope = JsonUtility.FromJson<ExportsListResponse>(responseText);
            }
            catch (Exception ex)
            {
                string parseError = $"Failed to parse versions response: {ex.Message}";
                logger?.Log(parseError, this, Logging.LogType.Error);
                return ("", parseError);
            }

            if (envelope?.data == null)
            {
                const string emptyError = "Failed to parse versions response.";
                logger?.Log(emptyError, this, Logging.LogType.Error);
                return ("", emptyError);
            }

            foreach (var entry in envelope.data)
            {
                if (entry.is_latest && entry.app_name == appName && entry.os == targetOs)
                    return (entry.version, "");
            }

            string notFoundError = $"No latest version found for app '{appName}' on '{targetOs}'.";
            logger?.Log(notFoundError, this, Logging.LogType.Warning);
            return ("", notFoundError);
        }

        private static string GetCurrentOs()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "windows";
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return "linux";
                default:
                    return null;
            }
        }

        private static string GetCurrentApp()
        {
            switch (Application.productName)
            {
                case "Feed the Realm":
                    return "ftr_game";
                case "Feed the Realm - World Editor":
                    return "ftr_world_editor";
                default:
                    return null;
            }
        }
    }
}
