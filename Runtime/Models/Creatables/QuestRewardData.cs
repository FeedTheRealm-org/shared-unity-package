using System;

namespace FTRShared.Runtime.Models
{
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
}
