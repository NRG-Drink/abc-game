using NRG.AbcGame.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NRG.AbcGame.Persistence;

public class FileSaver(string baseFolder)
{
    //private readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC-Game");

    public JsonSerializerOptions Options { get; init; } = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task Save(GameRun run)
    {
        EnsureFolderCreate();

        var fileName = $"{DateTime.Now:yyyy-MM-ddTHHmmss}_{run.Topic}";
        var path = Path.Combine(baseFolder, $"{fileName}.json");

        var fileContent = JsonSerializer.Serialize(run, Options);

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
