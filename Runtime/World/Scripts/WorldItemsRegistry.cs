using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Worlds
{
    /// <summary>
    /// Static registry that exposes the current world's items (consumables, weapons, etc.)
    /// and enemies to gameplay systems (loot, inventory, tooltips).
    /// Uses item ID as the canonical identifier for world items.
    /// </summary>
    public static class WorldItemsRegistry
    {
        /// <summary>
        /// Last world data registered by a loader (client or server).
        /// </summary>
        public static WorldData CurrentWorldData { get; private set; }

        // All items by id, regardless of concrete type.
        private static readonly Dictionary<string, ItemData> itemsById =
            new Dictionary<string, ItemData>();

        // Typed views for specific item categories when needed.
        private static readonly Dictionary<string, ConsumableItemData> consumablesById =
            new Dictionary<string, ConsumableItemData>();

        private static readonly Dictionary<string, WeaponItemData> weaponsById =
            new Dictionary<string, WeaponItemData>();

        // All world item IDs, regardless of concrete type
        private static readonly HashSet<string> worldItemIds = new HashSet<string>();

        /// <summary>
        /// Register world data so that other systems can query items/enemies.
        /// </summary>
        public static void RegisterWorldData(WorldData data)
        {
            CurrentWorldData = data;

            itemsById.Clear();
            consumablesById.Clear();
            weaponsById.Clear();
            worldItemIds.Clear();

            if (data == null)
            {
                Debug.LogWarning("[WorldItemsRegistry] RegisterWorldData called with null data");
                return;
            }

            int inputConsumablesCount =
                data.consumableItems != null ? data.consumableItems.Count : 0;
            int inputWeaponsCount = data.weaponItems != null ? data.weaponItems.Count : 0;

            if (data.consumableItems != null)
            {
                foreach (var consumable in data.consumableItems)
                {
                    if (consumable == null)
                    {
                        continue;
                    }

                    if (
                        !string.IsNullOrEmpty(consumable.id)
                        && (
                            string.IsNullOrEmpty(consumable.spriteFilepath)
                            || !string.IsNullOrEmpty(consumable.spriteFilepath)
                        )
                    )
                    {
                        RegisterItem(consumable, consumablesById);
                    }
                }
            }

            int registeredConsumablesCount = consumablesById.Count;

            if (data.weaponItems != null)
            {
                foreach (var weapon in data.weaponItems)
                {
                    if (weapon == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(weapon.id))
                    {
                        RegisterItem(weapon, weaponsById);
                    }
                }
            }

            int registeredWeaponsCount = weaponsById.Count;

            Debug.Log(
                $"[WorldItemsRegistry] Registered {registeredConsumablesCount} consumable items and {registeredWeaponsCount} weapon items "
                    + $"for world '{data.worldName}' (input consumables = {inputConsumablesCount}, input weapons = {inputWeaponsCount})."
            );

            // Validate that enemy loot references only items that exist in this world.
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

                    if (enemy.lootTable == null || enemy.lootTable.lootItems == null)
                    {
                        continue;
                    }

                    foreach (var lootItem in enemy.lootTable.lootItems)
                    {
                        if (lootItem == null || string.IsNullOrEmpty(lootItem.id))
                        {
                            continue;
                        }
                        if (!worldItemIds.Contains(lootItem.id))
                        {
                            Debug.LogWarning(
                                $"[WorldItemsRegistry] Enemy '{enemy.name}' loot item id '{lootItem.id}' not found in world items."
                            );
                            missingLootEntries++;
                        }
                    }
                }

                if (missingLootEntries == 0)
                {
                    Debug.Log(
                        "[WorldItemsRegistry] All enemy loot item IDs have matching items in this world."
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"[WorldItemsRegistry] Found {missingLootEntries} enemy loot entries "
                            + "without matching items. See warnings above for details."
                    );
                }
            }
            else
            {
                Debug.Log("[WorldItemsRegistry] No enemies defined in world data.");
            }
        }

        /// <summary>
        /// Returns true if the given ID matches any registered world item id.
        /// </summary>
        public static bool IsWorldItem(string id)
        {
            return !string.IsNullOrEmpty(id) && itemsById.ContainsKey(id);
        }

        /// <summary>
        /// Get consumable definition by its spriteId. Returns null if not found.
        /// </summary>
        public static ConsumableItemData GetConsumableBySpriteId(string spriteId)
        {
            // Deprecated: Use GetConsumableById instead.
            return GetConsumableById(spriteId);
        }

        /// <summary>
        /// Get consumable definition by its unique item id. Returns null if not found.
        /// </summary>
        public static ConsumableItemData GetConsumableById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            itemsById.TryGetValue(id, out var item);
            return item as ConsumableItemData;
        }

        /// <summary>
        /// Get weapon definition by its unique item id. Returns null if not found.
        /// </summary>
        public static WeaponItemData GetWeaponById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            itemsById.TryGetValue(id, out var item);
            return item as WeaponItemData;
        }

        /// <summary>
        /// Get any item (consumable, weapon, etc.) by its unique item id.
        /// Returns null if no matching item is registered.
        /// </summary>
        public static ItemData GetItemById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            itemsById.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Internal helper to register an item into both the polymorphic map
        /// and a typed dictionary, keeping worldItemIds in sync.
        /// </summary>
        private static void RegisterItem<T>(T item, Dictionary<string, T> typedDictionary)
            where T : ItemData
        {
            if (item == null || string.IsNullOrEmpty(item.id))
            {
                return;
            }

            typedDictionary[item.id] = item;
            itemsById[item.id] = item;
            worldItemIds.Add(item.id);
        }
    }
}
