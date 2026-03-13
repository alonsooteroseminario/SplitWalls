using System.Collections.ObjectModel;
using System.Windows;
using SplitWalls.Models;

namespace SplitWalls.UI
{
    public partial class ProfileEditorWindow : Window
    {
        private readonly ProfileEditorViewModel _vm;

        public ProfileEditorWindow()
        {
            InitializeComponent();
            _vm = new ProfileEditorViewModel();
            _vm.RedrawRequested += OnRedrawRequested;
            DataContext = _vm;
        }

        private void OnRedrawRequested(WallProfileConfig config,
                                        ObservableCollection<SegmentDef> segments,
                                        ObservableCollection<OpeningDef> openings)
        {
            ProfileCanvasRenderer.Render(PreviewCanvas, config, segments, openings);
        }

        private void PreviewCanvas_SizeChanged(object sender,
                                                SizeChangedEventArgs e)
        {
            // Re-render when the canvas is resized
            ProfileCanvasRenderer.Render(PreviewCanvas,
                _vm.CurrentConfig, _vm.Segments, _vm.Openings);
        }
    }
}
