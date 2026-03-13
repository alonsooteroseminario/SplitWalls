namespace SplitWalls.Models
{
    public class ProfileDefaults
    {
        public double PanelWidthMm { get; set; }      // 1220
        public double SeparatorWidthMm { get; set; }   // 4
        public double WallHeightMm { get; set; }       // 2440
        public bool DisableWallJoins { get; set; }     // true
        public double? SnapToGridMm { get; set; }      // 100 or null
    }
}
