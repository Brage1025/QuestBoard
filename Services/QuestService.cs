
using System.Text.Json;
using QuestBoard.Models;

namespace QuestBoard.Services;

public class QuestService
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "QuestBoard",
        "quests.json");

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    private List<Quest> _quests = [];
    private int _nextId = 1;

    public QuestService()
    {
        LoadQuests();
    }

    public List<Quest> GetQuests()
    {
        return _quests;
    }

    public void AddQuest(Quest quest)
    {
        quest.Id = _nextId++;
        _quests.Add(quest);
        SaveQuests();
    }

    public void UpdateQuest(Quest updatedQuest)
    {
        var existingQuest = _quests.FirstOrDefault(
            q => q.Id == updatedQuest.Id);

        if (existingQuest is null)
        {
            return;
        }

        existingQuest.Name = updatedQuest.Name;
        existingQuest.Description = updatedQuest.Description;
        existingQuest.Difficulty = updatedQuest.Difficulty;
        existingQuest.Reward = updatedQuest.Reward;

        SaveQuests();
    }

    public void UpdateStatus(int questId, QuestStatus status)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        if (quest is not null)
        {
            quest.Status = status;
            SaveQuests();
        }
    }

    public void AddObjective(int questId, string description)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        if (quest is null || string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        var nextObjectiveId = quest.Objectives.Count == 0
            ? 1
            : quest.Objectives.Max(o => o.Id) + 1;

        quest.Objectives.Add(new QuestObjective
        {
            Id = nextObjectiveId,
            Description = description.Trim()
        });

        SaveQuests();
    }

    public void ToggleObjective(int questId, int objectiveId)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        var objective = quest?.Objectives.FirstOrDefault(
            o => o.Id == objectiveId);

        if (objective is not null)
        {
            objective.IsCompleted = !objective.IsCompleted;
            SaveQuests();

            // Completing every objective completes the quest.
            if (quest!.Objectives.Count > 0 &&
                quest.Objectives.All(o => o.IsCompleted))
            {
                quest.Status = QuestStatus.Completed;
                SaveQuests();
            }
            else if (quest.Status == QuestStatus.Completed)
            {
                quest.Status = QuestStatus.InProgress;
                SaveQuests();
            }
        }
    }

    public void DeleteObjective(int questId, int objectiveId)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        var objective = quest?.Objectives.FirstOrDefault(
            o => o.Id == objectiveId);

        if (objective is not null)
        {
            quest!.Objectives.Remove(objective);
            SaveQuests();
        }
    }

    public void DeleteQuest(int questId)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        if (quest is not null)
        {
            _quests.Remove(quest);
            SaveQuests();
        }
    }

    private void LoadQuests()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);

            _quests = JsonSerializer.Deserialize<List<Quest>>(json)
                      ?? [];

            // Protect against older save files with no objectives.
            foreach (var quest in _quests)
            {
                quest.Objectives ??= [];
            }

            _nextId = _quests.Count == 0
                ? 1
                : _quests.Max(q => q.Id) + 1;

            return;
        }

        _quests =
        [
            new()
            {
                Id = 1,
                Name = "Goblin Problem",
                Description = "A band of goblins has been raiding farms outside the village.",
                Difficulty = QuestDifficulty.Easy,
                Status = QuestStatus.Available,
                Reward = "100 gp",
                Objectives =
                [
                    new() { Id = 1, Description = "Find the goblin camp" },
                    new() { Id = 2, Description = "Defeat the goblin leader" },
                    new() { Id = 3, Description = "Protect the nearby farms" }
                ]
            },
            new()
            {
                Id = 2,
                Name = "The Sunken Crypt",
                Description = "Something ancient has awakened beneath the old cemetery.",
                Difficulty = QuestDifficulty.Hard,
                Status = QuestStatus.InProgress,
                Reward = "750 XP + enchanted sword",
                Objectives =
                [
                    new() { Id = 1, Description = "Find the crypt entrance" },
                    new() { Id = 2, Description = "Defeat the undead guardian" },
                    new() { Id = 3, Description = "Recover the ancient relic" }
                ]
            },
            new()
            {
                Id = 3,
                Name = "Dragon's Hoard",
                Description = "Retrieve the lost crown from the dragon's mountain lair.",
                Difficulty = QuestDifficulty.Deadly,
                Status = QuestStatus.Available,
                Reward = "5,000 gp",
                Objectives =
                [
                    new() { Id = 1, Description = "Reach the mountain lair" },
                    new() { Id = 2, Description = "Overcome the dragon" },
                    new() { Id = 3, Description = "Retrieve the lost crown" }
                ]
            }
        ];

        _nextId = _quests.Max(q => q.Id) + 1;

        SaveQuests();
    }

    private void SaveQuests()
    {
        var directory = Path.GetDirectoryName(_filePath)!;

        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(_quests, _jsonOptions);

        File.WriteAllText(_filePath, json);
    }
}