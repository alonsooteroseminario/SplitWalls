using Autodesk.Revit.DB;

namespace SplitWalls
{
    internal static class WallJoinHelper
    {
        /// <summary>
        /// Disables wall joins at both ends of the given wall.
        /// Must be called inside an active Transaction.
        /// </summary>
        public static void DisableJoins(Wall wall)
        {
            if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
                WallUtils.DisallowWallJoinAtEnd(wall, 0);
            if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
                WallUtils.DisallowWallJoinAtEnd(wall, 1);
        }
    }
}
