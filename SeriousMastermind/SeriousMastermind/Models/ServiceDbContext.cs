using System.Linq;
using SeriousMastermind.Common.UtilityClasses;
using SQLite;

namespace SeriousMastermind.Models
{
    public class ServiceDbContext
    {
        private static bool _isInitialized;

        public string ConnectionString { get; }
        public SQLiteConnection Connect() => new SQLiteConnection(ConnectionString);

        public DbStatistics Statistics => new DbStatistics(Connect().Table<Statistic>().ToList());
        public DbStories Stories => new DbStories(Connect().Table<Story>().ToList());

        public ServiceDbContext()
        {
            var path = FileAccessHelper.GetLocalFilePath("database.sqlite");
            ConnectionString = path;
            CreateModel();
        }

        private static void CreateModel()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
            }
        }
    }
}
