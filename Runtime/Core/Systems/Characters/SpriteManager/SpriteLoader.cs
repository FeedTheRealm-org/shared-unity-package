using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    [SerializeField]
    private Logging.Logger logger;

    private Dictionary<
        FacingDirection,
        Dictionary<CharacterPartCategory, SpriteRenderer>
    > _cachedPartsPerDirections =
        new Dictionary<FacingDirection, Dictionary<CharacterPartCategory, SpriteRenderer>>();

    private SpriteConfigBuilder builder;
    private SpriteConfigDirector director;

    private void Awake()
    {
        builder = new SpriteConfigBuilder();
        director = new SpriteConfigDirector(builder);
        CachePartSpriteRenderers();
    }

    /* --- PART CHANGE HANDLERS --- */

    public void ChangeHelmet(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorHelmetSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Armor Helmet sprites", this);
    }

    public void ChangeBody(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorBodySpriteConfig());
        logger?.Log("[SpriteLoader] Changed Armor Body sprites", this);
    }

    public void ChangeLegs(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorLegsSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Armor Legs sprites", this);
    }

    public void ChangeHair(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildHairSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Hair sprites", this);
    }

    public void ChangeBeard(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildBeardSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Beard sprites", this);
    }

    public void ChangeEyeBrows(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEyeBrowsSpriteConfig());
        logger?.Log("[SpriteLoader] Changed EyeBrows sprites", this);
    }

    public void ChangeEyes(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEyesSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Eyes sprites", this);
    }

    public void ChangeMouth(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildMouthSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Mouth sprites", this);
    }

    public void ChangeBack(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildBackSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Back sprites", this);
    }

    public void ChangeEarrings(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEarringsSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Earrings sprites", this);
    }

    public void ChangeMask(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildMaskSpriteConfig());
        logger?.Log("[SpriteLoader] Changed Mask sprites", this);
    }

    public void ChangeEquipment(Texture2D texture)
    {
        ChangeTexture(null, director.BuildRangedBowEquipmentSpriteConfig()); // Reset bow
        ChangeTexture(texture, director.BuildEquipmentSpriteConfig());
    }

    public void ChangeConsumable(Texture2D texture)
    {
        ChangeTexture(null, director.BuildRangedBowEquipmentSpriteConfig()); // Reset bow
        ChangeTexture(texture, director.BuildConsumableSpriteConfig());
    }

    public void ChangeRangedHandheld(Texture2D texture)
    {
        ChangeTexture(null, director.BuildRangedBowEquipmentSpriteConfig()); // Reset bow
        ChangeTexture(texture, director.BuildRangedHandheldEquipmentSpriteConfig());
    }

    public void ChangeRangedBow(Texture2D texture)
    {
        ChangeTexture(null, director.BuildEquipmentSpriteConfig()); // Reset handheld
        ChangeTexture(texture, director.BuildRangedBowEquipmentSpriteConfig());
    }

    public void ChangeSkinColor(Color color)
    {
        SetPartColor(CharacterPartCategory.HeadSkin, color);
        SetPartColor(CharacterPartCategory.BodySkin, color);
        SetPartColor(CharacterPartCategory.ArmSkinR, color);
        SetPartColor(CharacterPartCategory.ArmSkinL, color);
        SetPartColor(CharacterPartCategory.HandSkinR, color);
        SetPartColor(CharacterPartCategory.HandSkinL, color);
        SetPartColor(CharacterPartCategory.LegSkinR, color);
        SetPartColor(CharacterPartCategory.LegSkinL, color);
    }

    public void ChangeHairColor(Color color)
    {
        SetPartColor(CharacterPartCategory.Hair, color);
        SetPartColor(CharacterPartCategory.Beard, color);
        SetPartColor(CharacterPartCategory.EyeBrows, color);
    }

    public void ChangeEyesColor(Color color)
    {
        SetPartColor(CharacterPartCategory.Eyes, color);
    }

    /* --- CORE SPRITE CHANGE LOGIC --- */

    /// <summary>
    /// Changes the texture for multiple sprite configurations [or removes it if texture is null!].
    /// </summary>
    private void ChangeTexture(
        Texture2D texture,
        List<SpriteConfig> confs,
        bool useFullRectIfZero = false
    )
    {
        foreach (var config in confs)
        {
            Sprite sprite = null;

            if (texture != null)
            {
                Rect finalRect = config.Rect;
                if (useFullRectIfZero && (finalRect.width == 0 || finalRect.height == 0))
                {
                    finalRect = new Rect(0, 0, texture.width, texture.height);
                }
                sprite = Sprite.Create(texture, finalRect, config.Pivot, config.PixelsPerUnit);
            }

            ReplacePartSprite(sprite, config.Direction, config.Part);
        }
    }

    /// <summary>
    /// Replaces the sprite of a part at the given path.
    /// Example: ReplacePartSprite(newSprite, "Parent", "Child", "TargetObject")
    /// </summary>
    private void ReplacePartSprite(
        Sprite newSprite,
        FacingDirection direction,
        params CharacterPartCategory[] pathSegments
    )
    {
        if (pathSegments == null || pathSegments.Length == 0)
            return;

        if (
            !_cachedPartsPerDirections.TryGetValue(
                direction,
                out Dictionary<CharacterPartCategory, SpriteRenderer> partsDict
            )
        )
            return;

        if (
            !partsDict.TryGetValue(pathSegments[0], out SpriteRenderer currentSprite)
            || currentSprite == null
        )
            return;

        currentSprite.sprite = newSprite;
        currentSprite.enabled = (newSprite != null);
    }

    private void SetPartColor(CharacterPartCategory part, Color color)
    {
        foreach (var partsByDirection in _cachedPartsPerDirections.Values)
        {
            if (!partsByDirection.TryGetValue(part, out var currentSprite) || currentSprite == null)
                continue;

            if (currentSprite != null)
                currentSprite.color = color;
        }
    }

    /* --- INITIALIZATION UTILS --- */

    private void CachePartSpriteRenderers()
    {
        foreach (FacingDirection direction in Enum.GetValues(typeof(FacingDirection)))
            CacheDirection(direction);
    }

    private void CacheDirection(FacingDirection dir)
    {
        var f = FindChildRecursive;
        var g = (Transform transform) => transform?.GetComponent<SpriteRenderer>();

        var dirTransform = f(transform, dir.ToString());
        if (dirTransform == null)
        {
            logger?.Log($"Direction {dir} not found!", this, Logging.LogType.Warning);
            return;
        }

        var cachedParts = new Dictionary<CharacterPartCategory, SpriteRenderer>();
        cachedParts[CharacterPartCategory.Hair] = g(f(dirTransform, "Hair"));
        cachedParts[CharacterPartCategory.Beard] = g(f(dirTransform, "Beard"));
        cachedParts[CharacterPartCategory.EyeBrows] = g(f(dirTransform, "Eyesbrows"));
        cachedParts[CharacterPartCategory.Eyes] = g(f(dirTransform, "Eyes"));
        cachedParts[CharacterPartCategory.Mouth] = g(f(dirTransform, "Mouth"));

        cachedParts[CharacterPartCategory.HeadSkin] = g(f(dirTransform, "Head"));
        cachedParts[CharacterPartCategory.BodySkin] = g(f(dirTransform, "Body"));
        cachedParts[CharacterPartCategory.ArmSkinR] = g(f(dirTransform, "ArmR"));
        cachedParts[CharacterPartCategory.ArmSkinL] = g(f(dirTransform, "ArmL"));
        cachedParts[CharacterPartCategory.HandSkinR] = g(f(dirTransform, "HandR"));
        cachedParts[CharacterPartCategory.HandSkinL] = g(f(dirTransform, "HandL"));
        cachedParts[CharacterPartCategory.LegSkinR] = g(f(dirTransform, "LegR"));
        cachedParts[CharacterPartCategory.LegSkinL] = g(f(dirTransform, "LegL"));

        cachedParts[CharacterPartCategory.ArmorBody] = g(
            dirTransform.Find("UpperBody")?.Find("Armor")
        );
        cachedParts[CharacterPartCategory.ArmorHelmet] = g(f(dirTransform, "Helmet"));

        cachedParts[CharacterPartCategory.ArmorArmR] = g(f(dirTransform, "ArmR")?.Find("Armor"));
        cachedParts[CharacterPartCategory.ArmorArmL] = g(f(dirTransform, "ArmL")?.Find("Armor"));
        cachedParts[CharacterPartCategory.ArmorSleeveR] = g(
            f(dirTransform, "ArmR")?.Find("Sleeve")
        );
        cachedParts[CharacterPartCategory.ArmorSleeveL] = g(
            f(dirTransform, "ArmL")?.Find("Sleeve")
        );
        cachedParts[CharacterPartCategory.ArmorHandR] = g(f(dirTransform, "HandR")?.Find("Armor"));
        cachedParts[CharacterPartCategory.ArmorHandL] = g(f(dirTransform, "HandL")?.Find("Armor"));
        cachedParts[CharacterPartCategory.ArmorLegR] = g(f(dirTransform, "LegR")?.Find("Armor"));
        cachedParts[CharacterPartCategory.ArmorLegL] = g(f(dirTransform, "LegL")?.Find("Armor"));

        cachedParts[CharacterPartCategory.EarringR] = g(f(dirTransform, "EarringR"));
        cachedParts[CharacterPartCategory.EarringL] = g(f(dirTransform, "EarringL"));
        cachedParts[CharacterPartCategory.Back] = g(f(dirTransform, "Back"));
        cachedParts[CharacterPartCategory.Mask] = g(f(dirTransform, "Mask"));
        cachedParts[CharacterPartCategory.EquipmentR] = g(f(dirTransform, "PrimaryWeapon"));

        cachedParts[CharacterPartCategory.BowLimbL] = g(f(dirTransform, "LimbL"));
        cachedParts[CharacterPartCategory.BowLimbU] = g(f(dirTransform, "LimbU"));
        cachedParts[CharacterPartCategory.BowHandle] = g(f(dirTransform, "Handle"));
        cachedParts[CharacterPartCategory.BowQuiver] = g(f(dirTransform, "Quiver"));

        _cachedPartsPerDirections[dir] = cachedParts;
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        Transform result = parent.Find(childName);
        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = FindChildRecursive(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }
}
