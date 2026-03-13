using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using SplitWalls.Models;

namespace SplitWalls.Services
{
    public class ProfileFileService
    {
        private static readonly string DefaultFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SplitWalls", "Profiles");

        public string GetDefaultFolder() => DefaultFolder;

        public void Save(WallProfileConfig config, string filePath = null)
        {
            if (filePath == null)
                filePath = Path.Combine(DefaultFolder, SanitizeFileName(config.Name) + ".txt");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(config);
            File.WriteAllText(filePath, json);
        }

        public WallProfileConfig Load(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<WallProfileConfig>(json);
        }

        public List<string> ListProfiles()
        {
            Directory.CreateDirectory(DefaultFolder);
            return new List<string>(Directory.GetFiles(DefaultFolder, "*.txt"));
        }

        public static WallProfileConfig CreateTemplate(string strategy)
        {
            return new WallProfileConfig
            {
                Name = strategy + " Template",
                Version = "2.0",
                Created = DateTime.Now.ToString("o"),
                Strategy = strategy,
                Defaults = new ProfileDefaults
                {
                    PanelWidthMm = 1220,
                    SeparatorWidthMm = 4,
                    WallHeightMm = 2440,
                    DisableWallJoins = true,
                    SnapToGridMm = 100
                },
                SplitRule = new SplitRule
                {
                    Method = "uniform",
                    PanelWidthMm = 1220,
                    SplitPointsMm = new List<double>()
                },
                Segments = new List<SegmentDef>(),
                Openings = new List<OpeningDef>()
            };
        }

        private string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "profile";
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
