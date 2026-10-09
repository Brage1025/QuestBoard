
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

    public void UpdateStatus(int questId, QuestStatus status)
    {
        var quest = _quests.FirstOrDefault(q => q.Id == questId);

        if (quest is not null)
        {
            quest.Status = status;
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

            _nextId = _quests.Count == 0
                ? 1
                : _quests.Max(q => q.Id) + 1;

            return;
        }

        // First launch: create the starter quests.
        _quests =
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