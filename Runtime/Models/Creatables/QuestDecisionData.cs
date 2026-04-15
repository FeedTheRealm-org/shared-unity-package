namespace FTRShared.Runtime.Models
{
    public struct QuestPromptData
    {
        public QuestData Quest;
        public uint TargetNetId;
        public string NpcId;

        public QuestPromptData(QuestData quest, uint targetNetId, string npcId = "")
        {
            Quest = quest;
            TargetNetId = targetNetId;
            NpcId = npcId;
        }
    }

    public class QuestDecisionData
    {
        public QuestData Quest;
        public bool IsAccepted;
        public uint TargetNetId;
        public string NpcId;

        public QuestDecisionData(
            QuestData questData,
            bool isAccepted,
            uint targetNetId,
            string npcId = ""
        )
        {
            Quest = questData;
            IsAccepted = isAccepted;
            TargetNetId = targetNetId;
            NpcId = npcId;
        }
    }
}
