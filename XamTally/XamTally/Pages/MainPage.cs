using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Outcoder.UI.Xaml;
using Xamarin.Forms;
using System.Runtime;
using System.Threading;

namespace XamTally
{
    public class MainPage : ContentPage
    {
        private TimeSpan _interval;
        private bool _timerStarted = false;
        private int _tallyCount;
        private Label _tallyLabel;
        private Label _timerLabel;
        private const int _timeInterval = 300;
        private DateTime _startTime;
        //private int _ticks = 0;
        private TimerState _timerState;
        private bool _isInPortrait;
		private Grid _grid;
		private ToolbarItem _toggleTimer;
		private StackLayout _tallyStack;
		private StackLayout _timeStack;
		private bool _orientationLocked;

        public MainPage()
        {
            Init();
            SetupUI();
            Content = BuildContent();
            BuildToolbar();
        }

        private void Init()
        {
            _interval = new TimeSpan(0, 1, 0);
            _tallyCount = 0;
            _timerState = new TimerState();
			_timerLabel = new Label { Text = _interval.ToString (@"mm\:ss\.f") };
            _isInPortrait = false; //TODO: Check orientation
			_orientationLocked = false;
        }

        private void UpdateTimer(Object state)
        {
			if (_timerState != null) {
				Device.BeginInvokeOnMainThread (() => {
					var timerState = (TimerState)state;
					_timerState.Counter++;
					var timeElapsed = TimeSpan.FromMilliseconds (_timeInterval * _timerState.Counter);
					var remaining = _interval - timeElapsed;
					var prefix = "";
					if(remaining.TotalSeconds < 0){
						_grid.BackgroundColor = Color.Red;
						prefix = "-";
					}
					else if(remaining.TotalSeconds < 10){
						_grid.BackgroundColor = Color.FromHex("FFFFA500");
					} 
					else{
						_grid.BackgroundColor = Color.Black;
					}
					_timerLabel.Text = prefix + remaining.ToString (@"mm\:ss\.f");
				});
			}
        }

        private void SetupUI()
        {
            _tallyLabel = new Label
            {
                Text = _tallyCount.ToString(),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White,
#if __IOS__
                Font = Font.SystemFontOfSize(123)
#else
                Font = Font.SystemFontOfSize(246)
#endif
            };
            _timerLabel = new Label
            {
                Text = _interval.ToString(@"mm\:ss\.f"),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White,
#if __IOS__
                Font = Font.SystemFontOfSize(54 )
#else
                Font = Font.SystemFontOfSize(108)
#endif                                
            };
        }

        private void BuildToolbar()
        {
			_toggleTimer = new ToolbarItem ("pause", "", ToggleTimer, ToolbarItemOrder.Primary, 2);
            var toolbarList = new List<ToolbarItem>
            {
                new ToolbarItem("decrease", "", DecreaseTally, ToolbarItemOrder.Primary, 0),
                new ToolbarItem("reset", "", ResetTally, ToolbarItemOrder.Primary, 1),
				_toggleTimer,
                new ToolbarItem("edit", "", Edit, ToolbarItemOrder.Primary, 3),
                new ToolbarItem("lock screen", "", LockScreen, ToolbarItemOrder.Secondary),
                new ToolbarItem("flip screen", "", FlipScreen, ToolbarItemOrder.Secondary),
                new ToolbarItem("about", "", About, ToolbarItemOrder.Secondary)
            };
            toolbarList.ForEach(tb => ToolbarItems.Add(tb));
        }

        private void About()
        {
        }

        private void FlipScreen()
        {
        }

        private void LockScreen()
        {
			_orientationLocked = !_orientationLocked;
        }

        private void Edit()
        {
        }

        private void ResetTally()
        {
            _tallyCount = 0;
            UpdateTally();
        }

        private void DecreaseTally()
        {
			_tallyCount = _tallyCount > 0 ? _tallyCount - 1 : 0;
            UpdateTally();
        }

        private View BuildContent()
        {
            _grid = new Grid
            {
				BackgroundColor = Color.Black,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };
			_tallyStack = new StackLayout();
			_tallyStack.Children.Add(new Label
            {
                Text = "Tap to increase tally",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White
            });
			_tallyStack.Children.Add(_tallyLabel);
			_grid.Children.Add(_tallyStack, 1, 0);

            _timeStack = new StackLayout();
			_timeStack.Children.Add(_timerLabel);
			_timeStack.Children.Add(new Button
            {
                Text = "Next set",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Command = new DelegateCommand(IncreaseTally)
            });
			_grid.Children.Add(_timeStack, 1, 1);

            return _grid;
        }

        private void IncreaseTally(object obj)
        {
            _tallyCount++;
            UpdateTally();
            ResetTimer();
			if (!_timerStarted) {
				ToggleTimer ();
			}
        }

        private void UpdateTally()
        {
            _tallyLabel.Text = _tallyCount.ToString();
        }

        private void ToggleTimer()
        {
            if (!_timerStarted)
            {
				var timerCallback = new TimerCallback(UpdateTimer);
				_timerState.Tmr = new Timer(timerCallback, _timerState, 0, _timeInterval);
				_toggleTimer.Name = "pause";
                //Device.StartTimer(TimeSpan.FromMilliseconds(300), OnTick);
            }
            else
            {
				_timerState.Tmr = null;
				_toggleTimer.Name = "start";
                //Device.StartTimer(TimeSpan.FromMilliseconds(300), () => false);
            }
            _timerStarted = !_timerStarted;
        }

        private void ResetTimer()
        {
			_startTime = DateTime.Now;
        }

        protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated (width, height);
			if (_orientationLocked)
				return;

			if (_grid.Children.Count > 0){
				if (width > height && _isInPortrait) {
					SetOrientationUI (Orientation.Landscape, Orientation.Portrait);
				} else if (height > width && !_isInPortrait) {
					SetOrientationUI (Orientation.Portrait, Orientation.Landscape);
				}
			}
        }

		private void SetOrientationUI(Orientation newOrientation, Orientation oldOrientation){
			if (newOrientation == Orientation.Landscape && oldOrientation == Orientation.Portrait) {
				UpdateOrientation (Orientation.Landscape);
				_grid.Children.Clear ();
				_grid.Children.Add (_tallyStack, 1, 0);            
				_grid.Children.Add (_timeStack, 1, 1);
			} else if (newOrientation == Orientation.Portrait && oldOrientation == Orientation.Landscape) {
				UpdateOrientation (Orientation.Portrait);
				_grid.Children.Clear ();
				_grid.Children.Add (_tallyStack, 0, 1);
				_grid.Children.Add (_timeStack, 1, 1);
			}
		}

        private void UpdateOrientation(Orientation orientation)
        {
            _isInPortrait = orientation == Orientation.Portrait;

            //TODO: setup grid according to orientation
        }
    }

    public enum Orientation
    {
        Portrait,
        Landscape
    }
}
