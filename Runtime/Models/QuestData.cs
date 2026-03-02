using System;
using Enums;
using UnityEngine;

namespace Models
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

        // TODO: add a type enum or a condition abstract class, and reward system

        public QuestData(
            string id,
            string title,
            string content,
            QuestType type,
            string targetId,
            int targetAmount,
            string targetInteractionId
        )
        {
            this.id = id;
            this.title = title;
            this.content = content;
            this.type = type;
            this.targetId = targetId;
            this.targetAmount = targetAmount;
            this.targetInteractionId = targetInteractionId;
        }
    }
}
