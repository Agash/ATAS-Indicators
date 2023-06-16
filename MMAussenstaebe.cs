using ATAS.Indicators.Drawing;
using OFT.Rendering;
using OFT.Rendering.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ATAS.Indicators.Technical
{
    [DisplayName("MM Aussenstaebe")]
    public class MMAussenstaebe : Indicator
    {
        private Aussenstab? _initialAussenstab = null;
        private readonly List<Aussenstab> _aussenstaebe = new();
        private bool _endOnChange = true;
        private bool _drawOnLive = true;
        private System.Drawing.Color _positiveColor = DefaultColors.Green.SetTransparency(0.8M);
        private System.Drawing.Color _negativeColor = DefaultColors.Red.SetTransparency(0.8M);

        public bool DrawOnLive
        {
            get => _drawOnLive;
            set => _drawOnLive = value;
        }

        public bool EndOnChange
        {
            get => _endOnChange;
            set
            {
                _endOnChange = value;
                //RecalculateValues();
            }
        }

        public System.Drawing.Color PositiveColor
        {
            get => _positiveColor;
            set => _positiveColor = value;
        }

        public System.Drawing.Color NegativeColor
        {
            get => _negativeColor;
            set => _negativeColor = value;
        }
        public MMAussenstaebe()
        {
            EnableCustomDrawing = true;
            SubscribeToDrawingEvents(DrawingLayouts.LatestBar | DrawingLayouts.Final);
        }

        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            foreach (var stab in _aussenstaebe.Where(x => x.LastBar != null))
            {
                if (stab.FirstBar < FirstVisibleBarNumber || stab.LastBar > LastVisibleBarNumber)
                    continue;

                var color = stab.Positive ? _positiveColor : _negativeColor;

                var y1 = ChartInfo.GetYByPrice(stab.CurrentHigh, false);
                var y2 = ChartInfo.GetYByPrice(stab.CurrentLow, false);
                var x1 = ChartInfo.GetXByBar(stab.FirstBar, false);
                var x2 = ChartInfo.GetXByBar(stab.LastBar!.Value, false);

                var rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                context.FillRectangle(color, rect);
            }

            var lastStab = _aussenstaebe.LastOrDefault();
            if (lastStab != null && _drawOnLive && lastStab.LastBar == null)
            {
                var color = lastStab.Positive ? _positiveColor : _negativeColor;

                var y1 = ChartInfo.GetYByPrice(lastStab.CurrentHigh, false);
                var y2 = ChartInfo.GetYByPrice(lastStab.CurrentLow, false);
                var x1 = ChartInfo.GetXByBar(lastStab.FirstBar, false);
                var x2 = ChartInfo.GetXByBar(CurrentBar, false);


                var rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                context.FillRectangle(color, rect);
            }

        }

        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar == 0)
                return;

            var candle = GetCandle(bar);
            var prevCandle = GetCandle(bar - 1);

            var isAussenstab = (candle.Open < candle.Close && candle.Close > prevCandle.High) || (candle.Open > candle.Close && candle.Close < prevCandle.Low);

            if (isAussenstab && _initialAussenstab == null)
            {
                _initialAussenstab = new Aussenstab
                {
                    FirstBar = bar,
                    CurrentHigh = candle.High,
                    CurrentLow = candle.Low,
                    Positive = candle.Open < candle.Close
                };
            }

            if (_aussenstaebe.Count < 1 && _initialAussenstab != null)
                _aussenstaebe.Add(_initialAussenstab);

            if (_aussenstaebe.Count > 0)
            {
                var lastAussenstab = _aussenstaebe.Last();

                if (lastAussenstab.LastBar == null && lastAussenstab.FirstBar != bar)
                {
                    if ((candle.Open < candle.Close && candle.Close > lastAussenstab.CurrentHigh) || (candle.Open > candle.Close && candle.Close < lastAussenstab.CurrentLow))
                    {
                        lastAussenstab.LastBar = _endOnChange ? bar : bar - 1;

                        if (isAussenstab)
                            _aussenstaebe.Add(new Aussenstab { CurrentHigh = candle.High, CurrentLow = candle.Low, FirstBar = bar, Positive = candle.Open < candle.Close });
                    }
                    else
                    {
                        lastAussenstab.CurrentHigh = Math.Max(lastAussenstab.CurrentHigh, candle.High);
                        lastAussenstab.CurrentLow = Math.Min(lastAussenstab.CurrentLow, candle.Low);
                    }
                }
                else if (lastAussenstab.LastBar != null && isAussenstab)
                {
                    _aussenstaebe.Add(new Aussenstab { CurrentHigh = candle.High, CurrentLow = candle.Low, FirstBar = bar, Positive = candle.Open < candle.Close });
                }
            }
        }

        protected override void RecalculateValues()
        {
            base.RecalculateValues();
            _aussenstaebe.Clear();
        }
    }
}
