using System.ComponentModel.DataAnnotations;

namespace NRG.AbcGame.AppUi.DataBase.Models;

public record SaveRun
{
    [Key]
    public int Id { get; init; }
    public required string Topic { get; init; }
    public required DateTime StartUTC { get; init; }
    public required TimeSpan Time { get; init; }

    // EF dependencies
    public ICollection<SaveRunInput> RunInputs { get; init; } = [];
}
