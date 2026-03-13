namespace SplitWalls.Models
{
    public class SegmentDef
    {
        public int Index { get; set; }
        public double StartMm { get; set; }
        public double EndMm { get; set; }
        public double WidthMm { get; set; }
        public string Profile { get; set; }     // "standard"|"U"|"L_left"|"L_right"|"T"|"I"|"borde"
        public string Label { get; set; }
        public string FireRating { get; set; }  // "none"|"1hr"|"2hr"|"3hr"
    }
}
