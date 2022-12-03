using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Graphics.Canvas.UI.Xaml;
using DevExpress.Text.Interop;
using Microsoft.Graphics.Canvas;
using static DevExpress.Mvvm.Native.Either;
using DevExpress.Pdf.Drawing.DirectX;
using DevExpress.WinUI.Grid;

namespace ClashSim
{
	public sealed partial class MainWindow : Window
	{
		RichTextBlock txtLog;
		Button btnBattle;
		CanvasControl fieldCanvas;

		public MainWindow()
		{
			this.InitializeComponent();

            AddControls();
		}

        private void AddControls()
        {
            btnBattle = new Button();
            btnBattle.Content = "Battle";
            btnBattle.Click += btnBattle_Click;
			
			btnBattle.SetValue(Grid.RowProperty, 0);

            gridLeft.Children.Add(btnBattle);
        
			fieldCanvas = new CanvasControl();
			fieldCanvas.Width = Draw.PlayingField_Width;
			fieldCanvas.Height = Draw.PlayingField_Height;
			fieldCanvas.SetValue(Grid.RowProperty, 1);
			fieldCanvas.Draw += FieldCanvas_Draw;
			gridMid.Children.Add(fieldCanvas);

			//fieldCanvas.BorderBrush = new SolidColorBrush(Colors.Black);
			//fieldCanvas.BorderThickness = new Thickness(1);
			//Microsoft.UI.Xaml.Controls.Border b = new Microsoft.UI.Xaml.Controls.Border();
			//gridMid.Children.Add(b);
			//b.Child = fieldCanvas;

			txtLog = new RichTextBlock();
			gridRight.Children.Add(txtLog);

			//txtLog.Width = double.NaN;
			//txtLog.Height = double.NaN;
			//txtLog.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 0, 0));

			//Paragraph paragraph = new Paragraph();
			//Run run = new Run();
			//run.Text = "This is some sample text to demonstrate some properties.";
			//paragraph.Inlines.Add(run);
			//txtLog.Blocks.Add(paragraph); 
			
        }

		private void FieldCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
		{
			CanvasDrawingSession s = args.DrawingSession;

			Draw.DrawField(s);


			//return;

			//const float defaultDpi = 96;
			//CanvasRenderTarget r = new CanvasRenderTarget(sender, (float)sender.ActualWidth, (float)sender.ActualHeight, defaultDpi);           
			
			//CanvasDrawingSession s = r.CreateDrawingSession();

			////Windows.UI.Color col = Windows.UI.Color.FromArgb(0, 255, 0, 0);

			//s.DrawRectangle(10.0f, 20.0f, 5f, 5f, col);

			//int width = 2;
			//int height = 3;
			//Windows.UI.Color[] colorArray = new Windows.UI.Color[width * height];
			//colorArray[0] = col;
			//colorArray[1] = col;
			//colorArray[2] = col;
			//colorArray[3] = col;
			//colorArray[4] = col;
			//colorArray[5] = col;
			//r.SetPixelColors(colorArray, 10, 20, width, height);

			//r.SetPixelBytes()
		}

		Match match;
		private void btnBattle_Click(object sender, RoutedEventArgs e)
        {
			match = new Match(fieldCanvas);
		}

		private void TestLog()
		{
			Paragraph paragraph = new Paragraph();
			Run run = new Run();
			run.Foreground = new SolidColorBrush(Colors.Red);
			run.Text = "This is some sample text to demonstrate some properties.";
			paragraph.Inlines.Add(run);
			txtLog.Blocks.Add(paragraph);
		}

    }

	public enum TowerType
	{
		Left,
		Right,
		King
	}
}
