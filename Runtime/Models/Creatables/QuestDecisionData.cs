namespace FTRShared.Runtime.Models
{
    public struct QuestPromptData
    {
        public QuestData Quest;
        public uint TargetNetId;

        public QuestPromptData(QuestData quest, uint targetNetId)
        {
            Quest = quest;
            TargetNetId = targetNetId;
        }
    }

    public class QuestDecisionData
    {
        public QuestData Quest;

        public bool IsAccepted;

        public uint TargetNetId;

        public QuestDecisionData(QuestData questData, bool isAccepted, uint targetNetId)
        {
            Quest = questData;
            IsAccepted = isAccepted;
            TargetNetId = targetNetId;
        }
    }
}
