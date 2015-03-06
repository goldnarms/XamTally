using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace XamTally
{
	public static class App
	{
		public static Page GetMainPage()
		{
		    var mainPage = new MainPage();
		    return new NavigationPage(mainPage);
		}
	}
}
