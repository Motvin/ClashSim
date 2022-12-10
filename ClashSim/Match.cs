using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashSim
{
	public class Match
	{
		DispatcherTimer timer;
		CanvasControl fieldCanvas;

		public Match(CanvasControl fieldCanvas)
		{
			this.fieldCanvas = fieldCanvas;

			this.fieldCanvas.Draw += FieldCanvas_Draw;
			timer = new DispatcherTimer();

			const int msInterval = 15;
            timer.Interval = new TimeSpan(0, 0, 0, 0, msInterval);
			timer.Tick += Timer_Tick;
            timer.Start();
		}

		private void FieldCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
		{
			Draw.DrawField(args.DrawingSession);
		}

		private void Timer_Tick(object sender, object e)
		{
			fieldCanvas.Invalidate();

			//???
		}
	}
}
