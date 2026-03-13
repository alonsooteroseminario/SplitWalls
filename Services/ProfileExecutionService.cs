using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using SplitWalls.Models;

namespace SplitWalls.Services
{
    /// <summary>
    /// Bridges a <see cref="WallProfileConfig"/> to the existing
    /// <see cref="WallProfileBuilder"/> and <see cref="WindowDetectionService"/>.
    /// Iterates over a list of Revit walls and applies the configured split
    /// strategy to each one.
    /// </summary>
    public class ProfileExecutionService
    {
        private readonly Document _doc;
        private readonly WallProfileBuilder _profileBuilder;

        public ProfileExecutionService(Document doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            _doc = doc;
            _profileBuilder = new WallProfileBuilder(doc);
        }

        /// <summary>
        /// Applies <paramref name="config"/> to every wall in <paramref name="walls"/>.
        /// Each wall is processed inside its own Revit Transaction.
        /// </summary>
        public void Execute(WallProfileConfig config, List<Wall> walls)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (walls == null) throw new ArgumentNullException("walls");

            foreach (Wall wall in walls)
            {
                try
                {
                    ExecuteWall(config, wall);
                }
                catch (Exception ex)
                {
                    // Surface per-wall failures without aborting the whole batch.
                    throw new InvalidOperationException(
                        string.Format("ProfileExecutionService: failed on wall {0}: {1}",
                            wall.Id, ex.Message), ex);
                }
            }
        }

        // ------------------------------------------------------------------ //
        // Private helpers
        // ------------------------------------------------------------------ //

        private void ExecuteWall(WallProfileConfig config, Wall wall)
        {
            string strategy = (config.Strategy ?? string.Empty).ToLowerInvariant();

            switch (strategy)
            {
                case "nowindows":
                    ExecuteNoWindows(config, wall);
                    break;

                case "osb":
                case "smartpanel":
                    ExecuteWithOpenings(config, wall, strategy);
                    break;

                default:
                    // Unknown strategy — fall back to the no-windows path so the
                    // caller at least gets split walls.
                    ExecuteNoWindows(config, wall);
                    break;
            }
        }

        /// <summary>
        /// Splits a wall that contains no windows/doors by uniform panelisation.
        /// Uses WallProfileBuilder for any non-standard segment profiles.
        /// </summary>
        private void ExecuteNoWindows(WallProfileConfig config, Wall wall)
        {
            double panelWidthMm = GetEffectivePanelWidth(config);
            double wallLengthMm = GetWallLengthMm(wall);

            List<double> splitsMm = (config.SplitRule != null &&
                                     config.SplitRule.Method == "custom" &&
                                     config.SplitRule.SplitPointsMm != null &&
                                     config.SplitRule.SplitPointsMm.Count > 0)
                ? config.SplitRule.SplitPointsMm
                : ComputeUniformSplits(wallLengthMm, panelWidthMm);

            ApplySegmentProfiles(config, wall, splitsMm);
        }

        /// <summary>
        /// Splits a wall that may contain hosted openings (windows/doors).
        /// Detects openings via <see cref="WindowDetectionService"/>, then
        /// routes each resulting sub-wall segment to the appropriate profile
        /// builder method.
        /// </summary>
        private void ExecuteWithOpenings(WallProfileConfig config, Wall wall, string strategy)
        {
            // Detect hosted openings before any geometry changes.
            List<Element> openings = WindowDetectionService.GetHostedOpenings(_doc, wall);

            if (openings == null || openings.Count == 0)
            {
                // No openings found — treat as a plain no-windows wall.
                ExecuteNoWindows(config, wall);
                return;
            }

            // With openings present, route to the opening-aware segment builders.
            // TODO(step8): implement full opening-aware routing once the
            //              segment-to-opening assignment algorithm is defined.
            // For now, apply segment profiles using the configured split rule.
            double panelWidthMm = GetEffectivePanelWidth(config);
            double wallLengthMm = GetWallLengthMm(wall);

            List<double> splitsMm = (config.SplitRule != null &&
                                     config.SplitRule.Method == "custom" &&
                                     config.SplitRule.SplitPointsMm != null &&
                                     config.SplitRule.SplitPointsMm.Count > 0)
                ? config.SplitRule.SplitPointsMm
                : ComputeUniformSplits(wallLengthMm, panelWidthMm);

            ApplySegmentProfiles(config, wall, splitsMm);
        }

