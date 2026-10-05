namespace QuestBoard.Models;

public class Quest
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public QuestDifficulty Difficulty { get; set; }

    public QuestStatus Status { get; set; }

    public string Reward { get; set; } = string.Empty;
}

public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard,
    Deadly
}

public enum QuestStatus
{
    Available,
    InProgress,
    Completed
}