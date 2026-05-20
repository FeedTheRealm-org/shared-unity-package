using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    /// <summary>
    /// Service to manage player character information.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerService", menuName = "Scriptable Objects/API/PlayerService")]
    public class PlayerService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}/player/character";

        private string GetWorldJoinTokenUrl() =>
            $"{apiConfig.Hostname}:{apiConfig.Port}/player/world-access/token";

        private string GetWorldJoinTokenConsumeUrl() =>
            $"{apiConfig.Hostname}:{apiConfig.Port}/player/world-access/token/consume";

        /// <summary>
        /// Update the character information such as name and bio.
        /// </summary>
        public IEnumerator PatchCharacterInfo(
            PatchCharacterInfoRequest payload,
            System.Action<CharacterInfoResponse, string> handler,
            bool isRetry = false
        )
        {
            var url = GetBaseUrl();
            var json = JsonConvert.SerializeObject(payload);

            var uwr = new UnityWebRequest(url, "PATCH");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            yield return uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                yield return session.EnsureValidSession();
                if (string.IsNullOrWhiteSpace(session.AccessToken))
                    handler?.Invoke(null, "Unauthorized and failed to refresh session.");
                yield return PatchCharacterInfo(payload, handler, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"CharacterInfo error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(null, res.detail);
            }
            else
            {
                var res = JsonConvert.DeserializeObject<DataEnvelope<CharacterInfoResponse>>(
                    responseText
                );
                logger.Log($"CharacterInfo response: {responseText}", this);
                handler?.Invoke(res.data, "");
            }
        }

        /// <summary>
        /// Retrieve the character information such as name and bio for a given user.
        /// If no userID is provided it retrieves the currently logged in userID.
        /// </summary>
        public IEnumerator GetCharacterInfo(
            System.Action<CharacterInfoResponse, string> handler,
            string UserID = null,
            bool isRetry = false
        )
        {
            var url = $"{GetBaseUrl()}/{(UserID ?? session.UserID)}";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            yield return uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                yield return session.EnsureValidSession();
                if (string.IsNullOrWhiteSpace(session.AccessToken))
                    handler?.Invoke(null, "Unauthorized and failed to refresh session.");
                yield return GetCharacterInfo(handler, UserID, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"CharacterInfo error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Warning
                );
                handler?.Invoke(null, res.detail);
            }
            else
            {
                var res = JsonConvert.DeserializeObject<DataEnvelope<CharacterInfoResponse>>(
                    responseText
                );
                logger.Log($"CharacterInfo response: {responseText}", this);
                handler?.Invoke(res.data, "");
            }
        }

        /// <summary>
        /// Retrieve the character information such as name and bio for a given user asynchronously.
        /// If no userID is provided it retrieves the currently logged in userID.
        /// </summary>
        public async Task<CharacterInfoResponse> GetCharacterInfoAsync(
            string UserID = null,
            bool isRetry = false
        )
        {
            var url = $"{GetBaseUrl()}/{(UserID ?? session.UserID)}";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await GetCharacterInfoAsync(UserID, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"GetCharacterInfo error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }
            else
            {
                var res = JsonConvert.DeserializeObject<DataEnvelope<CharacterInfoResponse>>(
                    responseText
                );
                logger.Log($"GetCharacterInfo response: {responseText}", this);
                return res.data;
            }
        }

        /// <summary>
        /// Update the character information such as name and bio asynchronously.
        /// </summary>
        public async Task<API.ApiResponse<CharacterInfoResponse>> PatchCharacterInfoAsync(
            PatchCharacterInfoRequest payload,
            bool isRetry = false
        )
        {
            var url = GetBaseUrl();
            var json = JsonConvert.SerializeObject(payload);

            var uwr = new UnityWebRequest(url, "PATCH");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                {
                    return new API.ApiResponse<CharacterInfoResponse>
                    {
                        status = 401,
                        error = new ErrorResponse
                        {
                            title = "Unauthorized",
                            detail = "Unauthorized and failed to refresh session.",
                        },
                    };
                }
                return await PatchCharacterInfoAsync(payload, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"PatchCharacterInfo error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return new API.ApiResponse<CharacterInfoResponse>
                {
                    status = (int)uwr.responseCode,
                    error = res ?? new ErrorResponse { detail = responseText },
                };
            }
            else
            {
                var res = JsonConvert.DeserializeObject<DataEnvelope<CharacterInfoResponse>>(
                    responseText
                );
                logger.Log($"PatchCharacterInfo response: {responseText}", this);
                return new API.ApiResponse<CharacterInfoResponse>
                {
                    data = res?.data,
                    status = (int)uwr.responseCode,
                };
            }
        }

        /// <summary>
        /// Issues a short-lived one-time token used by the game server to resolve and set the real user ID.
        /// </summary>
        public async Task<WorldJoinTokenResponse> IssueWorldJoinTokenAsync(
            string worldId,
            bool isRetry = false
        )
        {
            if (string.IsNullOrWhiteSpace(worldId))
            {
                logger.Log(
                    "IssueWorldJoinTokenAsync failed: worldId is empty",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var url = GetWorldJoinTokenUrl();
            var payload = new IssueWorldJoinTokenRequest { world_id = worldId };
            var json = JsonConvert.SerializeObject(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await IssueWorldJoinTokenAsync(worldId, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"IssueWorldJoinTokenAsync error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var envelope = JsonConvert.DeserializeObject<DataEnvelope<WorldJoinTokenResponse>>(
                responseText
            );
            logger.Log($"IssueWorldJoinTokenAsync response: {responseText}", this);
            return envelope?.data;
        }

        /// <summary>
        /// Consumes and burns a world join token, returning the resolved user ID.
        /// Intended for server-side usage when validating local player token transactions.
        /// </summary>
        public async Task<ConsumeWorldJoinTokenResponse> ConsumeWorldJoinTokenAsync(
            string tokenId,
            string authorizationToken = null,
            bool isRetry = false
        )
        {
            if (string.IsNullOrWhiteSpace(tokenId))
            {
                logger.Log(
                    "ConsumeWorldJoinTokenAsync failed: tokenId is empty",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var url = GetWorldJoinTokenConsumeUrl();
            var payload = new ConsumeWorldJoinTokenRequest { token_id = tokenId };
            var json = JsonConvert.SerializeObject(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Content-Type", "application/json");

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await ConsumeWorldJoinTokenAsync(tokenId, authorizationToken, isRetry: true);
            }

            var bearerToken = !string.IsNullOrWhiteSpace(authorizationToken)
                ? authorizationToken
                : session?.AccessToken;
            uwr.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

            await uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonConvert.DeserializeObject<ErrorResponse>(responseText);
                logger.Log(
                    $"ConsumeWorldJoinTokenAsync error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var envelope = JsonConvert.DeserializeObject<
                DataEnvelope<ConsumeWorldJoinTokenResponse>
            >(responseText);
            logger.Log($"ConsumeWorldJoinTokenAsync response: {responseText}", this);
            return envelope?.data;
        }
    }
}
