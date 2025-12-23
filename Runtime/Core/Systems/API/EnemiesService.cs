using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace API {
  [CreateAssetMenu(fileName = "EnemiesService", menuName = "Scriptable Objects/API/EnemiesService")]
  public class EnemiesService : ScriptableObject {
    [Header("Server settings")]
    [SerializeField] public string Hostname;
    [SerializeField] public int Port;

    [Header("General settings")]
    [SerializeField] private Logging.Logger logger;
    [SerializeField] private Session.Session session;

    private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/sprites/enemies";

    /// <summary>
    /// Upload an enemy sprite as multipart/form-data. The form field name used is `sprite`.
    /// Reuses SpriteCreatedData DTO used by item sprites.
    /// </summary>
    public IEnumerator UploadEnemySprite(byte[] fileBytes, string filename, string mimeType, System.Action<SpriteCreatedData, string> handler) {
      logger.Log($"Uploading enemy sprite '{filename}' ({(fileBytes?.Length ?? 0)} bytes) to {GetBaseUrl()}", this);

      var form = new WWWForm();
      form.AddBinaryData("sprite", fileBytes, filename, mimeType);

      var uwr = UnityWebRequest.Post(GetBaseUrl(), form);
      if (!string.IsNullOrEmpty(session.APIToken)) {
        uwr.SetRequestHeader("Authorization", $"Bearer {session.APIToken}");
      }

      logger.Log($"Sending multipart request for enemy sprite {filename}", this);

      yield return uwr.SendWebRequest();

      var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

      if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
        var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
        logger.Log($"UploadEnemySprite error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
        handler?.Invoke(null, res?.detail ?? responseText);
      } else {
        logger.Log($"UploadEnemySprite response: {responseText}", this);
        var envelope = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<DataEnvelope<SpriteCreatedData>>(responseText);
        handler?.Invoke(envelope?.data, "");
      }
    }
  }
}
