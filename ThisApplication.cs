using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Visual;
using forms = System.Windows.Forms;

namespace SplitWalls
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class ThisApplication : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			//Get application and document objects
			UIApplication uiApp = commandData.Application;
			UIDocument uidoc = uiApp.ActiveUIDocument;
			Document doc = uiApp.ActiveUIDocument.Document;
			var profileBuilder = new WallProfileBuilder(doc);
			Application app = uiApp.Application;

			
			string ruta = App.ExecutingAssemblyPath;

			// Get Active View
			View activeView = uidoc.ActiveView;

			//open ()


			BUTTON_GENERAL();


			void BUTTON_GENERAL() // Dividir Muros sin y con Ventanas, Ingresando el Valor del Ancho del Panel Deseado .
			{
				try
				{

					// form1
					double numero_final = 0;

					bool Muro_sin_Ventanas = false;
					bool Muro_OSB_con_Ventanas = false;
					bool Muro_SMART_PANEL_con_Ventanas = false;

					bool esquina_1 = false;
					bool esquina_2_otro_lado = false;

					bool _todoMuro = false;
					bool primera_ERA_VENT = false;


					using (var form = new Form1())
					{
						form.ShowDialog();

						if (form.DialogResult == forms.DialogResult.Cancel) return;

						if (form.DialogResult == forms.DialogResult.OK)
						{
							
							string numero = form.Options.AnchoPanel;

							if (numero =="")
							{
								TaskDialog.Show("Atención", "Por favor Ingresa un valor para el Ancho del Panel. No puede quedar en blanco . ");
								return;
							}
							double val = Convert.ToDouble(numero);
							numero_final = val; // double

							Muro_sin_Ventanas             = form.Options.MuroSinVentanas;
							Muro_OSB_con_Ventanas         = form.Options.MuroOsbConVentanas;
							Muro_SMART_PANEL_con_Ventanas = form.Options.MuroSmartPanelConVentanas;

							_todoMuro = form.Options.TodoMuro;
							//primera_ERA_VENT = form.checkBox_8;


							esquina_1           = form.Options.Esquina1;
							esquina_2_otro_lado = form.Options.Esquina2OtroLado;

							
						}

					}

					// close form1

					// MURO SIN VENTANAS
					int anchopanel_UI = (int)numero_final;// 1220 

					const string msgNoVent  = "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ";
					const string msgConVent = "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que tienen Ventanas. ";

					if (Muro_sin_Ventanas && _todoMuro && esquina_1)
						DispatchButton(anchopanel_UI, true, msgNoVent, e => e,
							w => Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(w as Wall, anchopanel_UI));
					else if (Muro_sin_Ventanas && _todoMuro && esquina_2_otro_lado)
						DispatchButton(anchopanel_UI, true, msgNoVent, Revision6_DYNO_DarVuelta_Muro_SinVentanas,
							w => Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(w as Wall, anchopanel_UI));
					else if (Muro_OSB_con_Ventanas && primera_ERA_VENT && esquina_1)
						DispatchButton(anchopanel_UI, false, null, e => e,
							w => Revision6_Button_2_OBS(anchopanel_UI, w));
					else if (Muro_OSB_con_Ventanas && primera_ERA_VENT && esquina_2_otro_lado)
						DispatchButton(anchopanel_UI, false, null, Revision6_DYNO_DarVuelta_Muro_ConVentanas,
							w => Revision6_Button_2_OBS(anchopanel_UI, w));
					else if (Muro_OSB_con_Ventanas && _todoMuro && esquina_1)
						DispatchButton(anchopanel_UI, true, msgNoVent, e => e,
							w => Revision6_Button_2_OBS_TODO_WALL(anchopanel_UI, w));
					else if (Muro_OSB_con_Ventanas && _todoMuro && esquina_2_otro_lado)
						DispatchButton(anchopanel_UI, true, msgNoVent, Revision6_DYNO_DarVuelta_Muro_ConVentanas,
							w => Revision6_Button_2_OBS_TODO_WALL(anchopanel_UI, w));
					else if (Muro_SMART_PANEL_con_Ventanas && primera_ERA_VENT && esquina_1)
						DispatchButton(anchopanel_UI, false, null, e => e,
							w => Revision6_Button_2_SMARTPANEL(anchopanel_UI, w));
					else if (Muro_SMART_PANEL_con_Ventanas && primera_ERA_VENT && esquina_2_otro_lado)
						DispatchButton(anchopanel_UI, false, null, Revision6_DYNO_DarVuelta_Muro_ConVentanas,
							w => Revision6_Button_2_SMARTPANEL(anchopanel_UI, w));
					else if (Muro_SMART_PANEL_con_Ventanas && _todoMuro && esquina_1)
						DispatchButton(anchopanel_UI, true, msgConVent, e => e,
							w => Revision6_Button_2_SMARTPANEL(anchopanel_UI, w));
					else if (Muro_SMART_PANEL_con_Ventanas && _todoMuro && esquina_2_otro_lado)
						DispatchButton(anchopanel_UI, true, msgConVent, Revision6_DYNO_DarVuelta_Muro_ConVentanas,
							w => Revision6_Button_2_SMARTPANEL(anchopanel_UI, w));


				}
				catch (Exception)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}

			} // Dividir Muros sin y con Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Object Selection .


			void DispatchButton(int anchopanel_UI, bool multiSelect, string pickMsg,
			                    Func<Element, Element> flip, Action<Element> strategy)
			{
				if (multiSelect)
				{
					var walls = new List<Element>();
					foreach (var r in uidoc.Selection.PickObjects(ObjectType.Element, pickMsg))
					    walls.Add(flip(doc.GetElement(r)));
					foreach (var w in walls)
					    strategy(w);
				}
				else
				{
					strategy(flip(doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element))));
				}
			}

			void Revision6_Button_2_OBS_TODO_WALL(int anchopanel_UI, Element _wall_)
			{


				List<Wall> lista_de_listas_walls = new List<Wall>();

				List<Wall> lista_wall_llegada = Revision6_Button_2_OBS_INPUT(anchopanel_UI, _wall_);

				Wall ultimo_wall = lista_wall_llegada.First();
				lista_de_listas_walls.Add(ultimo_wall);


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, lista_de_listas_walls.Last());


				if (windows_hosted.Count() == 0)
				{
					Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(ultimo_wall, anchopanel_UI);
				}

				else if (windows_hosted.Count() > 0)
				{

					int ii = 0;
					while (ii < windows_hosted.Count())
					{
						List<Wall> lista_wall_llegada_2 = Revision6_Button_2_OBS_INPUT(anchopanel_UI, lista_de_listas_walls.Last());
						Wall ultimo_wall_2 = lista_wall_llegada_2.First();
						lista_de_listas_walls.Add(ultimo_wall_2);


						List<Element> windows_hosted_2 = WindowDetectionService.GetHostedOpenings(doc, lista_de_listas_walls.Last());


						if (windows_hosted_2.Count() == 0)
						{
							Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(lista_de_listas_walls.Last(), anchopanel_UI);
							ii = windows_hosted.Count() + 1;

						}
						else if (windows_hosted_2.Count() > 0)
						{
							ii = 0;
						}

					}

				}


			}
			List<Wall> Revision6_Button_2_OBS_INPUT(int anchopanel_UI, Element _wall_)
			{
				List<Wall> lista_wall_final_retirada = new List<Wall>();

				try
				{


					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal


					List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, e);


					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(e, _anchopanel_);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{

							Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = Revision6_DYNO_PanelizarMuroInicial_OSB_1_VENTANA(e, _anchopanel_);

							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							double Ventada_dPH = lista_dPH.First();
							XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);

							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final.Add(wall);
							}


							lista_wall_final_retirada.Add(listaWalls_Final.Last());


						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{

							Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = Revision6_DYNO_PanelizarMuroInicial_OSB(e, _anchopanel_); // 1220


							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							double Ventada_dPH = lista_dPH[0];
							XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);

							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final.Add(wall);
							}


							lista_wall_final_retirada.Add(listaWalls_Final.Last());

						}


					}

				}
				catch (Exception)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				return lista_wall_final_retirada;

			}
			void Revision6_Button_2_OBS(int anchopanel_UI, Element _wall_)
			{
				try
				{


					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal


					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);

					IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
					List<Element> windows_hosted = new List<Element>();

					foreach (Element elem in windows)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == e.Id)
						{
							windows_hosted.Add(elem);
						}
					}


					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						int _anchopanel_2 = _anchopanel_ + 4;
						Revision6_DYNO_DividirMuroSinVentana(_wall_, _anchopanel_2);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{

							Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = Revision6_DYNO_PanelizarMuroInicial_OSB_1_VENTANA(e, _anchopanel_);

							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							double Ventada_dPH = lista_dPH.First();
							XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);

							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final.Add(wall);
							}


							BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo.First());
							FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();

							BoundingBoxContainsPointFilter filter_dVFo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo.First());
							FilteredElementCollector collector_dVFo = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dVFo = collector_dVFo.OfClass(typeof(Wall)).WherePasses(filter_dVFo).ToElements();

							BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
							FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();


							foreach (Wall wall_i in elements_dPH) // Elementos que contienen el punto dVIo
							{
								profileBuilder.BuildSolitario(wall_i, alturaventanas.First(), sillventanas.First(), Punto_Ventada_dPH);

							}

							if (anchoventanas.First() > 1220 / RevitUnitHelper.MmToFeet)
							{
								foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
								{

									profileBuilder.BuildSolitario(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

								}
							}

						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{

							Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = Revision6_DYNO_PanelizarMuroInicial_OSB(e, _anchopanel_); // 1220


							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							double Ventada_dPH = lista_dPH[0];
							XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);

							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final.Add(wall);
							}


							BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[0]);
							FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();

							BoundingBoxContainsPointFilter filter_dVFo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[0]);
							FilteredElementCollector collector_dVFo = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dVFo = collector_dVFo.OfClass(typeof(Wall)).WherePasses(filter_dVFo).ToElements();

							BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
							FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dVIo
							IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();


							foreach (Wall wall_i in elements_dPH) // Elementos que contienen el punto dVIo
							{
								profileBuilder.BuildSolitario(wall_i, alturaventanas[0], sillventanas[0], Punto_Ventada_dPH);

							}

							if (anchoventanas.First() > 1220 / RevitUnitHelper.MmToFeet)
							{
								foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
								{

									profileBuilder.BuildSolitario(wall_ii, alturaventanas[0], sillventanas[0], Puntos_Ventada_dVFo[0]);

								}
							}

						}


					}

				}
				catch (Exception)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}

			}


			void Revision6_Button_2_SMARTPANEL(int anchopanel_UI, Element _wall_)
			{
				try
				{


					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal

					Parameter height = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					double height_double = height.AsDouble();

					Parameter lenght = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
					double lenght_double = lenght.AsDouble();


					List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, activeView, e);


					Curve wallCurve = ((LocationCurve)e.Location).Curve;
					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					// Crear lista_d


					double distanta_total_wall = endParam - stParam;

					double alpha = distanta_total_wall % (_anchopanel_ / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

					double numero_paneles_total = (distanta_total_wall - alpha) / (_anchopanel_ / RevitUnitHelper.MmToFeet); // 7
					int numero_paneles_total_int = (int)numero_paneles_total; // 7

					List<double> lista_d = new List<double>();
					List<double> lista_d_muro4 = new List<double>();


					for (int i = 0; i < numero_paneles_total_int; i++)
					{
						lista_d.Add(stParam + ((_anchopanel_ / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
					}
					for (int i = 0; i < numero_paneles_total_int; i++)
					{
						lista_d_muro4.Add(stParam + ((_anchopanel_ / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
					}


					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(e, _anchopanel_);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{

							//Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_1_VENTANA(e, _anchopanel_); // 1220


							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							double Ventada_dPH = lista_dPH.First();
							XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);
							XYZ Nuevo_Punto_Ventada_dPH = new XYZ(Punto_Ventada_dPH.X, Punto_Ventada_dPH.Y, sillventanas.First());


							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final.Add(wall);
							}

							Wall ultimo_ultimo_wall = listaWalls_Final.Last();

							int n_paneles = numero_paneles_total_int;

							Wall primer_primero_wall = listaWalls_Final.First();

							List<ElementId> lista_NO_VIo = new List<ElementId>();

							IList<Wall> lista_walls = new List<Wall>();


							double alpho = anchoventanas.First() % (_anchopanel_ / RevitUnitHelper.MmToFeet);

							double numero = (anchoventanas.First() - alpho) / (_anchopanel_ / RevitUnitHelper.MmToFeet);
							int numero_int = (int)numero;

							double mitad = Ventada_dPH;
							double dVIo = Ventada_dPH - anchoventanas.First() / 2;
							double dVFo = Ventada_dPH + anchoventanas.First() / 2;

							double mitad_A = (Ventada_dPH + dVIo) / 2;
							double mitad_A_VIo = (mitad_A + dVIo) / 2;

							double mitad_B = (Ventada_dPH + dVFo) / 2;
							double mitad_B_VFo = (mitad_B + dVFo) / 2;


							XYZ Punto_mitad = wallCurve.Evaluate(mitad, false);
							XYZ Punto_dVIo = wallCurve.Evaluate(dVIo, false);
							XYZ Punto_dVFo = wallCurve.Evaluate(dVFo, false);

							XYZ Punto_mitad_A = wallCurve.Evaluate(mitad_A, false);
							XYZ Punto_mitad_A_VIo = wallCurve.Evaluate(mitad_A_VIo, false);

							XYZ Punto_mitad_B = wallCurve.Evaluate(mitad_B, false);
							XYZ Punto_mitad_B_VFo = wallCurve.Evaluate(mitad_B_VFo, false);

							BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
							FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dPH
							IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();


							bool bool_dVIo = false;
							BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo.First());
							FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
							IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();
							//TaskDialog.Show("ALERTA !elements_dVIo.Any()", elements_dVIo.Any().ToString());
							if (!elements_dVIo.Any())
							{
								double VIo = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);

								if (VIo == 0) // 1er i por Primera ventana
								{
									//TaskDialog.Show("ALERTA", "BORDE INICIAL");
									elements_dVIo.Add(primer_primero_wall);
								}
								else
								{

									//TaskDialog.Show("ALERTA", "VENTANA EXACTA");
									elements_dVIo.Add(elements_dPH.First());
									bool_dVIo = true;
								}

							}
							bool activador = false;
							bool bool_dVFo = false;
							BoundingBoxContainsPointFilter filter_dVFo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo.First());
							FilteredElementCollector collector_dVFo = new FilteredElementCollector(doc, activeView.Id);
							IList<Element> elements_dVFo = collector_dVFo.OfClass(typeof(Wall)).WherePasses(filter_dVFo).ToElements();


							if (!elements_dVFo.Any()) // BORDE FINAL
							{
								double VIo = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
								double VFo = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);
								double n_paneles_VF = Math.Round((VFo * n_paneles) / lenght_double, 0);

								double lenght_double_VF = Math.Round(n_paneles_VF * _anchopanel_ + (n_paneles_VF - 1) * (4 / RevitUnitHelper.MmToFeet), 0);
								double diff = lenght_double_VF - VIo;

								Curve wallCurve_actual = ((LocationCurve)elements_dVIo.First().Location).Curve;
								double stParam_actual = wallCurve_actual.GetEndParameter(0);
								double endParam_actual = wallCurve_actual.GetEndParameter(1);

								double diff2 = endParam_actual - stParam_actual;

								if (n_paneles_VF == n_paneles) // Ultimo i por ultima ventana
								{
									//TaskDialog.Show("ALERTA", "BORDE FINAL");
									elements_dVFo.Add(ultimo_ultimo_wall);
								}
								else
								{
									if ((anchoventanas.First() == _anchopanel_ / RevitUnitHelper.MmToFeet) || (anchoventanas.First() > _anchopanel_ / RevitUnitHelper.MmToFeet))
									{
										//TaskDialog.Show("ALERTA", "VENTANA EXACTA activador" + Environment.NewLine + (  anchoventanas[i] > _anchopanel_ ).ToString());
										activador = true; // VENTANA EXACTA
									}


									elements_dVFo.Add(elements_dPH.First());
									bool_dVFo = true;

								}
							}


							if (elements_dVIo.First().Id.ToString() == elements_dVFo.First().Id.ToString())
							{

								if (bool_dVFo || bool_dVIo)
								{
									if (!activador)
									{
										Wall targetWall = elements_dVIo.First() as Wall;
										if (sillventanas.First() == 0) // Es puerta
										{

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 1);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 1);

											if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
											{
												Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
												lista_walls.Add(wall_recibida);
											}
											else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
											{
												Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(targetWall, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
												lista_walls.Add(wall_recibida);
											}
											else
											{
												if (!(lista_NO_VIo.Contains(targetWall.Id)))
												{
													Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), Punto_Ventada_dPH);
													lista_walls.Add(wall_recibida);
												}
											}

										}
										else
										{
											if (!(lista_NO_VIo.Contains(targetWall.Id)))
											{
												using (Transaction trans = new Transaction(doc, "wall"))
												{
													trans.Start();

													XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo.First().X, Puntos_Ventada_dVFo.First().Y, Puntos_Ventada_dVFo.First().Z + alturaventanas.First());
													XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo.First().X, Puntos_Ventada_dVIo.First().Y, Puntos_Ventada_dVIo.First().Z);

													Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

													WallJoinHelper.DisableJoins(targetWall);
													trans.Commit();

													lista_walls.Add(targetWall);
												}
											}
										}

									}
									else
									{
										Wall targetWall = elements_dVIo.First() as Wall;
										if (!(lista_NO_VIo.Contains(targetWall.Id)))
										{

											profileBuilder.BuildTwoWallSolitario(targetWall, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

										}
									}
								}
								else
								{
									Wall targetWall = elements_dVIo.First() as Wall;
									if (sillventanas.First() == 0) // Es puerta
									{

										double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 1);
										double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 1);

										if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
										{
											Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
											lista_walls.Add(wall_recibida);
										}
										else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
										{
											Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(targetWall, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
											lista_walls.Add(wall_recibida);
										}
										else
										{
											if (!(lista_NO_VIo.Contains(targetWall.Id)))
											{
												Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), Punto_Ventada_dPH);
												lista_walls.Add(wall_recibida);
											}
										}


									}
									else
									{
										if (!(lista_NO_VIo.Contains(targetWall.Id)))
										{
											using (Transaction trans = new Transaction(doc, "wall"))
											{
												trans.Start();

												XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo.First().X, Puntos_Ventada_dVFo.First().Y, Puntos_Ventada_dVFo.First().Z + alturaventanas.First());
												XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo.First().X, Puntos_Ventada_dVIo.First().Y, Puntos_Ventada_dVIo.First().Z);

												Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

												WallJoinHelper.DisableJoins(targetWall);
												trans.Commit();

												lista_walls.Add(targetWall);
											}
										}
									}

								}


							}
							else
							{

								//								List<Wall> lista_walls = new List<Wall>();


								if (!(elements_dVIo.First().Id == elements_dPH.First().Id))
								{
									if (!(elements_dVFo.First().Id == elements_dPH.First().Id))
									{


										foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
										{
											if (sillventanas.First() == 0)
											{
												profileBuilder.BuildDoorLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
											}
											else
											{

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);
												Wall wallPanel = wall_i as Wall;
												if ((VI_nuf == 0)) // BORDE Inicio
												{
													if (wallPanel.Id == primer_primero_wall.Id)
													{
														//TaskDialog.Show("ALERTA", "DVIo");
														Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
														lista_walls.Add(wall_recibida_dVIo);
													}
												}
												else
												{

													Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
											}

										}


										foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
										{
											if (sillventanas.First() == 0)
											{

												Wall wallPanel = wall_ii as Wall;

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

												if (VF_nuf == Math.Round(lenght_double, 2))
												{
													// Transf. BORDE PUERTA a la DERECHA

													if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
													{
														profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													}

												}
												else
												{

													profileBuilder.BuildDoorRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

												}

											}
											else
											{

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

												Wall wallPanel = wall_ii as Wall;

												if ((VF_nuf == Math.Round(lenght_double, 2))) // BORDE Final
												{
													if (wallPanel.Id == ultimo_ultimo_wall.Id)
													{
														//TaskDialog.Show("ALERTA", "DVFo BORDE Final");
														Wall wall_recibida_dVFo = profileBuilder.BuildTwoWallSolitarioReturn(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
														lista_walls.Add(wall_recibida_dVFo);
													}

												}
												else
												{
													//TaskDialog.Show("ALERTA", "DVFo");
													Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
											}

										}


										foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
										{
											//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
											profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
										}


									}
									else
									{

										foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
										{
											if (sillventanas.First() == 0)
											{
												profileBuilder.BuildDoorLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
											}
											else
											{
												Wall wallPanel = wall_i as Wall;
												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

												if ((VI_nuf == 0)) // BORDE Inicio
												{
													if (wallPanel.Id == primer_primero_wall.Id)
													{
														//TaskDialog.Show("ALERTA", "DVIo");
														Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
														lista_walls.Add(wall_recibida_dVIo);
													}
												}
												else
												{

													Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
											}
										}


										foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
										{
											if (sillventanas.First() == 0)
											{

												Wall wallPanel = wall_ii as Wall;

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

												if (VF_nuf == Math.Round(lenght_double, 2))
												{
													// Transf. BORDE PUERTA a la DERECHA

													if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
													{
														profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													}

												}
												else
												{
													profileBuilder.BuildDoorRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

												}

											}
											else
											{
												Wall wallPanel = wall_ii as Wall;
												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

												if ((VF_nuf == Math.Round(lenght_double, 2))) // BORDE Final
												{
													if (wallPanel.Id == ultimo_ultimo_wall.Id)
													{
														//TaskDialog.Show("ALERTA", "DVFo BORDE Final");
														Wall wall_recibida_dVFo = profileBuilder.BuildTwoWallSolitarioReturn(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
														lista_walls.Add(wall_recibida_dVFo);
													}
												}
												else
												{
													//TaskDialog.Show("ALERTA", "DVFo");
													Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
											}
										}

									}
								}
								else
								{


									foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
									{
										if (sillventanas.First() == 0)
										{
											profileBuilder.BuildDoorLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
										}
										else
										{
											Wall wallPanel = wall_i as Wall;
											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

											if ((VI_nuf == 0)) // BORDE Inicio
											{
												if (wallPanel.Id == primer_primero_wall.Id)
												{
													//TaskDialog.Show("ALERTA", "DVIo");
													Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
											}
											else
											{

												Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
												lista_walls.Add(wall_recibida_dVIo);
											}
										}
									}


									foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
									{
										if (sillventanas.First() == 0)
										{

											Wall wallPanel = wall_ii as Wall;

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

											if (VF_nuf == Math.Round(lenght_double, 2))
											{
												// Transf. BORDE PUERTA a la DERECHA

												if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
												{
													profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
												}

											}
											else
											{
												profileBuilder.BuildDoorRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

											}

										}
										else
										{
											Wall wallPanel = wall_ii as Wall;
											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 2);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 2);

											if ((VF_nuf == Math.Round(lenght_double, 2))) // BORDE Final
											{
												if (wallPanel.Id == ultimo_ultimo_wall.Id)
												{
													//TaskDialog.Show("ALERTA", "DVFo BORDE Final");
													Wall wall_recibida_dVFo = profileBuilder.BuildTwoWallSolitarioReturn(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
											}
											else
											{
												//TaskDialog.Show("ALERTA", "DVFo");
												Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
												lista_walls.Add(wall_recibida_dVFo);
											}
										}
									}

								}


							}

						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{

							//Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> tupla = Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL(e, _anchopanel_); // 1220

							List<List<Wall>> aa = tupla.Item1; // Todos los Walls creados en el paso anterior

							List<Wall> listaWalls_Final_siCambia = aa[0];
							List<Wall> listaWalls_Final_noCambia = aa[1];


							List<List<XYZ>> bb = tupla.Item2; // Lista de puntos de dVIo y dVFo

							List<XYZ> Puntos_Ventada_dVIo = bb[0]; // Puntos dVI
							List<XYZ> Puntos_Ventada_dVFo = bb[1]; // Puntos DVF

							double cuenta = Puntos_Ventada_dVIo.Count();


							List<List<double>> cc = tupla.Item3; // lista con datos de ventanda 1. Ancho ventana, 2. Altura ventana, 3. Sill Ventana

							List<double> anchoventanas = cc[0];
							List<double> alturaventanas = cc[1];
							List<double> sillventanas = cc[2];
							List<double> lista_dPH = cc[3];

							IList<ElementId> listaWalls_Final_ID = new List<ElementId>();
							List<string> listaWalls_Final_ID_string = new List<string>();
							List<BoundingBoxXYZ> dd = tupla.Item4; // BOunding BOX de las Ventanas


							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								//listaWalls_Final.Add(wall);
							}


							foreach (Wall wall in listaWalls_Final) // No cambia nada se mantiene el mismo Muro
							{
								listaWalls_Final_ID.Add(wall.Id);
								listaWalls_Final_ID_string.Add(wall.Id.ToString());
							}

							Wall ultimo_ultimo_wall = listaWalls_Final.Last();

							Wall primer_primero_wall = listaWalls_Final.First();

							List<ElementId> lista_NO_VIo = new List<ElementId>();

							IList<Wall> lista_walls = new List<Wall>();

							int n_paneles = numero_paneles_total_int;

							for (int i = 0; i < anchoventanas.Count(); i++) // numero de Ventanas
							{


								double Ventada_dPH = lista_dPH[i];
								XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);
								XYZ Nuevo_Punto_Ventada_dPH = new XYZ(Punto_Ventada_dPH.X, Punto_Ventada_dPH.Y, sillventanas[i]);


								double alpho = anchoventanas[i] % (_anchopanel_ / RevitUnitHelper.MmToFeet);

								double numero = (anchoventanas[i] - alpho) / (_anchopanel_ / RevitUnitHelper.MmToFeet);
								int numero_int = (int)numero;

								double mitad = Ventada_dPH;
								double dVIo = Ventada_dPH - anchoventanas[i] / 2;
								double dVFo = Ventada_dPH + anchoventanas[i] / 2;

								double mitad_A = (Ventada_dPH + dVIo) / 2;
								double mitad_A_VIo = (mitad_A + dVIo) / 2;

								double mitad_B = (Ventada_dPH + dVFo) / 2;
								double mitad_B_VFo = (mitad_B + dVFo) / 2;


								XYZ Punto_mitad = wallCurve.Evaluate(mitad, false);
								XYZ Punto_dVIo = wallCurve.Evaluate(dVIo, false);
								XYZ Punto_dVFo = wallCurve.Evaluate(dVFo, false);

								XYZ Punto_mitad_A = wallCurve.Evaluate(mitad_A, false);
								XYZ Punto_mitad_A_VIo = wallCurve.Evaluate(mitad_A_VIo, false);

								XYZ Punto_mitad_B = wallCurve.Evaluate(mitad_B, false);
								XYZ Punto_mitad_B_VFo = wallCurve.Evaluate(mitad_B_VFo, false);


								BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
								FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
								IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();


								bool bool_dVIo = false;
								BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i]);
								FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
								IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();
								//TaskDialog.Show("ALERTA !elements_dVIo.Any()", elements_dVIo.Any().ToString());
								if (!elements_dVIo.Any())
								{
									double VIo = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);

									if (VIo == 0) // 1er i por Primera ventana
									{
										//TaskDialog.Show("ALERTA", "BORDE INICIAL");
										elements_dVIo.Add(primer_primero_wall);
									}
									else
									{

										//TaskDialog.Show("ALERTA", "VENTANA EXACTA");
										elements_dVIo.Add(elements_dPH.First());
										bool_dVIo = true;
									}

								}
								bool activador = false;
								bool bool_dVFo = false;
								BoundingBoxContainsPointFilter filter_dVFo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i]);
								FilteredElementCollector collector_dVFo = new FilteredElementCollector(doc, activeView.Id);
								IList<Element> elements_dVFo = collector_dVFo.OfClass(typeof(Wall)).WherePasses(filter_dVFo).ToElements();


								if (!elements_dVFo.Any()) // BORDE FINAL
								{
									double VIo = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
									double VFo = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);
									double n_paneles_VF = Math.Round((VFo * n_paneles) / lenght_double, 0);

									double lenght_double_VF = Math.Round(n_paneles_VF * _anchopanel_ + (n_paneles_VF - 1) * (4 / RevitUnitHelper.MmToFeet), 0);
									double diff = lenght_double_VF - VIo;

									Curve wallCurve_actual = ((LocationCurve)elements_dVIo.First().Location).Curve;
									double stParam_actual = wallCurve_actual.GetEndParameter(0);
									double endParam_actual = wallCurve_actual.GetEndParameter(1);

									double diff2 = endParam_actual - stParam_actual;

									if (n_paneles_VF == n_paneles) // Ultimo i por ultima ventana
									{
										//TaskDialog.Show("ALERTA", "BORDE FINAL");
										elements_dVFo.Add(ultimo_ultimo_wall);
									}
									else
									{
										if ((anchoventanas[i] == _anchopanel_ / RevitUnitHelper.MmToFeet) || (anchoventanas[i] > _anchopanel_ / RevitUnitHelper.MmToFeet))
										{
											//TaskDialog.Show("ALERTA", "VENTANA EXACTA activador" + Environment.NewLine + (  anchoventanas[i] > _anchopanel_ ).ToString());
											activador = true; // VENTANA EXACTA
										}


										elements_dVFo.Add(elements_dPH.First());
										bool_dVFo = true;

									}
								}


								if (elements_dVIo.First().Id == elements_dVFo.First().Id) // si VIo y VFo estan en el mismo WALL
								{

									if (i < (cuenta - 2))
									{


										Outline outline_2 = new Outline(dd[i + 2].Min, dd[i + 2].Max);
										BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);
										FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
										IList<Element> elements_outline_2 = coll_outline_2.WherePasses(bbfilter_2).ToElements();

										bool boolean_2 = false;

										for (int n = 0; n < elements_outline_2.Count(); n++)
										{
											if (elements_outline_2[n].Id == elements_dVIo.First().Id)
											{
												boolean_2 = true;
												n = n + coll_outline_2.Count();
											}
										}

										if (boolean_2)
										{
											if (sillventanas[i + 1] == 0) // Puerta 
											{
												if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
												{
													// PUERTA - PUERTA -PUERTAmed
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
												{
													// PUERTA - PUERTA -VENTANAmed
												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
												{
													// VENTANA - PUERTA -PUERTAmed
												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
												{
													// VENTANA - PUERTA -VENTANAmed
												}
											}
											else if (sillventanas[i + 1] > 0) // Ventana
											{
												if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
												{
													// PUERTA - VENTANA -PUERTAmed
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
												{
													// PUERTA - VENTANA -VENTANAmed
												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
												{
													// VENTANA - VENTANA -PUERTAmed
												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
												{
													// VENTANA - VENTANA -VENTANAmed
												}
											}


											i = i + 1;
										}
										else
										{

											BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
											FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
											// Elementos que contienen el punto dVIo
											IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

											BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
											FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
											IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


											bool boolean1_VI = false;
											bool boolean1_VF = false;

											for (int n = 0; n < elements_dVIo_2.Count(); n++)
											{
												if (elements_dVIo_2[n].Id == elements_dVIo.First().Id)
												{
													boolean1_VI = true;
													n = n + elements_dVIo_2.Count();
												}
											}
											for (int n = 0; n < elements_dVFo_2.Count(); n++)
											{
												if (elements_dVFo_2[n].Id == elements_dVIo.First().Id)
												{
													boolean1_VF = true;
													n = n + elements_dVFo_2.Count();
												}
											}

											if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
											{

												if (sillventanas[i] == 0 && sillventanas[i + 1] == 0)
												{
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] == 0 && sillventanas[i + 1] > 0)
												{
													//Puerta - Ventana
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													double VI_nuf2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
													double VF_nuf2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);


													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														if (VF_nuf == VI_nuf2)
														{

															Wall wall_recibida = profileBuilder.BuildOneWallSolitarioSpecialStartWall(wallPanel, anchoventanas[i], alturaventanas[i], alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															// Transf. BORDE PUERTA a la IZQUIERDA
															Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] == 0)
												{
													//Ventana - Puerta
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA

													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i + 1], anchoventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i + 1], alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] > 0)
												{

													Wall wallPanel = elements_dVIo.First() as Wall;

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wallPanel.Document.Create.NewOpening(wallPanel, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wallPanel);
														trans.Commit();


													}
													using (Transaction trans = new Transaction(doc, "wall2"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

														Opening openin = wallPanel.Document.Create.NewOpening(wallPanel, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wallPanel);
														trans.Commit();


													}

													lista_NO_VIo.Add(wallPanel.Id);
												}
												i = i + 1;
											}
											else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
											{
												if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // Ventana - Puerta
												{


													Wall wallPanel = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wall_recibida_dVIo);
														trans.Commit();


													}

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);

												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // Ventana - Ventana
												{

													Wall wallPanel = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wall_recibida_dVIo);
														trans.Commit();


													}

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados Puerta - Puerta
												{
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																										 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorRight(wallPanel, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo Puerta - Ventana
												{

													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);


													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{

														double VI_nuf_2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf_2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);
														// if
														if (VF_nuf == VI_nuf_2)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialCase(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i],
																														sillventanas[i + 1], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowRight(wallPanel, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}


													}
												}

											}
											else if (!boolean1_VI && !boolean1_VF) // no choca con nada
											{
												if (bool_dVFo || bool_dVIo)
												{
													if (!activador)
													{
														Wall targetWall = elements_dVIo.First() as Wall;
														if (sillventanas[i] == 0) // Es puerta
														{

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																lista_walls.Add(wall_recibida);
															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

																lista_walls.Add(wall_recibida);
															}
															else
															{
																if (!(lista_NO_VIo.Contains(targetWall.Id)))
																{
																	Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																	lista_walls.Add(wall_recibida);
																}
															}


														}
														else
														{

															if (!(lista_NO_VIo.Contains(targetWall.Id)))
															{
																using (Transaction trans = new Transaction(doc, "wall"))
																{
																	trans.Start();

																	XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																	XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																	Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

																	WallJoinHelper.DisableJoins(targetWall);
																	trans.Commit();

																	lista_walls.Add(targetWall);
																}
															}
														}

													}
													else
													{
														Wall targetWall = elements_dVIo.First() as Wall;
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{

															profileBuilder.BuildTwoWallSolitario(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														}
													}


												}
												else
												{
													Wall targetWall = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{
															Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}
														else
														{
															if (!(lista_NO_VIo.Contains(targetWall.Id)))
															{
																Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}


													}
													else
													{
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(targetWall);
																trans.Commit();

																lista_walls.Add(targetWall);
															}
														}
													}

												}
											}
										}
									}
									else
									{
										if (i < (cuenta - 1))
										{

											BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
											FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
											// Elementos que contienen el punto dVIo
											IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

											BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
											FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
											IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


											bool boolean1_VI = false;
											bool boolean1_VF = false;

											for (int n = 0; n < elements_dVIo_2.Count(); n++)
											{
												if (elements_dVIo_2[n].Id == elements_dVIo.First().Id)
												{
													boolean1_VI = true;
													n = n + elements_dVIo_2.Count();
												}
											}
											for (int n = 0; n < elements_dVFo_2.Count(); n++)
											{
												if (elements_dVFo_2[n].Id == elements_dVIo.First().Id)
												{
													boolean1_VF = true;
													n = n + elements_dVFo_2.Count();
												}
											}

											if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
											{

												if (sillventanas[i] == 0 && sillventanas[i + 1] == 0)
												{
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] == 0 && sillventanas[i + 1] > 0)
												{
													//Puerta - Ventana
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													double VI_nuf2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
													double VF_nuf2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);


													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{

														if (VF_nuf == VI_nuf2)
														{

															Wall wall_recibida = profileBuilder.BuildOneWallSolitarioSpecialStartWall(wallPanel, anchoventanas[i], alturaventanas[i], alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															// Transf. BORDE PUERTA a la IZQUIERDA
															Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] == 0)
												{
													//Ventana - Puerta
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA

													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = profileBuilder.BuildEdgeDoorRight(wallPanel, alturaventanas[i + 1], anchoventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(wallPanel.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(wallPanel, anchoventanas[i + 1], alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], Punto_Ventada_dPH);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] > 0)
												{

													Wall wallPanel = elements_dVIo.First() as Wall;

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wallPanel.Document.Create.NewOpening(wallPanel, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wallPanel);
														trans.Commit();


													}
													using (Transaction trans = new Transaction(doc, "wall2"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

														Opening openin = wallPanel.Document.Create.NewOpening(wallPanel, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wallPanel);
														trans.Commit();


													}

													lista_NO_VIo.Add(wallPanel.Id);
												}
												i = i + 1;
											}
											else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
											{
												if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // Ventana - Puerta
												{


													Wall wallPanel = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wall_recibida_dVIo);
														trans.Commit();


													}

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);

												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // Ventana - Ventana
												{

													Wall wallPanel = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														WallJoinHelper.DisableJoins(wall_recibida_dVIo);
														trans.Commit();


													}

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados Puerta - Puerta
												{
													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																										 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorRight(wallPanel, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo Puerta - Ventana
												{

													Wall wallPanel = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														double VI_nuf_2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf_2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);
														// if
														if (VF_nuf == VI_nuf_2)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialCase(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i],
																														sillventanas[i + 1], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowRight(wallPanel, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
													}
												}

											}
											else if (!boolean1_VI && !boolean1_VF) // no choca con nada
											{
												if (bool_dVFo || bool_dVIo)
												{
													if (!activador)
													{
														Wall targetWall = elements_dVIo.First() as Wall;
														if (sillventanas[i] == 0) // Es puerta
														{

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																lista_walls.Add(wall_recibida);
															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

																lista_walls.Add(wall_recibida);
															}
															else
															{
																if (!(lista_NO_VIo.Contains(targetWall.Id)))
																{
																	Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																	lista_walls.Add(wall_recibida);
																}
															}


														}
														else
														{
															if (!(lista_NO_VIo.Contains(targetWall.Id)))
															{
																using (Transaction trans = new Transaction(doc, "wall"))
																{
																	trans.Start();

																	XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																	XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																	Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

																	WallJoinHelper.DisableJoins(targetWall);
																	trans.Commit();

																	lista_walls.Add(targetWall);
																}
															}
														}

													}
													else
													{
														Wall targetWall = elements_dVIo.First() as Wall;
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{

															profileBuilder.BuildTwoWallSolitario(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														}
													}
												}
												else
												{
													Wall targetWall = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{
															Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}
														else
														{
															if (!(lista_NO_VIo.Contains(targetWall.Id)))
															{
																Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}


													}
													else
													{
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(targetWall);
																trans.Commit();

																lista_walls.Add(targetWall);
															}
														}
													}

												}
											}
										}
										else // no choca con nada
										{
											if (bool_dVFo || bool_dVIo)
											{
												if (!activador)
												{
													Wall targetWall = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{
															if (targetWall.Id == ultimo_ultimo_wall.Id)
															{
																Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

																lista_walls.Add(wall_recibida);
															}

														}
														else
														{
															if (!(lista_NO_VIo.Contains(targetWall.Id)))
															{
																Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}


													}
													else
													{
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(targetWall);
																trans.Commit();

																lista_walls.Add(targetWall);
															}
														}
													}

												}
												else
												{
													Wall targetWall = elements_dVIo.First() as Wall;
													if (!(lista_NO_VIo.Contains(targetWall.Id)))
													{

														profileBuilder.BuildTwoWallSolitario(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

													}
												}
											}
											else
											{
												Wall targetWall = elements_dVIo.First() as Wall;
												if (sillventanas[i] == 0) // Es puerta
												{

													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
													{
														Wall wall_recibida = profileBuilder.BuildEdgeDoorLeft(targetWall, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														lista_walls.Add(wall_recibida);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
													{
														if (targetWall.Id == ultimo_ultimo_wall.Id)
														{
															Wall wall_recibida = profileBuilder.BuildDoorLeft(targetWall, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}

													}
													else
													{
														if (!(lista_NO_VIo.Contains(targetWall.Id)))
														{
															Wall wall_recibida = profileBuilder.BuildU_Door(targetWall, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
															lista_walls.Add(wall_recibida);
														}
													}


												}
												else
												{
													if (!(lista_NO_VIo.Contains(targetWall.Id)))
													{
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = targetWall.Document.Create.NewOpening(targetWall, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(targetWall);
															trans.Commit();

															lista_walls.Add(targetWall);
														}
													}
												}

											}
										}
									}

								}
								else // si VIo y VFo NO estan en el mismo WALL
								{


									if (!(elements_dVIo.First().Id == elements_dPH.First().Id))
									{
										if (!(elements_dVFo.First().Id == elements_dPH.First().Id))
										{


											if (i < (cuenta - 2))
											{


												Outline outline_2 = new Outline(dd[i + 2].Min, dd[i + 2].Max);
												BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);
												FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
												IList<Element> elements_outline_2 = coll_outline_2.WherePasses(bbfilter_2).ToElements();

												bool boolean_2 = false;

												for (int n = 0; n < elements_outline_2.Count(); n++)
												{
													if (elements_outline_2[n].Id == elements_dVIo.First().Id)
													{
														boolean_2 = true;
														n = n + coll_outline_2.Count();
													}
												}

												if (boolean_2)
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}

													foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
													{
														//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													}


													if ((sillventanas[i + 1] == 0)) // en medio puerta
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
													}
													else if ((sillventanas[i + 1] > 0)) // en medio ventana
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 2], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}


													i = i + 1;
												}
												else
												{

													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


													bool boolean1_VI = false;
													bool boolean1_VF = false;

													for (int n = 0; n < elements_dVIo_2.Count(); n++)
													{
														if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VI = true;
															n = n + elements_dVIo_2.Count();
														}
													}
													for (int n = 0; n < elements_dVFo_2.Count(); n++)
													{
														if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VF = true;
															n = n + elements_dVFo_2.Count();
														}
													}

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1); // Puerta
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);  // Ventana
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{

																if (VF_nuf_1 == VI_nuf)
																{

																	Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}


														}


														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if (sillventanas[i] == 0)
														{
															Wall wallPanel = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if (VF_nuf == Math.Round(lenght_double, 1))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}

													}
												}
											}
											else
											{
												if (i < (cuenta - 1))
												{

													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


													bool boolean1_VI = false;
													bool boolean1_VF = false;

													for (int n = 0; n < elements_dVIo_2.Count(); n++)
													{
														if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VI = true;
															n = n + elements_dVIo_2.Count();
														}
													}
													for (int n = 0; n < elements_dVFo_2.Count(); n++)
													{
														if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VF = true;
															n = n + elements_dVFo_2.Count();
														}
													}

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{

																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}


														}


														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}


														if (sillventanas[i] == 0)
														{
															Wall wallPanel = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if (VF_nuf == Math.Round(lenght_double, 1))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}

													}
												}
												else // no choca con nada
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}

													foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
													{
														//profileBuilder.BuildSolitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														profileBuilder.BuildTwoWallSolitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													}


													if (sillventanas[i] == 0)
													{
														Wall wallPanel = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if (VF_nuf == Math.Round(lenght_double, 1))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}

												}
											}


										}
										else
										{


											if (i < (cuenta - 2))
											{


												Outline outline_2 = new Outline(dd[i + 2].Min, dd[i + 2].Max);
												BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);
												FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
												IList<Element> elements_outline_2 = coll_outline_2.WherePasses(bbfilter_2).ToElements();

												bool boolean_2 = false;

												for (int n = 0; n < elements_outline_2.Count(); n++)
												{
													if (elements_outline_2[n].Id == elements_dVIo.First().Id)
													{
														boolean_2 = true;
														n = n + coll_outline_2.Count();
													}
												}

												if (boolean_2)
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}


													}


													if ((sillventanas[i + 1] == 0)) // en medio puerta
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
													}
													else if ((sillventanas[i + 1] > 0)) // en medio ventana
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{

															//																	TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 2], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVIo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}


													i = i + 1;
												}
												else
												{

													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


													bool boolean1_VI = false;
													bool boolean1_VF = false;

													for (int n = 0; n < elements_dVIo_2.Count(); n++)
													{
														if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VI = true;
															n = n + elements_dVIo_2.Count();
														}
													}
													for (int n = 0; n < elements_dVFo_2.Count(); n++)
													{
														if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VF = true;
															n = n + elements_dVFo_2.Count();
														}
													}

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{

																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}


														}


														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if (sillventanas[i] == 0)
														{
															Wall wallPanel = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if (VF_nuf == Math.Round(lenght_double, 1))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}

													}
												}
											}
											else
											{
												if (i < (cuenta - 1))
												{

													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


													bool boolean1_VI = false;
													bool boolean1_VF = false;

													for (int n = 0; n < elements_dVIo_2.Count(); n++)
													{
														if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VI = true;
															n = n + elements_dVIo_2.Count();
														}
													}
													for (int n = 0; n < elements_dVFo_2.Count(); n++)
													{
														if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
														{
															boolean1_VF = true;
															n = n + elements_dVFo_2.Count();
														}
													}

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}

															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{

															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																WallJoinHelper.DisableJoins(wall_recibida_dVFo);
																trans.Commit();


															}
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{
																Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
															{

																if (VF_nuf_1 == VI_nuf)
																{

																	Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}


														}


														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall wallPanel = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
															}

														}


														if (sillventanas[i] == 0)
														{
															Wall wallPanel = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if (VF_nuf == Math.Round(lenght_double, 1))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall wallPanel = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}

													}
												}
												else // no choca con nada
												{

													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if (sillventanas[i] == 0)
													{
														Wall wallPanel = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if (VF_nuf == Math.Round(lenght_double, 1))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}

												}
											}

										}

									}
									else
									{


										if (i < (cuenta - 2))
										{


											Outline outline_2 = new Outline(dd[i + 2].Min, dd[i + 2].Max);
											BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);
											FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
											IList<Element> elements_outline_2 = coll_outline_2.WherePasses(bbfilter_2).ToElements();

											bool boolean_2 = false;

											for (int n = 0; n < elements_outline_2.Count(); n++)
											{
												if (elements_outline_2[n].Id == elements_dVIo.First().Id)
												{
													boolean_2 = true;
													n = n + coll_outline_2.Count();
												}
											}

											if (boolean_2)
											{
												if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
												{
													Wall wallPanel = elements_dVIo.First() as Wall;

													if (sillventanas[i] == 0)
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
														//lista_walls.Add(wall_recibida_dVIo);
													}
													else
													{
														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
													}


												}


												if ((sillventanas[i + 1] == 0)) // en medio puerta
												{
													if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
													{

														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
													{
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.Build3Opening_DoorDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
													{
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorDoor(wallPanel, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
													{
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.Build3Opening_WindowDoorWindow(wallPanel, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
												}
												else if ((sillventanas[i + 1] > 0)) // en medio ventana
												{
													if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
													{

														//																	TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 2], height_double);
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVIo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
													{
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVIo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
													{
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVIo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
													{
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVIo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}

												}


												i = i + 1;
											}
											else
											{

												BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
												FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
												// Elementos que contienen el punto dVIo
												IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

												BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
												FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
												IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


												bool boolean1_VI = false;
												bool boolean1_VF = false;

												for (int n = 0; n < elements_dVIo_2.Count(); n++)
												{
													if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
													{
														boolean1_VI = true;
														n = n + elements_dVIo_2.Count();
													}
												}
												for (int n = 0; n < elements_dVFo_2.Count(); n++)
												{
													if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
													{
														boolean1_VF = true;
														n = n + elements_dVFo_2.Count();
													}
												}

												if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
													{

														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVFo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVFo.Id);

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
													{

														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVFo);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{
															Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

														double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{

															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																									   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																									   , Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}

														}
														else
														{
															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																									 sillventanas[i + 1],
																																									 Puntos_Ventada_dVFo[i + 1],
																																									 Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
														}


													}


													i = i + 1;

												}
												else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
													{

														//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
													{
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
													{
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
													{
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}

												}
												else if (!boolean1_VI && !boolean1_VF) // no choca con nada
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if (sillventanas[i] == 0)
													{
														Wall wallPanel = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if (VF_nuf == Math.Round(lenght_double, 1))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}

												}
											}
										}
										else
										{
											if (i < (cuenta - 1))
											{

												BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
												FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
												// Elementos que contienen el punto dVIo
												IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

												BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
												FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
												IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();


												bool boolean1_VI = false;
												bool boolean1_VF = false;

												for (int n = 0; n < elements_dVIo_2.Count(); n++)
												{
													if (elements_dVIo_2[n].Id == elements_dVFo.First().Id)
													{
														boolean1_VI = true;
														n = n + elements_dVIo_2.Count();
													}
												}
												for (int n = 0; n < elements_dVFo_2.Count(); n++)
												{
													if (elements_dVFo_2[n].Id == elements_dVFo.First().Id)
													{
														boolean1_VF = true;
														n = n + elements_dVFo_2.Count();
													}
												}

												if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
													{

														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);


														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVFo);
															trans.Commit();


														}

														lista_NO_VIo.Add(wall_recibida_dVFo.Id);

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
													{

														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															WallJoinHelper.DisableJoins(wall_recibida_dVFo);
															trans.Commit();


														}
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{
															Wall wall_recibida = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorDoorLeft(wallPanel, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 1);

														double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 1)) && !(VI_nuf == 0)) // BORDE Final
														{

															if (VF_nuf_1 == VI_nuf)
															{

																Wall wall_recibida_dVIo = profileBuilder.BuildOneWallSolitarioSpecialEndWall(wallPanel, anchoventanas[i], alturaventanas[i]
																																									   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																									   , Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}

														}
														else
														{
															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildDoorRightSpecialCase(wallPanel, alturaventanas[i + 1], sillventanas[i],
																																									 sillventanas[i + 1],
																																									 Puntos_Ventada_dVFo[i + 1],
																																									 Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildU_DoorWindowLeft(wallPanel, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
														}


													}


													i = i + 1;

												}
												else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
													{

														//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildT_Door(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
													{
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorRight(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
													{
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI_DoorLeft(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
													{
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = profileBuilder.BuildI(wallPanel, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}

												}
												else if (!boolean1_VI && !boolean1_VF) // no choca con nada
												{
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall wallPanel = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
														}

													}


													if (sillventanas[i] == 0)
													{
														Wall wallPanel = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if (VF_nuf == Math.Round(lenght_double, 1))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall wallPanel = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}

												}
											}
											else // no choca con nada
											{

												if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
												{
													Wall wallPanel = elements_dVIo.First() as Wall;

													if (sillventanas[i] == 0)
													{
														Wall wall_recibida_dVIo = profileBuilder.BuildDoorLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
														//lista_walls.Add(wall_recibida_dVIo);
													}
													else
													{
														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 1))) // BORDE Inicio
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildTwoWallSolitarioReturn(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															Wall wall_recibida_dVIo = profileBuilder.BuildWindowLeft(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
													}

												}


												if (sillventanas[i] == 0)
												{
													Wall wallPanel = elements_dVFo.First() as Wall;

													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 1);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 1);

													if (VF_nuf == Math.Round(lenght_double, 1))
													{
														// Transf. BORDE PUERTA a la DERECHA

														if (wallPanel.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
														{
															profileBuilder.BuildOneWallSolitario(wallPanel, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}

													}
													else
													{
														Wall wall_recibida_dVFo = profileBuilder.BuildDoorRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}


												}
												else
												{
													Wall wallPanel = elements_dVFo.First() as Wall;
													Wall wall_recibida_dVFo = profileBuilder.BuildWindowRight(wallPanel, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													lista_NO_VIo.Add(wall_recibida_dVFo.Id);
												}

											}
										}

									}

								}


							}//fin del FOR

						}

					}

				}
				catch (Exception)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}


			} //OKKKKKKKKKKKKKKKKKKKK!
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_OSB(Wall e, int _anchopanel_)
			{


				List<List<Wall>> listadelistasWalls_Final = new List<List<Wall>>();
				List<Wall> listaWalls_Final_noCambia = new List<Wall>();
				List<Wall> listaWalls_Final_siCambia = new List<Wall>();
				List<Wall> lista_wallfinal = new List<Wall>();

				List<List<XYZ>> Puntos_Ventana = new List<List<XYZ>>();
				List<XYZ> Puntos_Ventana_dVIo = new List<XYZ>();
				List<XYZ> Puntos_Ventana_dVFo = new List<XYZ>();


				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<double> lista_dVIo = new List<double>();
				List<double> lista_dVFo = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();

				List<List<double>> lista_delistas_conDatosVentanas = new List<List<double>>();


				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual


				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				// Recolectar Ventanas


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, e);


				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor


				foreach (Element win in windows_hosted_sorted)
				{
					FamilyInstance win1 = win as FamilyInstance;
					FamilySymbol familySymbol = win1.Symbol;
					lista_FamilySymbol.Add(familySymbol);

					double dPH1 = win1.HostParameter; //3700
					lista_dPH.Add(dPH1);

					ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
					Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
					Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240


					double width1 = widthParam1.AsDouble(); // ancho ventana 1220
					lista_width1.Add(width1);
					double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
					lista_heigth1.Add(heigth1);
					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800


					List<double> salida = new List<double>();
					if (win1.Category.Name.ToString() == "Ventanas")
					{
						salida.Add(win_sill_height1);
					}
					else if (win1.Category.Name.ToString() == "Puertas")
					{
						salida.Add(0);
					}
					lista_win_sill_height1.Add(salida.First());

					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

					double dVIo = dPH1 - width1 / 2; //3090
					lista_dVIo.Add(dVIo);
					double dVFo = dPH1 + width1 / 2; //4310                  
					lista_dVFo.Add(dVFo);

					XYZ Point_dVIo = wallCurve.Evaluate(dVIo, false);
					XYZ Point_dVFo = wallCurve.Evaluate(dVFo, false);

					XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, salida.First());
					XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, salida.First());

					Puntos_Ventana_dVIo.Add(Nuevo_Point_dVIo);
					Puntos_Ventana_dVFo.Add(Nuevo_Point_dVFo);

				}


				List<Element> windows_hosted_sorted_2 = new List<Element>();
				foreach (Element win in windows_hosted_sorted)
				{
					if (!(win.Id.ToString() == windows_hosted_sorted.First().Id.ToString()))
					{
						windows_hosted_sorted_2.Add(win);
					}
				}


				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);


				// Crear lista_d


				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
				}


				// Crear lista_d_2 Separar Paneles Antes dVIo


				List<double> lista_d_2 = new List<double>(); // lista de paneles menor o igual a dVIo

				for (int i = 0; i < lista_d.Count(); i++)
				{
					double dVIo = lista_dPH[0] - (lista_width1[0] / 2);

					if (lista_d[i] <= dVIo)
					{
						lista_d_2.Add(lista_d[i]);
						lista_d_muro4_2.Add(lista_d_muro4[i]);
					}
				}


				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");


				foreach (Element win in windows_hosted_sorted)
				{
					doc.Delete(win.Id);
				}


				double d1 = stParam + (anchopanel / RevitUnitHelper.MmToFeet);
				double d1_4 = d1 + 4 / RevitUnitHelper.MmToFeet;

				XYZ Point_d1 = wallCurve.Evaluate(d1, false);
				XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

				double VIo = Math.Round(lista_dPH[0] - (lista_width1[0] / 2), 0);

				if ((VIo == Math.Round(stParam, 0))) // BORDE INICIAL VIo
				{
					// Crear solo wall de VEntana modificando el inicial

					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + 1220 / RevitUnitHelper.MmToFeet;
						sacar.Add(dVIo_1220);
					}


					XYZ Pto_dVIo = wallCurve.Evaluate(dVIo_0, false);
					XYZ Pto_dVIo_1220 = wallCurve.Evaluate(sacar.First(), false);

					// wall f1
					Line newLineF1 = Line.CreateBound(Pto_dVIo, Pto_dVIo_1220);


					if (!(lista_win_sill_height1.First() == 0))
					{
						Wall wall_F1 = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F1);

						WallJoinHelper.DisableJoins(wall_F1);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}

						((LocationCurve)wall_1.Location).Curve = newLineF1;
					listaWalls_Final_siCambia.Add(wall_1);

					//			            Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId,false);
					Wall wall_F1_arriba = wall_1 as Wall;

					WallJoinHelper.DisableJoins(wall_F1_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}


				}
				else
				{
					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;
						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						for (int i = 0; i < lista_d_2.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_2[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_2[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);
							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						//	           		}


					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)

						double ultima_dVIo = lista_dVIo.First() - 4 / RevitUnitHelper.MmToFeet;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)

						double ultima_dVIo = lista_dVIo.First() - 4 / RevitUnitHelper.MmToFeet;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
					}


					// Crear el MiniWall antes de la Ventana


					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{

						double ultimo_d = lista_d_muro4_2.Last();
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						Wall wall_peq = Wall.Create(doc, newLine_peq, wall_1.LevelId, false);
						listaWalls_Final_siCambia.Add(wall_peq);

						WallJoinHelper.DisableJoins(wall_peq);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_peq.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}

					}
					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						WallJoinHelper.DisableJoins(wall_1);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						WallJoinHelper.DisableJoins(wall_1);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
					}


					// Crear el wall de la Ventana con ancho de 1220
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + 1220 / RevitUnitHelper.MmToFeet;
						sacar.Add(dVIo_1220);
					}


					XYZ Pto_dVIo = wallCurve.Evaluate(dVIo_0, false);
					XYZ Pto_dVIo_1220 = wallCurve.Evaluate(sacar.First(), false);

					// wall f1
					Line newLineF1 = Line.CreateBound(Pto_dVIo, Pto_dVIo_1220);


					if (!(lista_win_sill_height1.First() == 0))
					{
						Wall wall_F1 = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F1);

						WallJoinHelper.DisableJoins(wall_F1);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}


					Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F1_arriba);

					WallJoinHelper.DisableJoins(wall_F1_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}

				}

				// AnchoVentana <= 1220
				if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
				{

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);
					listaWalls_Final_siCambia.Add(wall_F3);

					WallJoinHelper.DisableJoins(wall_F3);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

					if (lista_dPH.Count() == 1)
					{


					}
					else if (lista_dPH.Count() > 1)
					{
						// agregar ventanas en el restante
						for (int i = 0; i < windows_hosted_sorted_2.Count(); i++)
						{

							FamilySymbol familySymbol = lista_FamilySymbol[i + 1];

							double dPH1 = lista_dPH[i + 1]; //3700

							XYZ xyz_dPH1 = wallCurve.Evaluate(dPH1, false);

							XYZ xyz = new XYZ(xyz_dPH1.X, xyz_dPH1.Y, lista_win_sill_height1[i + 1]);

							if (!familySymbol.IsActive)
							{
								// Ensure the family symbol is activated.
								familySymbol.Activate();
								doc.Regenerate();
							}

							FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall_F3, StructuralType.NonStructural);

						}

					}
				}

				// AnchoVentana > 1220
				else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
				{

					double ultimo_dVFo = lista_dVFo.First();
					XYZ Point_ultimo_dVFo = wallCurve.Evaluate(ultimo_dVFo, false);


					double ultimo_dVIo = lista_dVIo.First();
					XYZ Point_ultimo_dVIo = wallCurve.Evaluate(ultimo_dVIo, false);


					double ultimo_dVIo_1224 = ultimo_dVIo + 1224 / RevitUnitHelper.MmToFeet;
					XYZ Point_ultimo_dVIo_1224 = wallCurve.Evaluate(ultimo_dVIo_1224, false);

					double extra_ventana = lista_width1.First() - 1220 / RevitUnitHelper.MmToFeet;

					double d_panel_ventana = ultimo_dVFo - extra_ventana;


					XYZ Pto_d_panel_ventana = wallCurve.Evaluate(d_panel_ventana, false);

					Line newLineF2 = Line.CreateBound(Point_ultimo_dVIo_1224, Point_ultimo_dVFo);

					if (!(lista_win_sill_height1.First() == 0))
					{
						Wall wall_F2 = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F2);

						WallJoinHelper.DisableJoins(wall_F2);

						Parameter WALL_USER_HEIGHT_PARAMF2 = wall_F2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF2.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF2.Set(lista_win_sill_height1.First());
						}
					}


					Wall wall_F2_arriba = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F2_arriba);

					WallJoinHelper.DisableJoins(wall_F2_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF2_arriba = wall_F2_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF2_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF2_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2_arriba = wall_F2_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2_arriba.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2_arriba.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}


					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3);

					WallJoinHelper.DisableJoins(wall_F3);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

					if (lista_dPH.Count() == 1)
					{


					}
					else if (lista_dPH.Count() > 1)
					{
						// agregar ventanas en el restante
						for (int i = 0; i < windows_hosted_sorted_2.Count(); i++)
						{

							FamilySymbol familySymbol = lista_FamilySymbol[i + 1];

							double dPH1 = lista_dPH[i + 1]; //3700

							XYZ xyz_dPH1 = wallCurve.Evaluate(dPH1, false);

							XYZ xyz = new XYZ(xyz_dPH1.X, xyz_dPH1.Y, lista_win_sill_height1[i + 1]);

							if (!familySymbol.IsActive)
							{
								// Ensure the family symbol is activated.
								familySymbol.Activate();
								doc.Regenerate();
							}

							FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall_F3, StructuralType.NonStructural);
						}

					}
				}

				trans.Commit();


				listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
				listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				//	    	}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			}
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_OSB_1_VENTANA(Wall e, int _anchopanel_)
			{


				List<List<Wall>> listadelistasWalls_Final = new List<List<Wall>>();
				List<Wall> listaWalls_Final_noCambia = new List<Wall>();
				List<Wall> listaWalls_Final_siCambia = new List<Wall>();
				List<Wall> lista_wallfinal = new List<Wall>();

				List<List<XYZ>> Puntos_Ventana = new List<List<XYZ>>();
				List<XYZ> Puntos_Ventana_dVIo = new List<XYZ>();
				List<XYZ> Puntos_Ventana_dVFo = new List<XYZ>();


				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<double> lista_dVIo = new List<double>();
				List<double> lista_dVFo = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();

				List<List<double>> lista_delistas_conDatosVentanas = new List<List<double>>();


				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual


				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				// Recolectar Ventanas


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, e);


				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor


				foreach (Element win in windows_hosted_sorted)
				{
					FamilyInstance win1 = win as FamilyInstance;
					FamilySymbol familySymbol = win1.Symbol;
					lista_FamilySymbol.Add(familySymbol);

					double dPH1 = win1.HostParameter; //3700
					lista_dPH.Add(dPH1);

					ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
					Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
					Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240


					double width1 = widthParam1.AsDouble(); // ancho ventana 1220
					lista_width1.Add(width1);
					double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
					lista_heigth1.Add(heigth1);
					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800

					if (win1.Category.Name.ToString() == "Ventanas")
					{
						lista_win_sill_height1.Add(win_sill_height1);
					}
					else if (win1.Category.Name.ToString() == "Puertas")
					{
						lista_win_sill_height1.Add(0);
					}

					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

					double dVIo = dPH1 - width1 / 2; //3090
					lista_dVIo.Add(dVIo);
					double dVFo = dPH1 + width1 / 2; //4310                  
					lista_dVFo.Add(dVFo);

					XYZ Point_dVIo = wallCurve.Evaluate(dVIo, false);
					XYZ Point_dVFo = wallCurve.Evaluate(dVFo, false);

					XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, lista_win_sill_height1.First());
					XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, lista_win_sill_height1.First());

					Puntos_Ventana_dVIo.Add(Nuevo_Point_dVIo);
					Puntos_Ventana_dVFo.Add(Nuevo_Point_dVFo);

				}


				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);


				// Crear lista_d


				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
				}


				// Crear lista_d_2 Separar Paneles Antes dVIo


				List<double> lista_d_2 = new List<double>(); // lista de paneles menor o igual a dVIo

				for (int i = 0; i < lista_d.Count(); i++)
				{
					double dVIo = lista_dPH.First() - (lista_width1.First() / 2);

					if (lista_d[i] <= dVIo)
					{
						lista_d_2.Add(lista_d[i]);
						lista_d_muro4_2.Add(lista_d_muro4[i]);
					}
				}


				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");


				foreach (Element win in windows_hosted_sorted)
				{
					doc.Delete(win.Id);
				}


				double d1 = stParam + (anchopanel / RevitUnitHelper.MmToFeet);
				double d1_4 = d1 + 4 / RevitUnitHelper.MmToFeet;

				XYZ Point_d1 = wallCurve.Evaluate(d1, false);
				XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

				double VIo = Math.Round(lista_dPH[0] - (lista_width1[0] / 2), 0);

				if ((VIo == Math.Round(stParam, 0))) // BORDE INICIAL VIo
				{
					// Crear solo wall de VEntana modificando el inicial

					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + 1220 / RevitUnitHelper.MmToFeet;
						sacar.Add(dVIo_1220);
					}


					XYZ Pto_dVIo = wallCurve.Evaluate(dVIo_0, false);
					XYZ Pto_dVIo_1220 = wallCurve.Evaluate(sacar.First(), false);

					// wall f1
					Line newLineF1 = Line.CreateBound(Pto_dVIo, Pto_dVIo_1220);


					if (!(lista_win_sill_height1.First() == 0))
					{
						Wall wall_F1 = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F1);

						WallJoinHelper.DisableJoins(wall_F1);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}

						((LocationCurve)wall_1.Location).Curve = newLineF1;
					listaWalls_Final_siCambia.Add(wall_1);

					//			            Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId,false);
					Wall wall_F1_arriba = wall_1 as Wall;

					WallJoinHelper.DisableJoins(wall_F1_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}

				}
				else
				{
					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;
						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						for (int i = 0; i < lista_d_2.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_2[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_2[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);
							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						//	           		}


					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)

						double ultima_dVIo = lista_dVIo.First() - 4 / RevitUnitHelper.MmToFeet;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)

						double ultima_dVIo = lista_dVIo.First() - 4 / RevitUnitHelper.MmToFeet;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
					}

					// Crear el MiniWall antes de la Ventana


					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{

						double ultimo_d = lista_d_muro4_2.Last();
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						Wall wall_peq = Wall.Create(doc, newLine_peq, wall_1.LevelId, false);
						listaWalls_Final_siCambia.Add(wall_peq);

						WallJoinHelper.DisableJoins(wall_peq);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_peq.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}

					}
					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						WallJoinHelper.DisableJoins(wall_1);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / RevitUnitHelper.MmToFeet;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						WallJoinHelper.DisableJoins(wall_1);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
					}


					// Crear el wall de la Ventana con ancho de 1220

					// wall f1
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
					{
						double dVIo_1220 = dVIo_0 + 1220 / RevitUnitHelper.MmToFeet;
						sacar.Add(dVIo_1220);
					}


					XYZ Pto_dVIo = wallCurve.Evaluate(dVIo_0, false);
					XYZ Pto_dVIo_1220 = wallCurve.Evaluate(sacar.First(), false);
					Line newLineF1 = Line.CreateBound(Pto_dVIo, Pto_dVIo_1220);

					if (!(lista_win_sill_height1.First() == 0))
					{

						Wall wall_F1 = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F1);

						WallJoinHelper.DisableJoins(wall_F1);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}


					Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F1_arriba);

					WallJoinHelper.DisableJoins(wall_F1_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}

				}


				// AnchoVentana <= 1220
				if (lista_width1.First() <= 1220 / RevitUnitHelper.MmToFeet)
				{

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);
					listaWalls_Final_siCambia.Add(wall_F3);

					WallJoinHelper.DisableJoins(wall_F3);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

				}

				// AnchoVentana > 1220
				else if (lista_width1.First() > 1220 / RevitUnitHelper.MmToFeet)
				{


					double ultimo_dVFo = lista_dVFo.First();
					XYZ Point_ultimo_dVFo = wallCurve.Evaluate(ultimo_dVFo, false);


					double ultimo_dVIo = lista_dVIo.First();
					XYZ Point_ultimo_dVIo = wallCurve.Evaluate(ultimo_dVIo, false);


					double ultimo_dVIo_1224 = ultimo_dVIo + 1224 / RevitUnitHelper.MmToFeet;
					XYZ Point_ultimo_dVIo_1224 = wallCurve.Evaluate(ultimo_dVIo_1224, false);

					double extra_ventana = lista_width1.First() - 1220 / RevitUnitHelper.MmToFeet;

					double d_panel_ventana = ultimo_dVFo - extra_ventana;


					XYZ Pto_d_panel_ventana = wallCurve.Evaluate(d_panel_ventana, false);
					Line newLineF2 = Line.CreateBound(Point_ultimo_dVIo_1224, Point_ultimo_dVFo);

					if (!(lista_win_sill_height1.First() == 0))
					{

						Wall wall_F2 = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F2);

						WallJoinHelper.DisableJoins(wall_F2);

						Parameter WALL_USER_HEIGHT_PARAMF2 = wall_F2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF2.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF2.Set(lista_win_sill_height1.First());
						}
					}

					Wall wall_F2_arriba = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F2_arriba);

					WallJoinHelper.DisableJoins(wall_F2_arriba);

					Parameter WALL_USER_HEIGHT_PARAMF2_arriba = wall_F2_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF2_arriba.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF2_arriba.Set(height_double - (lista_win_sill_height1.First() + lista_heigth1.First()));
					}

					Parameter WALL_USER_BASEOFFSET_PARAM_2_arriba = wall_F2_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					if (!WALL_USER_BASEOFFSET_PARAM_2_arriba.IsReadOnly)
					{
						WALL_USER_BASEOFFSET_PARAM_2_arriba.Set(lista_win_sill_height1.First() + lista_heigth1.First());
					}


					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3);

					WallJoinHelper.DisableJoins(wall_F3);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

				}

				trans.Commit();


				listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
				listadelistasWalls_Final.Add(listaWalls_Final_noCambia);

				//	    	}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			}
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL(Wall e, int _anchopanel_)
			{


				List<List<Wall>> listadelistasWalls_Final = new List<List<Wall>>();
				List<Wall> listaWalls_Final_noCambia = new List<Wall>();
				List<Wall> listaWalls_Final_siCambia = new List<Wall>();
				List<Wall> lista_wallfinal = new List<Wall>();

				List<List<XYZ>> Puntos_Ventana = new List<List<XYZ>>();
				List<XYZ> Puntos_Ventana_dVIo = new List<XYZ>();
				List<XYZ> Puntos_Ventana_dVFo = new List<XYZ>();

				List<BoundingBoxXYZ> lista_bb = new List<BoundingBoxXYZ>();


				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<double> lista_dVIo = new List<double>();
				List<double> lista_dVFo = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();

				List<List<double>> lista_delistas_conDatosVentanas = new List<List<double>>();


				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual


				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double endParam_4 = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				XYZ endPoint_4 = wallCurve.Evaluate(endParam_4, false);


				// Recolectar Ventanas


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, e);


				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor


				foreach (Element win in windows_hosted_sorted)
				{
					FamilyInstance win1 = win as FamilyInstance;
					FamilySymbol familySymbol = win1.Symbol;
					lista_FamilySymbol.Add(familySymbol);


					GeometryElement geoEle = win1.get_Geometry(new Options());
					BoundingBoxXYZ bb = geoEle.GetBoundingBox();
					lista_bb.Add(bb);


					double dPH1 = win1.HostParameter; //3700
					lista_dPH.Add(dPH1);

					ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
					Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
					Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240

					//				BuiltInCategory puerta = win

					double width1 = widthParam1.AsDouble(); // ancho ventana 1220
					lista_width1.Add(width1);
					double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
					lista_heigth1.Add(heigth1);
					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800

					List<double> salida = new List<double>();
					if (win.Category.Name.ToString() == "Puertas")
					{
						salida.Add(0);
					}
					else if (win.Category.Name.ToString() == "Ventanas")
					{
						salida.Add(win_sill_height1);
					}
					lista_win_sill_height1.Add(salida.First());


					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

					double dVIo = dPH1 - width1 / 2; //3090
					lista_dVIo.Add(dVIo);
					double dVFo = dPH1 + width1 / 2; //4310                  
					lista_dVFo.Add(dVFo);

					XYZ Point_dVIo = wallCurve.Evaluate(dVIo, false);
					XYZ Point_dVFo = wallCurve.Evaluate(dVFo, false);

					XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, salida.First());
					XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, salida.First());

					Puntos_Ventana_dVIo.Add(Nuevo_Point_dVIo);
					Puntos_Ventana_dVFo.Add(Nuevo_Point_dVFo);


				}

				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);


				// Crear lista_d


				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();

				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
				}


				if (longitud_double < anchopanel / RevitUnitHelper.MmToFeet)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return null;
				}
				else if (longitud_double == anchopanel / RevitUnitHelper.MmToFeet)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual igual al valor ingresado para la longitud");
					return null;
				}
				else if (windows_hosted.Count() == 0)
				{
					TaskDialog.Show("Dynoscript", "El Muro no contiene una Ventana");
					return null;
				}
				else
				{

					Transaction trans = new Transaction(doc);

					trans.Start("mysplitwall");


					foreach (Element win in windows_hosted_sorted)
					{
						doc.Delete(win.Id);
					}


					double d1 = stParam + (anchopanel / RevitUnitHelper.MmToFeet);
					double d1_4 = d1 + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}


						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.


						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						WallJoinHelper.DisableJoins(wall_F3_ultimo);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}


						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.


						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						WallJoinHelper.DisableJoins(wall_F3_ultimo);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}


						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.


						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						WallJoinHelper.DisableJoins(wall_F3_ultimo);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


					}


					trans.Commit();


					listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
					listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas, lista_bb);
				return tupla;
			}
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_1_VENTANA(Wall e, int _anchopanel_)
			{


				List<List<Wall>> listadelistasWalls_Final = new List<List<Wall>>();
				List<Wall> listaWalls_Final_noCambia = new List<Wall>();
				List<Wall> listaWalls_Final_siCambia = new List<Wall>();
				List<Wall> lista_wallfinal = new List<Wall>();

				List<List<XYZ>> Puntos_Ventana = new List<List<XYZ>>();
				List<XYZ> Puntos_Ventana_dVIo = new List<XYZ>();
				List<XYZ> Puntos_Ventana_dVFo = new List<XYZ>();


				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<double> lista_dVIo = new List<double>();
				List<double> lista_dVFo = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();

				List<List<double>> lista_delistas_conDatosVentanas = new List<List<double>>();


				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual


				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);


				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				// Recolectar Ventanas


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, e);


				FamilyInstance win1 = windows_hosted.First() as FamilyInstance;
				FamilySymbol familySymbol = win1.Symbol;
				lista_FamilySymbol.Add(familySymbol);

				double dPH1 = win1.HostParameter; //3700
				lista_dPH.Add(dPH1);

				ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
				Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
				Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240


				double width1 = widthParam1.AsDouble(); // ancho ventana 1220
				lista_width1.Add(width1);
				double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
				lista_heigth1.Add(heigth1);

				double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800

				if (win1.Category.Name.ToString() == "Puertas")
				{
					lista_win_sill_height1.Add(0);
				}
				else if (win1.Category.Name.ToString() == "Ventanas")
				{
					lista_win_sill_height1.Add(win_sill_height1);
				}

				double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

				double dVIo = dPH1 - width1 / 2; //3090
				lista_dVIo.Add(dVIo);
				double dVFo = dPH1 + width1 / 2; //4310                  
				lista_dVFo.Add(dVFo);

				XYZ Point_dVIo = wallCurve.Evaluate(dVIo, false);
				XYZ Point_dVFo = wallCurve.Evaluate(dVFo, false);

				XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, lista_win_sill_height1.First());
				XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, lista_win_sill_height1.First());

				Puntos_Ventana_dVIo.Add(Nuevo_Point_dVIo);
				Puntos_Ventana_dVFo.Add(Nuevo_Point_dVFo);


				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);


				// Crear lista_d


				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
				}


				// Crear lista_d_3 Separar Paneles Antes dVIo


				if (longitud_double < anchopanel / RevitUnitHelper.MmToFeet)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return null;
				}
				else if (longitud_double == anchopanel / RevitUnitHelper.MmToFeet)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual igual al valor ingresado para la longitud");
					return null;
				}
				else if (windows_hosted.Count() == 0)
				{
					TaskDialog.Show("Dynoscript", "El Muro no contiene una Ventana");
					return null;
				}
				else
				{

					Transaction trans = new Transaction(doc);

					trans.Start("mysplitwall");


					doc.Delete(win1.Id);


					double d1 = stParam + (anchopanel / RevitUnitHelper.MmToFeet);
					double d1_4 = d1 + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}


					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}


					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer

						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						WallJoinHelper.DisableJoins(wall_1);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);


						// Crear Walls si esta muy lejos de la esquina


						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
							double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							WallJoinHelper.DisableJoins(wall_N);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

					}

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.


					double ultimo_d_4 = lista_d_muro4.Last();
					XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

					Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint);

					Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3_ultimo);

					WallJoinHelper.DisableJoins(wall_F3_ultimo);

					Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
					}


					trans.Commit();


					listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
					listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			} // okkkkkkkkkkkkkkkkkkkkk!
			void Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(Wall e, int _anchopanel_)
			{


				List<List<Wall>> listadelistasWalls_Final = new List<List<Wall>>();
				List<Wall> listaWalls_Final_noCambia = new List<Wall>();
				List<Wall> listaWalls_Final_siCambia = new List<Wall>();
				List<Wall> lista_wallfinal = new List<Wall>();

				List<List<XYZ>> Puntos_Ventana = new List<List<XYZ>>();
				List<XYZ> Puntos_Ventana_dVIo = new List<XYZ>();
				List<XYZ> Puntos_Ventana_dVFo = new List<XYZ>();


				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<double> lista_dVIo = new List<double>();
				List<double> lista_dVFo = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();

				List<List<double>> lista_delistas_conDatosVentanas = new List<List<double>>();


				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual


				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double endParam_4 = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				XYZ endPoint_4 = wallCurve.Evaluate(endParam_4, false);


				// Recolectar Ventanas


				// Crear lista_d


				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / RevitUnitHelper.MmToFeet) * (i + 1) + (4 / RevitUnitHelper.MmToFeet) * (i + 1)));
				}


				// Crear lista_d_3 Separar Paneles Antes dVIo


				if (longitud_double < anchopanel / RevitUnitHelper.MmToFeet)
				{
				}
				else if (longitud_double == anchopanel / RevitUnitHelper.MmToFeet)
				{
				}
				else
				{

					Transaction trans = new Transaction(doc);

					trans.Start("mysplitwall");


					//				doc.Delete(win1.Id);


					double d1 = stParam + (anchopanel / RevitUnitHelper.MmToFeet);
					double d1_4 = d1 + 4 / RevitUnitHelper.MmToFeet;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);


					// Crear Wall Primer

					Line newLine01 = Line.CreateBound(stPoint, Point_d1);

					WallJoinHelper.DisableJoins(wall_1);

					((LocationCurve)wall_1.Location).Curve = newLine01;

					listaWalls_Final_siCambia.Add(wall_1);


					// Crear Walls si esta muy lejos de la esquina


					// Crear Walls Antes al dVIo
					for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
					{
						double di = lista_d_muro4[i] + (anchopanel / RevitUnitHelper.MmToFeet); // d2
						double di_4 = di + 4 / RevitUnitHelper.MmToFeet; // d2_4

						//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
						XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
						XYZ PointB = wallCurve.Evaluate(di, false);

						Line newLineN = Line.CreateBound(PointA, PointB);
						Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_N);

						WallJoinHelper.DisableJoins(wall_N);

						Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM12.Set(height_double);
						}

					}


					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.


					double ultimo_d_4 = lista_d_muro4.Last();
					XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

					Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

					Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3_ultimo);

					WallJoinHelper.DisableJoins(wall_F3_ultimo);

					Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
					}


					trans.Commit();


				}

			} // okkkkkkkkkkkkkkkkkkkkk!


			// 3 ventanas en 1 muro
			Wall Revision6_DYNO_DarVuelta_Muro_ConVentanas(Element _e_)
			{

				List<Wall> listaWalls_Final = new List<Wall>();

				//			Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige
				Wall wall_1 = _e_ as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double primero = stParam;
				double endParam = wallCurve.GetEndParameter(1);


				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, activeView, wall_1);


				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor

				List<double> lista_a = new List<double>();
				List<double> lista_width1 = new List<double>();
				List<double> lista_heigth1 = new List<double>();
				List<double> lista_win_sill_height1 = new List<double>();
				List<double> lista_dPH = new List<double>();
				List<FamilySymbol> lista_FamilySymbol = new List<FamilySymbol>();


				foreach (Element win in windows_hosted_sorted)
				{
					FamilyInstance win1 = win as FamilyInstance;
					FamilySymbol familySymbol = win1.Symbol;
					lista_FamilySymbol.Add(familySymbol);

					double dPH1 = win1.HostParameter; //3700
					lista_dPH.Add(dPH1);

					double a0 = endParam - dPH1;
					lista_a.Add(a0);

					ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
					Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
					Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240


					double width1 = widthParam1.AsDouble(); // ancho ventana 1220
					lista_width1.Add(width1);
					double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
					lista_heigth1.Add(heigth1);
					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800

					List<double> salida = new List<double>();
					if (win1.Category.Name.ToString() == "Ventanas")
					{
						salida.Add(win_sill_height1);
					}
					else if (win1.Category.Name.ToString() == "Puertas")
					{
						salida.Add(0);
					}

					lista_win_sill_height1.Add(salida.First());


					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

				}


				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");

				// CORREGIR WALL 1 EXISTENTE

				for (int i = 0; i < windows_hosted_sorted.Count(); i++)
				{
					doc.Delete(windows_hosted_sorted[i].Id);
				}

				Line newLine01 = Line.CreateBound(endPoint, stPoint);
				WallJoinHelper.DisableJoins(wall_1);

				((LocationCurve)wall_1.Location).Curve = newLine01;

				Curve wallCurve_2 = ((LocationCurve)wall_1.Location).Curve;
				double stParam_nuevo = wallCurve_2.GetEndParameter(0);
				double endParam_nuevo = wallCurve_2.GetEndParameter(1);

				lista_FamilySymbol.Reverse();
				lista_dPH.Reverse();
				lista_a.Reverse();
				lista_width1.Reverse();
				lista_heigth1.Reverse();
				lista_win_sill_height1.Reverse();

				// agregar ventanas en el restante
				for (int i = 0; i < lista_dPH.Count(); i++)
				{

					double dPH1_nuevo = stParam_nuevo + lista_a[i];

					FamilySymbol familySymbol = lista_FamilySymbol[i];


					Curve wallCurve_3 = ((LocationCurve)wall_1.Location).Curve;
					XYZ xyz_dPH1 = wallCurve_3.Evaluate(dPH1_nuevo, false);

					XYZ xyz = new XYZ(xyz_dPH1.X, xyz_dPH1.Y, lista_win_sill_height1[i]);

					// Create window.

					if (!familySymbol.IsActive)
					{
						// Ensure the family symbol is activated.
						familySymbol.Activate();
						doc.Regenerate();
					}

					FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall_1, StructuralType.NonStructural);

				}


				trans.Commit();


				return wall_1;

			}


			// SIN VANOS
			void Revision6_DYNO_DividirMuroSinVentana(Element e, int _anchopanel_)
			{


				List<Wall> listaWalls_Final = new List<Wall>();

				int anchopanel = _anchopanel_; //1220

				List<Wall> lista_wallfinal = new List<Wall>();

				//	        Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

				Wall wall_1 = e as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double Param1 = stParam + anchopanel / RevitUnitHelper.MmToFeet;

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / RevitUnitHelper.MmToFeet); // 3.38 * RevitUnitHelper.MmToFeet =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / RevitUnitHelper.MmToFeet); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + (i + 1) * (anchopanel / RevitUnitHelper.MmToFeet));
				}


				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				XYZ Point1 = wallCurve.Evaluate(Param1, false);


				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();

				// Recolectar Ventanas


				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas

				List<Element> windows_hosted = new List<Element>();

				foreach (Element elem in windows)
				{
					FamilyInstance fintance = elem as FamilyInstance;

					if (fintance.Host.Id == e.Id)
					{
						windows_hosted.Add(elem);
					}

				}


				if (longitud_double < anchopanel / RevitUnitHelper.MmToFeet)
				{
					//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return;
				}
				else if (longitud_double == anchopanel / RevitUnitHelper.MmToFeet)
				{
					//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual igual al valor ingresado para la longitud");
					return;
				}
				else if (windows_hosted.Count() > 0)
				{
					//TaskDialog.Show("Dynoscript", "El Muro Seleccionado tiene Ventanas. No se puede seleccionar con esta opción .");
					return;
				}
				else
				{

					Transaction trans = new Transaction(doc);

					trans.Start("mysplitwall");

					// CORREGIR WALL 1 EXISTENTE

					Line newLine01 = Line.CreateBound(stPoint, Point1);
					WallJoinHelper.DisableJoins(wall_1);

					((LocationCurve)wall_1.Location).Curve = newLine01;

					listaWalls_Final.Add(wall_1);

					// CREATE WALLS  Y  ARREGLAR LA ALTURA DE TODOS LOS MUROS


					for (int i = 0; i < lista_d.Count() - 1; i++) // count = 6
					{
						XYZ PointA = wallCurve.Evaluate(lista_d[i], false);
						XYZ PointB = wallCurve.Evaluate(lista_d[i + 1], false);

						Line newLineN = Line.CreateBound(PointA, PointB);
						Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);
						Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM12.Set(height_double);
						}
						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

						listaWalls_Final.Add(wall_N);
					}

					XYZ Point_Last = wallCurve.Evaluate(lista_d.Last(), false);


					Line newLineF = Line.CreateBound(Point_Last, endPoint);
					Wall wall_F = Wall.Create(doc, newLineF, wall_1.LevelId, false);

					listaWalls_Final.Add(wall_F);

					Parameter WALL_USER_HEIGHT_PARAM_F = wall_F.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM_F.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_F.Set(height_double);
					}
					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F, 1);

					trans.Commit();

					foreach (Wall wall in listaWalls_Final)
					{
						Revision6_DYNO_splitWall_agregar_separacion40(wall);
					}

				}
			}
			Wall Revision6_DYNO_DarVuelta_Muro_SinVentanas(Element _e_)
			{

				List<Wall> listaWalls_Final = new List<Wall>();

				//			Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige
				Wall wall_1 = _e_ as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");

				// CORREGIR WALL 1 EXISTENTE

				Line newLine01 = Line.CreateBound(endPoint, stPoint);
				WallJoinHelper.DisableJoins(wall_1);

				((LocationCurve)wall_1.Location).Curve = newLine01;

				trans.Commit();

				return wall_1;

			}
			// Separar con minipanel de 4 mm
			void Revision6_DYNO_splitWall_agregar_separacion40(Wall _wall_)
			{


				Wall wall_1 = _wall_ as Wall; //1220

				List<Wall> lista_wallfinal = new List<Wall>();

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // 2440

				double height_double = height.AsDouble(); // 2440

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				double p40 = endParam - 4 / RevitUnitHelper.MmToFeet;
				XYZ p40Point = wallCurve.Evaluate(p40, false);

				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");

				// CORREGIR WALL 1 EXISTENTE

				Line newLine01 = Line.CreateBound(stPoint, p40Point); // stPoint, (endPoint - 40)
				WallJoinHelper.DisableJoins(wall_1);

				((LocationCurve)wall_1.Location).Curve = newLine01;

				// CREATE WALLS de 40 mm


				trans.Commit();

			} // Wall seleccionado


			//close ()

			return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }
    }
}
