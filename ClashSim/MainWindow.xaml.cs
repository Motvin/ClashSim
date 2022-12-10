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
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;

namespace ClashSim
{
	public sealed partial class MainWindow : Window
	{
		RichTextBlock txtLog;
		CheckBox chkShowFullDeck;
		Button btnBattle;
		CanvasControl fieldCanvas;
		Dictionary<string, Card> nameAndLevelToCardDict;
		Deck currentDeckPlayerTop;
		Deck currentDeckPlayerBottom;

		Image[] deckImagePlayerTop = new Image[8];
		Image[] deckImagePlayerBottom = new Image[8];

		Grid gridMidPlayerTop;
		Grid gridMidPlayerBottom;

		Dictionary<int, PlayerLevel> playerLevelToInfoDict;

		public MainWindow()
		{
			this.InitializeComponent();

            AddControls();
		}

		// grid rows are positive values meand fixed Heights, 0 means Auto, negative values mean start sizing (i.e. -1 means *, -2 means 2*)
		public void AddGridRowDefinitions(Grid grid, params int[] gridRows)
		{
			foreach (int n in gridRows)
			{
				RowDefinition d = new RowDefinition();
				if (n == 0)
				{
					d.Height = GridLength.Auto;
				}
				else if (n > 0)
				{
					d.Height = new GridLength(n);
				}
				else
				{
					d.Height = new GridLength(-n, GridUnitType.Star);
				}

				grid.RowDefinitions.Add(d);
			}
		}

		// is it possible to have an array of a class type, but that class type automatically converts from an int to one of its values?
		// I don't think that would work exactly
		// ???there are also MinWidth/MinHeight and MaxWidth/MaxHeight, which are double values
		// need additional functions to set these if needed
		public void AddGridColumnDefinitions(Grid grid, params int[] gridCols)
		{
			foreach (int n in gridCols)
			{
				ColumnDefinition d = new ColumnDefinition();
				if (n == 0)
				{
					d.Width = GridLength.Auto;
				}
				else if (n > 0)
				{
					d.Width = new GridLength(n);
				}
				else
				{
					d.Width = new GridLength(-n, GridUnitType.Star);
				}

				grid.ColumnDefinitions.Add(d);
			}
		}

		public void InitImageArray(Image[] imageArray)
		{
			for (int i = 0; i < imageArray.Length; i++)
			{
				imageArray[i] = new Image();
				imageArray[i].CanDrag = true;
			}
		}

		public void SetImagesCanDrag(IEnumerable<Image> images, bool canDrag)
		{
			foreach (Image img in images)
			{
				img.CanDrag = canDrag;
				img.DragStarting += Img_DragStarting;
			}
		}

		private void Img_DragStarting(UIElement sender, DragStartingEventArgs args)
		{
			//args.Data.SetText("Hog Rider");
			Image img = (Image)sender;
			BitmapImage b = (BitmapImage)img.Source;
			string src = b.UriSource.LocalPath;
			//int asfd = 1;
			args.Data.SetText(src);
			args.AllowedOperations = DataPackageOperation.Link;

			//???
		}

		private void FieldCanvas_DragEnter(object sender, DragEventArgs e)
		{
			//e.Data.Text
				e.AcceptedOperation = DataPackageOperation.Link;
            e.DragUIOverride.Caption = "Giant";

			Point pt = e.GetPosition((UIElement)sender);

			int asfd = 1;
            //VisualStateManager.GoToState(fieldCanvas, "Inside", true);
            //bool hasText = e.DataView.Contains(StandardDataFormats.Text);
            //e.AcceptedOperation = hasText ? DataPackageOperation.Copy : DataPackageOperation.None;

            //var bitmap = new BitmapImage(new Uri("ms-appx:///Assets/dropcursor.png", UriKind.RelativeOrAbsolute));
            //// Anchor will define how to position the image relative to the pointer
            //Point anchor = new Point(0,52); // lower left corner of the image
            //e.DragUIOverride.SetContentFromBitmapImage(bitmap, anchor);
            //e.DragUIOverride.IsGlyphVisible = false;
            //e.DragUIOverride.IsCaptionVisible = false;
		}
		
		private void FieldCanvas_DragLeave(object sender, DragEventArgs e)
		{
			//???
		}

		private void FieldCanvas_DragOver(object sender, DragEventArgs e)
		{
			int asdf = 1;
		}

		private void FieldCanvas_Drop(object sender, DragEventArgs e)
		{
			//???
		}

		public static void InitPlayerAtLevel(Player p, int level, Dictionary<int, PlayerLevel> playerLevelToInfoDict)
		{
			p.PlayerLevel = level;

			if (playerLevelToInfoDict.TryGetValue(level, out PlayerLevel v))
			{
				p.KingTowerHitpoints = v.KingTowerHitpoints;
				p.LeftTowerHitpoints = v.PrincessTowerHitpoints;
				p.RightTowerHitpoints = v.PrincessTowerHitpoints;

				p.KingTowerDamage = v.KingTowerDamage;
				p.PrincessTowerDamage = v.PrincessTowerDamage;
			}
		}

