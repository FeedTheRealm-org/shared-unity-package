using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class NpcSpriteRepository : CharacterSpriteRepository
{
    public async Task<API.CharacterInfoResponse> LoadAsync(string characterId)
    {
        return null;
    }

    public async Task<API.CharacterInfoResponse> SaveAsync(
        string characterId,
        API.PatchCharacterInfoRequest data
    )
    {
        return null;
    }
}
