using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using System.IO;
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
			Application app = uiApp.Application;

			//View activeView = doc.ActiveView;
			string ruta = App.ExecutingAssemblyPath;

			//open ()
			#region MACRO

			#region Ejecución de Script
			splitWall_pick_122wall_conVanos();
			#endregion
			void splitWall_pick_122wall_conVanos()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				double numero_final = 0;

				using (var form = new Form1())
				{
					form.ShowDialog();

					if (form.DialogResult == forms.DialogResult.Cancel) return;

					if (form.DialogResult == forms.DialogResult.OK)
					{
						string numero = form.textString.ToString();
						//NumberFormatInfo provider = new NumberFormatInfo();
						//double val = Convert.ToDouble(numero, provider); 
						double val = Convert.ToDouble(numero);
						numero_final = val;
					}

				}

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");


				Reference ro = uidoc.Selection.PickObject(ObjectType.Element, "Please select a Wall");
				Element e = doc.GetElement(ro); // EL MURO SELECCIONADO
				ElementId eID = e.Id;

				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				FilteredElementCollector coll = new FilteredElementCollector(doc); // ventada seleccionada

				IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas

				List<Wall> wallslibres = new List<Wall>();

				List<Element> windows_hosted = new List<Element>();

				foreach (Element elem in windows)
				{
					FamilyInstance fintance = elem as FamilyInstance;

					if (fintance.Host.Id == e.Id)
					{
						windows_hosted.Add(elem);
					}

				}

				if (windows_hosted.Count() > 1)
				{

					List<XYZ> listPuntos_ACE = new List<XYZ>();
					List<XYZ> listPuntos_BDF = new List<XYZ>();
					List<XYZ> listaPuntos = new List<XYZ>();

					List<double> listDoubles = new List<double>();

					List<Line> listLines = new List<Line>();
					List<Line> listLines_murosolos = new List<Line>();

					Wall wall_I = e as Wall; // e = MURO SELECCIONADO
					Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;
					Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
					double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					//string msg0 = "Son " + windows_hosted.Count().ToString() + " ventandas encontradas en el proyecto actual\n" + "stParam = " + stParam.ToString() + "\n" + "endParam = " + endParam.ToString() + "\n" + Environment.NewLine;

					List<double> sill_heights = new List<double>();
					List<double> head_heights = new List<double>();

					listDoubles.Add(stParam);

					foreach (Element win in windows_hosted)
					{

						FamilyInstance winfi = win as FamilyInstance;

						ElementType type = doc.GetElement(win.GetTypeId()) as ElementType;
						Parameter widthParam = type.LookupParameter("Width");
						Parameter heightParam = type.LookupParameter("Height");
						double width = widthParam.AsDouble();
						double heigth = heightParam.AsDouble();

						double win_sill_height = win.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble();
						double win_head_height = win.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble();

						sill_heights.Add(win_sill_height);
						head_heights.Add(win_head_height);

						double dPH1 = winfi.HostParameter;
						double dPA = dPH1 - width / 2;
						double dPB = dPH1 + width / 2;

						listDoubles.Add(dPA);
						listDoubles.Add(dPB);

						listDoubles.Sort();

					}

					//TaskDialog.Show("salida", msg0);


					listDoubles.Add(endParam);

					listDoubles.Sort();

					double stprimero = listDoubles.First();
					double endultimo = listDoubles.Last();

					foreach (double elem in listDoubles)
					{
						XYZ P = wallCurve.Evaluate(elem, false);
						listaPuntos.Add(P);
					}

					Line newLineI = Line.CreateBound(listaPuntos[0], listaPuntos[1]);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					for (int i = 1; i < windows_hosted.Count() * 2; i = i + 2)
					{
						listPuntos_ACE.Add(listaPuntos[i]);
						listPuntos_BDF.Add(listaPuntos[i + 1]);
					}


					for (int i = 0; i < (listPuntos_BDF.Count()); i++) // 3
					{
						Line newLineN = Line.CreateBound(listPuntos_ACE[i], listPuntos_BDF[i]);
						listLines.Add(newLineN);
						if (i < (listPuntos_BDF.Count() - 1)) // 2
						{
							Line newLineNN = Line.CreateBound(listPuntos_BDF[i], listPuntos_ACE[i + 1]);
							listLines_murosolos.Add(newLineNN);
						}

					}
					int nn = listaPuntos.Count() - 1;
					Line newLineF = Line.CreateBound(listaPuntos[nn - 1], listaPuntos[nn]);

					foreach (Element win in windows_hosted)
					{
						doc.Delete(win.Id);
					}

					// CREAR MUROS

					((LocationCurve)wall_I.Location).Curve = newLineI; // Wall_I
					wallslibres.Add(wall_I);

					foreach (Line newLineNN in listLines_murosolos)
					{

						Wall wall_N = Wall.Create(doc, newLineNN, wall_I.LevelId, false); // Wall_2,4
						wallslibres.Add(wall_N);

						Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_N, 1);
						//sill_heights[n]; // [800, 800, 800]
						//head_heights[n]; // [2040, 2040, 2040]

						if (!WALL_USER_HEIGHT_PARAM.IsReadOnly) // height_double = 2440
						{
							WALL_USER_HEIGHT_PARAM.Set(height_double);
						}
					}



					int n = 0;
					foreach (Line newLineN in listLines)
					{

						Wall wall_N = Wall.Create(doc, newLineN, wall_I.LevelId, false); // Wall_1,3,5

						Wall wall_N_copy = Wall.Create(doc, newLineN, wall_I.LevelId, false);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N_copy, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_N_copy, 1);

						Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

						Parameter WALL_USER_HEIGHT_PARAM_copy = wall_N_copy.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

						Parameter WALL_USER_BASEOFFSET_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

						Parameter WALL_USER_BASEOFFSET_PARAM_copy = wall_N_copy.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

						//sill_heights[n]; // [800, 800, 800]
						//head_heights[n]; // [2040, 2040, 2040]

						if (!WALL_USER_HEIGHT_PARAM.IsReadOnly) // height_double = 2440
						{
							WALL_USER_HEIGHT_PARAM.Set(height_double - head_heights[n]);      // 400...............(height_double - win_head_height) = 2440 - 2040
							WALL_USER_HEIGHT_PARAM_copy.Set(sill_heights[n]); // 800 ............. win_sill_height = 800
							WALL_USER_BASEOFFSET_PARAM.Set(head_heights[n]); // 2040 .............win_head_height = 2040

							//WALL_USER_BASEOFFSET_PARAM_copy.Set(0);
						}

						n = n + 1;
					}

					Wall wall_F = Wall.Create(doc, newLineF, wall_I.LevelId, false); // Wall_F
					wallslibres.Add(wall_F);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F, 1);
					Parameter WALL_USER_HEIGHT_PARAM_F = wall_F.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM_F.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_F.Set(height_double);
					}

				}
				else if (windows_hosted.Count() == 1)
				{
					FamilyInstance window = windows_hosted.First() as FamilyInstance;
					LocationPoint window_point = window.Location as LocationPoint;
					XYZ window_point_xyz = window_point.Point;
					string window_point_s = window_point_xyz.ToString();

					TaskDialog.Show("salida 1", window_point_s);


					double a = window.HostParameter;
					Element window_elem = windows_hosted.First();
					ElementType type = doc.GetElement(window_elem.GetTypeId()) as ElementType;
					Parameter widthParam = type.LookupParameter("Width");
					Parameter heightParam = type.LookupParameter("Height");
					double width = widthParam.AsDouble();
					double heigth = heightParam.AsDouble();
					double altura1_window = window_elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
					double altura2_window = window_elem.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040


					Wall wall_1 = e as Wall; // e = MURO SELECCIONADO
					Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
					Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					double height_double = height.AsDouble();
					double height_altura2y4 = height_double - (altura1_window + heigth); // 400

					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					XYZ stPoint = wallCurve.Evaluate(stParam, false);
					XYZ endPoint = wallCurve.Evaluate(endParam, false);

					double PHOST = a;
					double PA = a - width / 2;
					double PB = a + width / 2;

					XYZ PHOST_point = wallCurve.Evaluate(PHOST, false);
					XYZ PA_point = wallCurve.Evaluate(PA, false);
					XYZ PB_point = wallCurve.Evaluate(PB, false);

					Line newLine1 = Line.CreateBound(stPoint, PA_point);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

					Line newLine2 = Line.CreateBound(PA_point, PB_point);
					Line newLine3 = Line.CreateBound(PB_point, endPoint);

					doc.Delete(window_elem.Id);

					((LocationCurve)wall_1.Location).Curve = newLine1;

					wallslibres.Add(wall_1);

					Wall wall_2 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
					Wall wall_4 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
					Wall wall_3 = Wall.Create(doc, newLine3, wall_1.LevelId, false);

					wallslibres.Add(wall_3);

					Parameter WALL_USER_HEIGHT_PARAM_2 = wall_2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_HEIGHT_PARAM_4 = wall_4.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_HEIGHT_PARAM_3 = wall_3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

					Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_2.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					Parameter WALL_USER_BASEOFFSET_PARAM_4 = wall_4.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

					if (!WALL_USER_HEIGHT_PARAM_2.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_2.Set(height_altura2y4);
						WALL_USER_BASEOFFSET_PARAM_2.Set(altura2_window);

					}
					if (!WALL_USER_HEIGHT_PARAM_4.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_4.Set(altura1_window);
					}
					if (!WALL_USER_HEIGHT_PARAM_3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_3.Set(height_double);
					}

				}
				else
				{
					Wall wall_1 = e as Wall;

					Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					XYZ stPoint = wallCurve.Evaluate(stParam, false);
					XYZ endPoint = wallCurve.Evaluate(endParam, false);

					Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
					double longitud_double = longitud.AsDouble();

					List<int> lista1 = new List<int>();
					double end = (longitud_double / numero_final) * 304.8;

					for (int i = 0; i < end; i++)
					{
						if ((numero_final * i / 304.8) <= longitud_double)
						{
							lista1.Add(i);
						}
					}

					List<int> rango = lista1;
					int n1 = lista1.Last();

					double resto = longitud_double - n1 * (numero_final / 304.8);
					double r = resto;

					List<double> lista2 = new List<double>();
					List<XYZ> lista3 = new List<XYZ>();
					List<Line> lista4 = new List<Line>();


					foreach (int m in rango)
					{
						if (m < (rango.Count() - 1))
						{
							double Point = stParam + (numero_final / 304.8) * (m + 1);
							lista2.Add(Point);
							XYZ Point_point = wallCurve.Evaluate(Point, false);
							lista3.Add(Point_point);

						}
					}

					// CORREGIR WALL 1 EXISTENTE

					Line newLine1 = Line.CreateBound(stPoint, lista3.First());
					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

					((LocationCurve)wall_1.Location).Curve = newLine1;

					int nn = 1;

					Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					double height_double = height.AsDouble();

					foreach (XYZ p in lista3)
					{
						if (nn < lista3.Count())
						{
							int index = lista3.IndexOf(p);
							Line newLineN = Line.CreateBound(p, lista3[index + 1]);
							lista4.Add(newLineN);
						}
						else
						{
							continue;
						}
						nn = nn + 1;
					}

					// CREATE WALLS  Y  ARREGLAR LA ALTURA DE TODOS LOS MUROS

					foreach (Line newLineN in lista4)
					{
						Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

						Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM.Set(height_double);
						}
					}

					Line newLine_final = Line.CreateBound(lista3.Last(), endPoint);
					Wall wall_final = Wall.Create(doc, newLine_final, wall_1.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAM_final = wall_final.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM_final.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM_final.Set(height_double);
					}

				}

				trans.Commit();

				foreach (Wall wall in wallslibres)
				{
					splitWall_pick_122wall_elem_UI(wall, numero_final);
				}


			} // Sí funciona para todo

			void splitWall()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");

				FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);
				IList<ElementId> wallids = FEC.ToElementIds() as IList<ElementId>;

				foreach (ElementId wallid in wallids)
				{
					Element e = doc.GetElement(wallid);
					if (e.Location == null) continue;
					Wall wall_1 = e as Wall;

					Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					XYZ stPoint = wallCurve.Evaluate(stParam, false);
					XYZ endPoint = wallCurve.Evaluate(endParam, false);

					//assume split the wall in the middle point
					double midParam = (endParam - stParam) / 2.0;
					XYZ midPoint = wallCurve.Evaluate(midParam, false);

					if (wallCurve is Line)
					{
						Line newLine1 = Line.CreateBound(stPoint, midPoint);

						//becasue we will update endpoit of original wall,
						//check if the endpoint has been joined with another successive wall 
						//disallow the join, in order to update the endpoint
						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						//align original wall with the new curve
						((LocationCurve)wall_1.Location).Curve = newLine1;

						//because the successive wall will also update endpoint (its first segment)
						//create new wall on the second segment of the first wall directly
						//then the new wall and successive wall will be joined automatically 
						Line newLine2 = Line.CreateBound(midPoint, endPoint);
						Wall wall_2 = Wall.Create(doc, newLine2, wall_1.LevelId, false);

					}
					else if (wallCurve is Arc)
					{

					}
					else
					{
						//other types of Walls
					}
				}
				trans.Commit();
			}

			void splitWall_pick()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");

				FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);
				IList<ElementId> wallids = FEC.ToElementIds() as IList<ElementId>;

				//Element e = doc.GetElement(wallid);
				Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element));
				if (e.Location == null)
				{
					//continue;
				}
				Wall wall_1 = e as Wall;

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				//assume split the wall in the middle point
				double midParam = (endParam - stParam) / 2.0;
				XYZ midPoint = wallCurve.Evaluate(midParam, false);

				double param122 = stParam + 1.22;
				XYZ param122Point = wallCurve.Evaluate(param122, false);

				if (wallCurve is Line)
				{
					Line newLine1 = Line.CreateBound(stPoint, midPoint);

					//becasue we will update endpoit of original wall,
					//check if the endpoint has been joined with another successive wall 
					//disallow the join, in order to update the endpoint
					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

					//align original wall with the new curve
					((LocationCurve)wall_1.Location).Curve = newLine1;

					//because the successive wall will also update endpoint (its first segment)
					//create new wall on the second segment of the first wall directly
					//then the new wall and successive wall will be joined automatically 
					Line newLine2 = Line.CreateBound(midPoint, endPoint);
					Wall wall_2 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
					//               Parameter WALL_USER_HEIGHT_PARAM2 = wall_2.get_Parameter(BuiltInParameter.MATCHLINE_TOP_PLANE);
					//               WALL_USER_HEIGHT_PARAM2.SetValueString("Unconnected");

					Parameter WALL_USER_HEIGHT_PARAM = wall_2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

					if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
					{

						WALL_USER_HEIGHT_PARAM.Set(height_double);
					}

				}
				else if (wallCurve is Arc)
				{

				}
				else
				{
					//other types of Walls
				}

				trans.Commit();
			}

			void splitWall_pick_122wall()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");

				FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);
				IList<ElementId> wallids = FEC.ToElementIds() as IList<ElementId>;

				//Element e = doc.GetElement(wallid);
				Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element));
				if (e.Location == null)
				{
					//continue;
				}
				Wall wall_1 = e as Wall;

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble(); // 27,8871391076115

				//            int i = 0;
				List<int> lista1 = new List<int>();
				double end = (longitud_double / 1220) * 304.8; // = 6.96721311

				for (int i = 0; i < end; i++)
				{
					if ((1220 * i / 304.8) <= longitud_double)
					{
						lista1.Add(i);
						//var lista_rango = Enumerable.Range(0, i);
					}
				}


				//            while (1220*i <= longitud_double) 
				//            {
				//            	lista1.Add(i);
				//            	i = i + 1;
				//            }

				//            string msg = "first: " + end.ToString() + Environment.NewLine + "....................................";
				//            foreach (int elem in lista1) 
				//            {
				//            	msg =  msg + Environment.NewLine + elem.ToString() + Environment.NewLine;
				//            }
				//            TaskDialog.Show("salida", msg);


				List<int> rango = lista1; // [0,1,2,3,4,5,6]
				int n1 = lista1.Last(); // 6
										//            int n2 = i - 1; // 7-1 = 6
				double resto = longitud_double - n1 * (1220 / 304.8); // 27.65... - 24.015748 = 3....
				double r = resto; //  = 3....

				List<double> lista2 = new List<double>();
				List<XYZ> lista3 = new List<XYZ>();
				List<Line> lista4 = new List<Line>();

				//            string msg2 = n1.ToString() + "    :    " + Environment.NewLine + r.ToString();
				//            TaskDialog.Show("salida", msg2);

				foreach (int m in rango)
				{
					if (m < (rango.Count() - 1))
					{
						double Point = stParam + (1220 / 304.8) * (m + 1);
						lista2.Add(Point); // 6 items en la lista [Poin1, Point2, Point3, Point4, Point5, Point6]
						XYZ Point_point = wallCurve.Evaluate(Point, false);
						lista3.Add(Point_point);

					}
				}

				// CORREGIR WALL 1 EXISTENTE

				Line newLine1 = Line.CreateBound(stPoint, lista3.First());
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				((LocationCurve)wall_1.Location).Curve = newLine1;

				int nn = 1;

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				foreach (XYZ p in lista3)
				{
					if (nn < lista3.Count())
					{
						int index = lista3.IndexOf(p);
						Line newLineN = Line.CreateBound(p, lista3[index + 1]);
						lista4.Add(newLineN);
					}
					else
					{
						continue;
					}
					nn = nn + 1;
				}

				// CREATE WALLS  Y  ARREGLAR LA ALTURA DE TODOS LOS MUROS

				foreach (Line newLineN in lista4)
				{
					Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM.Set(height_double);
					}
				}

				Line newLine_final = Line.CreateBound(lista3.Last(), endPoint);
				Wall wall_final = Wall.Create(doc, newLine_final, wall_1.LevelId, false);

				Parameter WALL_USER_HEIGHT_PARAM_final = wall_final.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAM_final.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_final.Set(height_double);
				}

				//              Parameter WALL_USER_HEIGHT_PARAM2 = wall_2.get_Parameter(BuiltInParameter.MATCHLINE_TOP_PLANE);
				//               WALL_USER_HEIGHT_PARAM2.SetValueString("Unconnected");





				//assume split the wall in the middle point
				//            double midParam = (endParam - stParam) / 2.0;
				//            XYZ midPoint = wallCurve.Evaluate(midParam, false);

				//            double param122 = stParam + 1.22;
				//            XYZ param122Point = wallCurve.Evaluate(param122, false);

				//            if (wallCurve is Line)
				//            {
				//           		Line newLine1 = Line.CreateBound(stPoint, midPoint);
				//
				//                //becasue we will update endpoit of original wall,
				//                //check if the endpoint has been joined with another successive wall 
				//                //disallow the join, in order to update the endpoint
				//                if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
				//                    WallUtils.DisallowWallJoinAtEnd(wall_1, 1);
				//					
				//               //align original wall with the new curve
				//                    ((LocationCurve)wall_1.Location).Curve = newLine1;
				//
				//               //because the successive wall will also update endpoint (its first segment)
				//               //create new wall on the second segment of the first wall directly
				//               //then the new wall and successive wall will be joined automatically 
				//               Line newLine2 = Line.CreateBound(midPoint, endPoint);
				//               Wall wall_2 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
				////               Parameter WALL_USER_HEIGHT_PARAM2 = wall_2.get_Parameter(BuiltInParameter.MATCHLINE_TOP_PLANE);
				////               WALL_USER_HEIGHT_PARAM2.SetValueString("Unconnected");
				//               
				//               Parameter WALL_USER_HEIGHT_PARAM = wall_2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				//        
				//               if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
				//               {
				//               	
				//               	WALL_USER_HEIGHT_PARAM.Set(height_double);
				//               }
				//               
				//            }
				//            else if (wallCurve is Arc)
				//            {
				//
				//            }
				//            else
				//            {
				//                //other types of Walls
				//            }

				trans.Commit();
			} // divide en parte igual wall sin ventanas

			void splitWall_pick_122wall_elem(Element e)
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");

				FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);
				IList<ElementId> wallids = FEC.ToElementIds() as IList<ElementId>;

				if (e.Location == null)
				{
					//continue;
				}
				Wall wall_1 = e as Wall;

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble(); // 27,8871391076115

				//            int i = 0;
				List<int> lista1 = new List<int>();
				double end = (longitud_double / 1220) * 304.8; // = 6.96721311

				for (int i = 0; i < end; i++)
				{
					if ((1220 * i / 304.8) <= longitud_double)
					{
						lista1.Add(i);

					}
				}


				List<int> rango = lista1; // [0,1,2,3,4,5,6]
				int n1 = lista1.Last(); // 6
										//            int n2 = i - 1; // 7-1 = 6
				double resto = longitud_double - n1 * (1220 / 304.8); // 27.65... - 24.015748 = 3....
				double r = resto; //  = 3....

				List<double> lista2 = new List<double>();
				List<XYZ> lista3 = new List<XYZ>();
				List<Line> lista4 = new List<Line>();



				foreach (int m in rango)
				{
					if (m < (rango.Count() - 1))
					{
						double Point = stParam + (1220 / 304.8) * (m + 1);
						lista2.Add(Point); // 6 items en la lista [Poin1, Point2, Point3, Point4, Point5, Point6]
						XYZ Point_point = wallCurve.Evaluate(Point, false);
						lista3.Add(Point_point);

					}
				}

				// CORREGIR WALL 1 EXISTENTE

				Line newLine1 = Line.CreateBound(stPoint, lista3.First());
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				((LocationCurve)wall_1.Location).Curve = newLine1;

				int nn = 1;

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				foreach (XYZ p in lista3)
				{
					if (nn < lista3.Count())
					{
						int index = lista3.IndexOf(p);
						Line newLineN = Line.CreateBound(p, lista3[index + 1]);
						lista4.Add(newLineN);
					}
					else
					{
						continue;
					}
					nn = nn + 1;
				}

				// CREATE WALLS  Y  ARREGLAR LA ALTURA DE TODOS LOS MUROS

				foreach (Line newLineN in lista4)
				{
					Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM.Set(height_double);
					}
				}

				Line newLine_final = Line.CreateBound(lista3.Last(), endPoint);
				Wall wall_final = Wall.Create(doc, newLine_final, wall_1.LevelId, false);

				Parameter WALL_USER_HEIGHT_PARAM_final = wall_final.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAM_final.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_final.Set(height_double);
				}


				trans.Commit();
			}

			void splitWall_pick_122wall_elem_UI(Element e, double numero)
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				double num = numero; //"1220"

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");

				FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);
				IList<ElementId> wallids = FEC.ToElementIds() as IList<ElementId>;

				if (e.Location == null)
				{
					//continue;
				}
				Wall wall_1 = e as Wall;

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble(); // 27,8871391076115

				//            int i = 0;
				List<int> lista1 = new List<int>();
				double end = (longitud_double / num) * 304.8; // = 6.96721311

				for (int i = 0; i < end; i++)
				{
					if ((num * i / 304.8) <= longitud_double)
					{
						lista1.Add(i);

					}
				}


				List<int> rango = lista1; // [0,1,2,3,4,5,6]
				int n1 = lista1.Last(); // 6
										//            int n2 = i - 1; // 7-1 = 6
				double resto = longitud_double - n1 * (num / 304.8); // 27.65... - 24.015748 = 3....
				double r = resto; //  = 3....

				List<double> lista2 = new List<double>();
				List<XYZ> lista3 = new List<XYZ>();
				List<Line> lista4 = new List<Line>();



				foreach (int m in rango)
				{
					if (m < (rango.Count() - 1))
					{
						double Point = stParam + (num / 304.8) * (m + 1);
						lista2.Add(Point); // 6 items en la lista [Poin1, Point2, Point3, Point4, Point5, Point6]
						XYZ Point_point = wallCurve.Evaluate(Point, false);
						lista3.Add(Point_point);

					}
				}

				// CORREGIR WALL 1 EXISTENTE

				Line newLine1 = Line.CreateBound(stPoint, lista3.First());
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				((LocationCurve)wall_1.Location).Curve = newLine1;

				int nn = 1;

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				foreach (XYZ p in lista3)
				{
					if (nn < lista3.Count())
					{
						int index = lista3.IndexOf(p);
						Line newLineN = Line.CreateBound(p, lista3[index + 1]);
						lista4.Add(newLineN);
					}
					else
					{
						continue;
					}
					nn = nn + 1;
				}

				// CREATE WALLS  Y  ARREGLAR LA ALTURA DE TODOS LOS MUROS

				foreach (Line newLineN in lista4)
				{
					Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAM = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAM.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAM.Set(height_double);
					}
				}

				Line newLine_final = Line.CreateBound(lista3.Last(), endPoint);
				Wall wall_final = Wall.Create(doc, newLine_final, wall_1.LevelId, false);

				Parameter WALL_USER_HEIGHT_PARAM_final = wall_final.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAM_final.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_final.Set(height_double);
				}


				trans.Commit();
			}

			void splitWall_pick_122wall_con1Vano()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");


				Reference ro = uidoc.Selection.PickObject(ObjectType.Element, "please select wall");
				Element e = doc.GetElement(ro); // EL MURO SELECCIONADO
				ElementId eID = e.Id;

				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				FilteredElementCollector coll = new FilteredElementCollector(doc);

				IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements();


				//		 	IEnumerable<Element> associate = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Where(m=>(m as FamilyInstance).Host.Id  == e.Id);
				string msg0 = windows.Count().ToString() + Environment.NewLine;
				List<Element> windows_hosted = new List<Element>();
				foreach (Element elem in windows)
				{
					FamilyInstance fintance = elem as FamilyInstance;
					//double a = fintance.HostParameter;
					if (fintance.Host.Id == e.Id)
					{
						windows_hosted.Add(elem);
					}
					//msg0 = msg0 + fintance.Host.Id.ToString() + Environment.NewLine + elem.Name.ToString() + Environment.NewLine;
				}

				FamilyInstance window = windows_hosted.First() as FamilyInstance;
				LocationPoint window_point = window.Location as LocationPoint;
				XYZ window_point_xyz = window_point.Point;
				string window_point_s = window_point_xyz.ToString();

				TaskDialog.Show("salida 1", window_point_s);


				double a = window.HostParameter;
				Element window_elem = windows_hosted.First();
				ElementType type = doc.GetElement(window_elem.GetTypeId()) as ElementType;
				Parameter widthParam = type.LookupParameter("Width");
				Parameter heightParam = type.LookupParameter("Height");
				double width = widthParam.AsDouble();
				double heigth = heightParam.AsDouble();
				double altura1_window = window_elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
				double altura2_window = window_elem.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040


				Wall wall_1 = e as Wall; // e = MURO SELECCIONADO
				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();
				double height_altura2y4 = height_double - (altura1_window + heigth); // 400

				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				double PHOST = a;
				double PA = a - width / 2;
				double PB = a + width / 2;

				XYZ PHOST_point = wallCurve.Evaluate(PHOST, false);
				XYZ PA_point = wallCurve.Evaluate(PA, false);
				XYZ PB_point = wallCurve.Evaluate(PB, false);

				Line newLine1 = Line.CreateBound(stPoint, PA_point);

				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				Line newLine2 = Line.CreateBound(PA_point, PB_point);
				Line newLine3 = Line.CreateBound(PB_point, endPoint);

				// eliminar window
				//			using (Transaction t = new Transaction(doc, "Delete Window"))
				//			{
				//				t.Start();
				//				
				//				t.Commit();
				//			}
				doc.Delete(window_elem.Id);

				((LocationCurve)wall_1.Location).Curve = newLine1;

				Wall wall_2 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
				Wall wall_4 = Wall.Create(doc, newLine2, wall_1.LevelId, false);
				Wall wall_3 = Wall.Create(doc, newLine3, wall_1.LevelId, false);




				Parameter WALL_USER_HEIGHT_PARAM_2 = wall_2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				Parameter WALL_USER_HEIGHT_PARAM_4 = wall_4.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				Parameter WALL_USER_HEIGHT_PARAM_3 = wall_3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_2.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				Parameter WALL_USER_BASEOFFSET_PARAM_4 = wall_4.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

				if (!WALL_USER_HEIGHT_PARAM_2.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_2.Set(height_altura2y4);
					WALL_USER_BASEOFFSET_PARAM_2.Set(altura2_window);

				}
				if (!WALL_USER_HEIGHT_PARAM_4.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_4.Set(altura1_window);
					//WALL_USER_BASEOFFSET_PARAM_4.Set(0);
				}
				if (!WALL_USER_HEIGHT_PARAM_3.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAM_3.Set(height_double);
				}




				//msg0 = msg0 + Environment.NewLine + width.ToString();
				//			foreach (Element i in windows_hosted) 
				//			{
				//				msg0 = msg0 + i.Id.ToString() + Environment.NewLine + "HostParameter: " + (a*304.3957244284934).ToString();
				//			}
				//TaskDialog.Show("salida", msg0);

				trans.Commit();
				splitWall_pick_122wall_elem(wall_3);
				splitWall_pick_122wall_elem(wall_1);

			} // divide cuando solo tiene 1 ventana

			void splitWall_pick_122wall_con3Vanos()
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Transaction trans = new Transaction(doc);
				trans.Start("mysplitwall");


				Reference ro = uidoc.Selection.PickObject(ObjectType.Element, "please select wall");
				Element e = doc.GetElement(ro); // EL MURO SELECCIONADO
				ElementId eID = e.Id;

				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				FilteredElementCollector coll = new FilteredElementCollector(doc);

				IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas

				//		 	IEnumerable<Element> associate = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Where(m=>(m as FamilyInstance).Host.Id  == e.Id);

				List<Element> windows_hosted = new List<Element>();

				foreach (Element elem in windows)
				{
					FamilyInstance fintance = elem as FamilyInstance;
					//double a = fintance.HostParameter;
					if (fintance.Host.Id == e.Id)
					{
						windows_hosted.Add(elem);
					}
					//msg0 = msg0 + fintance.Host.Id.ToString() + Environment.NewLine + elem.Name.ToString() + Environment.NewLine;
				}

				if (windows_hosted.Count() == 3)
				{
					string msg0 = windows_hosted.Count().ToString() + Environment.NewLine;
					List<XYZ> listPuntos_ACE = new List<XYZ>();
					List<XYZ> listPuntos_BDF = new List<XYZ>();

					List<double> listDoubles = new List<double>();

					List<Line> listLines = new List<Line>();
					List<Line> listLines_murosolos = new List<Line>();

					List<Line> listLines_pares = new List<Line>();
					List<Line> listLines_impares = new List<Line>();

					Wall wall_I = e as Wall; // e = MURO SELECCIONADO
					Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;
					Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
					double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					//				List<double> sill_heights = new List<double>();
					//				List<double> head_heights = new List<double>();


					FamilyInstance win1 = windows_hosted[0] as FamilyInstance;
					FamilyInstance win2 = windows_hosted[1] as FamilyInstance;
					FamilyInstance win3 = windows_hosted[2] as FamilyInstance;

					double dPH1 = win1.HostParameter;
					double dPH2 = win2.HostParameter;
					double dPH3 = win3.HostParameter;

					ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
					ElementType type2 = doc.GetElement(win2.GetTypeId()) as ElementType;
					ElementType type3 = doc.GetElement(win3.GetTypeId()) as ElementType;

					Parameter widthParam1 = type1.LookupParameter("Width"); // ancho 1220
					Parameter widthParam2 = type2.LookupParameter("Width"); // ancho 1220
					Parameter widthParam3 = type3.LookupParameter("Width"); // ancho 1220

					Parameter heightParam1 = type1.LookupParameter("Height"); // alto 1240
					Parameter heightParam2 = type2.LookupParameter("Height"); // alto 1240
					Parameter heightParam3 = type3.LookupParameter("Height"); // alto 1240

					double width1 = widthParam1.AsDouble(); // 1220
					double width2 = widthParam2.AsDouble(); // 1220
					double width3 = widthParam3.AsDouble(); // 1220

					double heigth1 = heightParam1.AsDouble(); // 1240
					double heigth2 = heightParam2.AsDouble(); // 1240
					double heigth3 = heightParam3.AsDouble(); // 1240

					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
					double win_sill_height2 = win2.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
					double win_sill_height3 = win3.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800

					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040
					double win_head_height2 = win2.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040
					double win_head_height3 = win3.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

					//				sill_heights.Add(win_sill_height);
					//				head_heights.Add(win_head_height);

					double dPA1 = dPH1 - width1 / 2; // 
					double dPB1 = dPH1 + width1 / 2;
					double dPA2 = dPH2 - width2 / 2;
					double dPB2 = dPH2 + width2 / 2;
					double dPA3 = dPH3 - width3 / 2;
					double dPB3 = dPH3 + width3 / 2;

					listDoubles.Add(stParam);
					listDoubles.Add(dPA1);
					listDoubles.Add(dPB1);
					listDoubles.Add(dPA2);
					listDoubles.Add(dPB2);
					listDoubles.Add(dPA3);
					listDoubles.Add(dPB3);
					listDoubles.Add(endParam);

					listDoubles.Sort();

					double stprimero = listDoubles[0];
					double segundo = listDoubles[1];
					double tercero = listDoubles[2];
					double cuarto = listDoubles[3];
					double quinto = listDoubles[4];
					double sexto = listDoubles[5];
					double septimo = listDoubles[6];
					double endoctavo = listDoubles[7];

					XYZ stPoint = wallCurve.Evaluate(stprimero, false);

					XYZ PA1 = wallCurve.Evaluate(segundo, false);
					XYZ PB1 = wallCurve.Evaluate(tercero, false);
					XYZ PA2 = wallCurve.Evaluate(cuarto, false);
					XYZ PB2 = wallCurve.Evaluate(quinto, false);
					XYZ PA3 = wallCurve.Evaluate(sexto, false);
					XYZ PB3 = wallCurve.Evaluate(septimo, false);

					XYZ endPoint = wallCurve.Evaluate(endoctavo, false);

					//				listPuntos_ACE.Add(PA);
					//				listPuntos_BDF.Add(PB);

					//				msg0 = msg0 + "Distancia HosParamter: " + dPH1.ToString() + Environment.NewLine + "dPA = " + dPA.ToString() + " / dPB = " + dPB.ToString() + Environment.NewLine;
					msg0 = msg0 + Environment.NewLine + "stParam = " + stprimero.ToString() + " : " + segundo.ToString() + " : " + tercero.ToString() + " : " + cuarto.ToString() + " : "
						+ quinto.ToString() + " : " +
						sexto.ToString() + " : " + septimo.ToString() + " : " + " endParam = " + endoctavo.ToString();

					TaskDialog.Show("salida", msg0);

					//				listPuntos_ACE.Add(endPoint);

					Line newLineI = Line.CreateBound(stPoint, PA1); // 1 Linea

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);
					//listLines.Add(newLineI);


					Line newLine2 = Line.CreateBound(PA1, PB1);
					Line newLine3 = Line.CreateBound(PB1, PA2);
					Line newLine4 = Line.CreateBound(PA2, PB2);
					Line newLine5 = Line.CreateBound(PB2, PA3);
					Line newLine6 = Line.CreateBound(PA3, PB3); // 5 lineas

					Line newLineF = Line.CreateBound(PB3, endPoint); // 1 linea

					foreach (Element win in windows_hosted)
					{
						doc.Delete(win.Id);
					}

					// CREAAR MUROS

					((LocationCurve)wall_I.Location).Curve = newLineI; // Wall_I


					Wall wall_2 = Wall.Create(doc, newLine2, wall_I.LevelId, false); // Wall_2
					Wall wall_2_copy = Wall.Create(doc, newLine2, wall_I.LevelId, false); // Wall_2

					Wall wall_3 = Wall.Create(doc, newLine3, wall_I.LevelId, false); // Wall_3

					Wall wall_4 = Wall.Create(doc, newLine4, wall_I.LevelId, false); // Wall_4
					Wall wall_4_copy = Wall.Create(doc, newLine4, wall_I.LevelId, false); // Wall_4

					Wall wall_5 = Wall.Create(doc, newLine5, wall_I.LevelId, false); // Wall_5

					Wall wall_6 = Wall.Create(doc, newLine6, wall_I.LevelId, false); // Wall_6
					Wall wall_6_copy = Wall.Create(doc, newLine6, wall_I.LevelId, false); // Wall_4


					Wall wall_F = Wall.Create(doc, newLineF, wall_I.LevelId, false); // Wall_F

					Parameter WALL_USER_HEIGHT_PARAM2 = wall_2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_HEIGHT_PARAM2_copy = wall_2_copy.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_BASEOFFSET_PARAM2 = wall_2.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					Parameter WALL_USER_BASEOFFSET_PARAM2_copy = wall_2_copy.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

					Parameter WALL_USER_HEIGHT_PARAM3 = wall_3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

					Parameter WALL_USER_HEIGHT_PARAM4 = wall_4.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_HEIGHT_PARAM4_copy = wall_4_copy.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_BASEOFFSET_PARAM4 = wall_4.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					Parameter WALL_USER_BASEOFFSET_PARAM4_copy = wall_4_copy.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);

					Parameter WALL_USER_HEIGHT_PARAM5 = wall_5.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

					Parameter WALL_USER_HEIGHT_PARAM6 = wall_6.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_HEIGHT_PARAM6_copy = wall_6_copy.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					Parameter WALL_USER_BASEOFFSET_PARAM6 = wall_6.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
					Parameter WALL_USER_BASEOFFSET_PARAM6_copy = wall_6_copy.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);


					Parameter WALL_USER_HEIGHT_PARAMF = wall_F.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_2, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_2, 1);
					//sill_heights[n]; // [800, 800, 800]
					//head_heights[n]; // [2040, 2040, 2040]

					if (!WALL_USER_HEIGHT_PARAM2.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAM2.Set(height_double - win_head_height1); // 400
						WALL_USER_BASEOFFSET_PARAM2.Set(win_head_height1); // 2040
						WALL_USER_HEIGHT_PARAM2_copy.Set(win_sill_height1); // 800
																			//WALL_USER_BASEOFFSET_PARAM2_copy.Set(0); // 800

					}
					if (!WALL_USER_HEIGHT_PARAM3.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAM3.Set(height_double);
					}
					if (!WALL_USER_HEIGHT_PARAM4.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAM4.Set(height_double - win_head_height2); // 400
						WALL_USER_BASEOFFSET_PARAM4.Set(win_head_height2); // 2040
						WALL_USER_HEIGHT_PARAM4_copy.Set(win_sill_height2); // 800
																			//WALL_USER_BASEOFFSET_PARAM4_copy.Set(0); // 800
					}
					if (!WALL_USER_HEIGHT_PARAM5.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAM5.Set(height_double);
					}
					if (!WALL_USER_HEIGHT_PARAM6.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAM6.Set(height_double - win_head_height3); // 400
						WALL_USER_BASEOFFSET_PARAM6.Set(win_head_height3); // 2040
						WALL_USER_HEIGHT_PARAM6_copy.Set(win_sill_height3); // 800
																			//WALL_USER_BASEOFFSET_PARAM6_copy.Set(0); // 800
					}
					if (!WALL_USER_HEIGHT_PARAMF.IsReadOnly) // height_double = 2440
					{
						WALL_USER_HEIGHT_PARAMF.Set(height_double);
					}
				}

				trans.Commit();

			} // si funciona

			
			#endregion
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
