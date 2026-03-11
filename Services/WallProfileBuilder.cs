using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace SplitWalls
{
	internal class WallProfileBuilder
	{
		private readonly Document _doc;

		internal WallProfileBuilder(Document doc)
		{
			_doc = doc;
		}

		private Wall ReplaceWallWithProfile(Wall source, IList<Curve> profile)
		{
			using (Transaction t = new Transaction(_doc, "wall"))
			{
				t.Start();
				Wall w = Wall.Create(_doc, profile, source.WallType.Id, source.LevelId, true);
				WallJoinHelper.DisableJoins(w);
				_doc.Delete(source.Id);
				t.Commit();
				return w;
			}
		}

		public Wall BuildU_DoorWindowRight(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Point_dVIo1 = _Point_dVIo_1;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;
			// INPUTS


			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, stPoint.Z + sill_v + hv);
			XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo1.X, Nuevo_Point_dVIo1.Y, stPoint.Z + sill_v + hv);


			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2);
			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2 + alturaventana2);

			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);
			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);

			IList<Curve> profile = new List<Curve>();


				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo1); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo1, Point_dVIo_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVIo_arriba, Point_dVFo_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVFo_arriba, Nuevo_Point_dVFo1); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo1, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, esquina1_abajo); // 1 Linea
				Line linea7 = Line.CreateBound(esquina1_abajo, Point_dVI2_abajo); // 1 Linea
				Line linea8 = Line.CreateBound(Point_dVI2_abajo, Point_dVI2_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(Point_dVI2_arriba, esquina1_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea11 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea12 = Line.CreateBound(stPoint_arriba, stPoint); // 1 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);


				//		        curve = new CurveArray();


				//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall BuildU_DoorDoorRight(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Point_dVIo1 = _Point_dVIo_1;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;
			// INPUTS


			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, stPoint.Z + sill_v + hv);
			XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo1.X, Nuevo_Point_dVIo1.Y, stPoint.Z + sill_v + hv);


			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2);
			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2 + alturaventana2);

			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);
			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);

			IList<Curve> profile = new List<Curve>();


				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo1); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo1, Point_dVIo_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVIo_arriba, Point_dVFo_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVFo_arriba, Nuevo_Point_dVFo1); // 1 Linea

				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo1, Nuevo_Point_dVIo2); // 1 Linea
				Line linea6 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea7 = Line.CreateBound(Point_dVI2_arriba, esquina1_arriba); // 1 Linea
				Line linea8 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(stPoint_arriba, stPoint); // 1 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);


				//		        curve = new CurveArray();


				//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall BuildU_DoorWindowLeft(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);
			//			XYZ Point_dVIo1 = _Point_dVIo_1;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo2 = _Point_dVFo_2;

			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			IList<Curve> profile = new List<Curve>();


				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, endPoint_arriba); // 1 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(Point_dVF1_arriba, Point_dVF1_abajo); // 1 Linea
				Line linea11 = Line.CreateBound(Point_dVF1_abajo, esquina2_abajo); // 1 Linea
				Line linea12 = Line.CreateBound(esquina2_abajo, stPoint); // 1 Linea

				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall BuildU_DoorDoorLeft(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);
			//			XYZ Point_dVIo1 = _Point_dVIo_1;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo2 = _Point_dVFo_2;

			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo1, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, endPoint_arriba); // 1 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(Point_dVF1_arriba, Nuevo_Point_dVFo1); // 1 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall Build3Opening_WindowDoorWindow(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  double _alturaventana_3,
																  double _sillventanda_3,
																	XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2,
																 XYZ _Point_dVIo_3,
																XYZ _Point_dVFo_3)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);

			XYZ Point_dVIo1 = _Point_dVIo_1;
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, endPoint.Z);

			XYZ Point_dVFo2 = _Point_dVFo_2;
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);

			XYZ Point_dVFo3 = _Point_dVFo_3;
			XYZ Nuevo_Point_dVFo3 = new XYZ(Point_dVFo3.X, Point_dVFo3.Y, endPoint.Z);

			XYZ Point_dVIo3 = _Point_dVIo_3;
			XYZ Nuevo_Point_dVIo3 = new XYZ(Point_dVIo3.X, Point_dVIo3.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;

			double alturaventana3 = _alturaventana_3;
			double sillventanda3 = _sillventanda_3;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);

			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_abajo = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2);


			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3 + alturaventana3);
			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			XYZ Point_dVIo1_arriba = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVIo1_abajo = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVIo3_arriba = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3 + alturaventana3);
			XYZ Point_dVIo3_abajo = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3);


			IList<Curve> profile = new List<Curve>();
				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, esquina1_abajo); // 1 Linea
				Line linea7 = Line.CreateBound(esquina1_abajo, Point_dVIo3_abajo); // 1 Linea
				Line linea8 = Line.CreateBound(Point_dVIo3_abajo, Point_dVIo3_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(Point_dVIo3_arriba, esquina1_arriba); // 1 Linea		        
				Line linea10 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea11 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea12 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea13 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea
				Line linea14 = Line.CreateBound(Point_dVF1_arriba, Point_dVF1_abajo); // 1 Linea
				Line linea15 = Line.CreateBound(Point_dVF1_abajo, esquina2_abajo); // 1 Linea				
				Line linea16 = Line.CreateBound(esquina2_abajo, stPoint); // 1 Linea					


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);
				profile.Add(linea13);
				profile.Add(linea14);
				profile.Add(linea15);
				profile.Add(linea16);

			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall Build3Opening_WindowDoorDoor(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  double _alturaventana_3,
																  double _sillventanda_3,
																	XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2,
																 XYZ _Point_dVIo_3,
																XYZ _Point_dVFo_3)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);

			XYZ Point_dVIo1 = _Point_dVIo_1;
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, endPoint.Z);

			XYZ Point_dVFo2 = _Point_dVFo_2;
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);

			XYZ Point_dVFo3 = _Point_dVFo_3;
			XYZ Nuevo_Point_dVFo3 = new XYZ(Point_dVFo3.X, Point_dVFo3.Y, endPoint.Z);

			XYZ Point_dVIo3 = _Point_dVIo_3;
			XYZ Nuevo_Point_dVIo3 = new XYZ(Point_dVIo3.X, Point_dVIo3.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;

			double alturaventana3 = _alturaventana_3;
			double sillventanda3 = _sillventanda_3;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);

			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_abajo = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2);


			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3 + alturaventana3);
			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			XYZ Point_dVIo1_arriba = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVIo1_abajo = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVIo3_arriba = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3 + alturaventana3);
			XYZ Point_dVIo3_abajo = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, Nuevo_Point_dVIo3); // 1 Linea
				Line linea6 = Line.CreateBound(Nuevo_Point_dVIo3, Point_dVIo3_arriba); // 1 Linea
				Line linea7 = Line.CreateBound(Point_dVIo3_arriba, esquina1_arriba); // 1 Linea
				Line linea8 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea11 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea
				Line linea12 = Line.CreateBound(Point_dVF1_arriba, Point_dVF1_abajo); // 1 Linea
				Line linea13 = Line.CreateBound(Point_dVF1_abajo, esquina2_abajo); // 1 Linea				
				Line linea14 = Line.CreateBound(esquina2_abajo, stPoint); // 1 Linea					


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);
				profile.Add(linea13);
				profile.Add(linea14);


			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall Build3Opening_DoorDoorWindow(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  double _alturaventana_3,
																  double _sillventanda_3,
																	XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2,
																 XYZ _Point_dVIo_3,
																XYZ _Point_dVFo_3)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);

			XYZ Point_dVIo1 = _Point_dVIo_1;
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, endPoint.Z);

			XYZ Point_dVFo2 = _Point_dVFo_2;
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);

			XYZ Point_dVFo3 = _Point_dVFo_3;
			XYZ Nuevo_Point_dVFo3 = new XYZ(Point_dVFo3.X, Point_dVFo3.Y, endPoint.Z);

			XYZ Point_dVIo3 = _Point_dVIo_3;
			XYZ Nuevo_Point_dVIo3 = new XYZ(Point_dVIo3.X, Point_dVIo3.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;

			double alturaventana3 = _alturaventana_3;
			double sillventanda3 = _sillventanda_3;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);

			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_abajo = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2);


			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3 + alturaventana3);
			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			XYZ Point_dVIo1_arriba = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVIo1_abajo = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVIo3_arriba = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3 + alturaventana3);
			XYZ Point_dVIo3_abajo = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo1, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, esquina1_abajo); // 1 Linea
				Line linea7 = Line.CreateBound(esquina1_abajo, Point_dVIo3_abajo); // 1 Linea
				Line linea8 = Line.CreateBound(Point_dVIo3_abajo, Point_dVIo3_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(Point_dVIo3_arriba, esquina1_arriba); // 1 Linea		        
				Line linea10 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea11 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea12 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea13 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea			
				Line linea14 = Line.CreateBound(Point_dVF1_arriba, Nuevo_Point_dVFo1); // 1 Linea					


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);
				profile.Add(linea13);
				profile.Add(linea14);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall Build3Opening_DoorDoorDoor(Wall _wall_,
																  double _alturaventana_1,
																  double _sillventanda_1,
																  double _alturaventana_2,
																  double _sillventanda_2,
																  double _alturaventana_3,
																  double _sillventanda_3,
																	XYZ _Point_dVIo_1,
																  XYZ _Point_dVFo_1,
																  XYZ _Point_dVIo_2,
																  XYZ _Point_dVFo_2,
																 XYZ _Point_dVIo_3,
																XYZ _Point_dVFo_3)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo1 = _Point_dVFo_1;
			XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, endPoint.Z);

			XYZ Point_dVIo1 = _Point_dVIo_1;
			XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, endPoint.Z);

			XYZ Point_dVFo2 = _Point_dVFo_2;
			XYZ Nuevo_Point_dVFo2 = new XYZ(Point_dVFo2.X, Point_dVFo2.Y, endPoint.Z);

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Nuevo_Point_dVIo2 = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, endPoint.Z);

			XYZ Point_dVFo3 = _Point_dVFo_3;
			XYZ Nuevo_Point_dVFo3 = new XYZ(Point_dVFo3.X, Point_dVFo3.Y, endPoint.Z);

			XYZ Point_dVIo3 = _Point_dVIo_3;
			XYZ Nuevo_Point_dVIo3 = new XYZ(Point_dVIo3.X, Point_dVIo3.Y, endPoint.Z);


			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;

			double alturaventana3 = _alturaventana_3;
			double sillventanda3 = _sillventanda_3;
			// INPUTS


			double hv = alturaventana1;
			double sill_v = sillventanda1;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);

			XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);
			XYZ Point_dVF2_abajo = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2);


			XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

			XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3 + alturaventana3);
			XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda3);

			XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

			XYZ Point_dVIo1_arriba = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1 + alturaventana1);
			XYZ Point_dVIo1_abajo = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z + sillventanda1);

			XYZ Point_dVIo3_arriba = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3 + alturaventana3);
			XYZ Point_dVIo3_abajo = new XYZ(Nuevo_Point_dVIo3.X, Nuevo_Point_dVIo3.Y, stPoint.Z + sillventanda3);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo1, Nuevo_Point_dVIo2); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo2, Point_dVI2_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVI2_arriba, Point_dVF2_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVF2_arriba, Nuevo_Point_dVFo2); // 1 Linea

				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo2, Nuevo_Point_dVIo3); // 1 Linea
				Line linea6 = Line.CreateBound(Nuevo_Point_dVIo3, Point_dVIo3_arriba); // 1 Linea

				Line linea7 = Line.CreateBound(Point_dVIo3_arriba, esquina1_arriba); // 1 Linea		        
				Line linea8 = Line.CreateBound(esquina1_arriba, endPoint_arriba); // 1 Linea
				Line linea9 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea10 = Line.CreateBound(stPoint_arriba, esquina2_arriba); // 1 Linea
				Line linea11 = Line.CreateBound(esquina2_arriba, Point_dVF1_arriba); // 1 Linea			
				Line linea12 = Line.CreateBound(Point_dVF1_arriba, Nuevo_Point_dVFo1); // 1 Linea					


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);


			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildEdgeDoorRight(Wall _wall_, double _alturaventana_1,
																			 double _anchoventana_1,
																			double _sillventanda_1,
																			 XYZ _Point_dVIo_1,
																			 XYZ _Point_dVFo_1,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo = _Point_dVIo_1;
			XYZ Point_dVFo = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;
			double anchoventana1 = _anchoventana_1;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sillventanda1 + alturaventana1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190


			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sillventanda1 + alturaventana1);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo, Point_arriba_DVIo); // 2 Linea
				Line linea3 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_DVFo); // 3 Linea
				Line linea4 = Line.CreateBound(Point_arriba_DVFo, endPoint_arriba); // 4 Linea

				Line linea5 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(stPoint_arriba, stPoint); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall BuildEdgeDoorLeft(Wall _wall_, double _alturaventana_1,
																			 double _anchoventana_1,
																			double _sillventanda_1,
																			 XYZ _Point_dVIo_1,
																			 XYZ _Point_dVFo_1,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo = _Point_dVIo_1;
			XYZ Point_dVFo = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;
			double anchoventana1 = _anchoventana_1;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sillventanda1 + alturaventana1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190


			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sillventanda1);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sillventanda1 + alturaventana1);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 2 Linea
				Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea
				Line linea4 = Line.CreateBound(stPoint_arriba, Point_arriba_DVIo); // 4 Linea

				Line linea5 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_DVFo); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_DVFo, Nuevo_Point_dVFo); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);


			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildU_Door(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_, XYZ _Point_dVFo_, XYZ _Point_dPH_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			XYZ Point_dVFo = _Point_dVFo_;
			XYZ Point_dVIo = _Point_dVIo_;

			XYZ Point_dPH = _Point_dPH_;
			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS


			double distacia_centerToLeft = Nueva_Point_dPH.DistanceTo(stPoint);
			double distancia_centerToBottom = stPoint.Z + sillventanda + alturaventana / 2;


			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);


			double hv = alturaventana;
			double sill_v = sillventanda;
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

			XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo.X, Nuevo_Point_dVFo.Y, stPoint.Z + sill_v + hv);
			XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo.X, Nuevo_Point_dVIo.Y, stPoint.Z + sill_v + hv);


			IList<Curve> profile = new List<Curve>();


				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo, Point_dVIo_arriba); // 1 Linea
				Line linea3 = Line.CreateBound(Point_dVIo_arriba, Point_dVFo_arriba); // 1 Linea
				Line linea4 = Line.CreateBound(Point_dVFo_arriba, Nuevo_Point_dVFo); // 1 Linea
				Line linea5 = Line.CreateBound(Nuevo_Point_dVFo, endPoint); // 1 Linea
				Line linea6 = Line.CreateBound(endPoint, endPoint_arriba); // 1 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, stPoint); // 1 Linea

				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);


				//		        curve = new CurveArray();


				//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);


			return ReplaceWallWithProfile(wall_I, profile);

		}

		public Wall BuildT_Door(Wall _wall_, double _alturaventana_1,
																			 double _alturaventana_2,
																			 double _sillventanda_1,
																			 double _sillventanda_2,
																			 XYZ _Point_dVFo_1,
																			XYZ _Point_dVIo_2,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo1 = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1 + hv1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1 + hv1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190

			double hv2 = alturaventana2;
			double sill_v2 = sillventanda2;

			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2 + hv2);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2 + hv2);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo, Nuevo_Point_dVIo); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo, Point_arriba_DVIo); // 2 Linea
				Line linea3 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_Esquina1); // 3 Linea
				Line linea4 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 4 Linea
				Line linea5 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 6 Linea
				Line linea7 = Line.CreateBound(Point_arriba_Esquina2, Point_arriba_DVFo); // 3 Linea

				Line linea8 = Line.CreateBound(Point_arriba_DVFo, Nuevo_Point_dVFo); // 4 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);


			return ReplaceWallWithProfile(wall_I, profile);
		} // forma T

		public Wall BuildI_DoorLeft(Wall _wall_, double _alturaventana_1,
																			 double _alturaventana_2,
																			 double _sillventanda_1,
																			 double _sillventanda_2,
																			 XYZ _Point_dVIo_2,
																			 XYZ _Point_dVFo_1,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo1 = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1 + hv1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1 + hv1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190

			double hv2 = alturaventana2;
			double sill_v2 = sillventanda2;

			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2 + hv2);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2 + hv2);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo, Point_arriba_DVIo); // 2 Linea
				Line linea3 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_Esquina1); // 3 Linea
				Line linea4 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 4 Linea

				Line linea5 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 6 Linea
				Line linea7 = Line.CreateBound(Point_arriba_Esquina2, Point_arriba_DVFo); // 7 Linea
				Line linea8 = Line.CreateBound(Point_arriba_DVFo, Point_bajo_DVFo); // 8 Linea
				Line linea9 = Line.CreateBound(Point_bajo_DVFo, Point_bajo_Esquina2); // 9 Linea
				Line linea10 = Line.CreateBound(Point_bajo_Esquina2, stPoint); // 10 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);


			return ReplaceWallWithProfile(wall_I, profile);

		} // forma I puerta al lado derecho

		public Wall BuildI_DoorRight(Wall _wall_, double _alturaventana_1,
																			 double _alturaventana_2,
																			 double _sillventanda_1,
																			 double _sillventanda_2,
																			 XYZ _Point_dVIo_2,
																			 XYZ _Point_dVFo_1,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo1 = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1 + hv1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1 + hv1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190

			double hv2 = alturaventana2;
			double sill_v2 = sillventanda2;

			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2 + hv2);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2 + hv2);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, Point_bajo_Esquina1); // 2 Linea
				Line linea3 = Line.CreateBound(Point_bajo_Esquina1, Point_bajo_DVIo); // 3 Linea 
				Line linea4 = Line.CreateBound(Point_bajo_DVIo, Point_arriba_DVIo); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_Esquina1); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 6 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea

				Line linea8 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 4 Linea
				Line linea9 = Line.CreateBound(Point_arriba_Esquina2, Point_arriba_DVFo); // 5 Linea
				Line linea10 = Line.CreateBound(Point_arriba_DVFo, Nuevo_Point_dVFo); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);


			return ReplaceWallWithProfile(wall_I, profile);
		} // forma I puerta al lado izquierdo		

		public Wall BuildI(Wall _wall_, double _alturaventana_1,
																			 double _alturaventana_2,
																			 double _sillventanda_1,
																			 double _sillventanda_2,
																			 XYZ _Point_dVIo_2,
																			 XYZ _Point_dVFo_1,
																			 double height_double)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo2 = _Point_dVIo_2;
			XYZ Point_dVFo1 = _Point_dVFo_1;

			double alturaventana1 = _alturaventana_1;
			double sillventanda1 = _sillventanda_1;

			double alturaventana2 = _alturaventana_2;
			double sillventanda2 = _sillventanda_2;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;


			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo2.X, Point_dVIo2.Y, stPoint.Z);

			double dis1 = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm1 = endParam - dis1; // 3800 - 610 = 3190

			double hv1 = alturaventana1;
			double sill_v1 = sillventanda1;


			XYZ Point_DVFo = wallCurve.Evaluate(dm1, false);
			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v1 + hv1);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v1 + hv1);


			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba


			double dis2 = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm2 = endParam - dis2; // 3800 - 610 = 3190

			double hv2 = alturaventana2;
			double sill_v2 = sillventanda2;

			XYZ Point_DVIo = wallCurve.Evaluate(dm2, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v2 + hv2);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v2 + hv2);


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, Point_bajo_Esquina1); // 2 Linea
				Line linea3 = Line.CreateBound(Point_bajo_Esquina1, Point_bajo_DVIo); // 3 Linea
				Line linea4 = Line.CreateBound(Point_bajo_DVIo, Point_arriba_DVIo); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_Esquina1); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 6 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 4 Linea
				Line linea9 = Line.CreateBound(Point_arriba_Esquina2, Point_arriba_DVFo); // 5 Linea
				Line linea10 = Line.CreateBound(Point_arriba_DVFo, Point_bajo_DVFo); // 6 Linea
				Line linea11 = Line.CreateBound(Point_bajo_DVFo, Point_bajo_Esquina2); // 7 Linea
				Line linea12 = Line.CreateBound(Point_bajo_Esquina2, stPoint); // 8 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);
				profile.Add(linea9);
				profile.Add(linea10);
				profile.Add(linea11);
				profile.Add(linea12);

			return ReplaceWallWithProfile(wall_I, profile);
		} // forma I

		public Wall BuildWindowLeft(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVIo = _Point_dVIo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);

			double dis = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;


			XYZ Point_DVIo = wallCurve.Evaluate(dm, false);
			XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v);
			XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, stPoint.Z + sill_v + hv);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + sill_v + hv);

			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba

			List<Wall> lista_wall_return = new List<Wall>();

			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, Point_bajo_Esquina1); // 2 Linea
				Line linea3 = Line.CreateBound(Point_bajo_Esquina1, Point_bajo_DVIo); // 3 Linea
				Line linea4 = Line.CreateBound(Point_bajo_DVIo, Point_arriba_DVIo); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_DVIo, Point_arriba_Esquina1); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 6 Linea
				Line linea7 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 7 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, stPoint); // 8 Linea

				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);

			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildWindowRight(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, stPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;


			XYZ Point_DVFo = wallCurve.Evaluate(dm, false);

			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, stPoint.Z + sill_v + hv);

			XYZ Point_bajo_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v);
			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sill_v + hv);

			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba

			List<Wall> lista_wall_return = new List<Wall>();


			IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 2 Linea 
				Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea
				Line linea4 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_Esquina2, Point_arriba_DVFo); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_DVFo, Point_bajo_DVFo); // 6 Linea
				Line linea7 = Line.CreateBound(Point_bajo_DVFo, Point_bajo_Esquina2); // 7 Linea
				Line linea8 = Line.CreateBound(Point_bajo_Esquina2, stPoint); // 8 Linea

				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);

			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildDoorLeft(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
		{


			Wall wall_I = _wall_;

			XYZ Point_dVIo = _Point_dVIo_;

			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;


			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);

			double dis = Nuevo_Point_dVIo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;

			XYZ Nuevo_Point_dVIo_actual = wallCurve.Evaluate(dm, false); // Point 2


			XYZ Nuevo_Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo_actual.X, Nuevo_Point_dVIo_actual.Y, stPoint.Z + hv);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + hv);
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, stPoint.Z + height_double); // Point 2 arriba
			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);// Point 1 arriba

			IList<Curve> profile = new List<Curve>();


				Line linea1 = Line.CreateBound(stPoint, Nuevo_Point_dVIo_actual); // 1 Linea
				Line linea2 = Line.CreateBound(Nuevo_Point_dVIo_actual, Nuevo_Point_dVIo_arriba); // 2 Linea
				Line linea3 = Line.CreateBound(Nuevo_Point_dVIo_arriba, Point_arriba_Esquina1); // 3 Linea
				Line linea4 = Line.CreateBound(Point_arriba_Esquina1, endPoint_arriba); // 4 Linea
				Line linea5 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(stPoint_arriba, stPoint); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);


			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildDoorRight(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;

			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;

			XYZ Nuevo_Point_dVFo_actual = wallCurve.Evaluate(dm, false);

			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, endPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba

			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, endPoint.Z + hv);

			XYZ Nuevo_Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo_actual.X, Nuevo_Point_dVFo_actual.Y, endPoint.Z + hv);

			IList<Curve> profile = new List<Curve>();

				//		        Revision5_InsertOpening_void(wall_I, alturaventana, dm, dm/2, alturaventana/2);


				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo_actual, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 2 Linea 
				Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea
				Line linea4 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_Esquina2, Nuevo_Point_dVFo_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(Nuevo_Point_dVFo_arriba, Nuevo_Point_dVFo_actual); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);


			return ReplaceWallWithProfile(wall_I, profile);
		}

		public Wall BuildDoorRightSpecialCase(Wall _wall_, double _alturaventana_, double sillventanda_0,
																									   double _sillventanda_, XYZ _Point_dVFo_,
																									  XYZ _Point_dVIo_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			XYZ Point_dVIo = _Point_dVIo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel

			List<Wall> lista_wall_return = new List<Wall>();

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);
			XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, endPoint.Z);
			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;

			double sill_v_0 = sillventanda_0;

			XYZ Nuevo_Point_dVFo_actual = wallCurve.Evaluate(dm, false);

			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, endPoint.Z + height_double);// Point 1 arriba 
			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba

			XYZ Point_arriba_Esquina2 = new XYZ(stPoint.X, stPoint.Y, endPoint.Z + hv);

			XYZ Nuevo_Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo_actual.X, Nuevo_Point_dVFo_actual.Y, endPoint.Z + hv);

			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				IList<Curve> profile = new List<Curve>();

				//		        Revision5_InsertOpening_void(wall_I, alturaventana, dm, dm/2, alturaventana/2);


				Line linea1 = Line.CreateBound(Nuevo_Point_dVFo_actual, endPoint); // 1 Linea
				Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 2 Linea 
				Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 3 Linea
				Line linea4 = Line.CreateBound(stPoint_arriba, Point_arriba_Esquina2); // 4 Linea
				Line linea5 = Line.CreateBound(Point_arriba_Esquina2, Nuevo_Point_dVFo_arriba); // 5 Linea
				Line linea6 = Line.CreateBound(Nuevo_Point_dVFo_arriba, Nuevo_Point_dVFo_actual); // 6 Linea


				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);


				Wall wall = Wall.Create(_doc, profile, wallType.Id, wall_I.LevelId, true);
				lista_wall_return.Add(wall);


				WallJoinHelper.DisableJoins(wall);


				Line linea1_casoEspecial = Line.CreateBound(stPoint, Nuevo_Point_dVIo);

				Wall wall_casoEspecial = Wall.Create(_doc, linea1_casoEspecial, wall_I.LevelId, false);

				Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_abajo.Set(endPoint.Z + sill_v_0);
				}

				_doc.Delete(wall_I.Id);

				trans.Commit();

			}
			return lista_wall_return.First();
		}

		public void BuildSolitario(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			List<Wall> ultimos_Walls_para_Agregar_40 = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, 0);

			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;

			XYZ X_4_p = wallCurve.Evaluate(endParam, false); // Point 1

			XYZ Point_DVFo = wallCurve.Evaluate(dm, false);

			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, sill_v);
			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, sill_v + hv);

			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, sill_v);
			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, sill_v + hv);

			XYZ Point_bajo_Esquina2 = new XYZ(X_4_p.X, X_4_p.Y, sill_v);
			XYZ Point_arriba_Esquina2 = new XYZ(X_4_p.X, X_4_p.Y, sill_v + hv);

			XYZ Point_bajo_Esquina3 = new XYZ(stPoint.X, stPoint.Y, sill_v);
			XYZ Point_arriba_Esquina3 = new XYZ(stPoint.X, stPoint.Y, sill_v + hv);

			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, height_double);// Point 1 arriba
			XYZ stPoint_arriba_ventana = new XYZ(stPoint.X, stPoint.Y, sill_v + hv);// Point 1 arriba 

			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, height_double); // Point 2 arriba
			XYZ X_endPoint_arriba = new XYZ(X_4_p.X, X_4_p.Y, height_double); // Point 2 arriba
			XYZ X_endPoint_abajo = new XYZ(X_4_p.X, X_4_p.Y, 0); // Point 2 arriba

			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, X_endPoint_abajo); // 1 Linea
				Line linea2 = Line.CreateBound(X_endPoint_abajo, Point_bajo_Esquina2); // 2 Linea 
				Line linea3 = Line.CreateBound(Point_bajo_Esquina2, Point_bajo_Esquina3); // 3 Linea
				Line linea4 = Line.CreateBound(Point_bajo_Esquina3, stPoint); // 4 Linea

				Line linea5 = Line.CreateBound(stPoint_arriba_ventana, Point_arriba_Esquina2); // 5 Linea
				Line linea6 = Line.CreateBound(Point_arriba_Esquina2, X_endPoint_arriba); // 6 Linea
				Line linea7 = Line.CreateBound(X_endPoint_arriba, stPoint_arriba); // 7 Linea
				Line linea8 = Line.CreateBound(stPoint_arriba, stPoint_arriba_ventana); // 8 Linea

				profile.Add(linea1);
				profile.Add(linea2);
				profile.Add(linea3);
				profile.Add(linea4);
				profile.Add(linea5);
				profile.Add(linea6);
				profile.Add(linea7);
				profile.Add(linea8);

				Wall wall = Wall.Create(_doc, profile, wallType.Id, wall_I.LevelId, true);

				WallJoinHelper.DisableJoins(wall);

				_doc.Delete(wall_I.Id);

				trans.Commit();

			}

		}

		public void BuildTwoWallSolitario(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			List<Wall> ultimos_Walls_para_Agregar_40 = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);

				WallJoinHelper.DisableJoins(wall_I);

				((LocationCurve)wall_I.Location).Curve = linea1;

				Parameter WALL_USER_HEIGHT_PARAMF1 = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1.Set(endPoint.Z + sill_v);
				}


				Wall wall_F1_arriba = Wall.Create(_doc, linea1, wall_I.LevelId, false);

				WallJoinHelper.DisableJoins(wall_F1_arriba);

				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(endPoint.Z + height_double - (sill_v + hv));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(endPoint.Z + sill_v + hv);
				}


				trans.Commit();

			}

		}

		public Wall BuildTwoWallSolitarioReturn(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			List<Wall> ultimos_Walls_para_Agregar_40 = new List<Wall>();
			List<Wall> lista_wall_return = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);
				Wall wall_F1_abajo = Wall.Create(_doc, linea1, wall_I.LevelId, false);

				WallJoinHelper.DisableJoins(wall_F1_abajo);

				//				((LocationCurve)wall_I.Location).Curve = linea1;

				Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1_abajo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1.Set(endPoint.Z + sillventanda);
				}


				Wall wall_F1_arriba = Wall.Create(_doc, linea1, wall_I.LevelId, false);

				WallJoinHelper.DisableJoins(wall_F1_arriba);

				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(endPoint.Z + height_double - (sill_v + hv));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_F1_arriba.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(endPoint.Z + sill_v + hv);
				}


				lista_wall_return.Add(wall_F1_abajo);

				_doc.Delete(wall_I.Id);

				trans.Commit();

			}
			return lista_wall_return.First();
		}

		public void BuildOneWallSolitario(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
		{


			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);

				WallJoinHelper.DisableJoins(wall_I);

				((LocationCurve)wall_I.Location).Curve = linea1;


				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(endPoint.Z + height_double - (sill_v + hv));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_I.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(endPoint.Z + sill_v + hv);
				}

				//	            _doc.Delete(wall_I.Id);

				trans.Commit();

			}

		}

		public Wall BuildOneWallSolitarioSpecialCase(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_,
																						double sillventanda_2, XYZ _Point_dVFo_)
		{

			List<Wall> lista_wall_return = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVFo = _Point_dVFo_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, endPoint.Z);

			double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;
			double sill_v = sillventanda;

			double sill_v_2 = sillventanda_2;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);

				Line linea1_casoEspecial = Line.CreateBound(Nuevo_Point_dVFo, endPoint);

				WallJoinHelper.DisableJoins(wall_I);

				((LocationCurve)wall_I.Location).Curve = linea1;


				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(endPoint.Z + height_double - (sill_v + hv));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_I.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(endPoint.Z + sill_v + hv);
				}


				Wall wall_casoEspecial = Wall.Create(_doc, linea1_casoEspecial, wall_I.LevelId, false);

				lista_wall_return.Add(wall_casoEspecial);

				Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_abajo.Set(endPoint.Z + sill_v_2);
				}


				//	            _doc.Delete(wall_I.Id);

				trans.Commit();

			}

			return lista_wall_return.First();

		}

		public Wall BuildOneWallSolitarioSpecialEndWall(Wall _wall_, double _anchoventana_, double _alturaventana_, double alturaventana_2,
																														 double _sillventanda_, double sillventanda_2, XYZ _Point_dVI1_)
		{

			List<Wall> lista_wall_return = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVI1 = _Point_dVI1_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVI1 = new XYZ(Point_dVI1.X, Point_dVI1.Y, endPoint.Z);

			double dis = Nuevo_Point_dVI1.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;

			double hv_2 = alturaventana_2;

			double sill_v = sillventanda;

			double sill_v_2 = sillventanda_2;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);

				Line linea1_casoEspecial = Line.CreateBound(stPoint, Nuevo_Point_dVI1);

				WallJoinHelper.DisableJoins(wall_I);

				((LocationCurve)wall_I.Location).Curve = linea1;


				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(stPoint.Z + height_double - (sill_v_2 + hv_2));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_I.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(stPoint.Z + sill_v_2 + hv_2);
				}


				Wall wall_casoEspecial = Wall.Create(_doc, linea1_casoEspecial, wall_I.LevelId, false);

				lista_wall_return.Add(wall_casoEspecial);

				Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_casoEspecial.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_abajo.Set(stPoint.Z + sill_v);
				}


				//	            _doc.Delete(wall_I.Id);

				trans.Commit();

			}

			return lista_wall_return.First();

		}

		public Wall BuildOneWallSolitarioSpecialStartWall(Wall _wall_, double _anchoventana_, double _alturaventana_, double alturaventana_2,
																														 double _sillventanda_, double sillventanda_2, XYZ _Point_dVI1_)
		{

			List<Wall> lista_wall_return = new List<Wall>();

			// INPUTS
			Wall wall_I = _wall_;

			XYZ Point_dVI1 = _Point_dVI1_;
			double alturaventana = _alturaventana_;
			double sillventanda = _sillventanda_;
			double anchoventana = _anchoventana_;
			// INPUTS

			Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

			WallType wallType = wall_I.WallType as WallType;

			Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
			double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

			double stParam = wallCurve.GetEndParameter(0);
			double endParam = wallCurve.GetEndParameter(1);

			double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel


			// Crear linea y corregir primero Wall no crear nada.
			XYZ stPoint = wallCurve.Evaluate(stParam, false); // Point 1
			XYZ endPoint = wallCurve.Evaluate(endParam, false); // Point 2

			XYZ Nuevo_Point_dVI1 = new XYZ(Point_dVI1.X, Point_dVI1.Y, endPoint.Z);

			double dis = Nuevo_Point_dVI1.DistanceTo(endPoint);

			double dm = endParam - dis; // 3800 - 610 = 3190

			double hv = alturaventana;

			double hv_2 = alturaventana_2;

			double sill_v = sillventanda;

			double sill_v_2 = sillventanda_2;


			using (Transaction trans = new Transaction(_doc, "wall"))

			{

				trans.Start();

				//		        IList<Curve> profile = new List<Curve>();

				Line linea1 = Line.CreateBound(stPoint, endPoint);

				Line linea1_casoEspecial = Line.CreateBound(Nuevo_Point_dVI1, endPoint);

				WallJoinHelper.DisableJoins(wall_I);

				((LocationCurve)wall_I.Location).Curve = linea1;


				Parameter WALL_USER_HEIGHT_PARAMF1_arriba = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_arriba.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_arriba.Set(stPoint.Z + height_double - (sill_v_2 + hv_2));
				}

				Parameter WALL_USER_BASEOFFSET_PARAM_2 = wall_I.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
				if (!WALL_USER_BASEOFFSET_PARAM_2.IsReadOnly)
				{
					WALL_USER_BASEOFFSET_PARAM_2.Set(stPoint.Z + sill_v_2 + hv_2);
				}


				Wall wall_casoEspecial = Wall.Create(_doc, linea1_casoEspecial, wall_I.LevelId, false);

				lista_wall_return.Add(wall_casoEspecial);

				Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_casoEspecial.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
				{
					WALL_USER_HEIGHT_PARAMF1_abajo.Set(stPoint.Z + sill_v_2);
				}


				//	            _doc.Delete(wall_I.Id);

				trans.Commit();

			}

			return lista_wall_return.First();

		}

	}
}
