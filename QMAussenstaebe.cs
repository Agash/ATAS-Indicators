using ATAS.Indicators.Drawing;
using OFT.Rendering;
using OFT.Rendering.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace ATAS.Indicators.Technical
{
    [DisplayName("QM Aussenstaebe")]
    public class QMAussenstaebe : Indicator
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
        public QMAussenstaebe() : base(true)
        {
            DrawAbovePrice = false;

            EnableCustomDrawing = true;
            SubscribeToDrawingEvents(DrawingLayouts.Historical | DrawingLayouts.LatestBar | DrawingLayouts.Final);
        }
        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            Rectangles.Clear();

            var firstVisible = FirstVisibleBarNumber - 20;
            var lastVisible = LastVisibleBarNumber + 20;
            foreach (var stab in _aussenstaebe.Where(x => x.Rectangle != null).DistinctBy(x => x.Id))
            {
                if (stab.FirstBar < firstVisible || stab.LastBar > lastVisible)
                    continue;

                if(stab.Rectangle != null)
                    Rectangles.Add(stab.Rectangle);
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
                Aussenstab? currentStab = null;
                var lastAussenstab = _aussenstaebe.Last();

                if (!lastAussenstab.LastBar.HasValue && lastAussenstab.FirstBar != bar)
                {
                    if (/** CurrentBar != bar && **/ ((candle.Open < candle.Close && candle.Close > lastAussenstab.CurrentHigh) || (candle.Open > candle.Close && candle.Close < lastAussenstab.CurrentLow)))
                    {
                        lastAussenstab.LastBar = _endOnChange ? bar : bar - 1;

                        if (isAussenstab)
                        {
                            var stab = new Aussenstab { CurrentHigh = candle.High, CurrentLow = candle.Low, FirstBar = bar, Positive = candle.Open < candle.Close };
                            _aussenstaebe.Add(stab);
                            currentStab = stab;
                        }
                    }
                    else
                    {
                        lastAussenstab.CurrentHigh = Math.Max(lastAussenstab.CurrentHigh, candle.High);
                        lastAussenstab.CurrentLow = Math.Min(lastAussenstab.CurrentLow, candle.Low);
                        _aussenstaebe.Add(lastAussenstab);
                        currentStab = lastAussenstab;
                    }
                }
                else if (lastAussenstab.LastBar.HasValue && isAussenstab)
                {
                    var stab = new Aussenstab { CurrentHigh = candle.High, CurrentLow = candle.Low, FirstBar = bar, Positive = candle.Open < candle.Close };
                    _aussenstaebe.Add(stab);
                    currentStab = stab;
                }

                if(_drawOnLive && CurrentBar == bar && currentStab != null && !currentStab.LastBar.HasValue)
                {
                    var color = currentStab.Positive ? _positiveColor : _negativeColor;
                    currentStab.Rectangle = new DrawingRectangle(currentStab.FirstBar, currentStab.CurrentHigh, CurrentBar - 1, currentStab.CurrentLow, new System.Drawing.Pen(color), new SolidBrush(color));

                    if(currentStab.Rectangle != null && currentStab.LastBar.HasValue)
                    {
                        currentStab.Rectangle = null;
                    }
                }

                if (lastAussenstab.LastBar.HasValue)
                {
                    if (lastAussenstab.Rectangle != null)
                    {
                        lastAussenstab.Rectangle = null;
                    }
                    var color = lastAussenstab.Positive ? _positiveColor : _negativeColor;
                    lastAussenstab.Rectangle = new DrawingRectangle(lastAussenstab.FirstBar, lastAussenstab.CurrentHigh, lastAussenstab.LastBar!.Value -1, lastAussenstab.CurrentLow, new System.Drawing.Pen(color), new SolidBrush(color));
                }
            }
        }

        protected override void RecalculateValues()
        {
            _aussenstaebe.Clear();
            base.RecalculateValues();
        }
    }
}
