using System.Collections.Generic;

namespace SplitWalls.Models
{
    public class SplitRule
    {
        public string Method { get; set; }               // "uniform" | "custom"
        public double PanelWidthMm { get; set; }
        public List<double> SplitPointsMm { get; set; }

        public SplitRule()
        {
            SplitPointsMm = new List<double>();
        }
    }
}
