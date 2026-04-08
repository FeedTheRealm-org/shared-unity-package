using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class QuestData
    {
        public string id = "";
        public string title = "";
        public string content = "";
        public int targetAmount = 0;
        public string targetId = "";
        public string targetInteractionId = "";
        public QuestType type;
        public List<QuestRewardData> rewards = new();

        public enum QuestRewardType
        {
            Gold,
            Item,
            LootTable,
        }

        [Serializable]
        public class QuestRewardData
        {
            public QuestRewardType rewardType;
            public int goldAmount = 0;
            public string itemId = "";
            public string lootTableId = "";

            public QuestRewardData() { }

            public QuestRewardData(
                QuestRewardType rewardType,
                int goldAmount,
                string itemId,
                string lootTableId
            )
            {
                this.rewardType = rewardType;
                this.goldAmount = goldAmount;
                this.itemId = itemId;
                this.lootTableId = lootTableId;
            }
        }

        public QuestData(
            string id,
            string title,
            string content,
            QuestType type,
            string targetId,
            int targetAmount,
            string targetInteractionId,
            List<QuestRewardData> rewards = null
        )
        {
            this.id = id;
            this.title = title;
            this.content = content;
            this.type = type;
            this.targetId = targetId;
            this.targetAmount = targetAmount;
            this.targetInteractionId = targetInteractionId;
            this.rewards = rewards ?? new List<QuestRewardData>();
        }
    }
}
