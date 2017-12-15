using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using DataService.Models;
using DataService.Common;

namespace DataService
{
    public class DataService : IDataService
    {
        private static readonly object _lock = new object();

        public IEnumerable<Statistic> GetTop(int n)
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

        public void AddResult(Statistic stat)
        {
            lock (_lock)
            {
                var db = new ServiceDbContext();
                stat.Id = db.Statistics.Select(s => s.Id).Max() + 1;
                stat.Name = stat.Name.ToLower();
                db.Statistics.RemoveBy(s => s.Name.ToLower() == stat.Name.ToLower() && s.CodeLength == stat.CodeLength && s.Tries == stat.Tries);
                db.SaveChanges();
                db.Statistics.AddOrUpdate(s => new { s.Name, s.CodeLength, s.Tries }, stat);
                db.SaveChanges();
            }
        }
    }
}
