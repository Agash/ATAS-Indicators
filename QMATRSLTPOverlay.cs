using ATAS.Indicators.Drawing;
using OFT.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ATAS.Indicators.Technical.DynamicLevels;

namespace ATAS.Indicators.Technical
{
    [DisplayName("QM ATR SL&TP (Overlay)")]
    public class QMATRSLTPOverlay : Indicator
    {
        #region Fields

        private readonly ATR _atr = new()
        {
            Period = 10
        };

        private decimal _multiplier = 1.0M;
        private decimal _crv = 2.0M;


        private readonly ValueDataSeries _renderSeriesLongSL = new("Long SL")
        {
            Color = DefaultColors.Red.SetTransparency(0.8M).Convert()
        };

        private readonly ValueDataSeries _renderSeriesLongTP = new("Long TP")
        {
            Color = DefaultColors.Green.SetTransparency(0.8M).Convert()
        };

        private readonly ValueDataSeries _renderSeriesShortSL = new("Short SL")
        {
            Color = DefaultColors.Red.SetTransparency(0.8M).Convert()
        };

        private readonly ValueDataSeries _renderSeriesShortTP = new("Short TP")
        {
            Color = DefaultColors.Green.SetTransparency(0.8M).Convert()
        };

        #endregion

        #region Properties

        [DisplayName("ATR Periode")]
        [Range(1, 10000)]
        public int Period
        {
            get => _atr.Period;
            set
            {
                _atr.Period = value;
                RecalculateValues();
            }
        }

        [DisplayName("Multiplier")]
        public decimal Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                RecalculateValues();
            }
        }

        [DisplayName("CRV")]
        public decimal CRV
        {
            get => _crv;
            set
            {
                _crv = value;
                RecalculateValues();
            }
        }

        public QMATRSLTPOverlay() : base(true)
        {
            Add(_atr);

            DataSeries[0] = _renderSeriesLongSL;
            DataSeries.Add(_renderSeriesLongTP);
            DataSeries.Add(_renderSeriesShortSL);
            DataSeries.Add(_renderSeriesShortTP);
        }

        #endregion
        protected override void OnCalculate(int bar, decimal value)
        {
            var atr = ((ValueDataSeries)_atr.DataSeries[0])[bar];
            var sl = atr * _multiplier;
            var tp = sl * _crv;

            var candle = GetCandle(bar);
            var pocPrice = candle.MaxVolumePriceInfo.Price;

            var longTP = pocPrice + tp;
            var longSL = pocPrice - sl;

            var shortTP = pocPrice - tp;
            var shortSL = pocPrice + sl;

            _renderSeriesLongTP[bar] = longTP;
            _renderSeriesLongSL[bar] = longSL;
            _renderSeriesShortTP[bar] = shortTP;
            _renderSeriesShortSL[bar] = shortSL;
        }
    }
}
