using SQLite;

namespace SeriousMastermind.Models
{
    [Table("tblStatistics")]
    public class Statistic
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CodeLength { get; set; }
        public int Tries { get; set; }
    }
}