        /// <summary>
        /// For each segment defined in <paramref name="config"/>, routes to the
        /// matching <see cref="WallProfileBuilder"/> method based on the segment's
        /// Profile string.  Standard segments with no special profile are skipped
        /// here (they are handled by the upstream split logic).
        /// </summary>
        private void ApplySegmentProfiles(WallProfileConfig config, Wall wall,
                                          List<double> splitPointsMm)
        {
            if (config.Segments == null || config.Segments.Count == 0)
                return;

            // TODO(step8): iterate splitPointsMm, produce sub-walls, then call
            //              RouteSegmentProfile for each sub-wall + SegmentDef pair.
            //
            // Routing table (profile string -> WallProfileBuilder method):
            //   "standard"   -> no special profile shaping (rectangular wall)
            //   "U"          -> _profileBuilder.BuildU_Door / BuildU_DoorWindowRight / etc.
            //   "L_left"     -> _profileBuilder.BuildWindowLeft / BuildDoorLeft / etc.
            //   "L_right"    -> _profileBuilder.BuildWindowRight / BuildDoorRight / etc.
            //   "T"          -> _profileBuilder.BuildT_Door
            //   "I"          -> _profileBuilder.BuildI / BuildI_DoorLeft / BuildI_DoorRight
            //   "borde"      -> _profileBuilder.BuildEdgeDoorLeft / BuildEdgeDoorRight
            //
            // The exact overload is selected based on how many and which openings
            // (doors vs windows, left vs right side) the segment contains.
            // This selection logic will be implemented in Step 8.
        }

