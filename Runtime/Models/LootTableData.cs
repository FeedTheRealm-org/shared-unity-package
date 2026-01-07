using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class LootTableData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Loot Table";

        [SerializeField]
        public int minGoldDropAmount = 0;

        [SerializeField]
        public int maxGoldDropAmount = 0;

        [SerializeField]
        public List<LootEntryData> lootItems = new();

        public LootTableData(
            string id,
            string name,
            int minGoldDropAmount,
            int maxGoldDropAmount,
            List<LootEntryData> lootItems
        )
        {
            this.id = id;
            this.name = name;
            this.minGoldDropAmount = minGoldDropAmount;
            this.maxGoldDropAmount = maxGoldDropAmount;
            this.lootItems = lootItems ?? new List<LootEntryData>();
        }

        /// <summary>
        /// Represents a single loot entry in a loot table.
        /// Stores only the referenced consumable item id and its
        /// individual drop probability (0-100).
        ///
        /// The id field refers to a ConsumableItemData.
        /// </summary>
        [Serializable]
        public class LootEntryData
        {
            /// <summary>
            /// Id of the consumable item this entry refers to.
            /// </summary>
            [SerializeField]
            public string id;

            /// <summary>
            /// Per-item drop probability (0-100).
            /// </summary>
            [SerializeField]
            public int dropProbability = 0;

            public LootEntryData() { }

            public LootEntryData(string id, int dropProbability)
            {
                this.id = id;
                this.dropProbability = dropProbability;
            }
        }
    }
}
