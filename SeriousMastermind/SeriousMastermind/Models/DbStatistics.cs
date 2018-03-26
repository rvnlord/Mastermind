using System;
using System.Collections.Generic;
using System.Linq;
using SeriousMastermind.Common.UtilityClasses;
using SeriousMastermind.Common;
using MoreLinq;

namespace SeriousMastermind.Models
{
    public class DbStatistics : CustomList<Statistic>
    {
        public DbStatistics(List<Statistic> statistics, bool isReadOnly = false) : base(isReadOnly)
        {
            _customList = statistics;
        }

        private static readonly object _lock = new object();

        public override bool Remove(Statistic item)
        {
            lock (_lock)
            {
                var db = new ServiceDbContext();
                db.Connect().Delete(item);
                return base.Remove(item);
            }
        }

        public override void Add(Statistic item)
        {
            lock (_lock)
            {
                var db = new ServiceDbContext();
                db.Connect().Insert(item);
                base.Add(item);
            }
        }

        public void AddOrUpdate(Func<Statistic, object> p, Statistic item)
        {
            var db = new ServiceDbContext();
            var statistics = db.Statistics;

            var existingStats = statistics.Where(s => s.HasSamePropertiesWithSameValues(p, item)).ToArray();
            if (existingStats.Length > 0)
            {
                var lowestId = existingStats.MinBy(x => x.Id).Id;
                foreach (var existingStat in existingStats)
                    Remove(existingStat);
                item.Id = lowestId;
            }
            Add(item);
        }

    }
}