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
using XamTally.Models;

namespace XamTally.Pages
{
    public class MainPage : ContentPage
    {
        private TimeSpan _interval;
        private bool _timerStarted = false;
        private int _tallyCount;
        private Label _tallyLabel;
        private Label _timerLabel;
        private int _timeInterval = 300;
        private DateTime _startTime;
        private int _ticks = 0;
        private TimerState _timerState;
        private bool _isInPortrait;

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
            var timerCallback = new TimerCallback(UpdateTimer);
            _timerState = new TimerState();
            _timerState.Tmr = new Timer(timerCallback, _timerState, 0, _timeInterval);
            _isInPortrait = false; //TODO: Check orientation
        }

        private void UpdateTimer(Object state)
        {
            var timerState = (TimerState)state;
            _timerState.Counter++;
            _timerLabel.Text = TimeSpan.FromMilliseconds(_timeInterval * _timerState.Counter).ToString(@"mm\:ss\.f");
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
            var toolbarList = new List<ToolbarItem>
            {
                new ToolbarItem("decrease", "", DecreaseTally, ToolbarItemOrder.Primary, 0),
                new ToolbarItem("reset", "", ResetTally, ToolbarItemOrder.Primary, 1),
                new ToolbarItem("pause", "", Pause, ToolbarItemOrder.Primary, 2),
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
        }

        private void Edit()
        {
        }

        private void Pause()
        {
        }

        private void ResetTally()
        {
            _tallyCount = 0;
            UpdateTally();
        }

        private void DecreaseTally()
        {
            _tallyCount--;
            UpdateTally();
        }

        private View BuildContent()
        {
            var grid = new Grid
            {
                BackgroundColor = Color.Red,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition()
                }
            };
            var tallyStack = new StackLayout();
            tallyStack.Children.Add(new Label
            {
                Text = "Tap to increase tally",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White
            });
            tallyStack.Children.Add(_tallyLabel);
            grid.Children.Add(tallyStack, 0, 0);

            var timeStack = new StackLayout();
            timeStack.Children.Add(_timerLabel);
            timeStack.Children.Add(new Button
            {
                Text = "Next set",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Command = new DelegateCommand(IncreaseTally)
            });
            grid.Children.Add(timeStack, 0, 1);

            return grid;
        }

        private void IncreaseTally(object obj)
        {
            _tallyCount++;
            UpdateTally();
            ResetTimer();
            ToggleTimer();
        }

        private void UpdateTally()
        {
            _tallyLabel.Text = _tallyCount.ToString();
        }

        private void ToggleTimer()
        {
            if (!_timerStarted)
            {
                _startTime = DateTime.Now;
                //Device.StartTimer(TimeSpan.FromMilliseconds(300), OnTick);
            }
            else
            {
                //Device.StartTimer(TimeSpan.FromMilliseconds(300), () => false);
            }
            _timerStarted = !_timerStarted;
        }

        private bool OnTick()
        {
            _timerLabel.Text = (DateTime.Now - _startTime).ToString(@"mm\:ss\.f");
            return true;
        }

        private void ResetTimer()
        {
            _timerStarted = false;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(width > height && _isInPortrait)
            {
                UpdateOrientation(Orientation.Landscape);
            }else if(height > width && !_isInPortrait){
                UpdateOrientation(Orientation.Portrait)
            }
        }

        private void UpdateOrientation(Orientation orientation)
        {
            _isInPortrait = _isInPortrait == orientation.Portrait;
            //TODO: setup grid according to orientation
        }
    }

    public enum Orientation
    {
        Portrait,
        Landscape
    }
}
