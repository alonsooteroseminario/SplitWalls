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
			Application app = uiApp.Application;

			
			string ruta = App.ExecutingAssemblyPath;

			// Get Active View
			View activeView = uidoc.ActiveView;

			//open ()
			#region MACRO

			#region Ejecución de Script

			BUTTON_GENERAL();


			#endregion

			#region BUTTON GENERAL

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
							
							string numero = form.textString.ToString();

							if (numero =="")
							{
								TaskDialog.Show("Atención", "Por favor Ingresa un valor para el Ancho del Panel. No puede quedar en blanco . ");
								return;
							}
							//NumberFormatInfo provider = new NumberFormatInfo();
							//double val = Convert.ToDouble(numero, provider); 
							double val = Convert.ToDouble(numero);
							numero_final = val; // double

							Muro_sin_Ventanas = form.checkBox_2;
							Muro_OSB_con_Ventanas = form.checkBox_3;
							Muro_SMART_PANEL_con_Ventanas = form.checkBox_6;

							_todoMuro = form.checkBox_7;
							//primera_ERA_VENT = form.checkBox_8;


							esquina_1 = form.checkBox_4;
							esquina_2_otro_lado = form.checkBox_5;

							

						}

					}

					// close form1

					// MURO SIN VENTANAS
					int anchopanel_UI = (int)numero_final;// 1220 

					if (Muro_sin_Ventanas && _todoMuro && esquina_1 ) // button 1
					{
						Revision6_BUTTON_1_mod(anchopanel_UI);
					}
					else if (Muro_sin_Ventanas && _todoMuro && esquina_2_otro_lado) // button 1 OTRO LADO
					{
						Revision6_BUTTON_1_Lado_Opuesto_mod(anchopanel_UI);
					}
					// MURO SIN VENTANAS


					// MURO OSB CON VENTANAS
					else if (Muro_OSB_con_Ventanas && primera_ERA_VENT && esquina_1) // Muro OSB solo 1era Ventana
					{
						Revision6_BUTTON_2_OBS_mod(anchopanel_UI);
					}
					else if (Muro_OSB_con_Ventanas && primera_ERA_VENT && esquina_2_otro_lado) // Muro OSB solo 1era Ventana OTRO LADO
					{
						Revision6_BUTTON_2_OBS_OtroLado_mod(anchopanel_UI);
					}
					else if (Muro_OSB_con_Ventanas && _todoMuro && esquina_1) // Muro OSB solo TODO MURO
					{
						Revision6_BUTTON_2_OBS_TODO_WALL_mod(anchopanel_UI);
					}
					else if (Muro_OSB_con_Ventanas && _todoMuro && esquina_2_otro_lado) // Muro OSB solo TODO MURO OTRO LADO
					{
						Revision6_BUTTON_2_OBS_TODO_WALL_OtroLado_mod(anchopanel_UI);
					}
					// MURO OSB CON VENTANAS



					// MURO SMART PANEL CON VENTANAS
					else if (Muro_SMART_PANEL_con_Ventanas && primera_ERA_VENT && esquina_1) // Muro SMART PANEL 1era Ventana 
					{
						Revision6_BUTTON_2_SMARTPANEL_mod(anchopanel_UI);
					}
					else if (Muro_SMART_PANEL_con_Ventanas && primera_ERA_VENT && esquina_2_otro_lado) // Muro SMART PANEL 1era Ventana  OTRO LADO
					{
						Revision6_BUTTON_2_SMARTPANEL_OtroLado_mod(anchopanel_UI);
					}


					else if (Muro_SMART_PANEL_con_Ventanas && _todoMuro && esquina_1) // Muro SMART PANEL 1era Ventana  OTRO LADO
					{
						Revision6_BUTTON_2_SMARTPANEL_TODO_WALL_mod(anchopanel_UI);
					}
					else if (Muro_SMART_PANEL_con_Ventanas && _todoMuro && esquina_2_otro_lado) // Muro SMART PANEL 1era Ventana  OTRO LADO
					{
						Revision6_BUTTON_2_SMARTPANEL_TODO_WALL_OtroLado_mod(anchopanel_UI);
					}
					// MURO SMART PANEL CON VENTANAS


					else
					{
						//TaskDialog.Show("Atención", "Por favor Elige una opción valida . No se seleccionó ninguna Opción");
						
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // Dividir Muros sin y con Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Object Selection .

			#endregion

			#region BUTTONS MODIFICADOS PARA ADDIN

			void Revision6_BUTTON_1_mod(int anchopanel_UI) // Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado .
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220;

					int _anchopanel_ = anchopanel_UI;

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{

						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(_wall_, _anchopanel_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK! Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Objects Selection .

			void Revision6_BUTTON_1_Lado_Opuesto_mod(int anchopanel_UI) // Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado .
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220;

					int _anchopanel_ = anchopanel_UI;

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(Revision6_DYNO_DarVuelta_Muro_SinVentanas(wall));
					}




					foreach (Wall _wall_ in lista_walls)
					{

						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(_wall_, _anchopanel_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK! Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Objects Selection .





			void Revision6_BUTTON_2_OBS_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Revision6_Button_2_OBS(anchopanel_UI, _wall_);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .

			void Revision6_BUTTON_2_OBS_OtroLado_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige


					Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(_wall_);

					Revision6_Button_2_OBS(anchopanel_UI, wall_iii);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .


			void Revision6_BUTTON_2_OBS_TODO_WALL_OtroLado_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall w in lista_walls)
					{
						Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(w);
						Revision6_BUTTON_2_OBS_TODO_WALL_INPUT(anchopanel_UI, wall_iii);
					}




				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .

			void Revision6_BUTTON_2_OBS_TODO_WALL_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{
						Revision6_BUTTON_2_OBS_TODO_WALL_INPUT(anchopanel_UI, _wall_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .





			void Revision6_BUTTON_2_SMARTPANEL_mod(int anchopanel_UI) //OKKKKKKKKKKKKKKKKKKKK!
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Revision6_Button_2_SMARTPANEL(anchopanel_UI, _wall_);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OKKKKKKKKKKKKKKKKKKKK! ! Panelizar antes del vano . Pick Object Selection .

			void Revision6_BUTTON_2_SMARTPANEL_OtroLado_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(_wall_);

					Revision6_Button_2_SMARTPANEL(anchopanel_UI, wall_iii);
				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .



			void Revision6_BUTTON_2_SMARTPANEL_TODO_WALL_OtroLado_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall w in lista_walls)
					{
						Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(w);
						Revision6_Button_2_SMARTPANEL(anchopanel_UI, wall_iii);
					}

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .

			void Revision6_BUTTON_2_SMARTPANEL_TODO_WALL_mod(int anchopanel_UI)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{
						Revision6_Button_2_SMARTPANEL(anchopanel_UI, _wall_);
					}





				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}


			} // OK ! Panelizar antes del vano . Pick Objects Selection .





			#endregion




			#region BUTTON 6ta Revision

			void Revision6_BUTTON_1() // Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado .
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220;

					int _anchopanel_ = anchopanel_UI;

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{

						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(_wall_, _anchopanel_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK! Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Objects Selection .

			void Revision6_BUTTON_1_Lado_Opuesto() // Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado .
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220;

					int _anchopanel_ = anchopanel_UI;

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(Revision6_DYNO_DarVuelta_Muro_SinVentanas(wall));
					}




					foreach (Wall _wall_ in lista_walls)
					{

						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(_wall_, _anchopanel_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK! Dividir Muros sin Ventanas, Ingresando el Valor del Ancho del Panel Deseado . Pick Objects Selection .





			void Revision6_BUTTON_2_OBS()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Revision6_Button_2_OBS(anchopanel_UI, _wall_);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .

			void Revision6_BUTTON_2_OBS_OtroLado()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige


					Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(_wall_);

					Revision6_Button_2_OBS(anchopanel_UI, wall_iii);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .


			void Revision6_BUTTON_2_OBS_TODO_WALL_OtroLado()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall w in lista_walls)
					{
						Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(w);
						Revision6_BUTTON_2_OBS_TODO_WALL_INPUT(anchopanel_UI, wall_iii);
					}




				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .

			void Revision6_BUTTON_2_OBS_TODO_WALL()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{
						Revision6_BUTTON_2_OBS_TODO_WALL_INPUT(anchopanel_UI, _wall_);
					}


				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .





			void Revision6_BUTTON_2_SMARTPANEL() //OKKKKKKKKKKKKKKKKKKKK!
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Revision6_Button_2_SMARTPANEL(anchopanel_UI, _wall_);

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OKKKKKKKKKKKKKKKKKKKK! ! Panelizar antes del vano . Pick Object Selection .

			void Revision6_BUTTON_2_SMARTPANEL_OtroLado()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(_wall_);

					Revision6_Button_2_SMARTPANEL(anchopanel_UI, wall_iii);
				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .



			void Revision6_BUTTON_2_SMARTPANEL_TODO_WALL_OtroLado()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que no tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall w in lista_walls)
					{
						Element wall_iii = Revision6_DYNO_DarVuelta_Muro_ConVentanas(w);
						Revision6_Button_2_SMARTPANEL(anchopanel_UI, wall_iii);
					}

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Objects Selection .

			void Revision6_BUTTON_2_SMARTPANEL_TODO_WALL()
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					int anchopanel_UI = 1220; // 1220

					List<Wall> lista_walls = new List<Wall>();

					IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Para aplicarlo a varios Muros a la vez, seleccióna solo muros que tienen Ventanas. ");

					foreach (Reference reference in references)
					{
						Element e = doc.GetElement(reference);
						Wall wall = e as Wall;
						lista_walls.Add(wall);
					}

					foreach (Wall _wall_ in lista_walls)
					{
						Revision6_Button_2_SMARTPANEL(anchopanel_UI, _wall_);
					}





				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}


			} // OK ! Panelizar antes del vano . Pick Objects Selection .




			#endregion



			#region 6ta Revision

			void Revision6_BUTTON_2_OBS_TODO_WALL_INPUT(int anchopanel_UI, Element _wall_)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//int anchopanel_UI = 1220; // 1220

					//Element _wall_ = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

					Revision6_Button_2_OBS_TODO_WALL(anchopanel_UI, _wall_);



				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			} // OK ! Panelizar antes del vano . Pick Object Selection .
			  // ESTE FUNCIONA ACTUALMENTE
			void Revision6_Button_2_OBS_TODO_WALL(int anchopanel_UI, Element _wall_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<Wall> lista_de_listas_walls = new List<Wall>();

				List<Wall> lista_wall_llegada = Revision6_Button_2_OBS_INPUT(anchopanel_UI, _wall_);

				Wall ultimo_wall = lista_wall_llegada.First();
				lista_de_listas_walls.Add(ultimo_wall);


				#region VENTANAS

				//			ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//			ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//			LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//			FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//	
				//			IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//			List<Element> windows_hosted = new List<Element>();
				//				
				//			foreach (Element elem in windows)
				//	        {
				//				FamilyInstance fintance = elem as FamilyInstance;
				//				if (fintance.Host.Id == lista_de_listas_walls.Last().Id)
				//				{
				//					windows_hosted.Add(elem);
				//				}
				//	        }

				#endregion

				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == lista_de_listas_walls.Last().Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}

				#endregion


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

						#region VENTANAS

						//					ElementClassFilter familyFilter_2 = new ElementClassFilter(typeof(FamilyInstance));
						//					ElementCategoryFilter MECategoryfilter_2 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
						//					LogicalAndFilter MEInstancesFilter_2 = new LogicalAndFilter(familyFilter_2, MECategoryfilter_2);
						//					FilteredElementCollector coll_2 = new FilteredElementCollector(doc, activeView.Id);
						//			
						//					IList<Element> windows_2 = coll_2.WherePasses(MEInstancesFilter_2).ToElements(); // todas las ventadas
						//					List<Element> windows_hosted_2 = new List<Element>();
						//					foreach (Element elem in windows_2)
						//			        {
						//						FamilyInstance fintance_2 = elem as FamilyInstance;
						//						if (fintance_2.Host.Id == lista_de_listas_walls.Last().Id)
						//						{
						//							windows_hosted_2.Add(elem);
						//						}
						//			        }					


						#region VENTANAS Y PUERTAS


						BuiltInCategory[] bics_familyIns_2 = new BuiltInCategory[]
							{

						BuiltInCategory.OST_Doors,
						BuiltInCategory.OST_Windows,

							};

						List<Element> windows_hosted_2 = new List<Element>();

						foreach (BuiltInCategory bic in bics_familyIns_2)
						{
							ElementClassFilter familyFilter_2 = new ElementClassFilter(typeof(FamilyInstance));
							// Create a category filter for MechanicalEquipment
							ElementCategoryFilter MECategoryfilter_2 = new ElementCategoryFilter(bic);
							// Create a logic And filter for all MechanicalEquipment Family
							LogicalAndFilter MEInstancesFilter_2 = new LogicalAndFilter(familyFilter_2, MECategoryfilter_2);
							// Apply the filter to the elements in the active document
							FilteredElementCollector MEcoll_2 = new FilteredElementCollector(doc);
							IList<Element> familyinstance_2 = MEcoll_2.WherePasses(MEInstancesFilter_2).ToElements();

							foreach (Element elem in familyinstance_2)
							{
								FamilyInstance fintance = elem as FamilyInstance;
								if (fintance.Host.Id == lista_de_listas_walls.Last().Id)
								{
									windows_hosted_2.Add(elem);
								}
							}
						}

						#endregion



						if (windows_hosted_2.Count() == 0)
						{
							//						int _anchopanel_2 = anchopanel_UI + 4;
							//						Revision6_DYNO_DividirMuroSinVentana( lista_de_listas_walls.Last(), _anchopanel_2);
							Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(lista_de_listas_walls.Last(), anchopanel_UI);
							ii = windows_hosted.Count() + 1;

						}
						else if (windows_hosted_2.Count() > 0)
						{
							ii = 0;
						}
						#endregion

					}

				}








			}
			// ESTE FUNCIONA ACTUALMENTE
			List<Wall> Revision6_Button_2_OBS_INPUT(int anchopanel_UI, Element _wall_)
			{
				List<Wall> lista_wall_final_retirada = new List<Wall>();

				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//// Get Active View
					//View activeView = this.ActiveUIDocument.ActiveView;

					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal



					#region VENTANAS

					//				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					//			 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
					//			 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					//			 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
					//	
					//			 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
					//				List<Element> windows_hosted = new List<Element>();
					//				
					//				foreach (Element elem in windows)
					//	            {
					//					FamilyInstance fintance = elem as FamilyInstance;
					//					if (fintance.Host.Id == e.Id) 
					//					{
					//						windows_hosted.Add(elem);
					//					}
					//	            }

					#endregion

					#region VENTANAS Y PUERTAS


					BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
						{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

						};

					List<Element> windows_hosted = new List<Element>();

					foreach (BuiltInCategory bic in bics_familyIns)
					{
						ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for MechanicalEquipment
						ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
						// Create a logic And filter for all MechanicalEquipment Family
						LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
						// Apply the filter to the elements in the active document
						FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
						IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

						foreach (Element elem in familyinstance)
						{
							FamilyInstance fintance = elem as FamilyInstance;
							if (fintance.Host.Id == e.Id)
							{
								windows_hosted.Add(elem);
							}
						}
					}

					#endregion

					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(e, _anchopanel_);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{
							#region 1 Ventana

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


							#endregion
						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{
							#region 2 o mas Ventanas

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

							#endregion
						}



					}

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}
				return lista_wall_final_retirada;

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_Button_2_OBS(int anchopanel_UI, Element _wall_)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//// Get Active View
					//View activeView = this.ActiveUIDocument.ActiveView;

					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal

					#region VENTANAS

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

					#endregion

					#region PUERTAS
					#endregion

					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						int _anchopanel_2 = _anchopanel_ + 4;
						Revision6_DYNO_DividirMuroSinVentana(_wall_, _anchopanel_2);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{
							#region 1 Ventana

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
								Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_i, alturaventanas.First(), sillventanas.First(), Punto_Ventada_dPH);

							}

							if (anchoventanas.First() > 1220 / 304.8)
							{
								foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
								{

									Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

								}
							}

							#endregion
						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{
							#region 2 o mas Ventanas

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
								Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_i, alturaventanas[0], sillventanas[0], Punto_Ventada_dPH);

							}

							if (anchoventanas.First() > 1220 / 304.8)
							{
								foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
								{

									Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[0], sillventanas[0], Puntos_Ventada_dVFo[0]);

								}
							}

							#endregion
						}


					}

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}

			}
			// ESTE FUNCIONA ACTUALMENTE


			void Revision6_Button_2_SMARTPANEL(int anchopanel_UI, Element _wall_)
			{
				try
				{
					//UIDocument uidoc = this.ActiveUIDocument;
					//Document doc = uidoc.Document;

					//// Get Active View
					//View activeView = this.ActiveUIDocument.ActiveView;

					int _anchopanel_ = anchopanel_UI; //1220

					Wall e = _wall_ as Wall; // Wall principal

					Parameter height = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					double height_double = height.AsDouble();

					Parameter lenght = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
					double lenght_double = lenght.AsDouble();

					#region VENTANAS Y PUERTAS


					BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
						{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

						};

					List<Element> windows_hosted = new List<Element>();

					foreach (BuiltInCategory bic in bics_familyIns)
					{
						ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for MechanicalEquipment
						ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
						// Create a logic And filter for all MechanicalEquipment Family
						LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
						// Apply the filter to the elements in the active document
						FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

						foreach (Element elem in familyinstance)
						{
							FamilyInstance fintance = elem as FamilyInstance;
							if (fintance.Host.Id == e.Id)
							{
								windows_hosted.Add(elem);
							}
						}
					}


					//				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					//			 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
					//			 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					//			 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
					//	
					//			 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
					//				
					//				
					//				foreach (Element elem in windows)
					//	            {
					//					FamilyInstance fintance = elem as FamilyInstance;
					//					if (fintance.Host.Id == e.Id) 
					//					{
					//						windows_hosted.Add(elem);
					//					}
					//	            }

					#endregion


					Curve wallCurve = ((LocationCurve)e.Location).Curve;
					double stParam = wallCurve.GetEndParameter(0);
					double endParam = wallCurve.GetEndParameter(1);

					// Crear lista_d

					#region Lista_d = [d1, d2, d3, d4, ...]

					double distanta_total_wall = endParam - stParam;

					double alpha = distanta_total_wall % (_anchopanel_ / 304.8); // 3.38 * 304.8 =

					double numero_paneles_total = (distanta_total_wall - alpha) / (_anchopanel_ / 304.8); // 7
					int numero_paneles_total_int = (int)numero_paneles_total; // 7

					List<double> lista_d = new List<double>();
					List<double> lista_d_muro4 = new List<double>();


					for (int i = 0; i < numero_paneles_total_int; i++)
					{
						lista_d.Add(stParam + ((_anchopanel_ / 304.8) * (i + 1) + (4 / 304.8) * i));
					}
					for (int i = 0; i < numero_paneles_total_int; i++)
					{
						lista_d_muro4.Add(stParam + ((_anchopanel_ / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
					}


					#endregion

					if (windows_hosted.Count() == 0) // Sin ventanas
					{
						Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(e, _anchopanel_);
					}
					else if (windows_hosted.Count() > 0) // Con ventanas
					{


						if (windows_hosted.Count() == 1) // solo 1 ventana
						{
							#region 1 Ventana

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


							#region INFO PHs

							double alpho = anchoventanas.First() % (_anchopanel_ / 304.8);

							double numero = (anchoventanas.First() - alpho) / (_anchopanel_ / 304.8);
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
							#endregion

							BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
							FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
							// Elementos que contienen el punto dPH
							IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();


							#region collectores elements_dVIo elements_dVFo elements_dPH
							bool VIo_previo_vacio = false;
							bool VFo_previo_vacio = false;
							bool bool_dVIo = false;
							BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo.First());
							FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
							IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();
							//TaskDialog.Show("ALERTA !elements_dVIo.Any()", elements_dVIo.Any().ToString());
							if (!elements_dVIo.Any())
							{
								VIo_previo_vacio = true;
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
								VFo_previo_vacio = true;
								double VIo = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
								double VFo = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);
								double n_paneles_VF = Math.Round((VFo * n_paneles) / lenght_double, 0);

								double lenght_double_VF = Math.Round(n_paneles_VF * _anchopanel_ + (n_paneles_VF - 1) * (4 / 304.8), 0);
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
									if ((anchoventanas.First() == _anchopanel_ / 304.8) || (anchoventanas.First() > _anchopanel_ / 304.8))
									{
										//TaskDialog.Show("ALERTA", "VENTANA EXACTA activador" + Environment.NewLine + (  anchoventanas[i] > _anchopanel_ ).ToString());
										activador = true; // VENTANA EXACTA
									}


									elements_dVFo.Add(elements_dPH.First());
									bool_dVFo = true;

								}
							}




							#endregion



							if (elements_dVIo.First().Id.ToString() == elements_dVFo.First().Id.ToString())
							{

								#region no choca con nada
								if (bool_dVFo || bool_dVIo)
								{
									if (!activador)
									{
										#region Ventana o Puerta Dentro de Muro Void
										Wall mierda = elements_dVIo.First() as Wall;
										if (sillventanas.First() == 0) // Es puerta
										{
											#region puerta

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

											if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
											{
												// Transf. BORDE PUERTA a la IZQUIERDA
												//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
												Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
												lista_walls.Add(wall_recibida);
											}
											else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
											{
												// Transf. BORDE PUERTA a la DERECHA
												//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
												Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
												lista_walls.Add(wall_recibida);
											}
											else
											{
												if (!(lista_NO_VIo.Contains(mierda.Id)))
												{
													Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), Punto_Ventada_dPH);
													lista_walls.Add(wall_recibida);
												}
											}

											#endregion
										}
										else
										{
											#region Ventana
											if (!(lista_NO_VIo.Contains(mierda.Id)))
											{
												using (Transaction trans = new Transaction(doc, "wall"))
												{
													trans.Start();

													XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo.First().X, Puntos_Ventada_dVFo.First().Y, Puntos_Ventada_dVFo.First().Z + alturaventanas.First());
													XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo.First().X, Puntos_Ventada_dVIo.First().Y, Puntos_Ventada_dVIo.First().Z);

													Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

													if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
														WallUtils.DisallowWallJoinAtEnd(mierda, 1);

													if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
														WallUtils.DisallowWallJoinAtEnd(mierda, 0);
													trans.Commit();

													lista_walls.Add(mierda);
												}
											}
											#endregion
										}

										#endregion
									}
									else
									{
										Wall mierda = elements_dVIo.First() as Wall;
										if (!(lista_NO_VIo.Contains(mierda.Id)))
										{
											#region Ventana Exacta

											Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(mierda, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

											#endregion
										}
									}
								}
								else
								{
									#region Ventana o Puerta Dentro de Muro Void
									Wall mierda = elements_dVIo.First() as Wall;
									if (sillventanas.First() == 0) // Es puerta
									{
										#region puerta

										double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
										double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

										if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
										{
											// Transf. BORDE PUERTA a la IZQUIERDA
											//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
											Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
											lista_walls.Add(wall_recibida);
										}
										else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
										{
											// Transf. BORDE PUERTA a la DERECHA
											//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
											Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas.First(), anchoventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), height_double);
											lista_walls.Add(wall_recibida);
										}
										else
										{
											if (!(lista_NO_VIo.Contains(mierda.Id)))
											{
												Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First(), Puntos_Ventada_dVFo.First(), Punto_Ventada_dPH);
												lista_walls.Add(wall_recibida);
											}
										}


										#endregion
									}
									else
									{
										#region Ventana
										if (!(lista_NO_VIo.Contains(mierda.Id)))
										{
											using (Transaction trans = new Transaction(doc, "wall"))
											{
												trans.Start();

												XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo.First().X, Puntos_Ventada_dVFo.First().Y, Puntos_Ventada_dVFo.First().Z + alturaventanas.First());
												XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo.First().X, Puntos_Ventada_dVIo.First().Y, Puntos_Ventada_dVIo.First().Z);

												Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

												if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
													WallUtils.DisallowWallJoinAtEnd(mierda, 1);

												if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
													WallUtils.DisallowWallJoinAtEnd(mierda, 0);
												trans.Commit();

												lista_walls.Add(mierda);
											}
										}
										#endregion
									}

									#endregion
								}
								#endregion


							}
							else
							{
								#region else

								//								List<Wall> lista_walls = new List<Wall>();


								if (!(elements_dVIo.First().Id == elements_dPH.First().Id))
								{
									if (!(elements_dVFo.First().Id == elements_dPH.First().Id))
									{

										#region dVIo

										foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
										{
											if (sillventanas.First() == 0)
											{
												Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
											}
											else
											{
												#region else
												//&&  !(VF_nuf == Math.Round(lenght_double, 0))
												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

												if ((VI_nuf == 0)) // BORDE Inicio
												{
													//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}

												else
												{

													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
												#endregion
											}

										}
										#endregion

										#region dVFo

										foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
										{
											if (sillventanas.First() == 0)
											{
												#region puerta

												Wall aaaaaa = wall_ii as Wall;

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

												if (VF_nuf == Math.Round(lenght_double, 0))
												{
													// Transf. BORDE PUERTA a la DERECHA

													if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
													{
														Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													}

												}
												else
												{

													Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

												}

												#endregion
											}
											else
											{
												#region else

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);



												if ((VF_nuf == Math.Round(lenght_double, 0))) // BORDE Final
												{
													//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
													Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
												else
												{
													//TaskDialog.Show("ALERTA", "AQUI");
													Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
												#endregion
											}

										}
										#endregion

										#region dPH

										#region PH


										foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
										{
											//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
											Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
										}


										#endregion


										#endregion

									}
									else
									{

										#region dVIo
										foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
										{
											if (sillventanas.First() == 0)
											{
												Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
											}
											else
											{
												#region else

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

												if ((VI_nuf == 0)) // BORDE Inicio
												{
													//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
												else
												{

													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVIo);
												}
												#endregion
											}
										}
										#endregion

										#region dVFo

										foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
										{
											if (sillventanas.First() == 0)
											{
												#region puerta

												Wall aaaaaa = wall_ii as Wall;

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

												if (VF_nuf == Math.Round(lenght_double, 0))
												{
													// Transf. BORDE PUERTA a la DERECHA

													if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
													{
														Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													}

												}
												else
												{
													Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

												}

												#endregion
											}
											else
											{
												#region else

												double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
												double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

												if ((VF_nuf == Math.Round(lenght_double, 0))) // BORDE Final
												{
													//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
													Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
												else
												{
													//TaskDialog.Show("ALERTA", "AQUI");
													Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
													lista_walls.Add(wall_recibida_dVFo);
												}
												#endregion
											}
										}
										#endregion

									}
								}
								else
								{

									#region dVIo

									foreach (Wall wall_i in elements_dVIo) // Elementos que contienen el punto dVIo
									{
										if (sillventanas.First() == 0)
										{
											Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
										}
										else
										{
											#region else

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

											if ((VI_nuf == 0)) // BORDE Inicio
											{
												//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
												Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_i, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
												lista_walls.Add(wall_recibida_dVIo);
											}
											else
											{

												Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(wall_i, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
												lista_walls.Add(wall_recibida_dVIo);
											}
											#endregion
										}
									}
									#endregion

									#region dVFo

									foreach (Wall wall_ii in elements_dVFo) // Elementos que contienen el punto dVFo
									{
										if (sillventanas.First() == 0)
										{
											#region puerta

											Wall aaaaaa = wall_ii as Wall;

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

											if (VF_nuf == Math.Round(lenght_double, 0))
											{
												// Transf. BORDE PUERTA a la DERECHA

												if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
												{
													Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
												}

											}
											else
											{
												Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());

											}

											#endregion
										}
										else
										{
											#region else

											double VI_nuf = Math.Round(lista_dPH.First() - anchoventanas.First() / 2, 0);
											double VF_nuf = Math.Round(lista_dPH.First() + anchoventanas.First() / 2, 0);

											if ((VF_nuf == Math.Round(lenght_double, 0))) // BORDE Final
											{
												//TaskDialog.Show("ALERTA", "AQUI PENDEJADA 2");
												Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(wall_ii, anchoventanas.First(), alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVIo.First());
												lista_walls.Add(wall_recibida_dVFo);
											}
											else
											{
												//TaskDialog.Show("ALERTA", "AQUI");
												Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(wall_ii, alturaventanas.First(), sillventanas.First(), Puntos_Ventada_dVFo.First());
												lista_walls.Add(wall_recibida_dVFo);
											}
											#endregion
										}
									}
									#endregion

								}


								#endregion
							}

							#endregion
						}

						else if (windows_hosted.Count() > 1) // 2 o mas ventanas
						{
							#region 2 o mas Ventanas

							//Curve wallCurve = ((LocationCurve)e.Location).Curve;

							List<Wall> listaWalls_Final = new List<Wall>();

							Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> tupla = Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL(e, _anchopanel_); // 1220

							#region Datos Extraidos de Ventanas
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

							#endregion

							#region Datos extraidos de Walls Panelizados
							foreach (Wall wall in listaWalls_Final_siCambia) // Cambiar Perfil de Muros
							{
								listaWalls_Final.Add(wall);
							}

							foreach (Wall wall in listaWalls_Final_noCambia) // No cambia nada se mantiene el mismo Muro
							{
								//listaWalls_Final.Add(wall);
							}
							#endregion


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

								#region for

								double Ventada_dPH = lista_dPH[i];
								XYZ Punto_Ventada_dPH = wallCurve.Evaluate(Ventada_dPH, false);
								XYZ Nuevo_Punto_Ventada_dPH = new XYZ(Punto_Ventada_dPH.X, Punto_Ventada_dPH.Y, sillventanas[i]);


								#region INFO PHs

								double alpho = anchoventanas[i] % (_anchopanel_ / 304.8);

								double numero = (anchoventanas[i] - alpho) / (_anchopanel_ / 304.8);
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
								#endregion


								BoundingBoxContainsPointFilter filter_dPH = new BoundingBoxContainsPointFilter(Punto_Ventada_dPH);
								FilteredElementCollector collector_dPH = new FilteredElementCollector(doc, activeView.Id);
								IList<Element> elements_dPH = collector_dPH.OfClass(typeof(Wall)).WherePasses(filter_dPH).ToElements();




								#region collectores elements_dVIo elements_dVFo elements_dPH
								bool VIo_previo_vacio = false;
								bool VFo_previo_vacio = false;
								bool bool_dVIo = false;
								BoundingBoxContainsPointFilter filter_dVIo = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i]);
								FilteredElementCollector collector_dVIo = new FilteredElementCollector(doc, activeView.Id);
								IList<Element> elements_dVIo = collector_dVIo.OfClass(typeof(Wall)).WherePasses(filter_dVIo).ToElements();
								//TaskDialog.Show("ALERTA !elements_dVIo.Any()", elements_dVIo.Any().ToString());
								if (!elements_dVIo.Any())
								{
									VIo_previo_vacio = true;
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
									VFo_previo_vacio = true;
									double VIo = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
									double VFo = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);
									double n_paneles_VF = Math.Round((VFo * n_paneles) / lenght_double, 0);

									double lenght_double_VF = Math.Round(n_paneles_VF * _anchopanel_ + (n_paneles_VF - 1) * (4 / 304.8), 0);
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
										if ((anchoventanas[i] == _anchopanel_ / 304.8) || (anchoventanas[i] > _anchopanel_ / 304.8))
										{
											//TaskDialog.Show("ALERTA", "VENTANA EXACTA activador" + Environment.NewLine + (  anchoventanas[i] > _anchopanel_ ).ToString());
											activador = true; // VENTANA EXACTA
										}


										elements_dVFo.Add(elements_dPH.First());
										bool_dVFo = true;

									}
								}




								#endregion

								if (elements_dVIo.First().Id == elements_dVFo.First().Id) // si VIo y VFo estan en el mismo WALL
								{

									#region si VIo y VFo estan en el mismo WALL
									if (i < (cuenta - 2))
									{
										#region if

										#region boolean [i+2]

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
										#endregion

										if (boolean_2)
										{
											#region if
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

											#endregion

											i = i + 1;
										}
										else
										{
											#region else

											#region collector dVI dVF [i+1]
											BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
											FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
											// Elementos que contienen el punto dVIo
											IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

											BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
											FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
											IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
											#endregion

											#region boolean1_VI y boolean1_VF [i+1]

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
											#endregion

											if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
											{
												#region if

												if (sillventanas[i] == 0 && sillventanas[i + 1] == 0)
												{
													#region  Puerta - Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] == 0 && sillventanas[i + 1] > 0)
												{
													//Puerta - Ventana
													#region  Puerta - Ventana
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] == 0)
												{
													//Ventana - Puerta
													#region  Ventana - Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA

													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i + 1], anchoventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i + 1], alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] > 0)
												{
													#region  Ventana - Ventana

													Wall aaaaaa = elements_dVIo.First() as Wall;

													#region Doble void
													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = aaaaaa.Document.Create.NewOpening(aaaaaa, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 1))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 0))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 0);
														trans.Commit();


													}
													using (Transaction trans = new Transaction(doc, "wall2"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

														Opening openin = aaaaaa.Document.Create.NewOpening(aaaaaa, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 1))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 0))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(aaaaaa.Id);
													#endregion
												}
												#endregion
												i = i + 1;
											}
											else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
											{
												#region else if
												if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // Ventana - Puerta
												{

													#region Ventana - Puerta

													Wall aaaaaa = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);

												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // Ventana - Ventana
												{
													#region Ventana - Ventana

													Wall aaaaaa = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados Puerta - Puerta
												{
													#region Puerta Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														// A
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																										 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return(aaaaaa, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													#endregion
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo Puerta - Ventana
												{

													#region Puerta Ventana
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);



													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														// B	
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{

														double VI_nuf_2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf_2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);
														// if
														if (VF_nuf == VI_nuf_2)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i],
																														sillventanas[i + 1], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return(aaaaaa, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}


													}
													#endregion
												}

												#endregion
											}
											else if (!boolean1_VI && !boolean1_VF) // no choca con nada
											{
												#region no choca con nada
												if (bool_dVFo || bool_dVIo)
												{
													if (!activador)
													{
														#region Ventana o Puerta Dentro de Muro Void
														Wall mierda = elements_dVIo.First() as Wall;
														if (sillventanas[i] == 0) // Es puerta
														{
															#region puerta

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA
																//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																lista_walls.Add(wall_recibida);
															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
																//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

																lista_walls.Add(wall_recibida);
															}
															else
															{
																if (!(lista_NO_VIo.Contains(mierda.Id)))
																{
																	Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																	lista_walls.Add(wall_recibida);
																}
															}

															#region borrador

															//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
															//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
															//													
															//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
															//													
															//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
															//													{
															//														// Transf. BORDE PUERTA a la IZQUIERDA
															//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
															//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															//														lista_walls.Add(wall_recibida);
															//														
															//													}
															//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
															//													{
															//														// Transf. BORDE PUERTA a la DERECHA
															//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
															//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															//														lista_walls.Add(wall_recibida);
															//													}
															//													else
															//													{
															//														
															//														
															//													}
															#endregion

															#endregion
														}
														else
														{
															#region Ventana

															if (!(lista_NO_VIo.Contains(mierda.Id)))
															{
																using (Transaction trans = new Transaction(doc, "wall"))
																{
																	trans.Start();

																	XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																	XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																	Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

																	if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																		WallUtils.DisallowWallJoinAtEnd(mierda, 1);

																	if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																		WallUtils.DisallowWallJoinAtEnd(mierda, 0);
																	trans.Commit();

																	lista_walls.Add(mierda);
																}
															}
															#endregion
														}

														#endregion
													}
													else
													{
														Wall mierda = elements_dVIo.First() as Wall;
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															#region Ventana Exacta

															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#endregion
														}
													}


												}
												else
												{
													#region Ventana o Puerta Dentro de Muro Void
													Wall mierda = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{
														#region puerta

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA
															//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
															//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}
														else
														{
															if (!(lista_NO_VIo.Contains(mierda.Id)))
															{
																Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}

														#region borrador

														//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
														//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
														//													
														//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
														//													
														//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
														//													{
														//														// Transf. BORDE PUERTA a la IZQUIERDA
														//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//														
														//													}
														//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
														//													{
														//														// Transf. BORDE PUERTA a la DERECHA
														//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//													}
														//													else
														//													{
														//														
														//														
														//													}
														#endregion

														#endregion
													}
													else
													{
														#region Ventana
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 0);
																trans.Commit();

																lista_walls.Add(mierda);
															}
														}
														#endregion
													}

													#endregion
												}
												#endregion
											}
											#endregion
										}
										#endregion
									}
									else
									{
										if (i < (cuenta - 1))
										{
											#region else if

											#region collector dVI dVF [i+1]
											BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
											FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
											// Elementos que contienen el punto dVIo
											IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

											BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
											FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
											IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
											#endregion

											#region boolean1_VI y boolean1_VF [i+1]

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
											#endregion

											if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
											{
												#region if

												if (sillventanas[i] == 0 && sillventanas[i + 1] == 0)
												{
													#region  Puerta - Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] == 0 && sillventanas[i + 1] > 0)
												{
													//Puerta - Ventana
													#region  Puerta - Ventana
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] == 0)
												{
													//Ventana - Puerta
													#region  Ventana - Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA

													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(aaaaaa, alturaventanas[i + 1], anchoventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida.Id);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(aaaaaa.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(aaaaaa, anchoventanas[i + 1], alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1], Punto_Ventada_dPH);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida.Document.Create.NewOpening(wall_recibida, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida.Id);
														}
													}

													#endregion
												}
												else if (sillventanas[i] > 0 && sillventanas[i + 1] > 0)
												{
													#region  Ventana - Ventana

													Wall aaaaaa = elements_dVIo.First() as Wall;

													#region Doble void
													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = aaaaaa.Document.Create.NewOpening(aaaaaa, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 1))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 0))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 0);
														trans.Commit();


													}
													using (Transaction trans = new Transaction(doc, "wall2"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

														Opening openin = aaaaaa.Document.Create.NewOpening(aaaaaa, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 1))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(aaaaaa, 0))
															WallUtils.DisallowWallJoinAtEnd(aaaaaa, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(aaaaaa.Id);
													#endregion
												}
												#endregion
												i = i + 1;
											}
											else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
											{
												#region else if
												if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // Ventana - Puerta
												{

													#region Ventana - Puerta

													Wall aaaaaa = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);

												}
												else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // Ventana - Ventana
												{
													#region Ventana - Ventana

													Wall aaaaaa = elements_dVIo.First() as Wall;
													Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i + 1], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1]);

													using (Transaction trans = new Transaction(doc, "wall"))
													{
														trans.Start();

														XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
														XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

														Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

														if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
															WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
														trans.Commit();


													}
													#endregion

													lista_NO_VIo.Add(wall_recibida_dVIo.Id);
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados Puerta - Puerta
												{
													#region Puerta Puerta
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														// A
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																										 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return(aaaaaa, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													#endregion
												}
												else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo Puerta - Ventana
												{

													#region Puerta Ventana
													Wall aaaaaa = elements_dVIo.First() as Wall;
													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														// B	
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA

													}
													else
													{
														double VI_nuf_2 = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf_2 = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);
														// if
														if (VF_nuf == VI_nuf_2)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i],
																														sillventanas[i + 1], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return(aaaaaa, alturaventanas[i],
																																				 sillventanas[i], alturaventanas[i + 1],
																																				 sillventanas[i + 1], Puntos_Ventada_dVIo[i],
																																				 Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1]);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}
													}
													#endregion
												}

												#endregion
											}
											else if (!boolean1_VI && !boolean1_VF) // no choca con nada
											{
												#region no choca con nada
												if (bool_dVFo || bool_dVIo)
												{
													if (!activador)
													{
														#region Ventana o Puerta Dentro de Muro Void
														Wall mierda = elements_dVIo.First() as Wall;
														if (sillventanas[i] == 0) // Es puerta
														{
															#region puerta

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA
																//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																lista_walls.Add(wall_recibida);
															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
																//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

																lista_walls.Add(wall_recibida);
															}
															else
															{
																if (!(lista_NO_VIo.Contains(mierda.Id)))
																{
																	Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																	lista_walls.Add(wall_recibida);
																}
															}

															#region borrador

															//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
															//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
															//													
															//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
															//													
															//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
															//													{
															//														// Transf. BORDE PUERTA a la IZQUIERDA
															//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
															//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															//														lista_walls.Add(wall_recibida);
															//														
															//													}
															//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
															//													{
															//														// Transf. BORDE PUERTA a la DERECHA
															//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
															//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															//														lista_walls.Add(wall_recibida);
															//													}
															//													else
															//													{
															//														
															//														
															//													}
															#endregion

															#endregion
														}
														else
														{
															#region Ventana
															if (!(lista_NO_VIo.Contains(mierda.Id)))
															{
																using (Transaction trans = new Transaction(doc, "wall"))
																{
																	trans.Start();

																	XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																	XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																	Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

																	if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																		WallUtils.DisallowWallJoinAtEnd(mierda, 1);

																	if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																		WallUtils.DisallowWallJoinAtEnd(mierda, 0);
																	trans.Commit();

																	lista_walls.Add(mierda);
																}
															}
															#endregion
														}

														#endregion
													}
													else
													{
														Wall mierda = elements_dVIo.First() as Wall;
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															#region Ventana Exacta

															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#endregion
														}
													}
												}
												else
												{
													#region Ventana o Puerta Dentro de Muro Void
													Wall mierda = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{
														#region puerta

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA
															//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
															//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}
														else
														{
															if (!(lista_NO_VIo.Contains(mierda.Id)))
															{
																Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}

														#region borrador

														//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
														//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
														//													
														//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
														//													
														//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
														//													{
														//														// Transf. BORDE PUERTA a la IZQUIERDA
														//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//														
														//													}
														//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
														//													{
														//														// Transf. BORDE PUERTA a la DERECHA
														//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//													}
														//													else
														//													{
														//														
														//														
														//													}
														#endregion

														#endregion
													}
													else
													{
														#region Ventana
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 0);
																trans.Commit();

																lista_walls.Add(mierda);
															}
														}
														#endregion
													}

													#endregion
												}
												#endregion
											}
											#endregion
										}
										else // no choca con nada
										{
											#region no choca con nada
											if (bool_dVFo || bool_dVIo)
											{
												if (!activador)
												{
													#region Ventana o Puerta Dentro de Muro Void
													Wall mierda = elements_dVIo.First() as Wall;
													if (sillventanas[i] == 0) // Es puerta
													{
														#region puerta

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA
															//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															lista_walls.Add(wall_recibida);
														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
															//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

															lista_walls.Add(wall_recibida);
														}
														else
														{
															if (!(lista_NO_VIo.Contains(mierda.Id)))
															{
																Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
																lista_walls.Add(wall_recibida);
															}
														}

														#region borrador

														//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
														//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
														//													
														//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
														//													
														//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
														//													{
														//														// Transf. BORDE PUERTA a la IZQUIERDA
														//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//														
														//													}
														//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
														//													{
														//														// Transf. BORDE PUERTA a la DERECHA
														//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
														//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														//														lista_walls.Add(wall_recibida);
														//													}
														//													else
														//													{
														//														
														//														
														//													}
														#endregion

														#endregion
													}
													else
													{
														#region Ventana
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																	WallUtils.DisallowWallJoinAtEnd(mierda, 0);
																trans.Commit();

																lista_walls.Add(mierda);
															}
														}
														#endregion
													}

													#endregion
												}
												else
												{
													Wall mierda = elements_dVIo.First() as Wall;
													if (!(lista_NO_VIo.Contains(mierda.Id)))
													{
														#region Ventana Exacta

														Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														#endregion
													}
												}
											}
											else
											{
												#region Ventana o Puerta Dentro de Muro Void
												Wall mierda = elements_dVIo.First() as Wall;
												if (sillventanas[i] == 0) // Es puerta
												{
													#region puerta

													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
													{
														// Transf. BORDE PUERTA a la IZQUIERDA
														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														lista_walls.Add(wall_recibida);
													}
													else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
													{
														// Transf. BORDE PUERTA a la DERECHA
														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
														//Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(mierda, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);

														lista_walls.Add(wall_recibida);
													}
													else
													{
														if (!(lista_NO_VIo.Contains(mierda.Id)))
														{
															Wall wall_recibida = Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(mierda, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], Punto_Ventada_dPH);
															lista_walls.Add(wall_recibida);
														}
													}

													#region borrador

													//													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i]/2, 0);
													//													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i]/2, 0);
													//													
													//													//TaskDialog.Show("Es puerta", VI_nuf.ToString() + Environment.NewLine + VF_nuf.ToString() + Environment.NewLine + lenght_double.ToString());
													//													
													//													if (  (VI_nuf == 0)  &&  !(VF_nuf == Math.Round(lenght_double, 0))  )
													//													{
													//														// Transf. BORDE PUERTA a la IZQUIERDA
													//														//TaskDialog.Show("BORDE PUERTA a la IZQUIERDA", VI_nuf.ToString());
													//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
													//														lista_walls.Add(wall_recibida);
													//														
													//													}
													//													else if (  (VF_nuf == Math.Round(lenght_double, 0))  &&  !(VI_nuf == 0)  )
													//													{
													//														// Transf. BORDE PUERTA a la DERECHA
													//														//TaskDialog.Show("BORDE PUERTA a la DERECHA", VI_nuf.ToString());
													//														Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(mierda, alturaventanas[i], anchoventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i], Puntos_Ventada_dVFo[i], height_double);
													//														lista_walls.Add(wall_recibida);
													//													}
													//													else
													//													{
													//														
													//														
													//													}
													#endregion

													#endregion
												}
												else
												{
													#region Ventana
													if (!(lista_NO_VIo.Contains(mierda.Id)))
													{
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = mierda.Document.Create.NewOpening(mierda, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 1))
																WallUtils.DisallowWallJoinAtEnd(mierda, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(mierda, 0))
																WallUtils.DisallowWallJoinAtEnd(mierda, 0);
															trans.Commit();

															lista_walls.Add(mierda);
														}
													}
													#endregion
												}

												#endregion
											}
											#endregion
										}
									}
									#endregion

								}
								else // si VIo y VFo NO estan en el mismo WALL
								{

									#region si VIo y VFo NO estan en el mismo WALL

									if (!(elements_dVIo.First().Id == elements_dPH.First().Id))
									{
										if (!(elements_dVFo.First().Id == elements_dPH.First().Id))
										{

											#region i < (cuenta -2) y  i < (cuenta -1)   y   no choca con nada

											if (i < (cuenta - 2))
											{
												#region if

												#region boolean [i+2]

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
												#endregion

												if (boolean_2)
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region dPH
													foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
													{
														//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													}
													#endregion

													#region VFo

													if ((sillventanas[i + 1] == 0)) // en medio puerta
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
													}
													else if ((sillventanas[i + 1] > 0)) // en medio ventana
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 2], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}

													}

													#endregion

													i = i + 1;
												}
												else
												{
													#region else

													#region collector dVI dVF [i+1]
													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
													#endregion

													#region boolean1_VI y boolean1_VF [i+1]

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
													#endregion

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH
														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{
															#region Puerta Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#region void

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion

															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{
															#region  Ventana Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															#region 2 puerta  EN medio y a la izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// A
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															#region Puerta en medio Ventana a la Izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0); // Puerta
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);  // Ventana
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// C
																#region C

																if (VF_nuf_1 == VI_nuf)
																{

																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}
																#endregion

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}

															#endregion

														}

														#endregion

														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH
														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														#endregion

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH
														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}
														#endregion

														#region VFo


														if (sillventanas[i] == 0)
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if (VF_nuf == Math.Round(lenght_double, 0))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														#endregion

													}
													#endregion
												}
												#endregion
											}
											else
											{
												if (i < (cuenta - 1))
												{
													#region else if

													#region collector dVI dVF [i+1]
													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
													#endregion

													#region boolean1_VI y boolean1_VF [i+1]

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
													#endregion

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH
														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{
															#region Puerta Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#region void

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion

															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{
															#region  Ventana Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															#region 2 puerta  EN medio y a la izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// A
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															#region Puerta en medio Ventana a la Izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// C
																#region C

																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}
																#endregion

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}

															#endregion

														}

														#endregion

														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH
														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														#endregion

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region dPH

														foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
														{
															//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}

														#endregion

														#region VFo


														if (sillventanas[i] == 0)
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if (VF_nuf == Math.Round(lenght_double, 0))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														#endregion

													}
													#endregion
												}
												else // no choca con nada
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region dPH
													foreach (Wall wall_ii in elements_dPH) // Elementos que contienen el punto dPH
													{
														//Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(wall_ii, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(wall_ii, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													}
													#endregion

													#region VFo


													if (sillventanas[i] == 0)
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if (VF_nuf == Math.Round(lenght_double, 0))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													#endregion

												}
											}

											#endregion

										}
										else
										{

											#region i < (cuenta -2) y  i < (cuenta -1)   y   no choca con nada

											if (i < (cuenta - 2))
											{
												#region if

												#region boolean [i+2]

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
												#endregion

												if (boolean_2)
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}


													}
													#endregion

													#region VFo

													if ((sillventanas[i + 1] == 0)) // en medio puerta
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																													alturaventanas[i + 1], sillventanas[i + 1],
																													alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																													Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																													Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
													}
													else if ((sillventanas[i + 1] > 0)) // en medio ventana
													{
														if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//																	TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 2], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);

															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																									 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																									 Puntos_Ventada_dVFo[i], height_double);
															#region void
															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

																Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
																trans.Commit();


															}
															#endregion

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}

													}

													#endregion

													i = i + 1;
												}
												else
												{
													#region else

													#region collector dVI dVF [i+1]
													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
													#endregion

													#region boolean1_VI y boolean1_VF [i+1]

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
													#endregion

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{
															#region Puerta Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#region void

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion

															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{
															#region  Ventana Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															#region 2 puerta  EN medio y a la izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// A
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															#region Puerta en medio Ventana a la Izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// C
																#region C

																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}
																#endregion

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}

															#endregion

														}

														#endregion

														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														#endregion

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo


														if (sillventanas[i] == 0)
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if (VF_nuf == Math.Round(lenght_double, 0))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														#endregion

													}
													#endregion
												}
												#endregion
											}
											else
											{
												if (i < (cuenta - 1))
												{
													#region else if

													#region collector dVI dVF [i+1]
													BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
													FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
													// Elementos que contienen el punto dVIo
													IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

													BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
													FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
													IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
													#endregion

													#region boolean1_VI y boolean1_VF [i+1]

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
													#endregion

													if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
														{
															#region Puerta Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															#region void

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion

															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);

														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
														{
															#region  Ventana Ventana

															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

															using (Transaction trans = new Transaction(doc, "wall"))
															{
																trans.Start();

																XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
																XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

																Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

																if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																	WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
																trans.Commit();


															}
															#endregion
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
														{
															#region 2 puerta  EN medio y a la izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// A
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}

															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
														{
															#region Puerta en medio Ventana a la Izquierda
															Wall aaaaaa = elements_dVFo.First() as Wall;
															double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

															double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																// Transf. BORDE PUERTA a la IZQUIERDA

															}
															else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
															{
																// Transf. BORDE PUERTA a la DERECHA
																// C
																#region C

																if (VF_nuf_1 == VI_nuf)
																{

																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																										   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																										   , Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																										 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																										 Puntos_Ventada_dVFo[i], height_double);

																	lista_NO_VIo.Add(wall_recibida.Id);
																}
																#endregion

															}
															else
															{
																if (VF_nuf_1 == VI_nuf)
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																										 sillventanas[i + 1],
																																										 Puntos_Ventada_dVFo[i + 1],
																																										 Puntos_Ventada_dVIo[i + 1]);
																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																					   sillventanas[i], alturaventanas[i + 1],
																																					   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																					   Puntos_Ventada_dVIo[i + 1],
																																					   Puntos_Ventada_dVFo[i + 1]);


																	lista_NO_VIo.Add(wall_recibida_dVIo.Id);
																}
															}

															#endregion

														}

														#endregion

														i = i + 1;

													}
													else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo

														if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
														{
															#region 2 puerta  ambos lados

															//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																									 Puntos_Ventada_dVIo[i + 1], height_double);
															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
														{
															#region puerta al lado izquierdo 
															//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
														{
															#region puerta al lado derecho
															//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
														{
															#region 2 Ventandas ambos lados
															//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															#endregion
														}
														#endregion

													}
													else if (!boolean1_VI && !boolean1_VF) // no choca con nada
													{
														#region VIo
														if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
														{
															Wall aaaaaa = elements_dVIo.First() as Wall;

															if (sillventanas[i] == 0)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																#region else
																double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
																double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

																if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																else
																{
																	Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																	//lista_walls.Add(wall_recibida_dVIo);
																}
																#endregion
															}

														}
														#endregion

														#region VFo


														if (sillventanas[i] == 0)
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;

															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if (VF_nuf == Math.Round(lenght_double, 0))
															{
																// Transf. BORDE PUERTA a la DERECHA

																if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
																{
																	Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																}

															}
															else
															{
																Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
																lista_NO_VIo.Add(wall_recibida_dVFo.Id);
															}


														}
														else
														{
															Wall aaaaaa = elements_dVFo.First() as Wall;
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}
														#endregion

													}
													#endregion
												}
												else // no choca con nada
												{

													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo


													if (sillventanas[i] == 0)
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if (VF_nuf == Math.Round(lenght_double, 0))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													#endregion

												}
											}
											#endregion

										}

									}
									else
									{

										#region i < (cuenta -2) y  i < (cuenta -1)   y   no choca con nada

										if (i < (cuenta - 2))
										{
											#region if

											#region boolean [i+2]

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
											#endregion

											if (boolean_2)
											{
												#region VIo
												if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
												{
													Wall aaaaaa = elements_dVIo.First() as Wall;

													if (sillventanas[i] == 0)
													{
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
														//lista_walls.Add(wall_recibida_dVIo);
													}
													else
													{
														#region else
														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														#endregion
													}


												}
												#endregion

												#region VFo

												if ((sillventanas[i + 1] == 0)) // en medio puerta
												{
													if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
													{
														#region 2 puerta  ambos lados

														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
													{
														#region puerta al lado izquierdo 
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
													{
														#region puerta al lado derecho
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(aaaaaa, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
													{
														#region 2 Ventandas ambos lados
														//TaskDialog.Show("2 puerta  ambos lados", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(aaaaaa, alturaventanas[i], sillventanas[i],
																												alturaventanas[i + 1], sillventanas[i + 1],
																												alturaventanas[i + 2], sillventanas[i + 2], Puntos_Ventada_dVIo[i],
																												Puntos_Ventada_dVFo[i], Puntos_Ventada_dVIo[i + 1], Puntos_Ventada_dVFo[i + 1],
																												Puntos_Ventada_dVIo[i + 2], Puntos_Ventada_dVFo[i + 2]);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
												}
												else if ((sillventanas[i + 1] > 0)) // en medio ventana
												{
													if ((sillventanas[i] == 0) && (sillventanas[i + 2] == 0)) // 2 puerta  ambos lados
													{
														#region 2 puerta  ambos lados

														//																	TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 2], height_double);
														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
															trans.Commit();


														}
														#endregion

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 2] > 0)) // puerta al lado izquierdo 
													{
														#region puerta al lado izquierdo 
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);
														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
															trans.Commit();


														}
														#endregion

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] == 0)) // puerta al lado derecho
													{
														#region puerta al lado derecho
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);

														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
															trans.Commit();


														}
														#endregion

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 2] > 0)) // 2 Ventandas ambos lados
													{
														#region 2 Ventandas ambos lados
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 2],
																								 sillventanas[i], sillventanas[i + 2], Puntos_Ventada_dVIo[i + 2],
																								 Puntos_Ventada_dVFo[i], height_double);
														#region void
														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i].X, Puntos_Ventada_dVFo[i].Y, Puntos_Ventada_dVFo[i].Z + alturaventanas[i]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i].X, Puntos_Ventada_dVIo[i].Y, Puntos_Ventada_dVIo[i].Z);

															Opening openin = wall_recibida_dVIo.Document.Create.NewOpening(wall_recibida_dVIo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVIo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVIo, 0);
															trans.Commit();


														}
														#endregion

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}

												}

												#endregion

												i = i + 1;
											}
											else
											{
												#region else

												#region collector dVI dVF [i+1]
												BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
												FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
												// Elementos que contienen el punto dVIo
												IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

												BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
												FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
												IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
												#endregion

												#region boolean1_VI y boolean1_VF [i+1]

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
												#endregion

												if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo

													if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
													{
														#region Puerta Ventana

														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														#region void

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
															trans.Commit();


														}
														#endregion

														#endregion
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
													{
														#region  Ventana Ventana

														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
													{
														#region 2 puerta  EN medio y a la izquierda
														Wall aaaaaa = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															// A
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
													{
														#region Puerta en medio Ventana a la Izquierda
														Wall aaaaaa = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

														double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															// C
															#region C

															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																									   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																									   , Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															#endregion

														}
														else
														{
															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																									 sillventanas[i + 1],
																																									 Puntos_Ventada_dVFo[i + 1],
																																									 Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
														}

														#endregion

													}

													#endregion

													i = i + 1;

												}
												else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo

													if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
													{
														#region 2 puerta  ambos lados

														//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
													{
														#region puerta al lado izquierdo 
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
													{
														#region puerta al lado derecho
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
													{
														#region 2 Ventandas ambos lados
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													#endregion

												}
												else if (!boolean1_VI && !boolean1_VF) // no choca con nada
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo


													if (sillventanas[i] == 0)
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if (VF_nuf == Math.Round(lenght_double, 0))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													#endregion

												}
												#endregion
											}
											#endregion
										}
										else
										{
											if (i < (cuenta - 1))
											{
												#region else if

												#region collector dVI dVF [i+1]
												BoundingBoxContainsPointFilter filter_dVIo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVIo[i + 1]);
												FilteredElementCollector collector_dVIo_2 = new FilteredElementCollector(doc, activeView.Id);
												// Elementos que contienen el punto dVIo
												IList<Element> elements_dVIo_2 = collector_dVIo_2.OfClass(typeof(Wall)).WherePasses(filter_dVIo_2).ToElements();

												BoundingBoxContainsPointFilter filter_dVFo_2 = new BoundingBoxContainsPointFilter(Puntos_Ventada_dVFo[i + 1]);
												FilteredElementCollector collector_dVFo_2 = new FilteredElementCollector(doc, activeView.Id);
												IList<Element> elements_dVFo_2 = collector_dVFo_2.OfClass(typeof(Wall)).WherePasses(filter_dVFo_2).ToElements();
												#endregion

												#region boolean1_VI y boolean1_VF [i+1]

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
												#endregion

												if (boolean1_VF && boolean1_VI) // choca con VI y VF [i+1]
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo

													if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0))
													{
														#region Puerta Ventana

														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														#region void

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
															trans.Commit();


														}
														#endregion

														#endregion
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);

													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0))
													{
														#region  Ventana Ventana

														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);

														using (Transaction trans = new Transaction(doc, "wall"))
														{
															trans.Start();

															XYZ arriba_dVFo = new XYZ(Puntos_Ventada_dVFo[i + 1].X, Puntos_Ventada_dVFo[i + 1].Y, Puntos_Ventada_dVFo[i + 1].Z + alturaventanas[i + 1]);
															XYZ abajo_dVIo = new XYZ(Puntos_Ventada_dVIo[i + 1].X, Puntos_Ventada_dVIo[i + 1].Y, Puntos_Ventada_dVIo[i + 1].Z);

															Opening openin = wall_recibida_dVFo.Document.Create.NewOpening(wall_recibida_dVFo, abajo_dVIo, arriba_dVFo);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 1))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 1);

															if (WallUtils.IsWallJoinAllowedAtEnd(wall_recibida_dVFo, 0))
																WallUtils.DisallowWallJoinAtEnd(wall_recibida_dVFo, 0);
															trans.Commit();


														}
														#endregion
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0))
													{
														#region 2 puerta  EN medio y a la izquierda
														Wall aaaaaa = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															// A
															Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);

															lista_NO_VIo.Add(wall_recibida.Id);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(aaaaaa, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


															lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														}

														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0))
													{
														#region Puerta en medio Ventana a la Izquierda
														Wall aaaaaa = elements_dVFo.First() as Wall;
														double VI_nuf = Math.Round(lista_dPH[i + 1] - anchoventanas[i + 1] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i + 1] + anchoventanas[i + 1] / 2, 0);

														double VI_nuf_1 = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf_1 = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															// Transf. BORDE PUERTA a la IZQUIERDA

														}
														else if ((VF_nuf == Math.Round(lenght_double, 0)) && !(VI_nuf == 0)) // BORDE Final
														{
															// Transf. BORDE PUERTA a la DERECHA
															// C
															#region C

															if (VF_nuf_1 == VI_nuf)
															{

																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(aaaaaa, anchoventanas[i], alturaventanas[i]
																																									   , alturaventanas[i + 1], sillventanas[i], sillventanas[i + 1]
																																									   , Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																									 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																									 Puntos_Ventada_dVFo[i], height_double);

																lista_NO_VIo.Add(wall_recibida.Id);
															}
															#endregion

														}
														else
														{
															if (VF_nuf_1 == VI_nuf)
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(aaaaaa, alturaventanas[i + 1], sillventanas[i],
																																									 sillventanas[i + 1],
																																									 Puntos_Ventada_dVFo[i + 1],
																																									 Puntos_Ventada_dVIo[i + 1]);
																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(aaaaaa, alturaventanas[i],
																																				   sillventanas[i], alturaventanas[i + 1],
																																				   sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																																				   Puntos_Ventada_dVIo[i + 1],
																																				   Puntos_Ventada_dVFo[i + 1]);


																lista_NO_VIo.Add(wall_recibida_dVIo.Id);
															}
														}

														#endregion

													}

													#endregion

													i = i + 1;

												}
												else if (boolean1_VI && !boolean1_VF) // choca solo con VI [i+1]
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo

													if ((sillventanas[i] == 0) && (sillventanas[i + 1] == 0)) // 2 puerta  ambos lados
													{
														#region 2 puerta  ambos lados

														//TaskDialog.Show("ALERTA LAOS: 2 puerta  ambos lados", elements_dVFo.First().Id.ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVFo[i],
																								 Puntos_Ventada_dVIo[i + 1], height_double);
														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] == 0) && (sillventanas[i + 1] > 0)) // puerta al lado izquierdo 
													{
														#region puerta al lado izquierdo 
														//TaskDialog.Show("puerta al lado izquierdo i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] == 0)) // puerta al lado derecho
													{
														#region puerta al lado derecho
														//TaskDialog.Show("puerta al lado derecho i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													else if ((sillventanas[i] > 0) && (sillventanas[i + 1] > 0)) // 2 Ventandas ambos lados
													{
														#region 2 Ventandas ambos lados
														//TaskDialog.Show("2 Ventandas ambos lados i < (cuenta -2) ", lista_walls.Count().ToString());
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_I_return(aaaaaa, alturaventanas[i], alturaventanas[i + 1],
																								 sillventanas[i], sillventanas[i + 1], Puntos_Ventada_dVIo[i + 1],
																								 Puntos_Ventada_dVFo[i], height_double);

														lista_NO_VIo.Add(wall_recibida_dVIo.Id);
														#endregion
													}
													#endregion

												}
												else if (!boolean1_VI && !boolean1_VF) // no choca con nada
												{
													#region VIo
													if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
													{
														Wall aaaaaa = elements_dVIo.First() as Wall;

														if (sillventanas[i] == 0)
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															#region else
															double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
															double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

															if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															else
															{
																Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
																//lista_walls.Add(wall_recibida_dVIo);
															}
															#endregion
														}

													}
													#endregion

													#region VFo


													if (sillventanas[i] == 0)
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;

														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if (VF_nuf == Math.Round(lenght_double, 0))
														{
															// Transf. BORDE PUERTA a la DERECHA

															if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
															{
																Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															}

														}
														else
														{
															Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
															lista_NO_VIo.Add(wall_recibida_dVFo.Id);
														}


													}
													else
													{
														Wall aaaaaa = elements_dVFo.First() as Wall;
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}
													#endregion

												}
												#endregion
											}
											else // no choca con nada
											{

												#region VIo
												if (!(lista_NO_VIo.Contains(elements_dVIo.First().Id)))
												{
													Wall aaaaaa = elements_dVIo.First() as Wall;

													if (sillventanas[i] == 0)
													{
														Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
														//lista_walls.Add(wall_recibida_dVIo);
													}
													else
													{
														#region else
														double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
														double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

														if ((VI_nuf == 0) && !(VF_nuf == Math.Round(lenght_double, 0))) // BORDE Inicio
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														else
														{
															Wall wall_recibida_dVIo = Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVIo[i]);
															//lista_walls.Add(wall_recibida_dVIo);
														}
														#endregion
													}

												}
												#endregion

												#region VFo


												if (sillventanas[i] == 0)
												{
													Wall aaaaaa = elements_dVFo.First() as Wall;

													double VI_nuf = Math.Round(lista_dPH[i] - anchoventanas[i] / 2, 0);
													double VF_nuf = Math.Round(lista_dPH[i] + anchoventanas[i] / 2, 0);

													if (VF_nuf == Math.Round(lenght_double, 0))
													{
														// Transf. BORDE PUERTA a la DERECHA

														if (aaaaaa.Id == ultimo_ultimo_wall.Id) // otra opcio: listaWalls_Final_ID.Last().Id == wall_ii.Id
														{
															Revision6_DYNO_Create_New_Wall_1MURO_Solitario(aaaaaa, anchoventanas[i], alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														}

													}
													else
													{
														Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
														lista_NO_VIo.Add(wall_recibida_dVFo.Id);
													}


												}
												else
												{
													Wall aaaaaa = elements_dVFo.First() as Wall;
													Wall wall_recibida_dVFo = Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(aaaaaa, alturaventanas[i], sillventanas[i], Puntos_Ventada_dVFo[i]);
													lista_NO_VIo.Add(wall_recibida_dVFo.Id);
												}
												#endregion

											}
										}
										#endregion

									}
									#endregion

								}


								#endregion

							}//fin del FOR

							#endregion
						}

					}

				}
				catch (Exception e)
				{
					//TaskDialog.Show("Error", e.Message.ToString());
					throw;
				}
				finally
				{

				}


			} //OKKKKKKKKKKKKKKKKKKKK!
			  // ESTE FUNCIONA ACTUALMENTE
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_OSB(Wall e, int _anchopanel_)
			{

				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				#endregion


				#region LISTAS

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

				#endregion

				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual

				#region Info Wall

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				#endregion

				// Recolectar Ventanas

				#region Recolectar Ventanas


				#region VENTANAS

				//	        ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//		 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//		 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//
				//		 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//			
				//			List<Element> windows_hosted = new List<Element>();
				//			
				//			foreach (Element elem in windows)
				//            {
				//				FamilyInstance fintance = elem as FamilyInstance;
				//
				//				if (fintance.Host.Id == e.Id) 
				//				{
				//					windows_hosted.Add(elem);
				//				}
				//
				//            }
				#endregion

				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == e.Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}

				#endregion

				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor

				#region INFO TODAS LAS VENTANAS DESPUES DE ORDENARLAS

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
				#endregion

				#region ELIMINAR PRImERA VENTANA

				List<Element> windows_hosted_sorted_2 = new List<Element>();
				foreach (Element win in windows_hosted_sorted)
				{
					if (!(win.Id.ToString() == windows_hosted_sorted.First().Id.ToString()))
					{
						windows_hosted_sorted_2.Add(win);
					}
				}

				#endregion

				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);

				#endregion

				// Crear lista_d

				#region Lista_d = [d1, d2, d3, d4, ...]

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
				}


				#endregion

				// Crear lista_d_2 Separar Paneles Antes dVIo

				#region Separar Paneles Antes dVIo listas_d2

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

				#endregion


				//	        if (longitud_double < anchopanel/304.8) 
				//			{
				//	           	//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
				//	           	return null;
				//			}
				//	        else if (longitud_double == anchopanel/304.8)
				//	        {
				//	           	//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual igual al valor ingresado para la longitud");
				//	           	return null;
				//	        }
				//	        else if (windows_hosted.Count()==0)
				//	        {
				//	        	//TaskDialog.Show("Dynoscript", "El Muro no contiene una Ventana");
				//	           	return null;
				//	        }
				//	        else
				//	        {

				#region else

				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");



				foreach (Element win in windows_hosted_sorted)
				{
					doc.Delete(win.Id);
				}



				double d1 = stParam + (anchopanel / 304.8);
				double d1_4 = d1 + 4 / 304.8;

				XYZ Point_d1 = wallCurve.Evaluate(d1, false);
				XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

				double VIo = Math.Round(lista_dPH[0] - (lista_width1[0] / 2), 0);

				if ((VIo == Math.Round(stParam, 0))) // BORDE INICIAL VIo
				{
					// Crear solo wall de VEntana modificando el inicial

					#region Crear Wall VENTANA
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + 1220 / 304.8;
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

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 0);

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
					//						
					//			            listaWalls_Final_siCambia.Add(wall_F1_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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

					#endregion

				}
				else
				{
					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;
						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls si esta muy lejos de la esquina

						//	           		if (( lista_dVIo.First() - d1 ) > anchopanel )
						//	           		{
						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d_2.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_2[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_2[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);
							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						//	           		}

						#endregion

					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)
						#region Crear Wall Primer cuando la Ventana esta muy cerca a la esquina

						double ultima_dVIo = lista_dVIo.First() - 4 / 304.8;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
						#endregion
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)
						#region Crear Wall Primer cuando la Ventana esta muy cerca a la esquina

						double ultima_dVIo = lista_dVIo.First() - 4 / 304.8;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
						#endregion
					}


					// Crear el MiniWall antes de la Ventana
					#region Crear el MiniWall antes de la Ventana



					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						#region d1 es Menor a dVIo y dVFo

						double ultimo_d = lista_d_muro4_2.Last();
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						Wall wall_peq = Wall.Create(doc, newLine_peq, wall_1.LevelId, false);
						listaWalls_Final_siCambia.Add(wall_peq);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_peq, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_peq, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_peq, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_peq, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_peq.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion

					}
					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						#region d1 es Mayor a dVIo y Menor a dVFo


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						#region d1 es Mayor a dVIo y Menor a dVFo


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion
					}



					#endregion


					// Crear el wall de la Ventana con ancho de 1220
					#region Crear Wall VENTANA
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + 1220 / 304.8;
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

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 0);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}



					Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F1_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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

					#endregion
				}

				// AnchoVentana <= 1220
				if (lista_width1.First() <= 1220 / 304.8)
				{
					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / 304.8;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);
					listaWalls_Final_siCambia.Add(wall_F3);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 0);

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
					#endregion
				}

				// AnchoVentana > 1220
				else if (lista_width1.First() > 1220 / 304.8)
				{
					#region wall pequeño ultimo SOLO Cuando Ventana es Mayor a 1220

					double ultimo_dVFo = lista_dVFo.First();
					XYZ Point_ultimo_dVFo = wallCurve.Evaluate(ultimo_dVFo, false);


					double ultimo_dVIo = lista_dVIo.First();
					XYZ Point_ultimo_dVIo = wallCurve.Evaluate(ultimo_dVIo, false);


					double ultimo_dVIo_1224 = ultimo_dVIo + 1224 / 304.8;
					XYZ Point_ultimo_dVIo_1224 = wallCurve.Evaluate(ultimo_dVIo_1224, false);

					double extra_ventana = lista_width1.First() - 1220 / 304.8;

					double d_panel_ventana = ultimo_dVFo - extra_ventana;


					XYZ Pto_d_panel_ventana = wallCurve.Evaluate(d_panel_ventana, false);

					Line newLineF2 = Line.CreateBound(Point_ultimo_dVIo_1224, Point_ultimo_dVFo);

					if (!(lista_win_sill_height1.First() == 0))
					{
						Wall wall_F2 = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F2);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F2, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F2, 0);

						Parameter WALL_USER_HEIGHT_PARAMF2 = wall_F2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF2.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF2.Set(lista_win_sill_height1.First());
						}
					}


					Wall wall_F2_arriba = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F2_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F2_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F2_arriba, 0);

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

					#endregion

					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / 304.8;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 0);

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
					#endregion
				}

				trans.Commit();


				listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
				listadelistasWalls_Final.Add(listaWalls_Final_noCambia);

				#endregion

				//	    	}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_OSB_1_VENTANA(Wall e, int _anchopanel_)
			{

				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				#endregion


				#region LISTAS

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

				#endregion

				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual

				#region Info Wall

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();
				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				#endregion

				// Recolectar Ventanas

				#region Recolectar Ventanas


				#region VENTANAS

				//	        ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//		 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//		 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//
				//		 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//			
				//			List<Element> windows_hosted = new List<Element>();
				//			
				//			foreach (Element elem in windows)
				//            {
				//				FamilyInstance fintance = elem as FamilyInstance;
				//
				//				if (fintance.Host.Id == e.Id) 
				//				{
				//					windows_hosted.Add(elem);
				//				}
				//
				//            }
				#endregion

				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == e.Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}

				#endregion

				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor

				#region INFO TODAS LAS VENTANAS DESPUES DE ORDENARLAS

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
				#endregion





				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);

				#endregion

				// Crear lista_d

				#region Lista_d = [d1, d2, d3, d4, ...]

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
				}


				#endregion

				// Crear lista_d_2 Separar Paneles Antes dVIo

				#region Separar Paneles Antes dVIo listas_d2

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

				#endregion


				//	        if (longitud_double < anchopanel/304.8) 
				//			{
				//
				//
				//			}
				//	        else if (longitud_double == anchopanel/304.8)
				//	        {
				//	           	//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual igual al valor ingresado para la longitud");
				//	           	return null;
				//	        }
				//	        else if (windows_hosted.Count()==0)
				//	        {
				//	        	//TaskDialog.Show("Dynoscript", "El Muro no contiene una Ventana");
				//	           	return null;
				//	        }
				//	        else
				//	        {
				#region else

				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");



				foreach (Element win in windows_hosted_sorted)
				{
					doc.Delete(win.Id);
				}


				double d1 = stParam + (anchopanel / 304.8);
				double d1_4 = d1 + 4 / 304.8;

				XYZ Point_d1 = wallCurve.Evaluate(d1, false);
				XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

				double VIo = Math.Round(lista_dPH[0] - (lista_width1[0] / 2), 0);

				if ((VIo == Math.Round(stParam, 0))) // BORDE INICIAL VIo
				{
					// Crear solo wall de VEntana modificando el inicial

					#region Crear Wall VENTANA
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + 1220 / 304.8;
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

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 0);

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
					//						
					//			            listaWalls_Final_siCambia.Add(wall_F1_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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

					#endregion
				}
				else
				{
					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;
						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls si esta muy lejos de la esquina

						//	           		if (( lista_dVIo.First() - d1 ) > anchopanel )
						//	           		{
						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d_2.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_2[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_2[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);
							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						//	           		}

						#endregion

					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)
						#region Crear Wall Primer cuando la Ventana esta muy cerca a la esquina

						double ultima_dVIo = lista_dVIo.First() - 4 / 304.8;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
						#endregion
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer (stPoint, Pto_dVIo)
						#region Crear Wall Primer cuando la Ventana esta muy cerca a la esquina

						double ultima_dVIo = lista_dVIo.First() - 4 / 304.8;
						XYZ pto_ultima_dVIo = wallCurve.Evaluate(ultima_dVIo, false);


						// Crear Wall Primer (stPoint, Pto_dVIo)
						Line newLine01 = Line.CreateBound(stPoint, pto_ultima_dVIo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);
						#endregion
					}

					// Crear el MiniWall antes de la Ventana
					#region Crear el MiniWall antes de la Ventana



					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						#region d1 es Menor a dVIo y dVFo

						double ultimo_d = lista_d_muro4_2.Last();
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						Wall wall_peq = Wall.Create(doc, newLine_peq, wall_1.LevelId, false);
						listaWalls_Final_siCambia.Add(wall_peq);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_peq, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_peq, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_peq, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_peq, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_peq.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion

					}
					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						#region d1 es Mayor a dVIo y Menor a dVFo


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion
					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						#region d1 es Mayor a dVIo y Menor a dVFo


						double ultimo_d = stParam;
						XYZ Point_ultimo_d = wallCurve.Evaluate(ultimo_d, false);

						double dVIo = lista_dVIo.First();
						double dVIo_4 = dVIo - 4 / 304.8;
						XYZ Point_dVIo_4 = wallCurve.Evaluate(dVIo_4, false);

						Line newLine_peq = Line.CreateBound(Point_ultimo_d, Point_dVIo_4);

						((LocationCurve)wall_1.Location).Curve = newLine_peq;
						listaWalls_Final_siCambia.Add(wall_1);


						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						Parameter WALL_USER_HEIGHT_PARAM_peq = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM_peq.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM_peq.Set(height_double);
						}
						#endregion
					}



					#endregion

					// Crear el wall de la Ventana con ancho de 1220
					#region Crear Wall VENTANA

					// wall f1
					double dVIo_0 = lista_dVIo.First();

					List<double> sacar = new List<double>();
					if (lista_width1.First() <= 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + lista_width1.First();
						sacar.Add(dVIo_1220);
					}
					else if (lista_width1.First() > 1220 / 304.8)
					{
						double dVIo_1220 = dVIo_0 + 1220 / 304.8;
						sacar.Add(dVIo_1220);
					}


					XYZ Pto_dVIo = wallCurve.Evaluate(dVIo_0, false);
					XYZ Pto_dVIo_1220 = wallCurve.Evaluate(sacar.First(), false);
					Line newLineF1 = Line.CreateBound(Pto_dVIo, Pto_dVIo_1220);

					if (!(lista_win_sill_height1.First() == 0))
					{

						Wall wall_F1 = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F1, 0);

						Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF1.Set(lista_win_sill_height1.First());
						}
					}



					Wall wall_F1_arriba = Wall.Create(doc, newLineF1, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F1_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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

					#endregion
				}




				// AnchoVentana <= 1220
				if (lista_width1.First() <= 1220 / 304.8)
				{
					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / 304.8;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);
					listaWalls_Final_siCambia.Add(wall_F3);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 0);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

					#endregion
				}

				// AnchoVentana > 1220
				else if (lista_width1.First() > 1220 / 304.8)
				{

					#region wall pequeño ultimo SOLO Cuando Ventana es Mayor a 1220

					double ultimo_dVFo = lista_dVFo.First();
					XYZ Point_ultimo_dVFo = wallCurve.Evaluate(ultimo_dVFo, false);


					double ultimo_dVIo = lista_dVIo.First();
					XYZ Point_ultimo_dVIo = wallCurve.Evaluate(ultimo_dVIo, false);


					double ultimo_dVIo_1224 = ultimo_dVIo + 1224 / 304.8;
					XYZ Point_ultimo_dVIo_1224 = wallCurve.Evaluate(ultimo_dVIo_1224, false);

					double extra_ventana = lista_width1.First() - 1220 / 304.8;

					double d_panel_ventana = ultimo_dVFo - extra_ventana;


					XYZ Pto_d_panel_ventana = wallCurve.Evaluate(d_panel_ventana, false);
					Line newLineF2 = Line.CreateBound(Point_ultimo_dVIo_1224, Point_ultimo_dVFo);

					if (!(lista_win_sill_height1.First() == 0))
					{

						Wall wall_F2 = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F2);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F2, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F2, 0);

						Parameter WALL_USER_HEIGHT_PARAMF2 = wall_F2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF2.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF2.Set(lista_win_sill_height1.First());
						}
					}

					Wall wall_F2_arriba = Wall.Create(doc, newLineF2, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F2_arriba);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F2_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F2_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F2_arriba, 0);

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


					#endregion


					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultima_dVFo = lista_dVFo.First();
					double ultima_dVFo_4 = ultima_dVFo + 4 / 304.8;

					XYZ Point_ultima_dVFo_4 = wallCurve.Evaluate(ultima_dVFo_4, false);

					Line newLineF3 = Line.CreateBound(Point_ultima_dVFo_4, endPoint);

					Wall wall_F3 = Wall.Create(doc, newLineF3, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3, 0);

					Parameter WALL_USER_HEIGHT_PARAMF3 = wall_F3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3.Set(height_double);
					}

					#endregion
				}

				trans.Commit();


				listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
				listadelistasWalls_Final.Add(listaWalls_Final_noCambia);

				#endregion
				//	    	}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL(Wall e, int _anchopanel_)
			{

				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				#endregion


				#region LISTAS

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

				#endregion

				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual

				#region Info Wall

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

				#endregion

				// Recolectar Ventanas

				#region Recolectar Ventanas


				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == e.Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}


				//				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//			 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//			 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//			 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//	
				//			 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//				
				//				
				//				foreach (Element elem in windows)
				//	            {
				//					FamilyInstance fintance = elem as FamilyInstance;
				//					if (fintance.Host.Id == e.Id) 
				//					{
				//						windows_hosted.Add(elem);
				//					}
				//	            }

				#endregion

				List<Element> windows_hosted_sorted = windows_hosted.OrderBy(x => (x as FamilyInstance).HostParameter).ToList(); // menor a mayor

				#region INFO TODAS LAS VENTANAS DESPUES DE ORDENARLAS

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

					//				if ((win_sill_height1 > 0) || (win_sill_height1 == 0))
					//				{
					//					
					//				}
					//				else
					//				{
					//					
					//				}


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


					//				string mensaje = width1.ToString() + Environment.NewLine 
					//								+ heigth1.ToString() + Environment.NewLine
					//								+ salida.First().ToString() + Environment.NewLine
					//								+ win.Category.Name.ToString() + Environment.NewLine
					//								+ dPH1.ToString() + Environment.NewLine ;
					//				
					//				TaskDialog.Show("mensaje", mensaje);
				}
				#endregion

				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);

				#endregion

				// Crear lista_d

				#region Lista_d = [d1, d2, d3, d4, ...]

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();

				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
				}


				#endregion


				if (longitud_double < anchopanel / 304.8)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return null;
				}
				else if (longitud_double == anchopanel / 304.8)
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


					double d1 = stParam + (anchopanel / 304.8);
					double d1_4 = d1 + 4 / 304.8;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion

						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 0);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


						#endregion
					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion

						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 0);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


						#endregion

					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion

						// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

						double ultimo_d_4 = lista_d_muro4.Last();
						XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

						Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

						Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_F3_ultimo);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 0);

						Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
						}


						#endregion
					}



					trans.Commit();


					listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
					listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>, List<BoundingBoxXYZ>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas, lista_bb);
				return tupla;
			}
			// ESTE FUNCIONA ACTUALMENTE
			Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_1_VENTANA(Wall e, int _anchopanel_)
			{

				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				#endregion


				#region LISTAS

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

				#endregion

				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual

				#region Info Wall

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

				#endregion

				// Recolectar Ventanas

				#region Recolectar Ventanas


				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

					BuiltInCategory.OST_Doors,
					BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == e.Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}


				//				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//			 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//			 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//			 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//	
				//			 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//				
				//				
				//				foreach (Element elem in windows)
				//	            {
				//					FamilyInstance fintance = elem as FamilyInstance;
				//					if (fintance.Host.Id == e.Id) 
				//					{
				//						windows_hosted.Add(elem);
				//					}
				//	            }

				#endregion



				#region INFO TODAS LAS VENTANAS DESPUES DE ORDENARLAS

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


				//				string mensaje = width1.ToString() + Environment.NewLine 
				//								+ heigth1.ToString() + Environment.NewLine
				//								+ lista_win_sill_height1.First().ToString() + Environment.NewLine ;
				//				
				//				TaskDialog.Show("mensaje", mensaje);

				#endregion



				lista_delistas_conDatosVentanas.Add(lista_width1);
				lista_delistas_conDatosVentanas.Add(lista_heigth1);
				lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				lista_delistas_conDatosVentanas.Add(lista_dPH);

				Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				Puntos_Ventana.Add(Puntos_Ventana_dVFo);

				#endregion

				// Crear lista_d

				#region Lista_d = [d1, d2, d3, d4, ...]

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				List<double> lista_d_muro4_2 = new List<double>();
				List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
				}


				#endregion

				// Crear lista_d_3 Separar Paneles Antes dVIo

				#region Separar Paneles Antes dVIo lista_d3

				List<double> lista_d_3 = new List<double>(); // lista de paneles menor o igual a dVIo



				for (int i = 0; i < lista_d.Count(); i++)
				{
					//				double dVFo = lista_dPH.First() + (lista_width1.First()/2);
					//				double dVIo = lista_dPH.First() - (lista_width1.First()/2);

					if (lista_d[i] < dVFo && lista_d[i] < dVIo)
					{
						lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
						lista_d_muro4_3.Add(lista_d_muro4[i]);
					}
					else if (lista_d[i] < dVFo && lista_d[i] > dVIo)
					{
						lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
						lista_d_muro4_3.Add(lista_d_muro4[i]);
					}
					else if (lista_d[i] > dVFo && lista_d[i] > dVIo)
					{
						lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
						lista_d_muro4_3.Add(lista_d_muro4[i]);

						i = i + lista_d.Count();
					}
				}


				#endregion


				if (longitud_double < anchopanel / 304.8)
				{
					TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return null;
				}
				else if (longitud_double == anchopanel / 304.8)
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



					double d1 = stParam + (anchopanel / 304.8);
					double d1_4 = d1 + 4 / 304.8;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);

					if (d1 < lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Menor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d_3.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_3[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_3[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion


					}

					else if (d1 > lista_dVIo.First() && d1 < lista_dVFo.First()) // d1 es Mayor a dVIo y Menor a dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d_3.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_3[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_3[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion

					}
					else if (d1 > lista_dVIo.First() && d1 > lista_dVFo.First()) // d1 es Mayor a dVIo y dVFo
					{
						// Crear Wall Primer

						#region Crear Wall Primer
						Line newLine01 = Line.CreateBound(stPoint, Point_d1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

						((LocationCurve)wall_1.Location).Curve = newLine01;

						listaWalls_Final_siCambia.Add(wall_1);

						#endregion

						// Crear Walls si esta muy lejos de la esquina

						#region Crear Walls siguientes

						// Crear Walls Antes al dVIo
						for (int i = 0; i < lista_d_3.Count() - 1; i++) // antes dVIo
						{
							double di = lista_d_muro4_3[i] + (anchopanel / 304.8); // d2
							double di_4 = di + 4 / 304.8; // d2_4

							//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
							XYZ PointA = wallCurve.Evaluate(lista_d_muro4_3[i], false);
							XYZ PointB = wallCurve.Evaluate(di, false);

							Line newLineN = Line.CreateBound(PointA, PointB);
							Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

							listaWalls_Final_siCambia.Add(wall_N);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

							if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
								WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

							Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
							if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
							{
								WALL_USER_HEIGHT_PARAM12.Set(height_double);
							}

						}

						#endregion
					}

					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultimo_d_4 = lista_d_muro4_3.Last();
					XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

					Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

					Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3_ultimo);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 0);

					Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
					}

					#endregion

					trans.Commit();


					listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
					listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				}

				Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>> tupla = new Tuple<List<List<Wall>>, List<List<XYZ>>, List<List<double>>>(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				return tupla;
			} // okkkkkkkkkkkkkkkkkkkkk!
			  // ESTE FUNCIONA ACTUALMENTE
			  // ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_PanelizarMuroInicial_SMARTPANEL_0_VENTANA(Wall e, int _anchopanel_)
			{

				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				#endregion


				#region LISTAS

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

				#endregion

				//INPUTS
				int anchopanel = _anchopanel_; //1220
				Wall wall_1 = e as Wall; // muro actual

				#region Info Wall

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

				#endregion

				// Recolectar Ventanas

				#region Recolectar Ventanas


				#region VENTANAS Y PUERTAS


				//				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
				//					{
				//					
				//				    BuiltInCategory.OST_Doors,
				//				    BuiltInCategory.OST_Windows,
				//
				//					};
				//
				//				List<Element> windows_hosted = new List<Element>();	
				//								
				//				foreach (BuiltInCategory bic in bics_familyIns)
				//				{
				//					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//					// Create a category filter for MechanicalEquipment
				//					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
				//					// Create a logic And filter for all MechanicalEquipment Family
				//					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//					// Apply the filter to the elements in the active document
				//					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
				//					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();
				//					
				//					foreach (Element elem in familyinstance)
				//					{
				//						FamilyInstance fintance = elem as FamilyInstance;
				//						if (fintance.Host.Id == e.Id) 
				//						{
				//							windows_hosted.Add(elem);
				//						}
				//					}
				//				}


				//				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//			 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//			 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//			 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//	
				//			 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//				
				//				
				//				foreach (Element elem in windows)
				//	            {
				//					FamilyInstance fintance = elem as FamilyInstance;
				//					if (fintance.Host.Id == e.Id) 
				//					{
				//						windows_hosted.Add(elem);
				//					}
				//	            }

				#endregion



				#region INFO TODAS LAS VENTANAS DESPUES DE ORDENARLAS

				//				FamilyInstance win1 = windows_hosted.First() as FamilyInstance;
				//				FamilySymbol familySymbol = win1.Symbol;
				//				lista_FamilySymbol.Add(familySymbol);
				//				
				//				double dPH1 = win1.HostParameter; //3700
				//				lista_dPH.Add(dPH1);
				//				
				//				ElementType type1 = doc.GetElement(win1.GetTypeId()) as ElementType;
				//				Parameter widthParam1 = type1.LookupParameter("Anchura"); // ancho ventana 1220
				//				Parameter heightParam1 = type1.LookupParameter("Altura"); // altura ventana 1240
				//				
				//
				//				double width1 = widthParam1.AsDouble(); // ancho ventana 1220
				//				lista_width1.Add(width1); 
				//				double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
				//				lista_heigth1.Add(heigth1);
				//				
				//				double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
				//
				//				if (win1.Category.Name.ToString() == "Puertas")
				//				{
				//					lista_win_sill_height1.Add(0);
				//				}
				//				else if (win1.Category.Name.ToString() == "Ventanas")
				//				{
				//					lista_win_sill_height1.Add(win_sill_height1);
				//				}
				//
				//				double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040
				//
				//				double dVIo = dPH1 - width1/2; //3090
				//				lista_dVIo.Add(dVIo);
				//				double dVFo = dPH1 + width1/2; //4310                  
				//				lista_dVFo.Add(dVFo);
				//				
				//				XYZ Point_dVIo = wallCurve.Evaluate(dVIo, false);
				//				XYZ Point_dVFo = wallCurve.Evaluate(dVFo, false);
				//				
				//				XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, lista_win_sill_height1.First());
				//				XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, lista_win_sill_height1.First());
				//				
				//				Puntos_Ventana_dVIo.Add(Nuevo_Point_dVIo);
				//				Puntos_Ventana_dVFo.Add(Nuevo_Point_dVFo);


				//				string mensaje = width1.ToString() + Environment.NewLine 
				//								+ heigth1.ToString() + Environment.NewLine
				//								+ lista_win_sill_height1.First().ToString() + Environment.NewLine ;
				//				
				//				TaskDialog.Show("mensaje", mensaje);

				#endregion


				//			
				//			lista_delistas_conDatosVentanas.Add(lista_width1);
				//			lista_delistas_conDatosVentanas.Add(lista_heigth1);
				//			lista_delistas_conDatosVentanas.Add(lista_win_sill_height1);
				//			lista_delistas_conDatosVentanas.Add(lista_dPH);
				//			
				//			Puntos_Ventana.Add(Puntos_Ventana_dVIo);
				//			Puntos_Ventana.Add(Puntos_Ventana_dVFo);

				#endregion

				// Crear lista_d

				#region Lista_d = [d1, d2, d3, d4, ...]

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();
				List<double> lista_d_muro4 = new List<double>();
				//	        List<double> lista_d_muro4_2 = new List<double>();
				//	        List<double> lista_d_muro4_3 = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * i));
				}
				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d_muro4.Add(stParam + ((anchopanel / 304.8) * (i + 1) + (4 / 304.8) * (i + 1)));
				}


				#endregion

				// Crear lista_d_3 Separar Paneles Antes dVIo

				#region Separar Paneles Antes dVIo lista_d3

				//			List<double> lista_d_3 = new List<double>(); // lista de paneles menor o igual a dVIo
				//			
				//			
				//			
				//			for (int i = 0; i < lista_d.Count(); i++) 
				//			{
				////				double dVFo = lista_dPH.First() + (lista_width1.First()/2);
				////				double dVIo = lista_dPH.First() - (lista_width1.First()/2);
				//				
				//				if (lista_d[i]<dVFo && lista_d[i]<dVIo) 
				//				{
				//					lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
				//					lista_d_muro4_3.Add(lista_d_muro4[i]);
				//				}
				//				else if (lista_d[i]<dVFo && lista_d[i]>dVIo)
				//				{
				//					lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
				//					lista_d_muro4_3.Add(lista_d_muro4[i]);
				//				}
				//				else if (lista_d[i]>dVFo && lista_d[i]>dVIo)
				//				{
				//					lista_d_3.Add(lista_d[i]); // menor o igual <= 1220
				//					lista_d_muro4_3.Add(lista_d_muro4[i]);
				//					
				//					i = i + lista_d.Count();
				//				}
				//			}


				#endregion


				if (longitud_double < anchopanel / 304.8)
				{
					//TaskDialog.Show("Mensaje", "El Muro tiene una longitud actual menor al valor ingresado para el ancho de Panel");
					//	           	return null;
				}
				else if (longitud_double == anchopanel / 304.8)
				{
					//TaskDialog.Show("Mensaje", "El Muro tiene una longitud actual igual al valor ingresado para el ancho de Panel");
					//	           	return null;
				}
				////	        else if (windows_hosted.Count()==0)
				////	        {
				////	        	TaskDialog.Show("Dynoscript", "El Muro no contiene una Ventana");
				////	           	return null;
				////	        }
				else
				{

					Transaction trans = new Transaction(doc);

					trans.Start("mysplitwall");




					//				doc.Delete(win1.Id);



					double d1 = stParam + (anchopanel / 304.8);
					double d1_4 = d1 + 4 / 304.8;

					XYZ Point_d1 = wallCurve.Evaluate(d1, false);
					XYZ Point_d1_4 = wallCurve.Evaluate(d1_4, false);


					// Crear Wall Primer

					#region Crear Wall Primer
					Line newLine01 = Line.CreateBound(stPoint, Point_d1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

					((LocationCurve)wall_1.Location).Curve = newLine01;

					listaWalls_Final_siCambia.Add(wall_1);

					#endregion

					// Crear Walls si esta muy lejos de la esquina

					#region Crear Walls siguientes

					// Crear Walls Antes al dVIo
					for (int i = 0; i < lista_d.Count() - 1; i++) // antes dVIo
					{
						double di = lista_d_muro4[i] + (anchopanel / 304.8); // d2
						double di_4 = di + 4 / 304.8; // d2_4

						//TaskDialog.Show("dynoscript", lista_d_3.Count().ToString());
						XYZ PointA = wallCurve.Evaluate(lista_d_muro4[i], false);
						XYZ PointB = wallCurve.Evaluate(di, false);

						Line newLineN = Line.CreateBound(PointA, PointB);
						Wall wall_N = Wall.Create(doc, newLineN, wall_1.LevelId, false);

						listaWalls_Final_siCambia.Add(wall_N);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 1))
							WallUtils.DisallowWallJoinAtEnd(wall_N, 1);

						if (WallUtils.IsWallJoinAllowedAtEnd(wall_N, 0))
							WallUtils.DisallowWallJoinAtEnd(wall_N, 0);

						Parameter WALL_USER_HEIGHT_PARAM12 = wall_N.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
						if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
						{
							WALL_USER_HEIGHT_PARAM12.Set(height_double);
						}

					}

					#endregion




					// wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					#region wall final final agregarle las ventanas que falta en el ubicación dPH respectiva.

					double ultimo_d_4 = lista_d_muro4.Last();
					XYZ Point_ultimo_d_4 = wallCurve.Evaluate(ultimo_d_4, false);

					Line newLineF3_ultimo = Line.CreateBound(Point_ultimo_d_4, endPoint_4);

					Wall wall_F3_ultimo = Wall.Create(doc, newLineF3_ultimo, wall_1.LevelId, false);

					listaWalls_Final_siCambia.Add(wall_F3_ultimo);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F3_ultimo, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F3_ultimo, 0);

					Parameter WALL_USER_HEIGHT_PARAMF3_ultimo = wall_F3_ultimo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF3_ultimo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF3_ultimo.Set(height_double);
					}

					#endregion

					trans.Commit();


					//			    listadelistasWalls_Final.Add(listaWalls_Final_siCambia);
					//			    listadelistasWalls_Final.Add(listaWalls_Final_noCambia);


				}

				//			Tuple< List<List<Wall>>, List<List<XYZ>>, List<List<double>> > tupla = new Tuple< List<List<Wall>>, List<List<XYZ>>, List<List<double>> >(listadelistasWalls_Final, Puntos_Ventana, lista_delistas_conDatosVentanas);
				//	        return tupla;
			} // okkkkkkkkkkkkkkkkkkkkk!
			  // ESTE FUNCIONA ACTUALMENTE




			Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVIo_1,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS

				List<Wall> lista_wall_return = new List<Wall>();


				XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, stPoint.Z + sill_v + hv);
				XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo1.X, Nuevo_Point_dVIo1.Y, stPoint.Z + sill_v + hv);


				XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2);
				XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2 + alturaventana2);

				XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);
				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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

					//		        Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					//		        Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					//		        Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					//		        Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea
					//		        
					//		        curve.Append(linea5);
					//		       	curve.Append(linea6);
					//		        curve.Append(linea7);
					//		        curve.Append(linea8);
					//		        arrcurve.Append(curve);
					//		        


					//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);



					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);

					//		        CurveLoopsProfile prof = app.Create.NewCurveLoopsProfile( arrcurve );
					//		        
					//		        CurveLoop curloop = new CurveLoop();
					//		        
					//		        foreach (Curve c in arrcurve) 
					//		        {
					//		        	
					//		        	profile.Add(c);
					//		        	
					//		        	
					//		        }

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE


			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVIo_1,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS

				List<Wall> lista_wall_return = new List<Wall>();


				XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, stPoint.Z + sill_v + hv);
				XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo1.X, Nuevo_Point_dVIo1.Y, stPoint.Z + sill_v + hv);


				XYZ esquina1_abajo = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2);
				XYZ esquina1_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sillventanda2 + alturaventana2);

				XYZ Point_dVI2_abajo = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2);
				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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

					//		        Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					//		        Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					//		        Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					//		        Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea
					//		        
					//		        curve.Append(linea5);
					//		       	curve.Append(linea6);
					//		        curve.Append(linea7);
					//		        curve.Append(linea8);
					//		        arrcurve.Append(curve);
					//		        


					//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);



					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);

					//		        CurveLoopsProfile prof = app.Create.NewCurveLoopsProfile( arrcurve );
					//		        
					//		        CurveLoop curloop = new CurveLoop();
					//		        
					//		        foreach (Curve c in arrcurve) 
					//		        {
					//		        	
					//		        	profile.Add(c);
					//		        	
					//		        	
					//		        }

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();

			}


			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2,
																	  XYZ _Point_dVFo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS




				//	        XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				//	        XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
				XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

				XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
				XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

				XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
				XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				//return lista_wall_return.First();

			}
			Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2,
																	  XYZ _Point_dVFo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS

				List<Wall> lista_wall_return = new List<Wall>();


				//	        XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				//	        XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
				XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

				XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
				XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

				XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
				XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				return lista_wall_return.First();

			}



			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2,
																	  XYZ _Point_dVFo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS




				//	        XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				//	        XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
				XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

				XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
				XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

				XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
				XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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




					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				//return lista_wall_return.First();

			}

			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(Wall _wall_,
																	  double _alturaventana_1,
																	  double _sillventanda_1,
																	  double _alturaventana_2,
																	  double _sillventanda_2,
																	  XYZ _Point_dVFo_1,
																	  XYZ _Point_dVIo_2,
																	  XYZ _Point_dVFo_2)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

				// INPUTS
				Wall wall_I = _wall_;

				Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

				WallType wallType = wall_I.WallType as WallType;

				List<Wall> lista_wall_return = new List<Wall>();

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

				//			XYZ Point_dPH = _Point_dPH_1;
				//			XYZ Nueva_Point_dPH = new XYZ(Point_dPH.X, Point_dPH.Y, stPoint.Z);


				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;
				// INPUTS




				//	        XYZ Nuevo_Point_dVFo1 = new XYZ(Point_dVFo1.X, Point_dVFo1.Y, stPoint.Z);
				//	        XYZ Nuevo_Point_dVIo1 = new XYZ(Point_dVIo1.X, Point_dVIo1.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVI2_arriba = new XYZ(Nuevo_Point_dVIo2.X, Nuevo_Point_dVIo2.Y, Nuevo_Point_dVIo2.Z + sillventanda2 + alturaventana2);
				XYZ Point_dVF2_arriba = new XYZ(Nuevo_Point_dVFo2.X, Nuevo_Point_dVFo2.Y, Nuevo_Point_dVFo2.Z + sillventanda2 + alturaventana2);

				XYZ esquina2_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1 + alturaventana1);
				XYZ esquina2_abajo = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + sillventanda1);

				XYZ Point_dVF1_arriba = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1 + alturaventana1);
				XYZ Point_dVF1_abajo = new XYZ(Nuevo_Point_dVFo1.X, Nuevo_Point_dVFo1.Y, Nuevo_Point_dVFo1.Z + sillventanda1);

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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




					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				return lista_wall_return.First();

			}




			// 3 ventanas en 1 muro
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}

			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				List<Wall> lista_wall_return = new List<Wall>();

				double alturaventana1 = _alturaventana_1;
				double sillventanda1 = _sillventanda_1;

				double alturaventana2 = _alturaventana_2;
				double sillventanda2 = _sillventanda_2;

				double alturaventana3 = _alturaventana_3;
				double sillventanda3 = _sillventanda_3;
				// INPUTS


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				return lista_wall_return.First();

			}








			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}

			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				List<Wall> lista_wall_return = new List<Wall>();

				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				return lista_wall_return.First();
			}








			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}


			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				List<Wall> lista_wall_return = new List<Wall>();

				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				//return lista_wall_return.First();			   
				return lista_wall_return.First();

			}



			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}

			Wall Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(Wall _wall_,
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
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				List<Wall> lista_wall_return = new List<Wall>();

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


				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana1;
				double sill_v = sillventanda1;
				//			
				//
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


				using (Transaction trans = new Transaction(doc, "wall"))
				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);


					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


				return lista_wall_return.First();
			}








			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(Wall _wall_, double _alturaventana_1,
																				 double _anchoventana_1,
																				double _sillventanda_1,
																				 XYZ _Point_dVIo_1,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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

				List<Wall> lista_wall_return = new List<Wall>();



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




				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//List<Wall> lista_wall_return = new List<Wall>();
				//lista_wall_return.Add(wall);    
				return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA(Wall _wall_, double _alturaventana_1,
																				 double _anchoventana_1,
																				double _sillventanda_1,
																				 XYZ _Point_dVIo_1,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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




				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//		   	List<Wall> lista_wall_return = new List<Wall>();
				//		   	lista_wall_return.Add(wall);
				//			return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(Wall _wall_, double _alturaventana_1,
																				 double _anchoventana_1,
																				double _sillventanda_1,
																				 XYZ _Point_dVIo_1,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				List<Wall> lista_wall_return = new List<Wall>();


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




				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();
			}

			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO(Wall _wall_, double _alturaventana_1,
																				 double _anchoventana_1,
																				double _sillventanda_1,
																				 XYZ _Point_dVIo_1,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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




				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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



					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//		   	List<Wall> lista_wall_return = new List<Wall>();
				//		   	lista_wall_return.Add(wall);
				//			return lista_wall_return.First();
			}










			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Wall_EditProfile_U_PUERTA(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_, XYZ _Point_dVFo_, XYZ _Point_dPH_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana;
				double sill_v = sillventanda;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo.X, Nuevo_Point_dVFo.Y, stPoint.Z + sill_v + hv);
				XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo.X, Nuevo_Point_dVIo.Y, stPoint.Z + sill_v + hv);


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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

					//		        Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					//		        Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					//		        Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					//		        Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea
					//		        
					//		        curve.Append(linea5);
					//		       	curve.Append(linea6);
					//		        curve.Append(linea7);
					//		        curve.Append(linea8);
					//		        arrcurve.Append(curve);
					//		        


					//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);



					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);

					//		        CurveLoopsProfile prof = app.Create.NewCurveLoopsProfile( arrcurve );
					//		        
					//		        CurveLoop curloop = new CurveLoop();
					//		        
					//		        foreach (Curve c in arrcurve) 
					//		        {
					//		        	
					//		        	profile.Add(c);
					//		        	
					//		        	
					//		        }

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_, XYZ _Point_dVFo_, XYZ _Point_dPH_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				List<Wall> lista_wall_return = new List<Wall>();


				double distacia_centerToLeft = Nueva_Point_dPH.DistanceTo(stPoint);
				double distancia_centerToBottom = stPoint.Z + sillventanda + alturaventana / 2;


				XYZ Nuevo_Point_dVFo = new XYZ(Point_dVFo.X, Point_dVFo.Y, stPoint.Z);
				XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, stPoint.Z);


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana;
				double sill_v = sillventanda;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo.X, Nuevo_Point_dVFo.Y, stPoint.Z + sill_v + hv);
				XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo.X, Nuevo_Point_dVIo.Y, stPoint.Z + sill_v + hv);


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        CurveArrArray arrcurve = new CurveArrArray();
					//		        CurveArray curve = new CurveArray();

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

					//		        Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					//		        Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					//		        Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					//		        Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea
					//		        
					//		        curve.Append(linea5);
					//		       	curve.Append(linea6);
					//		        curve.Append(linea7);
					//		        curve.Append(linea8);
					//		        arrcurve.Append(curve);
					//		        


					//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);



					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);

					//		        CurveLoopsProfile prof = app.Create.NewCurveLoopsProfile( arrcurve );
					//		        
					//		        CurveLoop curloop = new CurveLoop();
					//		        
					//		        foreach (Curve c in arrcurve) 
					//		        {
					//		        	
					//		        	profile.Add(c);
					//		        	
					//		        	
					//		        }

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);



					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE

			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVFo_1,
																				XYZ _Point_dVIo_2,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			} // forma T
			  // ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVFo_1,
																				XYZ _Point_dVIo_2,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				List<Wall> lista_wall_return = new List<Wall>();



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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				//		   	List<Wall> lista_wall_return = new List<Wall>();
				//		   	lista_wall_return.Add(wall);
				return lista_wall_return.First();
			} // forma T



			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			} // forma I puerta al lado derecho


			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				List<Wall> lista_wall_return = new List<Wall>();



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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);



					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();

			} // forma I puerta al lado derecho


			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			} // forma I puerta al lado izquierdo

			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				List<Wall> lista_wall_return = new List<Wall>();



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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			} // forma I puerta al lado izquierdo		




			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_I(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			} // forma I
			  // ESTE FUNCIONA ACTUALMENTE
			  // ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_return(Wall _wall_, double _alturaventana_1,
																				 double _alturaventana_2,
																				 double _sillventanda_1,
																				 double _sillventanda_2,
																				 XYZ _Point_dVIo_2,
																				 XYZ _Point_dVFo_1,
																				 double height_double)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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

				List<Wall> lista_wall_return = new List<Wall>();




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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);



					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					//lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			} // forma I








			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

				List<Wall> ultimos_Walls_para_Agregar_40 = new List<Wall>();

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

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();
			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					lista_wall_return.Add(wall);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();
			}
			// ESTE FUNCIONA ACTUALMENTE

			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
			{
				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;
				#endregion


				#region INPUTS

				Wall wall_I = _wall_;

				XYZ Point_dVIo = _Point_dVIo_;

				double alturaventana = _alturaventana_;
				double sillventanda = _sillventanda_;
				#endregion


				#region infoWAll

				Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

				WallType wallType = wall_I.WallType as WallType;

				Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
				double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel
				#endregion





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

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//Revision5_InsertOpening_void(wall_I, alturaventana, dis, (dm + dis/2), alturaventana/2);


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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
			{
				#region inicio

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;
				#endregion

				List<Wall> lista_wall_return = new List<Wall>();
				#region INPUTS

				Wall wall_I = _wall_;

				XYZ Point_dVIo = _Point_dVIo_;

				double alturaventana = _alturaventana_;
				double sillventanda = _sillventanda_;
				#endregion


				#region infoWAll

				Curve wallCurve = ((LocationCurve)wall_I.Location).Curve;

				WallType wallType = wall_I.WallType as WallType;

				Parameter height = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // altura primer Wall o  Wall_I
				double height_double = height.AsDouble(); // altura primer Wall o  Wall_I = 2440

				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double distanta_total_wall = endParam - stParam; // deberia ser 1220 == anchopanel
				#endregion





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

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//Revision5_InsertOpening_void(wall_I, alturaventana, dis, (dm + dis/2), alturaventana/2);


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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			}
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				using (Transaction trans = new Transaction(doc, "wall"))

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				List<Wall> lista_wall_return = new List<Wall>();

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

				using (Transaction trans = new Transaction(doc, "wall"))

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(Wall _wall_, double _alturaventana_, double sillventanda_0,
																										   double _sillventanda_, XYZ _Point_dVFo_,
																										  XYZ _Point_dVIo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				using (Transaction trans = new Transaction(doc, "wall"))

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


					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					lista_wall_return.Add(wall);




					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);


					Line linea1_casoEspecial = Line.CreateBound(stPoint, Nuevo_Point_dVIo);

					Wall wall_casoEspecial = Wall.Create(doc, linea1_casoEspecial, wall_I.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_abajo.Set(endPoint.Z + sill_v_0);
					}

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			}

			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVIo(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

				List<Wall> ultimos_Walls_para_Agregar_40 = new List<Wall>();

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

				XYZ Nuevo_Point_dVIo = new XYZ(Point_dVIo.X, Point_dVIo.Y, endPoint.Z);

				double dis = Nuevo_Point_dVIo.DistanceTo(endPoint);

				double dm = endParam - dis; // 3800 - 610 = 3190

				double hv = alturaventana;
				double sill_v = sillventanda;



				XYZ Point_DVIo = wallCurve.Evaluate(dm, false);
				XYZ Point_bajo_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, endPoint.Z + sill_v);
				XYZ Point_arriba_DVIo = new XYZ(Point_DVIo.X, Point_DVIo.Y, endPoint.Z + sill_v + hv);

				XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sill_v);
				XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + sill_v + hv);

				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, endPoint.Z + height_double);// Point 1 arriba 
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double); // Point 2 arriba

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVFo(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(Wall _wall_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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

				using (Transaction trans = new Transaction(doc, "wall"))

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

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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



				//				
				//	        XYZ X_4_p = wallCurve.Evaluate(endParam,false); // Point 1
				//			
				//			XYZ Point_DVFo = wallCurve.Evaluate(dm,false);
				//			
				//			XYZ Point_bajo_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, sill_v);
				//			XYZ Point_arriba_DVFo = new XYZ(Point_DVFo.X, Point_DVFo.Y, sill_v + hv);
				//			
				//			XYZ Point_bajo_Esquina1 = new XYZ(endPoint.X, endPoint.Y, sill_v);
				//			XYZ Point_arriba_Esquina1 = new XYZ(endPoint.X, endPoint.Y, sill_v + hv);
				//			
				//			XYZ Point_bajo_Esquina2 = new XYZ(X_4_p.X, X_4_p.Y, sill_v);
				//			XYZ Point_arriba_Esquina2 = new XYZ(X_4_p.X, X_4_p.Y, sill_v + hv);
				//			
				//			XYZ Point_bajo_Esquina3 = new XYZ(stPoint.X, stPoint.Y, sill_v);
				//			XYZ Point_arriba_Esquina3 = new XYZ(stPoint.X, stPoint.Y, sill_v + hv);
				//				
				//			XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, height_double);// Point 1 arriba
				//			XYZ stPoint_arriba_ventana = new XYZ(stPoint.X, stPoint.Y, sill_v + hv);// Point 1 arriba 
				//			
				//			XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, height_double); // Point 2 arriba
				//			XYZ X_endPoint_arriba = new XYZ(X_4_p.X, X_4_p.Y, height_double); // Point 2 arriba
				//			XYZ X_endPoint_abajo = new XYZ(X_4_p.X, X_4_p.Y, 0); // Point 2 arriba

				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

					((LocationCurve)wall_I.Location).Curve = linea1;

					Parameter WALL_USER_HEIGHT_PARAMF1 = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1.Set(endPoint.Z + sill_v);
					}



					Wall wall_F1_arriba = Wall.Create(doc, linea1, wall_I.LevelId, false);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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




					//		        Line linea1 = Line.CreateBound(stPoint, X_endPoint_abajo); // 1 Linea
					//		        Line linea2 = Line.CreateBound(X_endPoint_abajo, Point_bajo_Esquina2); // 2 Linea 
					//		        Line linea3 = Line.CreateBound(Point_bajo_Esquina2, Point_bajo_Esquina3); // 3 Linea
					//		        Line linea4 = Line.CreateBound(Point_bajo_Esquina3, stPoint); // 4 Linea
					//		        
					//		        Line linea5 = Line.CreateBound(stPoint_arriba_ventana, Point_arriba_Esquina2); // 5 Linea
					//		        Line linea6 = Line.CreateBound(Point_arriba_Esquina2, X_endPoint_arriba); // 6 Linea
					//		        Line linea7 = Line.CreateBound(X_endPoint_arriba, stPoint_arriba); // 7 Linea
					//		        Line linea8 = Line.CreateBound(stPoint_arriba, stPoint_arriba_ventana); // 8 Linea
					//		        
					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);
					//
					//		        Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
					//		       	
					//		        if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
					//		              WallUtils.DisallowWallJoinAtEnd(wall, 1);
					//		        
					//		        if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
					//		                	WallUtils.DisallowWallJoinAtEnd(wall, 0);
					//		        
					//		        doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE


			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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




				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);
					Wall wall_F1_abajo = Wall.Create(doc, linea1, wall_I.LevelId, false);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_abajo, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_abajo, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_abajo, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_abajo, 0);

					//				((LocationCurve)wall_I.Location).Curve = linea1;

					Parameter WALL_USER_HEIGHT_PARAMF1 = wall_F1_abajo.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1.Set(endPoint.Z + sillventanda);
					}



					Wall wall_F1_arriba = Wall.Create(doc, linea1, wall_I.LevelId, false);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_F1_arriba, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_F1_arriba, 0);

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

					doc.Delete(wall_I.Id);

					trans.Commit();

				}
				return lista_wall_return.First();
			}
			// ESTE FUNCIONA ACTUALMENTE



			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_1MURO_Solitario(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

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

					//	            doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE

			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_,
																							double sillventanda_2, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;


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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);

					Line linea1_casoEspecial = Line.CreateBound(Nuevo_Point_dVFo, endPoint);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

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



					Wall wall_casoEspecial = Wall.Create(doc, linea1_casoEspecial, wall_I.LevelId, false);

					Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_abajo.Set(endPoint.Z + sill_v_2);
					}


					//	            doc.Delete(wall_I.Id);

					trans.Commit();

				}

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_,
																							double sillventanda_2, XYZ _Point_dVFo_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;
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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);

					Line linea1_casoEspecial = Line.CreateBound(Nuevo_Point_dVFo, endPoint);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

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




					Wall wall_casoEspecial = Wall.Create(doc, linea1_casoEspecial, wall_I.LevelId, false);

					lista_wall_return.Add(wall_casoEspecial);

					Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_I.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_abajo.Set(endPoint.Z + sill_v_2);
					}


					//	            doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(Wall _wall_, double _anchoventana_, double _alturaventana_, double alturaventana_2,
																															 double _sillventanda_, double sillventanda_2, XYZ _Point_dVI1_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;
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


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					//		        IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint);

					Line linea1_casoEspecial = Line.CreateBound(stPoint, Nuevo_Point_dVI1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

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




					Wall wall_casoEspecial = Wall.Create(doc, linea1_casoEspecial, wall_I.LevelId, false);

					lista_wall_return.Add(wall_casoEspecial);

					Parameter WALL_USER_HEIGHT_PARAMF1_abajo = wall_casoEspecial.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
					if (!WALL_USER_HEIGHT_PARAMF1_abajo.IsReadOnly)
					{
						WALL_USER_HEIGHT_PARAMF1_abajo.Set(stPoint.Z + sill_v);
					}


					//	            doc.Delete(wall_I.Id);

					trans.Commit();

				}

				return lista_wall_return.First();

			}
			// ESTE FUNCIONA ACTUALMENTE


			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_dPH_dVFo(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_, XYZ _Point_dVFo_, XYZ _Point_dPH_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana;
				double sill_v = sillventanda;
				//			
				//
				//	        XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, height_double);
				//	        XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, height_double);
				//	        
				//	        XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo.X, Nuevo_Point_dVFo.Y, sill_v + hv);
				//			XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo.X, Nuevo_Point_dVIo.Y, sill_v + hv);


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					//		        Line linea1 = Line.CreateBound(stPoint, endPoint); // 1 Linea
					//		        Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 1 Linea
					//				Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
					//		        Line linea4 = Line.CreateBound(endPoint_arriba, stPoint); // 1 Linea
					//		        
					//		        Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					//		        Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					//		        Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					//		        Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea


					Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);


					//		        profile.Add(linea1);
					//		        profile.Add(linea2);
					//		        profile.Add(linea3);
					//		        profile.Add(linea4);
					//		        profile.Add(linea5);
					//		        profile.Add(linea6);
					//		        profile.Add(linea7);
					//		        profile.Add(linea8);
					//
					//		        Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_I, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_I, 0);

					//doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_dPH_dVFo_edit(Wall _wall_, double _anchoventana_, double _alturaventana_, double _sillventanda_, XYZ _Point_dVIo_, XYZ _Point_dVFo_, XYZ _Point_dPH_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//Application app = this.Application;

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


				//	        
				//	        double dis = Nuevo_Point_dVFo.DistanceTo(endPoint);
				//	           
				//	        double dm =  endParam - dis; // 3800 - 610 = 3190
				//	        
				double hv = alturaventana;
				double sill_v = sillventanda;
				//			
				//
				XYZ stPoint_arriba = new XYZ(stPoint.X, stPoint.Y, stPoint.Z + height_double);
				XYZ endPoint_arriba = new XYZ(endPoint.X, endPoint.Y, endPoint.Z + height_double);

				XYZ Point_dVFo_arriba = new XYZ(Nuevo_Point_dVFo.X, Nuevo_Point_dVFo.Y, stPoint.Z + sill_v + hv);
				XYZ Point_dVIo_arriba = new XYZ(Nuevo_Point_dVIo.X, Nuevo_Point_dVIo.Y, stPoint.Z + sill_v + hv);


				using (Transaction trans = new Transaction(doc, "wall"))

				{

					trans.Start();

					IList<Curve> profile = new List<Curve>();

					Line linea1 = Line.CreateBound(stPoint, endPoint); // 1 Linea
					Line linea2 = Line.CreateBound(endPoint, endPoint_arriba); // 1 Linea
					Line linea3 = Line.CreateBound(endPoint_arriba, stPoint_arriba); // 1 Linea
					Line linea4 = Line.CreateBound(endPoint_arriba, stPoint); // 1 Linea

					Line linea5 = Line.CreateBound(Point_dVIo, Point_dVFo); // 1 Linea
					Line linea6 = Line.CreateBound(Point_dVFo, Point_dVFo_arriba); // 1 Linea
					Line linea7 = Line.CreateBound(Point_dVFo_arriba, Point_dVIo_arriba); // 1 Linea
					Line linea8 = Line.CreateBound(Point_dVIo_arriba, Point_dVIo); // 1 Linea


					//Revision6_InsertOpening_void(wall_I, alturaventana, anchoventana, distacia_centerToLeft, distancia_centerToBottom);


					profile.Add(linea1);
					profile.Add(linea2);
					profile.Add(linea3);
					profile.Add(linea4);
					profile.Add(linea5);
					profile.Add(linea6);
					profile.Add(linea7);
					profile.Add(linea8);

					Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
						WallUtils.DisallowWallJoinAtEnd(wall, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
						WallUtils.DisallowWallJoinAtEnd(wall, 0);

					doc.Delete(wall_I.Id);

					trans.Commit();

				}


			}
			// ESTE FUNCIONA ACTUALMENTE

			Opening Revision6_InsertOpening(Wall wall, double height, double width, double centerToLeft = -1.0, double centerToBottom = -1.0)
			{
				LocationCurve locCurve = wall.Location as LocationCurve;

				XYZ start = locCurve.Curve.GetEndPoint(0);
				XYZ end = locCurve.Curve.GetEndPoint(1);

				XYZ location = (start + end) / 2;

				if (centerToLeft >= 0)
					location = start + (end - start).Normalize() * centerToLeft;

				double wallHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
				double topOffset = wallHeight / 2 + height / 2;
				double baseOffset = wallHeight / 2 - height / 2;

				if (centerToBottom >= 0)
				{
					topOffset = centerToBottom + height / 2;
					baseOffset = centerToBottom - height / 2;
				}

				XYZ leftBottom = new XYZ(location.X - width / 2, location.Y, baseOffset);
				XYZ topRight = new XYZ(location.X + width / 2, location.Y, topOffset);

				Opening opening = wall.Document.Create.NewOpening(wall, leftBottom, topRight);

				return opening;
			}

			void Revision6_InsertOpening_void(Wall wall, double height, double width, double centerToLeft = -1.0, double centerToBottom = -1.0)
			{
				LocationCurve locCurve = wall.Location as LocationCurve;

				XYZ start = locCurve.Curve.GetEndPoint(0);
				XYZ end = locCurve.Curve.GetEndPoint(1);

				XYZ location = (start + end) / 2;

				if (centerToLeft >= 0)
					location = start + (end - start).Normalize() * centerToLeft;

				double wallHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
				double topOffset = wallHeight / 2 + height / 2;
				double baseOffset = wallHeight / 2 - height / 2;

				if (centerToBottom >= 0)
				{
					topOffset = centerToBottom + height / 2;
					baseOffset = centerToBottom - height / 2;
				}

				XYZ leftBottom = new XYZ(location.X - width / 2, location.Y, baseOffset);
				XYZ topRight = new XYZ(location.X + width / 2, location.Y, topOffset);

				Opening opening = wall.Document.Create.NewOpening(wall, leftBottom, topRight);

				//		    return opening;
			}





			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_DarVuelta_Muro_ConVentanas(Element _e_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				List<Wall> listaWalls_Final = new List<Wall>();

				//			Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige
				Wall wall_1 = _e_ as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double primero = stParam;
				double endParam = wallCurve.GetEndParameter(1);



				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				#region VENTANAS Y PUERTAS


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{

				   BuiltInCategory.OST_Doors,
				   BuiltInCategory.OST_Windows,

					};

				List<Element> windows_hosted = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> familyinstance = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in familyinstance)
					{
						FamilyInstance fintance = elem as FamilyInstance;
						if (fintance.Host.Id == wall_1.Id)
						{
							windows_hosted.Add(elem);
						}
					}
				}



				#endregion


				//	        // Recolectar Ventanas
				//	        ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 	ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
				//		 	LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//		 	FilteredElementCollector coll = new FilteredElementCollector(doc, activeView.Id);
				//
				//		 	IList<Element> windows = coll.WherePasses(MEInstancesFilter).ToElements(); // todas las ventadas
				//			
				//			List<Element> windows_hosted = new List<Element>();
				//			
				//			foreach (Element elem in windows)
				//            {
				//				FamilyInstance fintance = elem as FamilyInstance;
				//
				//				if (fintance.Host.Id == _e_.Id) 
				//				{
				//					windows_hosted.Add(elem);
				//				}
				//
				//            }

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
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

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

					// Create window
					// unliss you specified a host, Rebit will create the family instance as orphabt object.
					FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall_1, StructuralType.NonStructural);

				}



				trans.Commit();



				return wall_1;

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_Girar180_Muro_ConVentanas(Element _e_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<Wall> listaWalls_Final = new List<Wall>();

				//			Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige
				Wall wall_1 = _e_ as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;

				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter longi = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longi_double = longi.AsDouble(); // 1220

				double mid = stParam + longi_double / 2;



				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				// Get the mid point of curve
				XYZ midPoint = wallCurve.Evaluate(mid, false);
				XYZ midHigh = midPoint.Add(XYZ.BasisZ);

				Line axisLine = Line.CreateBound(midPoint, endPoint);

				//Rotate elements on their axis

				// Convert Radian to Degree
				double angle = 180;
				double RotateAngle = angle * (Math.PI / 180);



				bool rotated = ((LocationCurve)wall_1.Location).Rotate(axisLine, RotateAngle);

				rotated = true;

				TaskDialog.Show("Dynoscript", angle.ToString() + Environment.NewLine + RotateAngle.ToString() + Environment.NewLine + (Math.PI / 180).ToString()
								+ Environment.NewLine + rotated.ToString());
				#region INFO VENTANAS


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

					if (fintance.Host.Id == _e_.Id)
					{
						windows_hosted.Add(elem);
					}

				}

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
					Parameter widthParam1 = type1.LookupParameter("Width"); // ancho ventana 1220
					Parameter heightParam1 = type1.LookupParameter("Height"); // altura ventana 1240


					double width1 = widthParam1.AsDouble(); // ancho ventana 1220
					lista_width1.Add(width1);
					double heigth1 = heightParam1.AsDouble(); // altura ventana 1240
					lista_heigth1.Add(heigth1);
					double win_sill_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble(); // 800
					lista_win_sill_height1.Add(win_sill_height1);


					double win_head_height1 = win1.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM).AsDouble(); // 2040

				}

				#endregion

				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");

				// CORREGIR WALL 1 EXISTENTE

				for (int i = 0; i < windows_hosted_sorted.Count(); i++)
				{
					doc.Delete(windows_hosted_sorted[i].Id);
				}

				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				lista_FamilySymbol.Reverse();
				lista_dPH.Reverse();
				lista_a.Reverse();
				lista_width1.Reverse();
				lista_heigth1.Reverse();
				lista_win_sill_height1.Reverse();

				// agregar ventanas en el restante
				for (int i = 0; i < lista_dPH.Count(); i++)
				{

					double dPH1_nuevo = lista_dPH[i];

					FamilySymbol familySymbol = lista_FamilySymbol[i];



					XYZ xyz_dPH1 = wallCurve.Evaluate(dPH1_nuevo, false);

					XYZ xyz = new XYZ(xyz_dPH1.X, xyz_dPH1.Y, lista_win_sill_height1[i]);

					// Create window.

					if (!familySymbol.IsActive)
					{
						// Ensure the family symbol is activated.
						familySymbol.Activate();
						doc.Regenerate();
					}

					// Create window
					// unliss you specified a host, Rebit will create the family instance as orphabt object.
					FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall_1, StructuralType.NonStructural);

				}



				trans.Commit();



				return wall_1;

			}
			// ESTE FUNCIONA ACTUALMENTE


			// SIN VANOS
			// ESTE FUNCIONA ACTUALMENTE
			void Revision6_DYNO_DividirMuroSinVentana(Element e, int _anchopanel_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<Wall> listaWalls_Final = new List<Wall>();

				int anchopanel = _anchopanel_; //1220

				List<Wall> lista_wallfinal = new List<Wall>();

				//	        Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)); // Wall que se elige

				Wall wall_1 = e as Wall; // muro actual

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				double Param1 = stParam + anchopanel / 304.8;

				double distanta_total_wall = endParam - stParam;

				double alpha = distanta_total_wall % (anchopanel / 304.8); // 3.38 * 304.8 =

				double numero_paneles_total = (distanta_total_wall - alpha) / (anchopanel / 304.8); // 7
				int numero_paneles_total_int = (int)numero_paneles_total; // 7

				List<double> lista_d = new List<double>();

				for (int i = 0; i < numero_paneles_total_int; i++)
				{
					lista_d.Add(stParam + (i + 1) * (anchopanel / 304.8));
				}


				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				double height_double = height.AsDouble();

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);

				XYZ Point1 = wallCurve.Evaluate(Param1, false);


				Parameter longitud = wall_1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				double longitud_double = longitud.AsDouble();

				// Recolectar Ventanas

				#region Recolectar Ventanas

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

				#endregion

				if (longitud_double < anchopanel / 304.8)
				{
					//TaskDialog.Show("Dynoscript", "El Muro tiene una longitud actual menor al valor ingresado para la longitud");
					return;
				}
				else if (longitud_double == anchopanel / 304.8)
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
					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

					if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
						WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

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
			// ESTE FUNCIONA ACTUALMENTE
			Wall Revision6_DYNO_DarVuelta_Muro_SinVentanas(Element _e_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

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
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

				((LocationCurve)wall_1.Location).Curve = newLine01;

				trans.Commit();

				return wall_1;

			}
			// ESTE FUNCIONA ACTUALMENTE
			// ESTE FUNCIONA ACTUALMENTE
			// Separar con minipanel de 4 mm
			void Revision6_DYNO_splitWall_agregar_separacion40(Wall _wall_)
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Wall wall_1 = _wall_ as Wall; //1220

				List<Wall> lista_wallfinal = new List<Wall>();

				Curve wallCurve = ((LocationCurve)wall_1.Location).Curve;
				double stParam = wallCurve.GetEndParameter(0);
				double endParam = wallCurve.GetEndParameter(1);

				Parameter height = wall_1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM); // 2440

				double height_double = height.AsDouble(); // 2440

				XYZ stPoint = wallCurve.Evaluate(stParam, false);
				XYZ endPoint = wallCurve.Evaluate(endParam, false);


				double p40 = endParam - 4 / 304.8;
				XYZ p40Point = wallCurve.Evaluate(p40, false);

				Transaction trans = new Transaction(doc);

				trans.Start("mysplitwall");

				// CORREGIR WALL 1 EXISTENTE

				Line newLine01 = Line.CreateBound(stPoint, p40Point); // stPoint, (endPoint - 40)
				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 1))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 1);

				if (WallUtils.IsWallJoinAllowedAtEnd(wall_1, 0))
					WallUtils.DisallowWallJoinAtEnd(wall_1, 0);

				((LocationCurve)wall_1.Location).Curve = newLine01;

				// CREATE WALLS de 40 mm

				//						Line newLine40 = Line.CreateBound(p40Point, endPoint); // (endPoint - 40), endPoint
				//						Wall wall_40 = Wall.Create(doc, newLine40, wall_1.LevelId,false);
				//						            	
				//						Parameter WALL_USER_HEIGHT_PARAM12 = wall_40.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
				//						if (!WALL_USER_HEIGHT_PARAM12.IsReadOnly)
				//						{
				//						   WALL_USER_HEIGHT_PARAM12.Set(height_double);
				//						}

				trans.Commit();

			} // Wall seleccionado
			  // ESTE FUNCIONA ACTUALMENTE




			#endregion

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
