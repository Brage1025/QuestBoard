using QuestBoard.Models;

namespace QuestBoard.Services;

public class QuestService
{
    private readonly List<Quest> _quests =
    [
        new()
        {
            Id = 1,
            Name = "Goblin Problem",
            Description = "A band of goblins has been raiding farms outside the village.",
            Difficulty = QuestDifficulty.Easy,
            Status = QuestStatus.Available,
            Reward = "100 gp"
        },

        new()
        {
            Id = 2,
            Name = "The Sunken Crypt",
            Description = "Something ancient has awakened beneath the old cemetery.",
            Difficulty = QuestDifficulty.Hard,
            Status = QuestStatus.InProgress,
            Reward = "750 XP + enchanted sword"
        },

        new()
        {
            Id = 3,
            Name = "Dragon's Hoard",
            Description = "Retrieve the lost crown from the dragon's mountain lair.",
            Difficulty = QuestDifficulty.Deadly,
            Status = QuestStatus.Available,
            Reward = "5,000 gp"
        }
    ];

    public List<Quest> GetQuests()
    {
        return _quests;
    }
    public void AddQuest(Quest quest)
    {
        quest.Id = _quests.Count == 0
            ? 1
            : _quests.Max(q => q.Id) + 1;

        _quests.Add(quest);
    }
}

