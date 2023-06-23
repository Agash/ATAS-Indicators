using ATAS.Indicators.Drawing;
using System;

namespace ATAS.Indicators.Technical
{
    internal class Aussenstab
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int FirstBar { get; set; }
        public decimal CurrentHigh { get; set; }
        public decimal CurrentLow { get; set; }
        public int? LastBar { get; set; }
        public bool Positive { get; set; }
        
        public DrawingRectangle? Rectangle { get; set; }
    }
}
