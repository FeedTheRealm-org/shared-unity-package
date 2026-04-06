using System;

public interface ICharacterIdSource
{
    string CharacterId { get; }
    event Action<string> OnCharacterIdChanged;
}
