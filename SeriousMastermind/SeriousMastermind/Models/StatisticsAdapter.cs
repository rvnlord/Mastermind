using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;

namespace SeriousMastermind.Models
{
    public class StatisticsAdapter : BaseAdapter<Statistic>
    {
        public TextView PlaceView;
        public TextView NameView;
        public TextView CodeLengthView;
        public TextView TriesView;

        public List<Statistic> Statistics { get; set; }

        public StatisticsAdapter(List<Statistic> statistics)
        {
            Statistics = statistics;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if (view == null)
            {
                var inflater = LayoutInflater.From(parent.Context);
                view = inflater.Inflate(Resource.Layout.StatisticsListViewItem, parent, false);
            }

            PlaceView = (TextView)view.FindViewById(Resource.Id.lblPlaceView);
            NameView = (TextView)view.FindViewById(Resource.Id.lblNameView);
            CodeLengthView = (TextView)view.FindViewById(Resource.Id.lblCodeLengthView);
            TriesView = (TextView)view.FindViewById(Resource.Id.lblTriesView);

            var statistic = Statistics[position];
            var place = Statistics.OrderByDescending(s => s.CodeLength)
                .ThenBy(s => s.Tries).Select((s, i) => new { s.CodeLength, s.Tries }).Distinct()
                .Select((a, i) => new { Place = i, a.CodeLength, a.Tries })
                .Single(a => a.CodeLength == statistic.CodeLength && a.Tries == statistic.Tries).Place + 1;
            PlaceView.Text = place.ToString();
            NameView.Text = statistic.Name;
            CodeLengthView.Text = $"{statistic.CodeLength}r";
            TriesView.Text = $"{statistic.Tries}p";
            
            return view;
        }

        public override int Count => Statistics.Count;

        public override Statistic this[int position] => Statistics[position];

        public override long GetItemId(int position)
        {
            return Statistics[position].Id;
        }
    }
}