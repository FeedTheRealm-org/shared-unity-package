using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class NPCDialogData
    {
        public string dialogId;

        // Map message id to quest id using lists
        public List<string> messageIds = new List<string>();

        public List<string> questIds = new List<string>();

        public NPCDialogData(string dialogId)
        {
            this.dialogId = dialogId;
            this.messageIds = new List<string>();
            this.questIds = new List<string>();
        }

        public Dictionary<string, string> GetMessageQuestMap()
        {
            var map = new Dictionary<string, string>();
            for (int i = 0; i < messageIds.Count && i < questIds.Count; i++)
            {
                map[messageIds[i]] = questIds[i];
            }
            return map;
        }

        public void SetMessageQuestMap(Dictionary<string, string> map)
        {
            messageIds.Clear();
            questIds.Clear();
            foreach (var kvp in map)
            {
                messageIds.Add(kvp.Key);
                questIds.Add(kvp.Value);
            }
        }
    }
}
