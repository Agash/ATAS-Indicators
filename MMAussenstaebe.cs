using Microsoft.VisualBasic;
using OFT.Rendering.Context;
using OFT.Rendering.Tools;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ATAS.Indicators.Technical
{
    [DisplayName("MM Aussenstaebe")]
    public class MMAussenstaebe : Indicator
    {
        private Aussenstab? _initialAussenstab = null;
        public MMAussenstaebe()
        {
            EnableCustomDrawing = true;

            //Subscribing only to drawing on final layout
            SubscribeToDrawingEvents(DrawingLayouts.Final);
        }

        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            // creating pen, width 4px
            var pen = new RenderPen(Color.BlueViolet, 4);

            //drawing horizontal line
            context.DrawLine(pen, 0, MouseLocationInfo.LastPosition.Y, ChartArea.Width, MouseLocationInfo.LastPosition.Y);

            //drawing vertical line
            context.DrawLine(pen, MouseLocationInfo.LastPosition.X, 0, MouseLocationInfo.LastPosition.X, ChartArea.Height);

            var candle = GetCandle(MouseLocationInfo.BarBelowMouse);

            if (candle != null)
            {
                var font = new RenderFont("Arial", 14);
                var text = $"Total candle volume={candle.Volume}";
                var textSize = context.MeasureString(text, font);
                var textRectangle = new Rectangle(MouseLocationInfo.LastPosition.X + 10, MouseLocationInfo.LastPosition.Y + 10, (int)textSize.Width, (int)textSize.Height);

                context.FillRectangle(Color.CornflowerBlue, textRectangle);
                context.DrawString(text, font, Color.AliceBlue, textRectangle);
            }
        }

        protected override void OnCalculate(int bar, decimal value)
        {
            var candle = GetCandle(bar);
            var prevCandle = GetCandle(bar - 1);

            var isAussenstab = (candle.Open < candle.Close && candle.Close > prevCandle.High) || (candle.Open > candle.Close && candle.Close < prevCandle.Low);

            if(isAussenstab && _initialAussenstab == null)
            {
                _initialAussenstab = new Aussenstab
                {

                };
            }
        }
    }
}