		private void AddControls()
        {
            btnBattle = new Button();
            btnBattle.Content = "Battle";
			btnBattle.AccessKey = "B";
            btnBattle.Click += btnBattle_Click;

			chkShowFullDeck = new CheckBox();
			chkShowFullDeck.Content = "Show full deck";
			chkShowFullDeck.IsChecked = true;
			chkShowFullDeck.Click += ChkShowFullDeck_Click;

			playerLevelToInfoDict = Db.GetPlayerLevelInfo(Globals.ConnStr);

			nameAndLevelToCardDict = Db.GetCards(Globals.ConnStr);
			currentDeckPlayerTop = Db.GetDeck(Globals.ConnStr, "Simple", nameAndLevelToCardDict);
			currentDeckPlayerBottom = currentDeckPlayerTop;

			chkShowFullDeck.SetValue(Grid.RowProperty, 0);
			btnBattle.SetValue(Grid.RowProperty, 1);

			AddGridRowDefinitions(gridLeft, 0, 0, 0, 0, -1);

            gridLeft.Children.Add(chkShowFullDeck);
            gridLeft.Children.Add(btnBattle);
        
			Grid gridPlayerTop = new Grid();
			gridPlayerTop.SetValue(Grid.RowProperty, 0);
			gridMid.Children.Add(gridPlayerTop);

			// divide into 8 equal sized columns
			AddGridColumnDefinitions(gridPlayerTop, -1, -1, -1, -1, -1, -1, -1, -1);
			//AddGridRowDefinitions(gridPlayerTop, 60, 200);
			AddGridRowDefinitions(gridPlayerTop, 300);

			InitImageArray(deckImagePlayerTop);
			SetImagesCanDrag(deckImagePlayerTop, true);

			InitImageArray(deckImagePlayerBottom);
			SetImagesCanDrag(deckImagePlayerBottom, true);

			for (int i = 0; i < deckImagePlayerTop.Length; i++)
			{
				deckImagePlayerTop[i].SetValue(Grid.ColumnProperty, i);
				deckImagePlayerTop[i].SetValue(Grid.RowProperty, 0);
				gridPlayerTop.Children.Add(deckImagePlayerTop[i]);
			}

			fieldCanvas = new CanvasControl();
			fieldCanvas.Width = Draw.PlayingField_Width;
			fieldCanvas.Height = Draw.PlayingField_Height;
			fieldCanvas.SetValue(Grid.RowProperty, 1);
			fieldCanvas.AllowDrop = true;
			fieldCanvas.Draw += FieldCanvas_Draw;
			fieldCanvas.DragOver += FieldCanvas_DragOver;
			fieldCanvas.DragEnter += FieldCanvas_DragEnter;
			fieldCanvas.DragLeave += FieldCanvas_DragLeave;
			fieldCanvas.Drop += FieldCanvas_Drop;
			gridMid.Children.Add(fieldCanvas);

			Grid gridPlayerBottom = new Grid();
			gridPlayerBottom.SetValue(Grid.RowProperty, 2);
			gridMid.Children.Add(gridPlayerBottom);

			// divide into 8 equal sized columns
			AddGridColumnDefinitions(gridPlayerBottom, -1, -1, -1, -1, -1, -1, -1, -1);
			//AddGridRowDefinitions(gridPlayerBottom, 200, 60);
			AddGridRowDefinitions(gridPlayerBottom, 300);

			for (int i = 0; i < deckImagePlayerBottom.Length; i++)
			{
				deckImagePlayerBottom[i].SetValue(Grid.ColumnProperty, i);
				gridPlayerBottom.Children.Add(deckImagePlayerBottom[i]);
			}

			//fieldCanvas.BorderBrush = new SolidColorBrush(Colors.Black);
			//fieldCanvas.BorderThickness = new Thickness(1);
			//Microsoft.UI.Xaml.Controls.Border b = new Microsoft.UI.Xaml.Controls.Border();
			//gridMid.Children.Add(b);
			//b.Child = fieldCanvas;

			txtLog = new RichTextBlock();
			gridRight.Children.Add(txtLog);

			ShowFullDeck(true, chkShowFullDeck.IsChecked.Value);
			ShowFullDeck(false, chkShowFullDeck.IsChecked.Value);

			ShowPlayerDeck(true, currentDeckPlayerTop);
			ShowPlayerDeck(false, currentDeckPlayerBottom);

			//txtLog.Width = double.NaN;
			//txtLog.Height = double.NaN;
			//txtLog.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 0, 0));

			//Paragraph paragraph = new Paragraph();
			//Run run = new Run();
			//run.Text = "This is some sample text to demonstrate some properties.";
			//paragraph.Inlines.Add(run);
			//txtLog.Blocks.Add(paragraph); 
			
        }

		private void ChkShowFullDeck_Click(object sender, RoutedEventArgs e)
		{
			ShowFullDeck(true, chkShowFullDeck.IsChecked.Value);
			ShowFullDeck(false, chkShowFullDeck.IsChecked.Value);
		}

		public void ShowFullDeck(bool isTopPlayer, bool isFullDeck)
		{

		}

		public void ShowPlayerDeck(bool isTopPlayer, Deck deck)
		{
			for (int i = 0; i < deck.Cards.Length; i++)
			{
				Card c = deck.Cards[i];
				if (isTopPlayer)
				{
					deckImagePlayerTop[i].Source = new BitmapImage(new Uri("E:/Proj/ClashSim/Images/" + c.Name + ".png"));
				}
				else
				{
					deckImagePlayerBottom[i].Source = new BitmapImage(new Uri("E:/Proj/ClashSim/Images/" + c.Name + ".png"));
				}
			}
		}

		private void FieldCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
		{

			CanvasDrawingSession s = args.DrawingSession;

			CanvasSpriteBatch sb = s.CreateSpriteBatch();
			sb.Draw(new CanvasBitmap(), new Rect());
			sb.DrawFromSpriteSheet(new CanvasBitmap(), new Rect());
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

		private void btnBattle_Click(object sender, RoutedEventArgs e)
        {
			Match match = new Match(fieldCanvas);
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
