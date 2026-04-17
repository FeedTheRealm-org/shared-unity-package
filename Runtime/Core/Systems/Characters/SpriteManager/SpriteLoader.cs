using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    [SerializeField]
    private Logging.Logger logger;

    private Dictionary<
        FacingDirection,
        Dictionary<CharacterPartCategory, Transform>
    > _cachedPartsPerDirections =
        new Dictionary<FacingDirection, Dictionary<CharacterPartCategory, Transform>>();

    private SpriteConfigBuilder builder;
    private SpriteConfigDirector director;

    private void Awake()
    {
        builder = new SpriteConfigBuilder();
        director = new SpriteConfigDirector(builder);
        CachePartTransforms();
    }

    /* --- PART CHANGE HANDLERS --- */

    public void ChangeHelmet(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorHelmetSpriteConfig());
        logger.Log("[SpriteLoader] Changed Armor Helmet sprites", this);
    }

    public void ChangeBody(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorBodySpriteConfig());
        logger.Log("[SpriteLoader] Changed Armor Body sprites", this);
    }

    public void ChangeLegs(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildArmorLegsSpriteConfig());
        logger.Log("[SpriteLoader] Changed Armor Legs sprites", this);
    }

    public void ChangeHair(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildHairSpriteConfig());
        logger.Log("[SpriteLoader] Changed Hair sprites", this);
    }

    public void ChangeBeard(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildBeardSpriteConfig());
        logger.Log("[SpriteLoader] Changed Beard sprites", this);
    }

    public void ChangeEyeBrows(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEyeBrowsSpriteConfig());
        logger.Log("[SpriteLoader] Changed EyeBrows sprites", this);
    }

    public void ChangeEyes(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEyesSpriteConfig());
        logger.Log("[SpriteLoader] Changed Eyes sprites", this);
    }

    public void ChangeMouth(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildMouthSpriteConfig());
        logger.Log("[SpriteLoader] Changed Mouth sprites", this);
    }

    public void ChangeBack(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildBackSpriteConfig());
        logger.Log("[SpriteLoader] Changed Back sprites", this);
    }

    public void ChangeEarrings(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEarringsSpriteConfig());
        logger.Log("[SpriteLoader] Changed Earrings sprites", this);
    }

    public void ChangeMask(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildMaskSpriteConfig());
        logger.Log("[SpriteLoader] Changed Mask sprites", this);
    }

    public void ChangeEquipment(Texture2D texture)
    {
        ChangeTexture(texture, director.BuildEquipmentSpriteConfig(), true);
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
                out Dictionary<CharacterPartCategory, Transform> partsDict
            )
        )
            return;

        if (
            !partsDict.TryGetValue(pathSegments[0], out Transform currentTransform)
            || currentTransform == null
        )
            return;

        SpriteRenderer spriteRenderer = currentTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite = newSprite;
        spriteRenderer.enabled = (newSprite != null);
    }

    /* --- INITIALIZATION UTILS --- */

    private void CachePartTransforms()
    {
        foreach (FacingDirection direction in Enum.GetValues(typeof(FacingDirection)))
            CacheDirection(direction);
    }

    private void CacheDirection(FacingDirection dir)
    {
        var f = FindChildRecursive;

        var dirTransform = f(transform, dir.ToString());
        if (dirTransform == null)
        {
            logger.Log($"Direction {dir} not found!", this, Logging.LogType.Warning);
            return;
        }

        var cachedParts = new Dictionary<CharacterPartCategory, Transform>();
        cachedParts[CharacterPartCategory.Hair] = f(dirTransform, "Hair");
        cachedParts[CharacterPartCategory.Beard] = f(dirTransform, "Beard");
        cachedParts[CharacterPartCategory.EyeBrows] = f(dirTransform, "Eyesbrows");
        cachedParts[CharacterPartCategory.Eyes] = f(dirTransform, "Eyes");
        cachedParts[CharacterPartCategory.Mouth] = f(dirTransform, "Mouth");

        cachedParts[CharacterPartCategory.ArmorBody] = dirTransform
            .Find("UpperBody")
            ?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorHelmet] = f(dirTransform, "Helmet");

        cachedParts[CharacterPartCategory.ArmorArmR] = f(dirTransform, "ArmR")?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorArmL] = f(dirTransform, "ArmL")?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorSleeveR] = f(dirTransform, "ArmR")?.Find("Sleeve");
        cachedParts[CharacterPartCategory.ArmorSleeveL] = f(dirTransform, "ArmL")?.Find("Sleeve");
        cachedParts[CharacterPartCategory.ArmorHandR] = f(dirTransform, "HandR")?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorHandL] = f(dirTransform, "HandL")?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorLegR] = f(dirTransform, "LegR")?.Find("Armor");
        cachedParts[CharacterPartCategory.ArmorLegL] = f(dirTransform, "LegL")?.Find("Armor");

        cachedParts[CharacterPartCategory.EarringR] = f(dirTransform, "EarringR");
        cachedParts[CharacterPartCategory.EarringL] = f(dirTransform, "EarringL");
        cachedParts[CharacterPartCategory.Back] = f(dirTransform, "Back");
        cachedParts[CharacterPartCategory.Mask] = f(dirTransform, "Mask");
        cachedParts[CharacterPartCategory.EquipmentR] = f(dirTransform, "PrimaryWeapon");

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
