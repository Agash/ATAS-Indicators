namespace ATAS.Indicators.Technical
{
    internal class Aussenstab
    {
        public int FirstBar { get; set; }
        public decimal CurrentHigh { get; set; }
        public decimal CurrentLow { get; set; }
        public int? LastBar { get; set; }
        public bool Positive { get; set; }
        // public Rectangle? Rectangle { get; set; }
    }
}
