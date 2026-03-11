using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace SplitWalls
{
    internal static class WindowDetectionService
    {
        /// <summary>
        /// Returns all doors and windows (FamilyInstances) hosted on the given wall,
        /// searching the entire document.
        /// Must be called inside an active Transaction context (read-only is fine).
        /// </summary>
        public static List<Element> GetHostedOpenings(Document doc, Element wall)
        {
            return Collect(doc, null, wall);
        }

        /// <summary>
        /// Returns all doors and windows (FamilyInstances) hosted on the given wall,
        /// restricting the search to elements visible in the specified view.
        /// </summary>
        public static List<Element> GetHostedOpenings(Document doc, View view, Element wall)
        {
            return Collect(doc, view, wall);
        }

        private static List<Element> Collect(Document doc, View view, Element wall)
        {
            BuiltInCategory[] categories = { BuiltInCategory.OST_Doors, BuiltInCategory.OST_Windows };
            var result = new List<Element>();
            ElementId wallId = wall.Id;

            foreach (BuiltInCategory bic in categories)
            {
                ElementClassFilter classFilter = new ElementClassFilter(typeof(FamilyInstance));
                ElementCategoryFilter catFilter = new ElementCategoryFilter(bic);
                LogicalAndFilter andFilter = new LogicalAndFilter(classFilter, catFilter);

                FilteredElementCollector collector = view != null
                    ? new FilteredElementCollector(doc, view.Id)
                    : new FilteredElementCollector(doc);

                foreach (Element elem in collector.WherePasses(andFilter).ToElements())
                {
                    FamilyInstance fi = elem as FamilyInstance;
                    if (fi.Host.Id == wallId)
                        result.Add(elem);
                }
            }

            return result;
        }
    }
}
