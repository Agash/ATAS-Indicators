using ATAS.Indicators.Drawing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATAS.Indicators.Technical
{
    [DisplayName("QM ATR SL&TP")]
    public class QMATRSLTP : Indicator
    {
        #region Fields

        private readonly ATR _atr = new()
        {
            Period = 5
        };

        private bool _showAsTicks = true;

        private decimal _multiplier = 1.0M;
        private decimal _crv = 2.0M;

        private readonly ValueDataSeries _renderSeriesATR = new("Visualisierung ATR")
        {
            Color = DefaultColors.Blue.Convert()
        };
        private readonly ValueDataSeries _renderSeriesSL = new("Visualisierung Stop Loss")
        {
            Color = DefaultColors.Red.Convert()
        };
        private readonly ValueDataSeries _renderSeriesTP = new("Visualisierung Take Profit")
        {
            Color = DefaultColors.Green.Convert()
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

        [DisplayName("Show as Ticks")]
        public bool ShowAsTicks
        {
            get => _showAsTicks;
            set
            {
                _showAsTicks = value;
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

        public QMATRSLTP() : base(true)
        {
            Panel = IndicatorDataProvider.NewPanel;

            Add(_atr);

            DataSeries[0] = _renderSeriesATR;
            DataSeries.Add(_renderSeriesSL);
            DataSeries.Add(_renderSeriesTP);
        }

        #endregion
        protected override void OnCalculate(int bar, decimal value)
        {
            var atr = ((ValueDataSeries)_atr.DataSeries[0])[bar];
            var sl = atr * _multiplier;
            var tp = sl * _crv;

            if (_showAsTicks)
            {
                sl /= InstrumentInfo.TickSize;
                tp /= InstrumentInfo.TickSize;
            }

            _renderSeriesATR[bar] = atr;
            _renderSeriesSL[bar] = sl;
            _renderSeriesTP[bar] = tp;

        }
    }
}
