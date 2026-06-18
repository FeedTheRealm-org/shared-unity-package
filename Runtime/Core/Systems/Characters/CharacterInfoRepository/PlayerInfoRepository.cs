using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class PlayerInfoRepository : CharacterInfoRepository
{
    [Inject]
    private API.PlayerService playerService;

    [Inject]
    private Session.Session session;

    public async Task<API.CharacterInfoResponse> LoadAsync(string characterId)
    {
        return await playerService.GetCharacterInfoAsync(characterId);
    }

    public async Task<API.CharacterInfoResponse> SaveAsync(
        string _characterId,
        API.PatchCharacterInfoRequest data
    )
    {
        var payload = new API.PatchCharacterInfoRequest
        {
            character_name = data.character_name,
            character_bio = data.character_bio,
            skin_color = data.skin_color,
            hair_color = data.hair_color,
            eye_color = data.eye_color,
            category_sprites =
                data.category_sprites != null
                    ? new Dictionary<string, string>(data.category_sprites)
                    : new Dictionary<string, string>(),
        };

        var response = await playerService.PatchCharacterInfoAsync(payload);
        if (response?.data != null && session != null)
        {
            session.IsFirstLogin = false;
            session.CharacterName = response.data.character_name;
        }

        if (response.error != null)
            ShowToastError(response.error.detail);

        return response?.data;
    }

    private void ShowToastError(string message)
    {
        if (!string.IsNullOrEmpty(message) && message.Length > 1)
            message = char.ToUpper(message[0]) + message.Substring(1);
        ToastNotification.Show(message, "error", Color.red);
    }
}
