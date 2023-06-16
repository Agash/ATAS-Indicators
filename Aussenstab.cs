using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAS.Indicators.Technical
{
    internal class Aussenstab
    {
        public int OpenTime { get; set; }
        public decimal CurrentHigh { get; set; }
        public decimal CurrentLow { get; set; }
        public int EndTime { get; set; }
        public bool Positive { get; set; }
        public Rectangle Box { get; set; }
    }
}
