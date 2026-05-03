using System.Collections.Generic;
using System.Linq;
using FTRShared.Runtime.Models;
using UnityEngine;

public static class CosmeticIconLoader
{
    private static SpriteConfigBuilder _configBuilder;
    private static SpriteConfigDirector _configDirector;

    public static Sprite CreateCroppedSprite(Texture2D texture, string categoryName)
    {
        if (texture == null || string.IsNullOrEmpty(categoryName))
            return null;

        var part = CosmeticPartResolver.Resolve(categoryName);
        if (part == CharacterPartCategory.None)
            return null;

        EnsureDirectorInitialized();

        var configs = GetSpriteConfigsForPart(part);
        var frontConfig = configs.FirstOrDefault(c => c.Direction == FacingDirection.Front);

        if (frontConfig == null)
            return null;

        Rect rect = frontConfig.Rect;

        if (
            rect.x + rect.width > texture.width
            || rect.y + rect.height > texture.height
            || rect.width <= 0
            || rect.height <= 0
        )
        {
            return null;
        }

        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), frontConfig.PixelsPerUnit);
    }

    public static Sprite CreateFullSprite(Texture2D texture)
    {
        if (texture == null)
            return null;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    private static List<SpriteConfig> GetSpriteConfigsForPart(CharacterPartCategory part)
    {
        return part switch
        {
            CharacterPartCategory.ArmorHelmet => _configDirector.BuildArmorHelmetSpriteConfig(),
            CharacterPartCategory.ArmorBody => _configDirector.BuildArmorBodySpriteConfig(),
            CharacterPartCategory.ArmorArmR => _configDirector.BuildArmorArmsSpriteConfig(),
            CharacterPartCategory.ArmorArmL => _configDirector.BuildArmorArmsSpriteConfig(),
            CharacterPartCategory.ArmorSleeveR => _configDirector.BuildArmorSleevesSpriteConfig(),
            CharacterPartCategory.ArmorSleeveL => _configDirector.BuildArmorSleevesSpriteConfig(),
            CharacterPartCategory.ArmorHandR => _configDirector.BuildArmorHandsSpriteConfig(),
            CharacterPartCategory.ArmorHandL => _configDirector.BuildArmorHandsSpriteConfig(),
            CharacterPartCategory.ArmorLegR => _configDirector.BuildArmorLegsSpriteConfig(),
            CharacterPartCategory.ArmorLegL => _configDirector.BuildArmorLegsSpriteConfig(),
            CharacterPartCategory.Hair => _configDirector.BuildHairSpriteConfig(),
            CharacterPartCategory.Beard => _configDirector.BuildBeardSpriteConfig(),
            CharacterPartCategory.EyeBrows => _configDirector.BuildEyeBrowsSpriteConfig(),
            CharacterPartCategory.Eyes => _configDirector.BuildEyesSpriteConfig(),
            CharacterPartCategory.Mouth => _configDirector.BuildMouthSpriteConfig(),
            CharacterPartCategory.EarringR => _configDirector.BuildEarringsSpriteConfig(),
            CharacterPartCategory.EarringL => _configDirector.BuildEarringsSpriteConfig(),
            CharacterPartCategory.Back => _configDirector.BuildBackSpriteConfig(),
            CharacterPartCategory.Mask => _configDirector.BuildMaskSpriteConfig(),
            CharacterPartCategory.EquipmentR => _configDirector.BuildEquipmentSpriteConfig(),
            _ => new List<SpriteConfig>(),
        };
    }

    private static void EnsureDirectorInitialized()
    {
        if (_configBuilder == null)
        {
            _configBuilder = new SpriteConfigBuilder();
            _configDirector = new SpriteConfigDirector(_configBuilder);
        }
    }
}
