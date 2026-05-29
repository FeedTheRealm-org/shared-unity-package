using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTRShared.Runtime.Core.Cache;
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

    [Inject]
    private CacheManager cacheManager;

    private string characterId;
    private SpriteLoader spriteLoader;
    private CharacterInfoRepository infoRepository;
    private ICharacterIdSource characterIdSource;
    private ICharacterNameController nameController;
    private bool isInitialized = false;
    private bool isLocalPlayer = false;

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
        CharacterInfoRepository infoRepository,
        ICharacterIdSource characterIdSource,
        ICharacterNameController nameController = null,
        bool isLocalPlayer = false
    )
    {
        this.spriteLoader = spriteLoader;
        this.infoRepository = infoRepository;
        this.characterIdSource = characterIdSource;
        this.nameController = nameController;
        this.isLocalPlayer = isLocalPlayer;

        isInitialized = true;

        if (characterIdSource != null && string.IsNullOrEmpty(characterIdSource.CharacterId))
            characterIdSource.OnCharacterIdChanged += InitForCharacterId;
        else
            InitForCharacterId(characterIdSource.CharacterId);
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
            case CharacterPartCategory.EquipmentR:
                spriteLoader.ChangeEquipment(texture);
                break;
            case CharacterPartCategory.Consumable:
                spriteLoader.ChangeConsumable(texture);
                break;
            case CharacterPartCategory.WeaponRangedBow:
                spriteLoader.ChangeRangedBow(texture);
                break;
            case CharacterPartCategory.WeaponRangedHandheld:
                spriteLoader.ChangeRangedHandheld(texture);
                break;
            default:
                logger?.Log($"No handler for: {part}", this, Logging.LogType.Warning);
                break;
        }
    }

    public void ChangeSkinColor(Color color)
    {
        if (!isInitialized)
            return;

        spriteLoader.ChangeSkinColor(color);
    }

    public void ChangeHairColor(Color color)
    {
        if (!isInitialized)
            return;

        spriteLoader.ChangeHairColor(color);
    }

    public void ChangeEyesColor(Color color)
    {
        if (!isInitialized)
            return;

        spriteLoader.ChangeEyesColor(color);
    }

    public CharacterPartCategory GetPartCategoryFromCategoryName(string categoryName)
    {
        categoryName = categoryName.Replace(" ", "").Replace("_", "").Replace("-", "");
        if (Enum.TryParse(categoryName, true, out CharacterPartCategory part))
        {
            return part;
        }

        logger?.Log(
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
        API.CharacterInfoResponse characterInfo = await infoRepository.LoadAsync(characterId);
        if (characterInfo == null || characterInfo.category_sprites == null)
        {
            logger?.Log(
                $"SpriteManager: Character info not found for characterId '{characterId}'.",
                this,
                Logging.LogType.Warning
            );
            return;
        }

        if (nameController != null && !isLocalPlayer)
        {
            nameController.SetName(characterInfo.character_name ?? "Guest");
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

            var updatedAt = DateTimeOffset.MinValue;
            if (!spriteUrlsById.TryGetValue(entry.Value, out var spriteUrl))
            {
                var sprite = await assetsService.GetSpriteByIdAsync(entry.Value);
                if (sprite == null || string.IsNullOrEmpty(sprite.sprite_url))
                    continue;

                spriteUrl = sprite.sprite_url;
                spriteUrlsById[entry.Value] = spriteUrl;
                updatedAt = DateTimeHelper.ParseDateTimeOffset(sprite.updated_at);
            }

            if (string.IsNullOrEmpty(spriteUrl))
                continue;

            if (!cachedCategoryTexturesByUrl.TryGetValue(spriteUrl, out Texture2D texture))
            {
                texture = await cacheManager.GetSprite(spriteUrl, updatedAt);
                if (texture == null)
                    continue;
                cachedCategoryTexturesByUrl[spriteUrl] = texture;
            }

            if (!existingCategories.TryGetValue(entry.Key, out var name))
                continue;

            var category = GetPartCategoryFromCategoryName(name);
            ChangeSprite(category, texture);
        }

        if (characterInfo.skin_color != null)
            ChangeSkinColor(ToColor(characterInfo.skin_color));
        if (characterInfo.hair_color != null)
            ChangeHairColor(ToColor(characterInfo.hair_color));
        if (characterInfo.eye_color != null)
            ChangeEyesColor(ToColor(characterInfo.eye_color));
    }

    private static Color ToColor(API.CharacterColorHsv hsv)
    {
        var h = Mathf.Repeat(hsv.h, 360f) / 360f;
        var s = Mathf.Clamp01(hsv.s / 100f);
        var v = Mathf.Clamp01(hsv.v / 100f);
        return Color.HSVToRGB(h, s, v);
    }
}
