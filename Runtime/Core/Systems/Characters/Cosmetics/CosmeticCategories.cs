using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    public static class CosmeticCategories
    {
        public static readonly Dictionary<string, CharacterPartCategory[]> Groupings =
            new Dictionary<string, CharacterPartCategory[]>
            {
                {
                    "Armor Set",
                    new[]
                    {
                        CharacterPartCategory.ArmorBody,
                        CharacterPartCategory.ArmorHelmet,
                        CharacterPartCategory.ArmorArmR,
                        CharacterPartCategory.ArmorArmL,
                        CharacterPartCategory.ArmorSleeveR,
                        CharacterPartCategory.ArmorSleeveL,
                        CharacterPartCategory.ArmorHandR,
                        CharacterPartCategory.ArmorHandL,
                        CharacterPartCategory.ArmorLegR,
                        CharacterPartCategory.ArmorLegL,
                    }
                },
                { "Hair", new[] { CharacterPartCategory.Hair } },
                { "Beard", new[] { CharacterPartCategory.Beard } },
                { "Eye Brows", new[] { CharacterPartCategory.EyeBrows } },
                { "Eyes", new[] { CharacterPartCategory.Eyes } },
                { "Mouth", new[] { CharacterPartCategory.Mouth } },
                {
                    "Earrings",
                    new[] { CharacterPartCategory.EarringR, CharacterPartCategory.EarringL }
                },
                { "Back", new[] { CharacterPartCategory.Back } },
                { "Mask", new[] { CharacterPartCategory.Mask } },
            };
    }
}
