using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ClashSim
{
    class TextLog
    {
		RichTextBlock txt;

		public TextLog(RichTextBlock txt)
		{
			this.txt = txt;
		}

		public void AddLine(string str, Windows.UI.Color color)
		{
			Paragraph paragraph = new();
			Run run = new();
			run.Text = str;
			paragraph.Foreground = new SolidColorBrush(color);
			paragraph.Inlines.Add(run);
			txt.Blocks.Add(paragraph);
		}
	}
}
