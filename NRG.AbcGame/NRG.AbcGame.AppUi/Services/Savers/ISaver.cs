using NRG.AbcGame.AppUi.DataBase.Models;

namespace NRG.AbcGame.AppUi.Services.Savers;

public interface ISaver
{
    Task SaveAsync(GameRun run);
}
