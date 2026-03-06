using System;
using Enums;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class QuestData
    {
        [SerializeField]
        public string Id;

        [SerializeField]
        public string Title;

        [SerializeField]
        public string Content;

        [SerializeField]
        public int TargetAmount;

        [SerializeField]
        public string TargetId;

        [SerializeField]
        public string TargetInteractionId;

        [SerializeField]
        public QuestType Type;

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
            this.Id = id;
            this.Title = title;
            this.Content = content;
            this.Type = type;
            this.TargetId = targetId;
            this.TargetAmount = targetAmount;
            this.TargetInteractionId = targetInteractionId;
        }
    }
}
