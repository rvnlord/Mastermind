using System;
using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using SeriousMastermind.Common;
using SeriousMastermind.Models;
using MoreLinq;
using static SeriousMastermind.MainActivity;

namespace SeriousMastermind
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class StoryActivity : Activity
    {
        private TextView _lblStoryDescription;
        private TextView _lblStoryContent;
        private Button _btnBackToMain;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Story);

            _lblStoryDescription = FindViewById<TextView>(Resource.Id.lblStoryDescription);
            _lblStoryContent = FindViewById<TextView>(Resource.Id.lblStoryCOntent);
            _btnBackToMain = FindViewById<Button>(Resource.Id.btnBackToMain);

            _btnBackToMain.Click += btnBackToMain_Click;

            var db = new ServiceDbContext();
            var player = Intent.GetStringExtra("PlayerName") ?? "Unspecified";
            var statistics = db.Statistics.Where(s => s.Name.Eq(player)).ToList();
            var bestRowLength = statistics.MaxBy(s => s.CodeLength).CodeLength;
            var bestTriesNum = statistics.Where(s => s.CodeLength == bestRowLength).MinBy(s => s.Tries).Tries;

            var triesV = Enumerable.Range(1, MaxTries).ToArray();
            var clV = Enumerable.Range(1, MaxCodeLength).ToArray();
            var allV = triesV.Cartesian(clV, (tr, cl) => new Statistic { CodeLength = cl, Tries = tr })
                .OrderByDescending(s => s.CodeLength).ThenBy(s => s.Tries).ToArray();
            var idx = Array.IndexOf(allV, allV.Single(v => v.CodeLength == bestRowLength && v.Tries == bestTriesNum));
            var allL = allV.Length;
            var unlockedN = allL - idx;
            
            _lblStoryDescription.Text = $"Story (unlocked {unlockedN} of {allL})";
            var indent = Enumerable.Repeat(" ", 8).JoinAsString();
            _lblStoryContent.Text = indent + 
                db.Stories.ToList().First().Content.Split(" ").Split(allL).Take(unlockedN).SelectMany(w => w).JoinAsString(" ").Replace("\\n", "\n\n" + indent) +
                (unlockedN == allL ? "" : "...");
        }

        private void btnBackToMain_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}