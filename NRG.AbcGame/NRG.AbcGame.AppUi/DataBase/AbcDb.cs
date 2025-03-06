using Microsoft.EntityFrameworkCore;
using NRG.AbcGame.AppUi.DataBase.Models;

namespace NRG.AbcGame.AppUi.DataBase;

public class AbcDb(DbContextOptions options) : DbContext(options)
{
    public DbSet<SaveRun> SaveRuns { get; init; } = null!;
    public DbSet<SaveInput> SaveInputs { get; init; } = null!;
    public DbSet<SaveRunInput> SaveRunInputs { get; init; } = null!;
}
