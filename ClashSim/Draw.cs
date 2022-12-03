using Microsoft.Graphics.Canvas;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashSim
{
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

	public class PlayField
	{
		public const int dropPositionCellWidth = 38; // # of pixels wide
		public const int dropPositionCellHeight = 31; // # of pixels high

		public const int dropPositionsCols = 18;
		public const int dropPositionsRows = 32;

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
	}

	public static class Draw
	{
		public const int PlayingField_Width = 684;
		public const int PlayingField_Height = 992;

		public const int River_X = 0;
		public const int River_Y = (PlayingField_Height - River_Height) / 2;
		public const int River_Width = PlayingField_Width;
		public const int River_Height = 50;

		public const int BehindKing_X = 0;
		public const int BehindKing_XRight = PlayingField_Width - (BehindKing_X + BehindKing_Width);
		public const int BehindKing_Y = 0;
		public const int BehindKing_YBottom = PlayingField_Height - (BehindKing_Y + BehindKing_Height);
		public const int BehindKing_Width = 225;
		public const int BehindKing_Height = 32;

		public const int Bridge_X =  110;
		public const int Bridge_XRight =  PlayingField_Width - (Bridge_X + Bridge_Width);
		public const int Bridge_Y = (PlayingField_Height - Bridge_Height) / 2;
		public const int Bridge_Width = 50;
		public const int Bridge_Height = 50;

		public const int FirePit_XLeft = 0;
		public const int FirePit_XRight = PlayingField_Width - FirePit_Width;
		public const int FirePit_YTop = River_Y - FirePit_Height;
		public const int FirePit_YBottom = River_Y + River_Height;
		public const int FirePit_Width = 40;
		public const int FirePit_Height = 40;

		public const int Tower_X =  90;
		public const int Tower_XRight =  PlayingField_Width - (Tower_X + Tower_Width);
		public const int Tower_Y = 180;
		public const int Tower_YBottom = PlayingField_Height - (Tower_Y + Tower_Height);
		public const int Tower_Width = 90;
		public const int Tower_Height = 90;

		public const int KingTower_X = (PlayingField_Width - KingTower_Width) / 2;
		public const int KingTower_Y = 60;
		public const int KingTower_YBottom = PlayingField_Height - (KingTower_Y + KingTower_Height);
		public const int KingTower_Width = 120;
		public const int KingTower_Height = 120;

		public static void DrawField(CanvasDrawingSession s)
		{
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
		}

		private static void DrawTower(CanvasDrawingSession s, TowerType towerType, bool isTop)
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

		private static void DrawBorder(CanvasDrawingSession s)
		{
			s.DrawRectangle(0, 0, PlayingField_Width, PlayingField_Height, Windows.UI.Color.FromArgb(255, 0, 0, 0), 1); // black
		}

		private static void DrawRiver(CanvasDrawingSession s)
		{
			s.FillRectangle(River_X, River_Y, River_Width, River_Height, Windows.UI.Color.FromArgb(255, 155, 156, 255)); // blue
		}

		private static void DrawBridges(CanvasDrawingSession s)
		{
			s.FillRectangle(Bridge_X, Bridge_Y, Bridge_Width, Bridge_Height, Windows.UI.Color.FromArgb(255, 214, 202, 177)); // stone color
			s.FillRectangle(Bridge_XRight, Bridge_Y, Bridge_Width, Bridge_Height, Windows.UI.Color.FromArgb(255, 214, 202, 177)); // stone color
		}

		private static void DrawFirePits(CanvasDrawingSession s)
		{
			s.FillRectangle(FirePit_XLeft, FirePit_YTop, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XRight, FirePit_YTop, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XLeft, FirePit_YBottom, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
			s.FillRectangle(FirePit_XRight, FirePit_YBottom, FirePit_Width, FirePit_Height, Windows.UI.Color.FromArgb(255, 200, 100, 100)); // reddish color
		}

		private static void DrawBTopAndBottomClosedAreas(CanvasDrawingSession s)
		{
			Windows.UI.Color color = Windows.UI.Color.FromArgb(255, 201, 208, 220); // grayish

			// top
			s.FillRectangle(BehindKing_X, BehindKing_Y, BehindKing_Width, BehindKing_Height, color);
			s.FillRectangle(BehindKing_XRight, BehindKing_Y, BehindKing_Width, BehindKing_Height, color);

			// bottom
			s.FillRectangle(BehindKing_X, BehindKing_YBottom, BehindKing_Width, BehindKing_Height, color);
			s.FillRectangle(BehindKing_XRight, BehindKing_YBottom, BehindKing_Width, BehindKing_Height, color);
		}
	}
}
