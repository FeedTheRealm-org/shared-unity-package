using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class MessageQuestAssignment
    {
        public string messageId;
        public string questId;

        public MessageQuestAssignment() { }

        public MessageQuestAssignment(string messageId, string questId)
        {
            this.messageId = messageId;
            this.questId = questId;
        }
    }

    [Serializable]
    public class NPCDialogData
    {
        public string dialogId;

        public string onQuestAcceptedDialogId = "";
        public string repeatableQuestCooldown = "";

        public List<MessageQuestAssignment> questAssignments = new List<MessageQuestAssignment>();

        public NPCDialogData() { }

        public NPCDialogData(string dialogId)
        {
            this.dialogId = dialogId;
            this.questAssignments = new List<MessageQuestAssignment>();
        }

        public bool HasQuestAssigned => questAssignments != null && questAssignments.Count > 0;

        public bool IsRepeatable => !string.IsNullOrEmpty(repeatableQuestCooldown);

        public Dictionary<string, string> GetMessageQuestMap()
        {
            var map = new Dictionary<string, string>();
            foreach (var a in questAssignments)
                map[a.messageId] = a.questId;
            return map;
        }

        public void SetMessageQuestMap(Dictionary<string, string> map)
        {
            questAssignments.Clear();
            foreach (var kvp in map)
                questAssignments.Add(new MessageQuestAssignment(kvp.Key, kvp.Value));
        }
    }
}
