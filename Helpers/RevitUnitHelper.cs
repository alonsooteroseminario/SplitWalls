namespace SplitWalls
{
    internal static class RevitUnitHelper
    {
        /// <summary>
        /// Revit internal units are decimal feet. 1 foot = 304.8 mm.
        /// Divide mm by this to get feet; multiply feet by this to get mm.
        /// </summary>
        public const double MmToFeet = 304.8;

        /// <summary>Default panel width used when no value is entered (mm).</summary>
        public const int DefaultPanelWidthMm = 1220;

        /// <summary>Gap/separator strip width between panels (mm).</summary>
        public const int SeparatorWidthMm = 4;
    }
}
