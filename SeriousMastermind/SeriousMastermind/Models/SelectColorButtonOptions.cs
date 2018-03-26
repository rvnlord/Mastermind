using Android.Widget;

namespace SeriousMastermind.Models
{
    public class SelectColorButtonOptions
    {
        public ImageView Button { get; set; }
        public ColorsAvailable Color { get; set; }
        public bool IsSelected { get; set; }
        public int ImageId { get; set; }
    }
}