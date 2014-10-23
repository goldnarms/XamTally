using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Runtime;

namespace XamTally.Pages
{
    internal class MainPage : ContentPage
    {
        public MainPage()
        {
            Content = BuildContent();
        }

        private View BuildContent()
        {
            var grid = new Grid()
            {
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = new ColumnDefinitionCollection()
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
            var stackLayout = new StackLayout();
            stackLayout.Children.Add(new Label {
                Text = "Tap to increase tally",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White
            });
            grid.Children.Add(stackLayout,1, 1);
            return grid;
        }
    }
}
