using NRG.AbcGame.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NRG.AbcGame.Persistence;

public class FileManager(string baseFolder)
{
    private const string SettingsFileName = "abc-game-settings.json";
    private readonly string _settingsFilePath = Path.Combine(baseFolder, SettingsFileName);

    public JsonSerializerOptions JsonOptions { get; init; } = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task<GameSettings> LoadSettingsOrDefaultAsync()
    {
        EnsureFolderCreate();
        if (!File.Exists(_settingsFilePath))
        {
            return new();
        }

        var fileContent = await File.ReadAllTextAsync(_settingsFilePath);
        var settings = JsonSerializer.Deserialize<GameSettings>(fileContent);

        return settings ?? new();
    }

    public async Task SaveSettings(GameSettings settings)
    {
        var fileContent = JsonSerializer.Serialize(settings, JsonOptions);
        await File.WriteAllTextAsync(_settingsFilePath, fileContent);
    }

    public async Task SaveAsync(GameRun run)
    {
        EnsureFolderCreate();

        var fileName = $"{DateTime.Now:yyyy-MM-ddTHHmmss}_{run.Topic}{(run.IsCancelled ? "_cancelled" : string.Empty)}";
        var path = Path.Combine(baseFolder, $"{fileName}.json");

        var fileContent = JsonSerializer.Serialize(run, JsonOptions);

        await File.WriteAllTextAsync(path, fileContent);
    }

    private void EnsureFolderCreate()
    {
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
    }
}
