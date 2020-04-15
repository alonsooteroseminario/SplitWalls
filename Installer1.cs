using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Ookii.Dialogs.Wpf;

namespace SplitWalls
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }
        string myAddinDLL = "SplitWalls";

        public override void Uninstall(System.Collections.IDictionary stateSaver)
        {
            string sDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "%programdata%\\Autodesk\\Revit\\Addins\\2019";
            bool exists = System.IO.Directory.Exists(sDir);

            //2 August 2019: Start, The next 3 lines were added in Take 10 in order prevent double loading of packages.
            //Microsoft.Win32.RegistryKey rkbase = null;
            //rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            //rkbase.DeleteSubKeyTree("SOFTWARE\\Wow6432Node\\Default Company Name\\SplitWalls Packages");
            //2 August 2019: End.

            if (exists)
            {
                try
                {
                    foreach (string d in Directory.GetDirectories(sDir))
                    {
                        //DirSearch.Add(d);
                        File.Delete(d + "\\" + myAddinDLL + ".addin");
                    }
                }
                catch (System.Exception excpt)
                {
                    System.Windows.Forms.MessageBox.Show(excpt.Message);
                }
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {

            ////2 August 2019: The next 4 lines were added in Take 10 in order prevent double loading of packages.
            //Microsoft.Win32.RegistryKey rkbase = null;
            //rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            //rkbase.CreateSubKey("SOFTWARE\\Wow6432Node\\Default Company Name\\SplitWalls Packages", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("XceedVersion", typeof(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid).Assembly.FullName);
            //rkbase.CreateSubKey("SOFTWARE\\Wow6432Node\\Default Company Name\\SplitWalls Packages", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("OokiiVersion", typeof(Ookii.Dialogs.Wpf.CredentialDialog).Assembly.FullName);
            ////2 August 2019: End.


            string sDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "%programdata%\\Autodesk\\Revit\\Addins\\2019";
            bool exists = System.IO.Directory.Exists(sDir);

            if (!exists) System.IO.Directory.CreateDirectory(sDir);

            XElement XElementAddIn = new XElement("AddIn", new XAttribute("Type", "Application"));

            XElementAddIn.Add(new XElement("Name", "LAOS Split Walls"));
            XElementAddIn.Add(new XElement("Assembly", this.Context.Parameters["targetdir"].Trim() + myAddinDLL + ".dll"));  // /TargetDir=value1 // this.Context.Parameters["targetdir"].Trim() +
            XElementAddIn.Add(new XElement("AddInId", "FCCCC12C - 861E-48F7 - BB56 - 9B2110C0CB3A")); //DatabaseMethods.writeDebug(Guid.NewGuid().ToString());
            XElementAddIn.Add(new XElement("FullClassName", myAddinDLL + ".App"));
            XElementAddIn.Add(new XElement("VendorId", "LAOS"));
            XElementAddIn.Add(new XElement("VendorDescription", "Luis Alonso Otero Seminario, www.dynoscript.com"));

            XElement XElementRevitAddIns = new XElement("RevitAddIns");
            XElementRevitAddIns.Add(XElementAddIn);

            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    //DirSearch.Add(d);
                    new XDocument(XElementRevitAddIns).Save(d + "\\" + myAddinDLL + ".addin");
                    //files.AddRange(DirSearch.Add(d));
                }
            }
            catch (System.Exception excpt)
            {
                System.Windows.Forms.MessageBox.Show(excpt.Message);
            }
        }
    }


}
