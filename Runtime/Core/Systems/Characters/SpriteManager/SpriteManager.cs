using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class SpriteManager : MonoBehaviour
{
    [Inject]
    private API.AssetsService assetsService;

    [Inject]
    private API.PlayerService playerService;

    [Inject]
    private Logging.Logger logger;

    private string characterId;
    private SpriteLoader spriteLoader;
    private CharacterSpriteRepository spriteRepository;
    private ICharacterIdSource characterIdSource;
    private bool isInitialized = false;

    private Dictionary<string, Texture2D> cachedCategoryTexturesByUrl =
        new Dictionary<string, Texture2D>();

    private void OnDestroy()
    {
        if (characterIdSource != null)
            characterIdSource.OnCharacterIdChanged -= InitForCharacterId;
        foreach (var texture in cachedCategoryTexturesByUrl.Values)
        {
            if (texture != null)
                Destroy(texture);
        }
        cachedCategoryTexturesByUrl.Clear();
    }

    public void Initialize(
        SpriteLoader spriteLoader,
        CharacterSpriteRepository spriteRepository,
        ICharacterIdSource characterIdSource
    )
    {
        this.spriteLoader = spriteLoader;
        this.spriteRepository = spriteRepository;
        this.characterIdSource = characterIdSource;

        if (characterIdSource != null && string.IsNullOrEmpty(characterIdSource.CharacterId))
            characterIdSource.OnCharacterIdChanged += InitForCharacterId;
        else
            InitForCharacterId(characterIdSource.CharacterId);

        isInitialized = true;
    }

    public void ChangeSprite(CharacterPartCategory part, Texture2D texture)
    {
        if (part == CharacterPartCategory.None || !isInitialized)
            return;

        switch (part)
        {
            case CharacterPartCategory.ArmorHelmet:
                spriteLoader.ChangeHelmet(texture);
                break;
            case CharacterPartCategory.ArmorBody:
                spriteLoader.ChangeBody(texture);
                break;
            case CharacterPartCategory.ArmorLegL:
            case CharacterPartCategory.ArmorLegR:
                spriteLoader.ChangeLegs(texture);
                break;
            case CharacterPartCategory.Hair:
                spriteLoader.ChangeHair(texture);
                break;
            case CharacterPartCategory.Beard:
                spriteLoader.ChangeBeard(texture);
                break;
            case CharacterPartCategory.EyeBrows:
                spriteLoader.ChangeEyeBrows(texture);
                break;
            case CharacterPartCategory.Eyes:
                spriteLoader.ChangeEyes(texture);
                break;
            case CharacterPartCategory.Mouth:
                spriteLoader.ChangeMouth(texture);
                break;
            case CharacterPartCategory.Back:
                spriteLoader.ChangeBack(texture);
                break;
            case CharacterPartCategory.EarringR:
            case CharacterPartCategory.EarringL:
                spriteLoader.ChangeEarrings(texture);
                break;
            case CharacterPartCategory.Mask:
                spriteLoader.ChangeMask(texture);
                break;
            default:
                logger.Log($"No handler for: {part}", this, Logging.LogType.Warning);
                break;
        }
    }

    public CharacterPartCategory GetPartCategoryFromCategoryName(string categoryName)
    {
        categoryName = categoryName.Replace(" ", "").Replace("_", "").Replace("-", "");
        if (Enum.TryParse(categoryName, true, out CharacterPartCategory part))
        {
            return part;
        }

        logger.Log(
            $"SpriteManager: Unknown category name {categoryName}",
            this,
            Logging.LogType.Warning
        );
        return CharacterPartCategory.None;
    }

    private void InitForCharacterId(string characterId)
    {
        _ = InitForCharacterIdAsync(characterId);
    }

    private async Task InitForCharacterIdAsync(string characterId)
    {
        this.characterId = characterId;

        // Get character info category_id -> sprite_id
        API.CharacterInfoResponse characterInfo = await spriteRepository.LoadAsync(characterId);
        if (characterInfo == null || characterInfo.category_sprites == null)
        {
            logger.Log(
                $"SpriteManager: Character info not found for characterId '{characterId}'.",
                this,
                Logging.LogType.Warning
            );
            return;
        }

        // Get category_id -> category_name
        var categoriesResponse = await assetsService.GetCategoriesAsync();
        if (categoriesResponse == null)
            return;
        var existingCategories = new Dictionary<string, string>();
        foreach (var category in categoriesResponse.category_list)
            existingCategories[category.category_id] = category.category_name;

        // Get sprite_id -> sprite_url, download texture and apply to sprite loader
        var spriteUrlsById = new Dictionary<string, string>();
        foreach (var entry in characterInfo.category_sprites)
        {
            if (string.IsNullOrEmpty(entry.Value))
                continue;

            if (!spriteUrlsById.TryGetValue(entry.Value, out var spriteUrl))
            {
                var sprite = await assetsService.GetSpriteByIdAsync(entry.Value);
                if (sprite == null || string.IsNullOrEmpty(sprite.sprite_url))
                    continue;

                spriteUrl = sprite.sprite_url;
                spriteUrlsById[entry.Value] = spriteUrl;
            }

            if (string.IsNullOrEmpty(spriteUrl))
                continue;

            if (!cachedCategoryTexturesByUrl.TryGetValue(spriteUrl, out Texture2D texture))
            {
                texture = await assetsService.DownloadTexture2D(spriteUrl);
                if (texture == null)
                    continue;
                cachedCategoryTexturesByUrl[spriteUrl] = texture;
            }

            if (!existingCategories.TryGetValue(entry.Key, out var name))
                continue;

            var category = GetPartCategoryFromCategoryName(name);
            ChangeSprite(category, texture);
        }
    }
}
