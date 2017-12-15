using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using AndroidDemo.Models;
using AndroidDemo.ServiceReferences;
using DataService.Models;

namespace AndroidDemo
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : Activity
    {
        private static readonly Random Rng = new Random();
        private static DataServiceClientWrapper _client;
        private Button _btnNewGame;
        private Button _btnCheck;
        private Button _btnStatistics;
        private EditText _txtCodeLength;
        private EditText _txtTriesNumber;
        private EditText _txtName;
        private ListView _lvStatistics;
        private LinearLayout _llContent;
        private Dictionary<ColorsAvailable, SelectColorButtonOptions> _selectColors;
        private List<ColorButtonOptions> _currentRowColors;
        private List<ColorButtonOptions> _currentRowColorAnswers;
        private LinearLayout _llColorSelectionContainer;
        private int _tries;
        private int _codeLength;
        private TextView _lblTriesLeft;
        private ColorsAvailable[] _code;

        private const int MaxTries = 10;
        private const int MaxCodeLength = 6;

        public int Tries
        {
            get => _tries;
            set => _tries = value > MaxTries || value < 0 ? -1 : value;
        }

        public int CodeLength
        {
            get => _codeLength;
            set => _codeLength = value > MaxCodeLength || value < 0 ? -1 : value;
        }

        public GameState GameState { get; set; } = GameState.None;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _btnNewGame = FindViewById<Button>(Resource.Id.btnNewGame);
            _btnCheck = FindViewById<Button>(Resource.Id.btnChack);
            _btnStatistics = FindViewById<Button>(Resource.Id.btnStatistics);
            _llColorSelectionContainer = FindViewById<LinearLayout>(Resource.Id.llColorSelectionContainer);
            _llColorSelectionContainer.Visibility = ViewStates.Gone;
            _selectColors = new Dictionary<ColorsAvailable, SelectColorButtonOptions>();
            _currentRowColors = new List<ColorButtonOptions>();
            _currentRowColorAnswers = new List<ColorButtonOptions>();
            _lblTriesLeft = FindViewById<TextView>(Resource.Id.lblTriesLeft);
            _lvStatistics = FindViewById<ListView>(Resource.Id.lvStatistics);

            const ColorsAvailable red = ColorsAvailable.Red;
            _selectColors.Add(red, new SelectColorButtonOptions
            {
                Color = red,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor1),
                ImageId = Resource.Drawable.color_1
            });
            const ColorsAvailable green = ColorsAvailable.Green;
            _selectColors.Add(green, new SelectColorButtonOptions
            {
                Color = green,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor2),
                ImageId = Resource.Drawable.color_2
            });
            const ColorsAvailable blue = ColorsAvailable.Blue;
            _selectColors.Add(blue, new SelectColorButtonOptions
            {
                Color = blue,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor3),
                ImageId = Resource.Drawable.color_3
            });
            const ColorsAvailable yellow = ColorsAvailable.Yellow;
            _selectColors.Add(yellow, new SelectColorButtonOptions
            {
                Color = yellow,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor4),
                ImageId = Resource.Drawable.color_4
            });
            const ColorsAvailable brown = ColorsAvailable.Brown;
            _selectColors.Add(brown, new SelectColorButtonOptions
            {
                Color = brown,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor5),
                ImageId = Resource.Drawable.color_5
            });
            const ColorsAvailable orange = ColorsAvailable.Orange;
            _selectColors.Add(orange, new SelectColorButtonOptions
            {
                Color = orange,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor6),
                ImageId = Resource.Drawable.color_6
            });
            const ColorsAvailable black = ColorsAvailable.Black;
            _selectColors.Add(black, new SelectColorButtonOptions
            {
                Color = black,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor7),
                ImageId = Resource.Drawable.color_7
            });
            const ColorsAvailable white = ColorsAvailable.White;
            _selectColors.Add(white, new SelectColorButtonOptions
            {
                Color = white,
                Button = FindViewById<ImageButton>(Resource.Id.imgbtnSelectColor8),
                ImageId = Resource.Drawable.color_8
            });

            _txtCodeLength = FindViewById<EditText>(Resource.Id.txtCodeLength);
            _txtTriesNumber = FindViewById<EditText>(Resource.Id.txtTriesNumber);
            _txtName = FindViewById<EditText>(Resource.Id.txtName);
            _llContent = FindViewById<LinearLayout>(Resource.Id.llContent);

            _btnNewGame.Click += btnNewGame_Click;
            _btnCheck.Click += btnCheck_Click;
            _btnStatistics.Click += btnStatistics_Click;
            _llColorSelectionContainer.Click += llColorSelectionContainer_Click;
            foreach (var b in _selectColors)
                b.Value.Button.Click += btnSelectColor_Click;
            _lvStatistics.Touch += lvStatistics_Touch;
        }

        private void lvStatistics_Touch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    _lvStatistics.Parent.RequestDisallowInterceptTouchEvent(true);
                    break;
                case MotionEventActions.Up:
                    _lvStatistics.Parent.RequestDisallowInterceptTouchEvent(false);
                    break;
            }

            _lvStatistics.OnTouchEvent(e.Event);
        }

        private void llColorSelectionContainer_Click(object sender, EventArgs e)
        {
            foreach (var b in _selectColors)
            {
                b.Value.IsSelected = false;
                b.Value.Button.Background = new ColorDrawable(Color.Transparent);
            }
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            RemoveInput();
            var clicked = _selectColors.Single(b => b.Value.Button == sender as ImageView);

            clicked.Value.IsSelected = !clicked.Value.IsSelected;
            foreach (var b in _selectColors.Where(bo => !Equals(bo, clicked)))
                b.Value.IsSelected = false;

            foreach (var b in _selectColors)
            {
                b.Value.Button.Background = new ColorDrawable(!b.Value.IsSelected
                    ? Color.Transparent
                    : Color.Argb(255, 0, 50, 150));
            }
        }

        private async void btnCheck_Click(object sender, EventArgs e)
        {
            RemoveInput();

            if (string.IsNullOrWhiteSpace(_txtName.Text) || _txtName.Text.Length < 3)
            {
                ShowMessageBox("Błąd", "Aby w przypadku wygranej znaleźć się na liście wyników należy uzupełnić swoje imię");
                return;
            }

            if (GameState == GameState.Won)
            {
                ShowMessageBox("Informacja", "Wygrana");
                return;
            }
            if (GameState == GameState.Lost)
            {
                ShowMessageBox("Informacja", "Przegrana");
                return;
            }
            if (GameState != GameState.Running || CodeLength == -1 || Tries == -1)
                return;

            _txtName.Enabled = false;
            _txtTriesNumber.Enabled = false;
            _txtCodeLength.Enabled = false;

            // Wyniki
            var correctAnswers = 0;
            for (var i = 0; i < _code.Length; i++)
            {
                var c = _code[i];
                var userSel = _currentRowColors[i];
                var answer = _currentRowColorAnswers[i];

                if (userSel.Color == c)
                {
                    answer.Color = ColorsAvailable.Black;
                    correctAnswers++;
                }
                else if (_code.Count(any => any == userSel.Color) > 0)
                    answer.Color = ColorsAvailable.White;
                else
                    answer.Color = ColorsAvailable.None;

                if (answer.Color != ColorsAvailable.None)
                    answer.Button.SetImageResource(_selectColors[answer.Color].ImageId);
            }

            if (correctAnswers == CodeLength)
                GameState = GameState.Won;
            else if (Tries == 0)
                GameState = GameState.Lost;

            AddGameRow();

            if (GameState == GameState.Won)
            {
                ShowMessageBox("Informacja", "Wygrana");

                _client = new DataServiceClientWrapper();

                _btnStatistics.Enabled = false;
                var btnStatisticsText = _btnStatistics.Text;
                _btnStatistics.Text = "Aktualizowanie Statystyk...";

                await Task.Run(() =>
                {
                    _client.AddResult(new Statistic
                    {
                        Name = _txtName.Text,
                        CodeLength = Convert.ToInt32(_txtCodeLength.Text),
                        Tries = Convert.ToInt32(_txtTriesNumber.Text) - Tries - 1
                    });
                    RunOnUiThread(() =>
                    {
                        if (_client.Error != null)
                            ShowMessageBox("Błąd", _client.Error.Message);
                    });
                    var stats = _client.GetTop(10)?.ToList();
                    RunOnUiThread(() =>
                    {
                        if (_client.Error != null)
                            ShowMessageBox("Błąd", _client.Error.Message);
                        else
                            _lvStatistics.Adapter = new StatisticsAdapter(stats);
                    });
                });

                _btnStatistics.Enabled = true;
                _btnStatistics.Text = btnStatisticsText;
                _txtName.Enabled = true;
                _txtTriesNumber.Enabled = true;
                _txtCodeLength.Enabled = true;
            }
            else if (GameState == GameState.Lost)
            {
                _txtName.Enabled = true;
                _txtTriesNumber.Enabled = true;
                _txtCodeLength.Enabled = true;
                ShowMessageBox("Informacja", "Przegrana");
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            RemoveInput();

            CodeLength = string.IsNullOrEmpty(_txtCodeLength.Text) ? -1 : int.Parse(_txtCodeLength.Text);
            Tries = string.IsNullOrEmpty(_txtTriesNumber.Text) ? -1 : int.Parse(_txtTriesNumber.Text);

            if (CodeLength <= 0 || Tries <= 0)
            {
                var message = "Nie można rozpocząć nowej Gry ponieważ nie wybrano poprawnej " +
                              $"{(CodeLength <= 0 ? "Długości Kodu (maks 6)" : "")}" +
                              $"{(CodeLength <= 0 && Tries <= 0 ? " i " : "")}" +
                              $"{(Tries <= 0 ? "Ilości Prób (maks 10)" : "")}";

                ShowMessageBox("Błąd", message);
                return;
            }

            GameState = GameState.Running;
            _code = Enumerable.Range(0, CodeLength).Select(v => (ColorsAvailable)Rng.Next(1, 8)).ToArray();
            _llContent.RemoveAllViews();
            _llColorSelectionContainer.Visibility = ViewStates.Visible;

            AddGameRow();
        }

        private async void btnStatistics_Click(object sender, EventArgs e)
        {
            RemoveInput();
            _client = new DataServiceClientWrapper();

            _btnStatistics.Enabled = false;
            var btnStatisticsText = _btnStatistics.Text;
            _btnStatistics.Text = "Ładowanie Statystyk...";
            
            await Task.Run(() =>
            {
                var stats = _client.GetTop(10)?.ToList();
                RunOnUiThread(() =>
                {
                    if (_client.Error != null)
                        ShowMessageBox("Błąd", _client.Error.Message);
                    else
                        _lvStatistics.Adapter = new StatisticsAdapter(stats);
                });
            });
            _btnStatistics.Enabled = true;
            _btnStatistics.Text = btnStatisticsText;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            var clicked = (ImageView)sender;
            var clickedOpt = _currentRowColors.SingleOrDefault(c => c.Button == clicked);
            if (clickedOpt == null)
                return;
            var selColorOpt = _selectColors.SingleOrDefault(c => c.Value.IsSelected);
            if (selColorOpt.Value == null)
                return;

            if (clickedOpt.Color != selColorOpt.Value.Color)
            {
                clicked.SetImageResource(selColorOpt.Value.ImageId);
                clickedOpt.Color = selColorOpt.Value.Color;
            }
            else
            {
                clicked.SetImageDrawable(new ColorDrawable(Color.Transparent));
                clickedOpt.Color = ColorsAvailable.None;
            }
        }

        private void AddGameRow()
        {
            // Nowy wiersz
            _currentRowColors.ForEach(c => c.Button.Click -= btnColor_Click);
            _currentRowColors.Clear();
            _currentRowColorAnswers.Clear();

            if (Tries > 0 && GameState == GameState.Running)
            {
                var llRow = new LinearLayout(this)
                {
                    Tag = "GameRow",
                    Orientation = Orientation.Horizontal,
                    LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    {
                        BottomMargin = ConvertDpToPixels(5.0f),
                        Gravity = GravityFlags.Center
                    }
                };

                var rows = new List<LinearLayout> { llRow };

                for (var i = 0; i < _llContent.ChildCount; i++)
                {
                    var currRow = _llContent.GetChildAt(i) as LinearLayout;
                    if (currRow != null && currRow.Tag.ToString() == "GameRow")
                        rows.Add(currRow);
                }
                foreach (var row in rows)
                    ((ViewGroup)row.Parent)?.RemoveView(row);
                foreach (var row in rows)
                    _llContent.AddView(row);

                for (var i = 0; i < CodeLength; i++)
                {
                    var btnColor = new ImageView(this) { LayoutParameters = new LinearLayout.LayoutParams(ConvertDpToPixels(40), ConvertDpToPixels(40)) };
                    llRow.AddView(btnColor);
                    btnColor.SetBackgroundResource(Resource.Drawable.color_hole);
                    btnColor.SetScaleType(ImageView.ScaleType.FitCenter);
                    btnColor.Click += btnColor_Click;
                    _currentRowColors.Add(new ColorButtonOptions { Color = ColorsAvailable.None, Button = btnColor });
                }

                var llAnswers = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical,
                    LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    {
                        LeftMargin = ConvertDpToPixels(20),
                        Gravity = GravityFlags.Center
                    }
                };

                llRow.AddView(llAnswers);

                var llAnswersRow1 = new LinearLayout(this)
                {
                    Orientation = Orientation.Horizontal,
                    LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) { Gravity = GravityFlags.Center }
                };

                llAnswers.AddView(llAnswersRow1);

                var llAnswersRow2 = new LinearLayout(this)
                {
                    Orientation = Orientation.Horizontal,
                    LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) { Gravity = GravityFlags.Center }
                };

                llAnswers.AddView(llAnswersRow2);

                var halfOfRow = Math.Ceiling(CodeLength / 2.0);
                for (var i = 0; i < CodeLength; i++)
                {
                    var vwColorAnswer = new ImageView(this) { LayoutParameters = new LinearLayout.LayoutParams(ConvertDpToPixels(20), ConvertDpToPixels(20)) };

                    if (i < halfOfRow)
                        llAnswersRow1.AddView(vwColorAnswer);
                    else
                        llAnswersRow2.AddView(vwColorAnswer);

                    vwColorAnswer.SetBackgroundResource(Resource.Drawable.color_hole);
                    vwColorAnswer.SetScaleType(ImageView.ScaleType.FitCenter);
                    _currentRowColorAnswers.Add(new ColorButtonOptions
                    {
                        Color = ColorsAvailable.None,
                        Button = vwColorAnswer
                    });
                }
            }

            _lblTriesLeft.Text = $"Prób: {Tries--}";
        }

        private int ConvertDpToPixels(float dpValue)
        {
            return (int)(Resources.DisplayMetrics.Density * dpValue);
        }

        private void RemoveInput()
        {
            var imm = (InputMethodManager)GetSystemService(InputMethodService);
            imm.HideSoftInputFromWindow(_txtCodeLength.WindowToken, 0);
            imm.HideSoftInputFromWindow(_txtTriesNumber.WindowToken, 0);
            imm.HideSoftInputFromWindow(_txtName.WindowToken, 0);
        }

        private void ShowMessageBox(string title, string message)
        {
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle(title);

            var tv = new TextView(this)
            {
                Text = message,
                TextSize = ConvertDpToPixels(12),
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                {
                    Gravity = GravityFlags.Center,
                    LeftMargin = ConvertDpToPixels(10),
                    RightMargin = ConvertDpToPixels(10),
                    TopMargin = ConvertDpToPixels(10),
                    BottomMargin = ConvertDpToPixels(10),
                }
            };

            var ll = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };

            alert.SetView(ll);
            ll.AddView(tv);
            alert.SetPositiveButton("OK", (EventHandler<DialogClickEventArgs>)null);
            RunOnUiThread(() => alert.Show());
        }
    }

    public enum ColorsAvailable
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Brown = 5,
        Orange = 6,
        Black = 7,
        White = 8
    }

    public enum GameState
    {
        None = 0,
        Running = 1,
        Lost = 2,
        Won = 3
    }
}

