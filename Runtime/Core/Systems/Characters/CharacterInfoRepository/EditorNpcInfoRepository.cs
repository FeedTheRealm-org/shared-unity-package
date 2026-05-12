using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class EditorNpcInfoRepository : CharacterInfoRepository
{
    private API.CharacterInfoResponse defaultData;
    private Action<Dictionary<string, string>> onDataChanged;

    public EditorNpcInfoRepository(
        API.CharacterInfoResponse defaultData,
        Action<Dictionary<string, string>> onDataChanged
    )
    {
        this.defaultData = defaultData;
        this.onDataChanged = onDataChanged;
    }

    public async Task<API.CharacterInfoResponse> LoadAsync(string characterId)
    {
        return defaultData;
    }

    public async Task<API.CharacterInfoResponse> SaveAsync(
        string characterId,
        API.PatchCharacterInfoRequest data
    )
    {
        onDataChanged?.Invoke(data.category_sprites);
        return new API.CharacterInfoResponse
        {
            character_name = data.character_name,
            character_bio = data.character_bio,
            skin_color = data.skin_color,
            hair_color = data.hair_color,
            eye_color = data.eye_color,
            category_sprites = new Dictionary<string, string>(data.category_sprites),
        };
    }
}
