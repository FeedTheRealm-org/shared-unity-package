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
