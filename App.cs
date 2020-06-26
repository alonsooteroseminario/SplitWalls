using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace SplitWalls
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class App : IExternalApplication
    {
        public static string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public Result OnStartup(UIControlledApplication application)
        {
            // Crear Tab 1 Dynoscript
            string tabName = "TecnoTruss";
            application.CreateRibbonTab(tabName);

            // Crear Panel 1
            RibbonPanel panel11 = application.CreateRibbonPanel(tabName, "Crear Paneles");


            // agregar un boton
            PushButton button11 = panel11.AddItem(new PushButtonData("SplitWallsButton", "Dividir Muros", ExecutingAssemblyPath, "SplitWalls.ThisApplication")) as PushButton;


            // agregar la imagen al button1
            button11.LargeImage = new BitmapImage(new Uri("pack://application:,,,/SplitWalls;component/Resources/split-(2).png"));

            button11.ToolTip = "Dividir Muros para fabricación de Paneles Standarizados";
            button11.LongDescription = "Ingresa el Ancho de los Paneles (mm) que deseas Dividir y después Selecciona qué muro deseas Dividir: ";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }


    }
}
