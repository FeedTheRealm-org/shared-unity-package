using System;
using UnityEngine;

namespace Models
{
    /// <summary>
    /// Represents a single loot entry in a loot table, including
    /// the consumable item data and its individual drop probability.
    ///
    /// Field names mirror ConsumableItemData so existing JSON lootItems
    /// (without dropProbability) still deserialize correctly; new data
    /// simply adds the per-item probability.
    /// </summary>
    [Serializable]
    public class LootEntryData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Consumable Item";

        [SerializeField]
        public string description = "";

        [SerializeField]
        public EffectType effectType = EffectType.None;

        [SerializeField]
        public int value = 0;

        [SerializeField]
        public float duration = 0f;

        [SerializeField]
        public float cooldown = 0f;

        [SerializeField]
        public int maxStack = 0;

        [SerializeField]
        public string spriteId = "None";

        /// <summary>
        /// Per-item drop probability (0-100).
        /// </summary>
        [SerializeField]
        public int dropProbability = 0;

        public LootEntryData() { }

        public LootEntryData(
            string id,
            string name,
            string description,
            EffectType effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack,
            string spriteId,
            int dropProbability
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.effectType = effectType;
            this.value = value;
            this.duration = duration;
            this.cooldown = cooldown;
            this.maxStack = maxStack;
            this.spriteId = spriteId;
            this.dropProbability = dropProbability;
        }
    }
}
