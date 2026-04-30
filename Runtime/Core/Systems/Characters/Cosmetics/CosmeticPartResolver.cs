using System;

namespace FTRShared.Runtime.Models
{
    /// <summary>
    /// Resolves CharacterPartCategory given a categoryName from cosmetic.
    /// </summary>
    public static class CosmeticPartResolver
    {
        public static CharacterPartCategory Resolve(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return CharacterPartCategory.None;

            string normalized = categoryName.Replace(" ", "").Replace("_", "").Replace("-", "");

            return normalized switch
            {
                "ArmorHelmet" or "Helmet" => CharacterPartCategory.ArmorHelmet,
                "ArmorBody" or "Body" or "Chest" => CharacterPartCategory.ArmorBody,
                "ArmorArmR" or "ArmR" or "RightArm" => CharacterPartCategory.ArmorArmR,
                "ArmorArmL" or "ArmL" or "LeftArm" => CharacterPartCategory.ArmorArmL,
                "ArmorSleeveR" or "SleeveR" or "RightSleeve" => CharacterPartCategory.ArmorSleeveR,
                "ArmorSleeveL" or "SleeveL" or "LeftSleeve" => CharacterPartCategory.ArmorSleeveL,
                "ArmorHandR" or "HandR" or "RightHand" => CharacterPartCategory.ArmorHandR,
                "ArmorHandL" or "HandL" or "LeftHand" => CharacterPartCategory.ArmorHandL,
                "ArmorLegR" or "LegR" or "RightLeg" => CharacterPartCategory.ArmorLegR,
                "ArmorLegL" or "LegL" or "LeftLeg" => CharacterPartCategory.ArmorLegL,
                "ArmorLegs" or "Legs" => CharacterPartCategory.ArmorLegR,

                "Hair" => CharacterPartCategory.Hair,
                "Beard" => CharacterPartCategory.Beard,
                "EyeBrows" or "Eyebrows" or "Brows" => CharacterPartCategory.EyeBrows,
                "Eyes" => CharacterPartCategory.Eyes,
                "Mouth" => CharacterPartCategory.Mouth,

                "EarringR" or "EarringRight" or "RightEarring" => CharacterPartCategory.EarringR,
                "EarringL" or "EarringLeft" or "LeftEarring" => CharacterPartCategory.EarringL,
                "Earrings" => CharacterPartCategory.EarringR,

                "Back" => CharacterPartCategory.Back,
                "Mask" => CharacterPartCategory.Mask,
                "EquipmentR" or "Equipment" or "Weapon" or "PrimaryWeapon" =>
                    CharacterPartCategory.EquipmentR,

                _ => CharacterPartCategory.None,
            };
        }

        public static bool IsSpriteSheetPart(string categoryName)
        {
            var part = Resolve(categoryName);
            return part != CharacterPartCategory.None && part != CharacterPartCategory.EquipmentR;
        }
    }
}
