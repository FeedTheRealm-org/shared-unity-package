using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
  [CreateAssetMenu(fileName = "ItemsService", menuName = "Scriptable Objects/API/ItemsService")]
  public class ItemsService : ScriptableObject
  {
    [Header("Server settings")]
    [SerializeField] public string Hostname;
    [SerializeField] public int Port;

    [Header("General settings")]
    [SerializeField] private Logging.Logger logger;
    [SerializeField] private Session.Session session;

    private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/sprites/items";

    /// <summary>
    /// Upload an item sprite as multipart/form-data. The form field name used is `sprite`.
    /// </summary>
    public async Task UploadItemSprite(byte[] fileBytes, string filename, string mimeType, System.Action<SpriteCreatedData, string> handler)
    {
      logger.Log($"Uploading sprite '{filename}' ({(fileBytes?.Length ?? 0)} bytes) to {GetBaseUrl()}", this);

      var form = new WWWForm();
      form.AddBinaryData("sprite", fileBytes, filename, mimeType);

      // Use UnityWebRequest.Post with the WWWForm (same approach as ModelService.UploadAssets)
      var uwr = UnityWebRequest.Post(GetBaseUrl(), form);
      if (!string.IsNullOrEmpty(session.APIToken))
      {
        uwr.SetRequestHeader("Authorization", $"Bearer {session.APIToken}");
      }

      logger.Log($"Sending multipart request for {filename}", this);

      await uwr.SendWebRequest();

      var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

      if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
      {
        var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
        logger.Log($"UploadItemSprite error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
        handler?.Invoke(null, res?.detail ?? responseText);
      }
      else
      {
        logger.Log($"UploadItemSprite response: {responseText}", this);
        var envelope = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<DataEnvelope<SpriteCreatedData>>(responseText);
        handler?.Invoke(envelope?.data, "");
      }
    }
  }
}
