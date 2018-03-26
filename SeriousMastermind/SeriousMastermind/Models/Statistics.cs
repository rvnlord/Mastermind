using System.Collections.Generic;
using System.Linq;
using SeriousMastermind.Common;

namespace SeriousMastermind.Models
{
    public class Statistics
    {
        private static readonly object _lock = new object();
        
        public static IEnumerable<Statistic> GetTop(int n)
        {
            lock (_lock)
            {
                var statistics = new ServiceDbContext().Statistics
                    .OrderByDescending(s => s.CodeLength)
                    .ThenBy(s => s.Tries)
                    .Take(n).ToList();
                statistics.ForEach(s => s.Name = $"{s.Name.Take(1).ToUpper()}{s.Name.Skip(1).ToLower()}");
                return statistics;
            }
        }

        public static void AddResult(Statistic stat)
        {
            lock (_lock)
            {
                var db = new ServiceDbContext();
                var dbStatistics = db.Statistics;
                stat.Id = dbStatistics.Any() ? db.Statistics.Select(s => s.Id).Max() + 1 : 0;
                stat.Name = stat.Name.ToLower();
                db.Statistics.AddOrUpdate(s => new { s.Name, s.CodeLength, s.Tries }, stat);
            }
        }
    }
}