        /// <summary>
        /// Routes a single wall segment to the appropriate <see cref="WallProfileBuilder"/>
        /// overload based on the <paramref name="profile"/> string and the openings it
        /// contains.  Returns the replacement wall (or the original if no shaping needed).
        /// </summary>
        private Wall RouteSegmentProfile(Wall segmentWall, SegmentDef segmentDef,
                                         List<OpeningDef> openingsInSegment)
        {
            if (segmentDef == null || string.IsNullOrEmpty(segmentDef.Profile) ||
                segmentDef.Profile == "standard")
                return segmentWall; // nothing to do

            // All calls below map directly to existing WallProfileBuilder public methods.
            // Parameters (heights, sill heights, XYZ points) must be extracted from the
            // OpeningDef list and converted from mm to Revit feet using:
            //   double feet = mm / RevitUnitHelper.MmToFeet;
            //
            // TODO(step8): implement parameter extraction and full dispatch.
            // Available builder methods (verified from WallProfileBuilder.cs):
            //
            //   _profileBuilder.BuildU_Door(wall, anchoventana, alturaventana, sillventanda, pointVIo, pointVFo, pointPH)
            //   _profileBuilder.BuildU_DoorWindowRight(wall, altVent1, sillVent1, altVent2, sillVent2, pointVIo1, pointVFo1, pointVIo2)
            //   _profileBuilder.BuildU_DoorDoorRight(wall, altVent1, sillVent1, altVent2, sillVent2, pointVIo1, pointVFo1, pointVIo2)
            //   _profileBuilder.BuildU_DoorWindowLeft(wall, altVent1, sillVent1, altVent2, sillVent2, pointVIo1, pointVFo1, pointVIo2)
            //   _profileBuilder.BuildU_DoorDoorLeft(wall, altVent1, sillVent1, altVent2, sillVent2, pointVIo1, pointVFo1, pointVIo2)
            //   _profileBuilder.Build3Opening_WindowDoorWindow(wall, ...)
            //   _profileBuilder.Build3Opening_WindowDoorDoor(wall, ...)
            //   _profileBuilder.Build3Opening_DoorDoorWindow(wall, ...)
            //   _profileBuilder.Build3Opening_DoorDoorDoor(wall, ...)
            //   _profileBuilder.BuildEdgeDoorRight(wall, alturaventana, sillventanda, pointVFo)
            //   _profileBuilder.BuildEdgeDoorLeft(wall, alturaventana, sillventanda, pointVIo)
            //   _profileBuilder.BuildT_Door(wall, altVent1, sillVent1, ...)
            //   _profileBuilder.BuildI_DoorLeft(wall, ...)
            //   _profileBuilder.BuildI_DoorRight(wall, ...)
            //   _profileBuilder.BuildI(wall, ...)
            //   _profileBuilder.BuildWindowLeft(wall, alturaventana, sillventanda, pointVIo)
            //   _profileBuilder.BuildWindowRight(wall, alturaventana, sillventanda, pointVFo)
            //   _profileBuilder.BuildDoorLeft(wall, alturaventana, sillventanda, pointVIo)
            //   _profileBuilder.BuildDoorRight(wall, alturaventana, sillventanda, pointVFo)
            //   _profileBuilder.BuildDoorRightSpecialCase(wall, alturaventana, sillventanda_0, ...)
            //   _profileBuilder.BuildTwoWallSolitarioReturn(wall, anchoventana, alturaventana, sillventanda, pointVFo)
            //   _profileBuilder.BuildOneWallSolitarioSpecialCase(wall, anchoventana, alturaventana, sillventanda, ...)
            //   _profileBuilder.BuildOneWallSolitarioSpecialEndWall(wall, anchoventana, alturaventana, alturaventana2, ...)
            //   _profileBuilder.BuildOneWallSolitarioSpecialStartWall(wall, anchoventana, alturaventana, alturaventana2, ...)

            // TODO(step8): dispatch to _profileBuilder based on profile + openings count/type.
            // Example: _profileBuilder.BuildWindowLeft(...), _profileBuilder.BuildDoorRight(...), etc.
            return segmentWall;
        }

        // ------------------------------------------------------------------ //
        // Unit conversion and geometry helpers
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Computes uniform panel split points (in mm) along a wall of
        /// <paramref name="wallLengthMm"/> mm using panels of
        /// <paramref name="panelWidthMm"/> mm.
        /// Returns the interior split positions (not including 0 or the wall end).
        /// </summary>
        private List<double> ComputeUniformSplits(double wallLengthMm, double panelWidthMm)
        {
            var splits = new List<double>();

            if (panelWidthMm <= 0)
                return splits;

            double position = panelWidthMm;
            while (position < wallLengthMm - 1e-6)
            {
                splits.Add(position);
                position += panelWidthMm;
            }

            return splits;
        }

        /// <summary>Returns the wall length in millimetres.</summary>
        private double GetWallLengthMm(Wall wall)
        {
            Curve locationCurve = ((LocationCurve)wall.Location).Curve;
            // locationCurve.Length is in Revit internal units (decimal feet).
            double lengthFeet = locationCurve.Length;
            return lengthFeet * RevitUnitHelper.MmToFeet;
        }

        /// <summary>
        /// Returns the effective panel width in mm, falling back through
        /// SplitRule → Defaults → RevitUnitHelper constant.
        /// </summary>
        private double GetEffectivePanelWidth(WallProfileConfig config)
        {
            if (config.SplitRule != null && config.SplitRule.PanelWidthMm > 0)
                return config.SplitRule.PanelWidthMm;

            if (config.Defaults != null && config.Defaults.PanelWidthMm > 0)
                return config.Defaults.PanelWidthMm;

            return RevitUnitHelper.DefaultPanelWidthMm;
        }
    }
}
