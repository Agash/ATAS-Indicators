using ATAS.Indicators;
using ATAS.Indicators.Drawing;
using OFT.Rendering.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Security.AccessControl;
using System.Windows.Media;
using System.Xml.Linq;
using Utils.Common.Logging;

namespace ATAS.Indicators.Technical
{
    [DisplayName("MM CandleSticks")]
    public class MMCandleStick : Indicator
    {
        #region Fields

        private readonly ValueDataSeries _renderSeriesOutsideBarPositiv = new("Visualisierung Outside Bar Positiv")
        {
            Color = DefaultColors.Fuchsia.Convert(),
            VisualType = VisualMode.Hide,
            Width = 1
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

        readonly PaintbarsDataSeries _bars = new("Bars") { IsHidden = true };

        private Color _positiveInsideBarColor = DefaultColors.Aqua.Convert();
        private Color _negativeInsideBarColor = DefaultColors.Blue.Convert();

        private Color _positiveOutsideBarColor = DefaultColors.Fuchsia.Convert();
        private Color _negativeOutsideBarColor = DefaultColors.Purple.Convert();

        private Color _positiveAStabColor = DefaultColors.Orange.Convert();
        private Color _negativeAStabColor = DefaultColors.Yellow.Convert();

        private bool _showPositiveInsideBar = true;
        private bool _showNegativeInsideBar = true;
        private bool _showPositiveOutsideBar = true;
        private bool _showNegativeOutsideBar = true;
        private bool _showPositiveAStab = false;
        private bool _showNegativeAStab = false;

        private bool _paintBars = true;

        #endregion
        #region Properties

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
                return;
            }

            var candle = GetCandle(bar);
            var prevCandle = GetCandle(bar - 1);
            Color? barColor = null;

            switch(candle.Open < candle.Close)
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

            if (_paintBars && barColor != null)
                _bars[bar] = barColor;

        }
    }
}
