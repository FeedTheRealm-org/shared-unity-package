using System.Collections.Generic;
using Models;

namespace Worlds
{
    /// <summary>
    /// Static registry that exposes the current world's consumable items and enemies
    /// to gameplay systems (loot, inventory, tooltips).
    /// Uses spriteId as the canonical identifier for world items.
    /// </summary>
    public static class WorldItemsRegistry
    {
        /// <summary>
        /// Last world data registered by a loader (client or server).
        /// </summary>
        public static WorldData CurrentWorldData { get; private set; }

        private static readonly Dictionary<string, ConsumableItem> consumablesBySpriteId =
            new Dictionary<string, ConsumableItem>();

        private static readonly HashSet<string> worldItemSpriteIds = new HashSet<string>();

        /// <summary>
        /// Register world data so that other systems can query items/enemies.
        /// </summary>
        public static void RegisterWorldData(WorldData data)
        {
            CurrentWorldData = data;

            consumablesBySpriteId.Clear();
            worldItemSpriteIds.Clear();

            if (data == null || data.consumableItems == null)
            {
                return;
            }

            foreach (var consumable in data.consumableItems)
            {
                if (consumable == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(consumable.spriteId))
                {
                    continue;
                }

                consumablesBySpriteId[consumable.spriteId] = consumable;
                worldItemSpriteIds.Add(consumable.spriteId);
            }
        }

        /// <summary>
        /// Returns true if the given ID matches a world consumable spriteId.
        /// </summary>
        public static bool IsWorldItem(string id)
        {
            return !string.IsNullOrEmpty(id) && worldItemSpriteIds.Contains(id);
        }

        /// <summary>
        /// Get consumable definition by its spriteId. Returns null if not found.
        /// </summary>
        public static ConsumableItem GetConsumableBySpriteId(string spriteId)
        {
            if (string.IsNullOrEmpty(spriteId))
            {
                return null;
            }

            consumablesBySpriteId.TryGetValue(spriteId, out var consumable);
            return consumable;
        }
    }
}
