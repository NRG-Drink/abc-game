using System.ComponentModel.DataAnnotations;

namespace NRG.AbcGame.AppUi.DataBase.Models;

public record SaveInput
{
    [Key]
    public int Id { get; init; }
    public required char Letter { get; init; }
    public required string Value { get; init; }

    // EF dependencies
    public ICollection<SaveRunInput> RunInputs { get; init; } = [];
}
