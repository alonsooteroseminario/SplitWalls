namespace SplitWalls.Models
{
    public class OpeningDef
    {
        public int Index { get; set; }
        public double XMm { get; set; }
        public double YMm { get; set; }
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public string Type { get; set; }        // "window" | "door"
    }
}
