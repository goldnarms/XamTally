using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XamTally.Pages;

namespace XamTally
{
	public class App
	{
		public static Page GetMainPage()
		{
		    return new MainPage();
		}
	}
}
