using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class PlayerSpriteRepository : CharacterSpriteRepository
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
            category_sprites =
                data.category_sprites != null
                    ? new Dictionary<string, string>(data.category_sprites)
                    : new Dictionary<string, string>(),
        };

        var characterInfo = await playerService.PatchCharacterInfoAsync(payload);
        if (characterInfo != null && session != null)
        {
            session.IsFirstLogin = false;
            session.CharacterName = characterInfo.character_name;
        }

        return characterInfo;
    }
}
