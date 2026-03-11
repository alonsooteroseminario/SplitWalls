namespace SplitWalls
{
    /// <summary>
    /// Data captured from the panel-options dialog (Form1).
    /// </summary>
    public class PanelOptions
    {
        public string AnchoPanel                { get; set; }
        public bool   MuroSinVentanas           { get; set; }
        public bool   MuroOsbConVentanas        { get; set; }
        public bool   MuroSmartPanelConVentanas { get; set; }
        public bool   TodoMuro                  { get; set; }
        public bool   Esquina1                  { get; set; }
        public bool   Esquina2OtroLado          { get; set; }
    }
}
