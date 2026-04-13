using System.Collections.Generic;
using System.Threading.Tasks;

public interface CharacterInfoRepository
{
    Task<API.CharacterInfoResponse> LoadAsync(string characterId);

    Task<API.CharacterInfoResponse> SaveAsync(
        string characterId,
        API.PatchCharacterInfoRequest data
    );
}
