using System.Collections.Generic;
using SeriousMastermind.Common.UtilityClasses;

namespace SeriousMastermind.Models
{
    public class DbStories : CustomList<Story>
    {
        public DbStories(List<Story> stories, bool isReadOnly = false) : base(isReadOnly)
        {
            _customList = stories;
        }
    }
}