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
		Button btnTest;
		CanvasControl fieldCanvas;

		public MainWindow()
		{
			this.InitializeComponent();

            AddControls();
		}

        private void AddControls()
        {
            btnTest = new Button();
            btnTest.Content = "Test";
            btnTest.Click += BtnTest_Click;
			
			btnTest.SetValue(Grid.RowProperty, 0);

            gridLeft.Children.Add(btnTest);
        
			fieldCanvas = new CanvasControl();
			fieldCanvas.Width = PlayingField_Width;
			fieldCanvas.Height = PlayingField_Height;
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

			DrawBorder(s);

			// need to only draw if it isn't distroyed - base this on PlayerInfo???
			DrawTower(s, TowerType.Left, true);
			DrawTower(s, TowerType.King, true);
			DrawTower(s, TowerType.Right, true);

			DrawTower(s, TowerType.Left, false);
			DrawTower(s, TowerType.King, false);
			DrawTower(s, TowerType.Right, false);

			DrawRiver(s);

			DrawBridges(s);

			DrawFirePits(s);

			DrawBTopAndBottomClosedAreas(s);

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

		const int PlayingField_Width = 684;
		const int PlayingField_Height = 992;

		const int River_X = 0;
		const int River_Y = (PlayingField_Height - River_Height) / 2;
		const int River_Width = PlayingField_Width;
		const int River_Height = 50;

		const int BehindKing_X = 0;
		const int BehindKing_XRight = PlayingField_Width - (BehindKing_X + BehindKing_Width);
		const int BehindKing_Y = 0;
		const int BehindKing_YBottom = PlayingField_Height - (BehindKing_Y + BehindKing_Height);
		const int BehindKing_Width = 225;
		const int BehindKing_Height = 32;

		const int Bridge_X =  110;
		const int Bridge_XRight =  PlayingField_Width - (Bridge_X + Bridge_Width);
		const int Bridge_Y = (PlayingField_Height - Bridge_Height) / 2;
		const int Bridge_Width = 50;
		const int Bridge_Height = 50;

		const int FirePit_XLeft = 0;
		const int FirePit_XRight = PlayingField_Width - FirePit_Width;
		const int FirePit_YTop = River_Y - FirePit_Height;
		const int FirePit_YBottom = River_Y + River_Height;
		const int FirePit_Width = 40;
		const int FirePit_Height = 40;

		const int Tower_X =  90;
		const int Tower_XRight =  PlayingField_Width - (Tower_X + Tower_Width);
		const int Tower_Y = 180;
		const int Tower_YBottom = PlayingField_Height - (Tower_Y + Tower_Height);
		const int Tower_Width = 90;
		const int Tower_Height = 90;

		const int KingTower_X = (PlayingField_Width - KingTower_Width) / 2;
		const int KingTower_Y = 60;
		const int KingTower_YBottom = PlayingField_Height - (KingTower_Y + KingTower_Height);
		const int KingTower_Width = 120;
		const int KingTower_Height = 120;

		public struct DropPosition
		{
			public DropPosition(bool ground, bool air)
			{
				AllowGround= ground;
				AllowAir = air;
			}

			public static DropPosition All()
			{
				return new DropPosition(true, true);
			}

			public static DropPosition None()
			{
				return new DropPosition(false, false);
			}

			public static DropPosition Air()
			{
				return new DropPosition(false, true);
			}

			public static DropPosition Ground()
			{
				return new DropPosition(true, false);
			}

			public bool AllowGround { get; set; }
			public bool AllowAir { get; set; }
		}

		const int dropPositionCellWidth = 38; // # of pixels wide
		const int dropPositionCellHeight = 31; // # of pixels high

		const int dropPositionsCols = 18;
		const int dropPositionsRows = 32;

		DropPosition[,] startingDropPositionArray = new DropPosition[dropPositionsRows, dropPositionsCols]
		{
			// 0
			{
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None()
			}
			,
			// 1
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 2
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 3
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 4
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 5
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 6
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 7
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 8
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 9
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 10
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 11
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 12
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 13
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 14
			{
				DropPosition.None(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.None()
			}
			,
			// 15
			{
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.All(), DropPosition.None(), DropPosition.None(),
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None(),
				DropPosition.None(), DropPosition.None(),DropPosition.All(), DropPosition.None(), DropPosition.None(), DropPosition.None()
			}
			,
			// 16
			{
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.All(), DropPosition.None(), DropPosition.None(),
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None(),
				DropPosition.None(), DropPosition.None(),DropPosition.All(), DropPosition.None(), DropPosition.None(), DropPosition.None()
			}
			,
			// 17
			{
				DropPosition.None(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.None()
			}
			,
			// 18
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 19
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 20
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 21
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 22
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 23
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 24
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 25
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 26
			{
				DropPosition.All(), DropPosition.All(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.All(), DropPosition.All()
			}
			,
			// 27
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 28
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 29
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 30
			{
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.All(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.All(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All()
			}
			,
			// 31
			{
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None(),
				DropPosition.All(), DropPosition.All(),DropPosition.All(), DropPosition.All(), DropPosition.All(), DropPosition.All(),
				DropPosition.None(), DropPosition.None(),DropPosition.None(), DropPosition.None(), DropPosition.None(), DropPosition.None()
			}
		};

		private void DrawTower(CanvasDrawingSession s, TowerType towerType, bool isTop)
		{
			float x, y, w, h;

			if (towerType == TowerType.King)
			{
				w = KingTower_Width;
				h = KingTower_Height;
				x = KingTower_X;

				y = (isTop ? KingTower_Y : KingTower_YBottom);
			}
			else
			{
				w = Tower_Width;
				h = Tower_Height;

				x = (towerType == TowerType.Left ? Tower_X : Tower_XRight);

				y = (isTop ? Tower_Y : Tower_YBottom);
			}

			s.DrawRectangle(x, y, w, h, Windows.UI.Color.FromArgb(255, 0, 0, 0), 1); // black
		}

		private void DrawBorder(CanvasDrawingSession s)
		{
			s.DrawRectangle(0, 0, PlayingField_Width, PlayingField_Height, Windows.UI.Color.FromArgb(255, 0, 0, 0), 1); // black
		}

		private void DrawRiver(CanvasDrawingSession s)
		{
			s.FillRectangle(River_X, River_Y, River_Width, River_Height, Windows.UI.Color.FromArgb(255, 155, 156, 255)); // blue
		}

		private void DrawBridges(CanvasDrawingSession s)
		{
			s.FillRectangle(Bridge_X, Bridge_Y, Bridge_Width, Bridge_Height, Windows.UI.Color.FromArgb(255, 214, 202, 177)); // stone color
			s.FillRectangle(Bridge_XRight, Bridge_Y, Bridge_Width, Bridge_Height, Windows.UI.Color.FromArgb(255, 214, 202, 177)); // stone color
		}

		private void DrawFirePits(CanvasDrawingSession s)
		{
			s.FillRectangle(FirePit_XLeft, FirePit_YTop, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XRight, FirePit_YTop, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XLeft, FirePit_YBottom, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XRight, FirePit_YBottom, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
		}

		private void DrawBTopAndBottomClosedAreas(CanvasDrawingSession s)
		{
			Windows.UI.Color color = Windows.UI.Color.FromArgb(255, 201, 208, 220); // grayish

			// top
			s.FillRectangle(BehindKing_X, BehindKing_Y, BehindKing_Width, BehindKing_Height, color);
			s.FillRectangle(BehindKing_XRight, BehindKing_Y, BehindKing_Width, BehindKing_Height, color);

			// bottom
			s.FillRectangle(BehindKing_X, BehindKing_YBottom, BehindKing_Width, BehindKing_Height, color);
			s.FillRectangle(BehindKing_XRight, BehindKing_YBottom, BehindKing_Width, BehindKing_Height, color);
		}

		private void BtnTest_Click(object sender, RoutedEventArgs e)
        {

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
