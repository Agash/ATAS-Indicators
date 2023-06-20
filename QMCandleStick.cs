using ATAS.Indicators.Drawing;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ATAS.Indicators.Technical
{
    [DisplayName("QM CandleSticks")]
    public class QMCandleStick : Indicator
    {
        #region Fields

        private readonly ValueDataSeries _renderSeriesOutsideBarPositiv = new("Visualisierung Outside Bar Positiv")
        {
            Color = DefaultColors.Fuchsia.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1,
        };

        private readonly ValueDataSeries _renderSeriesOutsideBarNegativ = new("Visualisierung Outside Bar Negativ")
        {
            Color = DefaultColors.Purple.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesInsideBarPositiv = new("Visualisierung Inside Bar Positiv")
        {
            Color = DefaultColors.Aqua.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesInsideBarNegativ = new("Visualisierung Inside Bar Negativ")
        {
            Color = DefaultColors.Blue.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesAStabPositiv = new("Visualisierung A-Stab Positiv")
        {
            Color = DefaultColors.Teal.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesAStabNegativ = new("Visualisierung A-Stab Negativ")
        {
            Color = DefaultColors.White.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesHammer = new("Visualisierung Hammer")
        {
            Color = DefaultColors.Yellow.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        private readonly ValueDataSeries _renderSeriesShootingStar = new("Visualisierung Shooting Star")
        {
            Color = DefaultColors.Yellow.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
        };

        readonly PaintbarsDataSeries _bars = new("Bars") { IsHidden = true };

        private Color _positiveInsideBarColor = DefaultColors.Aqua.Convert();
        private Color _negativeInsideBarColor = DefaultColors.Blue.Convert();

        private Color _positiveOutsideBarColor = DefaultColors.Fuchsia.Convert();
        private Color _negativeOutsideBarColor = DefaultColors.Purple.Convert();

        private Color _positiveAStabColor = DefaultColors.Teal.Convert();
        private Color _negativeAStabColor = DefaultColors.White.Convert();

        private Color _hammerColor = DefaultColors.Yellow.Convert();
        private Color _shootingStarColor = DefaultColors.Yellow.Convert();

        private bool _showPositiveInsideBar = true;
        private bool _showNegativeInsideBar = true;
        private bool _showPositiveOutsideBar = true;
        private bool _showNegativeOutsideBar = true;
        private bool _showPositiveAStab = false;
        private bool _showNegativeAStab = false;
        private bool _showHammer = true;
        private bool _showShootingStar = true;

        private bool _paintBars = true;

        private decimal _fibLevelHammers = 0.382M;

        #endregion
        #region Properties

        public decimal FibLevelHammers
        {
            get => _fibLevelHammers;
            set
            {
                _fibLevelHammers = value;
                RecalculateValues();
            }
        }
        public Color PositiveInsideBarColor
        {
            get => _positiveInsideBarColor;
            set
            {
                _positiveInsideBarColor = value;
                RecalculateValues();
            }
        }

        public Color NegativeInsideBarColor
        {
            get => _negativeInsideBarColor;
            set
            {
                _negativeInsideBarColor = value;
                RecalculateValues();
            }
        }

        public Color PositiveOutsideBarColor
        {
            get => _positiveOutsideBarColor;
            set
            {
                _positiveOutsideBarColor = value;
                RecalculateValues();
            }
        }

        public Color NegativeOutsideBarColor
        {
            get => _negativeOutsideBarColor;
            set
            {
                _negativeOutsideBarColor = value;
                RecalculateValues();
            }
        }

        public Color PositiveAStabColor
        {
            get => _positiveAStabColor;
            set
            {
                _positiveAStabColor = value;
                RecalculateValues();
            }
        }

        public Color NegativeAStabColor
        {
            get => _negativeAStabColor;
            set
            {
                _negativeAStabColor = value;
                RecalculateValues();
            }
        }

        public Color HammerColor
        {
            get => _hammerColor;
            set
            {
                _hammerColor = value;
                RecalculateValues();
            }
        }

        public Color ShootingStarColor
        {
            get => _shootingStarColor;
            set
            {
                _shootingStarColor = value;
                RecalculateValues();
            }
        }

        public bool PaintBars
        {
            get => _paintBars;
            set
            {
                _paintBars = value;
                RecalculateValues();
            }
        }
        public bool ShowPositiveInsideBar
        {
            get => _showPositiveInsideBar;
            set
            {
                _showPositiveInsideBar = value;
                RecalculateValues();
            }
        }
        public bool ShowNegativeInsideBar
        {
            get => _showNegativeInsideBar;
            set
            {
                _showNegativeInsideBar = value;
                RecalculateValues();
            }
        }
        public bool ShowPositiveOutsideBar
        {
            get => _showPositiveOutsideBar;
            set
            {
                _showPositiveOutsideBar = value;
                RecalculateValues();
            }
        }
        public bool ShowNegativeOutsideBar
        {
            get => _showNegativeOutsideBar;
            set
            {
                _showNegativeOutsideBar = value;
                RecalculateValues();
            }
        }
        public bool ShowPositiveAStab
        {
            get => _showPositiveAStab;
            set
            {
                _showPositiveAStab = value;
                RecalculateValues();
            }
        }
        public bool ShowNegativeAStab
        {
            get => _showNegativeAStab;
            set
            {
                _showNegativeAStab = value;
                RecalculateValues();
            }
        }
        public bool ShowHammer
        {
            get => _showHammer;
            set
            {
                _showHammer = value;
                RecalculateValues();
            }
        }
        public bool ShowShootingStar
        {
            get => _showShootingStar;
            set
            {
                _showShootingStar = value;
                RecalculateValues();
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            DataSeries[0] = _bars;
            DataSeries.Add(_renderSeriesOutsideBarPositiv);
            DataSeries.Add(_renderSeriesOutsideBarNegativ);
            DataSeries.Add(_renderSeriesInsideBarPositiv);
            DataSeries.Add(_renderSeriesInsideBarNegativ);
            DataSeries.Add(_renderSeriesAStabPositiv);
            DataSeries.Add(_renderSeriesAStabNegativ);
            DataSeries.Add(_renderSeriesHammer);
            DataSeries.Add(_renderSeriesShootingStar);
        }

        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar == 0)
            {
                _renderSeriesOutsideBarPositiv.Clear();
                _renderSeriesOutsideBarNegativ.Clear();
                _renderSeriesInsideBarPositiv.Clear();
                _renderSeriesInsideBarNegativ.Clear();
                _renderSeriesAStabPositiv.Clear();
                _renderSeriesAStabNegativ.Clear();
                _renderSeriesHammer.Clear();
                _renderSeriesShootingStar.Clear();
                return;
            }

            var candle = GetCandle(bar);
            var prevCandle = GetCandle(bar - 1);


            var candleSize = Math.Abs(candle.High - candle.Low);
            var isHammer = (candle.High - (_fibLevelHammers * candleSize)) < Math.Min(candle.Open, candle.Close);
            var isShootingStar = (candle.Low + (_fibLevelHammers * candleSize)) > Math.Max(candle.Open, candle.Close);

            Color? barColor = null;

            switch (candle.Open < candle.Close)
            {
                case true when candle.High < prevCandle.High && candle.Low > prevCandle.Low && _showPositiveInsideBar:
                    _renderSeriesInsideBarPositiv[bar] = candle.High;
                    barColor = _positiveInsideBarColor;
                    break;
                case false when candle.High < prevCandle.High && candle.Low > prevCandle.Low && _showNegativeInsideBar:
                    _renderSeriesInsideBarNegativ[bar] = candle.Low;
                    barColor = _negativeInsideBarColor;
                    break;
                case true when candle.High > prevCandle.High && candle.Low < prevCandle.Low && _showPositiveOutsideBar:
                    _renderSeriesOutsideBarPositiv[bar] = candle.High;
                    barColor = _positiveOutsideBarColor;
                    break;
                case false when candle.High > prevCandle.High && candle.Low < prevCandle.Low && _showNegativeOutsideBar:
                    _renderSeriesOutsideBarNegativ[bar] = candle.Low;
                    barColor = _negativeOutsideBarColor;
                    break;
                case true when candle.Close > prevCandle.High && _showPositiveAStab:
                    _renderSeriesAStabPositiv[bar] = candle.High;
                    barColor = _positiveAStabColor;
                    break;
                case false when candle.Close < prevCandle.Low && _showNegativeAStab:
                    _renderSeriesAStabNegativ[bar] = candle.Low;
                    barColor = _negativeAStabColor;
                    break;
            }

            if (isHammer)
            {
                _renderSeriesHammer[bar] = candle.High;
                barColor = _hammerColor;
            }

            if (isShootingStar)
            {
                _renderSeriesShootingStar[bar] = candle.Low;
                barColor = _shootingStarColor;
            }

            if (_paintBars)
                _bars[bar] = barColor;

        }
    }
}
