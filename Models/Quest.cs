using System.ComponentModel.DataAnnotations;

namespace QuestBoard.Models;

public class Quest
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public QuestDifficulty Difficulty { get; set; }

    public QuestStatus Status { get; set; }

    [Required]
    [StringLength(100)]
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