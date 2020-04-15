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
            // Todo el codigo para crear los botonoes en la Ribbon. crear panel  
            RibbonPanel panel11 = application.CreateRibbonPanel("Split Walls");

            // agregar un boton
            PushButton button11 = panel11.AddItem(new PushButtonData("SplitWallsButton", "Split Walls", ExecutingAssemblyPath, "SplitWalls.ThisApplication")) as PushButton;


            // agregar la imagen al button1
            button11.LargeImage = new BitmapImage(new Uri("pack://application:,,,/SplitWalls;component/Resources/split-(2).png"));

            button11.ToolTip = "Dividir Muros";
            button11.LongDescription = "Divide los Muros o los elementos de Categoría 'Walls' en partes iguales indicándole la longitud.";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }


    }
}
