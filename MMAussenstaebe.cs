using ATAS.Indicators.Drawing;
using OFT.Rendering;
using OFT.Rendering.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Color = System.Drawing.Color;

namespace ATAS.Indicators.Technical
{
    [DisplayName("MM Aussenstaebe")]
    public class MMAussenstaebe : Indicator
    {
        private Aussenstab? _initialAussenstab = null;
        private readonly List<Aussenstab> _aussenstaebe = new();
        private bool _endOnChange = true;
        private bool _drawOnLive = true;
        private Color _positiveColor = DefaultColors.Green.SetTransparency(0.8M);
        private Color _negativeColor = DefaultColors.Red.SetTransparency(0.8M);

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
                RecalculateValues();
            }
        }

        public Color PositiveColor
        {
            get => _positiveColor;
            set => _positiveColor = value;
        }

        public Color NegativeColor
        {
            get => _negativeColor;
            set => _negativeColor = value;
        }
        public MMAussenstaebe()
        {
            EnableCustomDrawing = true;
            DrawAbovePrice = false;
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

            Rectangles.Clear();
            var firstVisible = FirstVisibleBarNumber - 20;
            var lastVisible = LastVisibleBarNumber + 20;

            foreach (var stab in _aussenstaebe.Where(x => x.LastBar != null))
            {
                if (stab.FirstBar < firstVisible || stab.LastBar > lastVisible)
                    continue;

                var color = stab.Positive ? _positiveColor : _negativeColor;
                Rectangles.Add(new DrawingRectangle(stab.FirstBar, stab.CurrentHigh - 1, stab.LastBar!.Value - 1, stab.CurrentLow, new System.Drawing.Pen(color), new SolidBrush(color)));
            }

            var lastStab = _aussenstaebe.LastOrDefault();
            if (lastStab != null && _drawOnLive && lastStab.LastBar == null)
            {
                var color = lastStab.Positive ? _positiveColor : _negativeColor;
                Rectangles.Add(new DrawingRectangle(lastStab.FirstBar, lastStab.CurrentHigh, CurrentBar - 1, lastStab.CurrentLow, new System.Drawing.Pen(color), new SolidBrush(color)));
            }
        }

        protected override void RecalculateValues()
        {
            _aussenstaebe.Clear();
            base.RecalculateValues();
        }
    }
}
