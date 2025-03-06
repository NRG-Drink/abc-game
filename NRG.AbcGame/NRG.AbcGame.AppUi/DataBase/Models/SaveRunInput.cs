using System.ComponentModel.DataAnnotations;

namespace NRG.AbcGame.AppUi.DataBase.Models;

public record SaveRunInput
{
    [Key]
    public int Id { get; init; }
    public required DateTime TimeUTC { get; init; }

    // EF dependencies
    public required SaveRun Run { get; init; }
    public required SaveInput Input { get; init; }
}