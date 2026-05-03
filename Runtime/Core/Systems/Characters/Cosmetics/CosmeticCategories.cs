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
                        CharacterPartCategory.ArmorLegR,
                    }
                },
                { "Hair", new[] { CharacterPartCategory.Hair } },
                { "Beard", new[] { CharacterPartCategory.Beard } },
                { "Eye Brows", new[] { CharacterPartCategory.EyeBrows } },
                { "Eyes", new[] { CharacterPartCategory.Eyes } },
                { "Mouth", new[] { CharacterPartCategory.Mouth } },
                { "Earrings", new[] { CharacterPartCategory.EarringR } },
                { "Back", new[] { CharacterPartCategory.Back } },
                { "Mask", new[] { CharacterPartCategory.Mask } },
            };
    }
}
