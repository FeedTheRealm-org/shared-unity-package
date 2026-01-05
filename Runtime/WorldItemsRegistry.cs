using System.Collections.Generic;
using Models;
using UnityEngine;

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

        private static readonly Dictionary<string, ConsumableItemData> consumablesBySpriteId =
            new Dictionary<string, ConsumableItemData>();

        private static readonly HashSet<string> worldItemSpriteIds = new HashSet<string>();

        /// <summary>
        /// Register world data so that other systems can query items/enemies.
        /// </summary>
        public static void RegisterWorldData(WorldData data)
        {
            CurrentWorldData = data;

            consumablesBySpriteId.Clear();
            worldItemSpriteIds.Clear();

            if (data == null)
            {
                Debug.LogWarning("[WorldItemsRegistry] RegisterWorldData called with null data");
                return;
            }

            int inputConsumablesCount =
                data.consumableItems != null ? data.consumableItems.Count : 0;

            if (data.consumableItems != null)
            {
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

            int registeredConsumablesCount = consumablesBySpriteId.Count;

            Debug.Log(
                $"[WorldItemsRegistry] Registered {registeredConsumablesCount} world consumable items "
                    + $"for world '{data.worldName}' (input list count = {inputConsumablesCount})."
            );

            // Validate that enemy loot references only consumables that exist in this world.
            if (data.enemies != null && data.enemies.Count > 0)
            {
                int missingLootEntries = 0;

                for (int i = 0; i < data.enemies.Count; i++)
                {
                    var enemy = data.enemies[i];
                    if (enemy == null)
                    {
                        continue;
                    }

                    if (enemy.lootItems == null || enemy.lootItems.Count == 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < enemy.lootItems.Count; j++)
                    {
                        var loot = enemy.lootItems[j];
                        if (loot == null)
                        {
                            continue;
                        }

                        // if (string.IsNullOrEmpty(loot.spriteId))
                        // {
                        //     continue;
                        // }

                        // if (!consumablesBySpriteId.ContainsKey(loot.spriteId))
                        // {
                        //     missingLootEntries++;
                        //     Debug.LogWarning(
                        //         "[WorldItemsRegistry] Enemy loot item references missing consumable: "
                        //             + $"enemy='{enemy.name}', itemName='{loot.id}', spriteId='{loot.spriteId}'. "
                        //             + "This spriteId is not present in WorldData.consumableItems."
                        //     );
                        // }
                    }
                }

                if (missingLootEntries == 0)
                {
                    Debug.Log(
                        "[WorldItemsRegistry] All enemy loot spriteIds have matching consumables in this world."
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"[WorldItemsRegistry] Found {missingLootEntries} enemy loot entries "
                            + "without matching consumables. See warnings above for details."
                    );
                }
            }
            else
            {
                Debug.Log("[WorldItemsRegistry] No enemies defined in world data.");
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
        public static ConsumableItemData GetConsumableBySpriteId(string spriteId)
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
