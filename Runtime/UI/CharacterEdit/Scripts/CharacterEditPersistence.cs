using System.Threading.Tasks;
using UnityEngine;

public abstract class CharacterEditPersistence : ScriptableObject
{
    public virtual bool ShowBio => true;
    public virtual bool CanEditName => true;

    public abstract Task<API.CharacterInfoResponse> LoadAsync();

    public abstract Task<API.CharacterInfoResponse> SaveAsync(API.PatchCharacterInfoRequest data);
}
