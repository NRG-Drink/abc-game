using NRG.AbcGame.AppUi.DataBase;
using NRG.AbcGame.AppUi.DataBase.Models;

namespace NRG.AbcGame.AppUi.Services.Savers;

public class DbSaver(AbcDb db) : ISaver
{
    public async Task SaveAsync(GameRun gameRun)
    {
        var saveRun = new SaveRun()
        {
            StartUTC = gameRun.StartUTC,
            Time = gameRun.Time,
            Topic = gameRun.Topic
        };

        var saveInputs = gameRun.Values
            .SelectMany(kv => kv.Value
                //.ExceptBy(
                //    db.SaveInputs.Select(e => e.Value),
                //    e => e.Value
                //)
                .Select(e => new SaveRunInput()
                {
                    Run = saveRun,
                    TimeUTC = e.TimeUTC,
                    Input = new SaveInput()
                    {
                        Letter = kv.Key,
                        Value = e.Value
                    }
                })
            );

        var agg = saveInputs.Aggregate(new List<SaveRunInput>(), (acc, e) =>
        {
            var found = db.SaveInputs.FirstOrDefault(x => x.Value == e.Input.Value);
            if (found is null)
            {
                acc.Add(e);
            } 
            else
            {
                acc.Add(e with { Input = found });
            }

            return acc;
        });

        await db.AddRangeAsync(agg);
        await db.SaveChangesAsync();
    }
}
