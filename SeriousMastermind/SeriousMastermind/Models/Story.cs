using SQLite;

namespace SeriousMastermind.Models
{
    [Table("tblStories")]
    public class Story
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Content { get; set; }
    }
}