using System;
using System.Collections.Generic;

namespace SplitWalls.Models
{
    public class WallProfileConfig
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Created { get; set; }
        public string Strategy { get; set; }  // "noWindows" | "osb" | "smartPanel"
        public ProfileDefaults Defaults { get; set; }
        public SplitRule SplitRule { get; set; }
        public List<SegmentDef> Segments { get; set; }
        public List<OpeningDef> Openings { get; set; }

        public WallProfileConfig()
        {
            Defaults = new ProfileDefaults();
            SplitRule = new SplitRule();
            Segments = new List<SegmentDef>();
            Openings = new List<OpeningDef>();
        }
    }
}
