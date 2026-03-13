using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using SplitWalls.Models;
using SplitWalls.Services;

namespace SplitWalls.UI
{
    public class ProfileEditorViewModel : INotifyPropertyChanged
    {
        private readonly ProfileFileService _fileService;
        private WallProfileConfig _config;
        private string _currentFilePath;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<WallProfileConfig, ObservableCollection<SegmentDef>, ObservableCollection<OpeningDef>> RedrawRequested;

        // ── Bound properties ──────────────────────────────────────────────
        public string Name
        {
            get => _config.Name;
            set { _config.Name = value; OnPropertyChanged(); RequestRedraw(); }
        }

        public string Strategy
        {
            get => _config.Strategy;
            set { _config.Strategy = value; OnPropertyChanged(); RequestRedraw(); }
        }

        public double PanelWidthMm
        {
            get => _config.Defaults.PanelWidthMm;
            set { _config.Defaults.PanelWidthMm = value; OnPropertyChanged(); RequestRedraw(); }
        }

        public double SeparatorWidthMm
        {
            get => _config.Defaults.SeparatorWidthMm;
            set { _config.Defaults.SeparatorWidthMm = value; OnPropertyChanged(); RequestRedraw(); }
        }

        public double WallHeightMm
        {
            get => _config.Defaults.WallHeightMm;
            set { _config.Defaults.WallHeightMm = value; OnPropertyChanged(); RequestRedraw(); }
        }

        public bool DisableWallJoins
        {
            get => _config.Defaults.DisableWallJoins;
            set { _config.Defaults.DisableWallJoins = value; OnPropertyChanged(); }
        }

        public string SplitMethod
        {
            get => _config.SplitRule.Method;
            set { _config.SplitRule.Method = value; OnPropertyChanged(); }
        }

        public WallProfileConfig CurrentConfig => _config;

        public ObservableCollection<SegmentDef> Segments { get; private set; }
        public ObservableCollection<OpeningDef> Openings { get; private set; }

        // ── Static lists for ComboBox binding ─────────────────────────────
        public List<string> Strategies { get; } = new List<string> { "noWindows", "osb", "smartPanel" };
        public List<string> SplitMethods { get; } = new List<string> { "uniform", "custom" };
        public List<string> ProfileTypes { get; } = new List<string> { "standard", "U", "L_left", "L_right", "T", "I", "borde" };
        public List<string> FireRatings { get; } = new List<string> { "none", "1hr", "2hr", "3hr" };

        // ── Commands ──────────────────────────────────────────────────────
        public ICommand NewCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand AutoSplitCommand { get; }
        public ICommand AddOpeningCommand { get; }
        public ICommand RemoveOpeningCommand { get; }

        public ProfileEditorViewModel()
        {
            _fileService = new ProfileFileService();
            _config = ProfileFileService.CreateTemplate("osb");
            Segments = new ObservableCollection<SegmentDef>(_config.Segments);
            Openings = new ObservableCollection<OpeningDef>(_config.Openings);

            NewCommand = new RelayCommand(_ => NewProfile());
            LoadCommand = new RelayCommand(_ => LoadProfile());
            SaveCommand = new RelayCommand(_ => SaveProfile());
            SaveAsCommand = new RelayCommand(_ => SaveProfileAs());
            AutoSplitCommand = new RelayCommand(_ => AutoSplit());
            AddOpeningCommand = new RelayCommand(_ => AddOpening());
            RemoveOpeningCommand = new RelayCommand(p => RemoveOpening(p as OpeningDef));
        }

        private void NewProfile()
        {
            _config = ProfileFileService.CreateTemplate("osb");
            _currentFilePath = null;
            SyncFromConfig();
        }

        private void LoadProfile()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Load Wall Profile",
                Filter = "Profile Files (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = _fileService.GetDefaultFolder()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _config = _fileService.Load(dlg.FileName);
                _currentFilePath = dlg.FileName;
                SyncFromConfig();
            }
        }

        private void SaveProfile()
        {
            if (_currentFilePath == null) { SaveProfileAs(); return; }
            _config.Segments = new List<SegmentDef>(Segments);
            _config.Openings = new List<OpeningDef>(Openings);
            _fileService.Save(_config, _currentFilePath);
        }

        private void SaveProfileAs()
        {
            var dlg = new SaveFileDialog
            {
                Title = "Save Wall Profile",
                Filter = "Profile Files (*.txt)|*.txt",
                FileName = _config.Name,
                InitialDirectory = _fileService.GetDefaultFolder()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _currentFilePath = dlg.FileName;
                SaveProfile();
            }
        }

        private void AutoSplit()
        {
            // Generate uniform segments across a nominal 6000mm wall for preview
            const double wallLength = 6000.0;
            Segments.Clear();
            var x = 0.0;
            var idx = 0;
            while (x < wallLength - 1)
            {
                var width = Math.Min(PanelWidthMm, wallLength - x);
                Segments.Add(new SegmentDef
                {
                    Index = idx++,
                    StartMm = x,
                    EndMm = x + width,
                    WidthMm = width,
                    Profile = "standard",
                    Label = "",
                    FireRating = "none"
                });
                x += width + SeparatorWidthMm;
            }
            RequestRedraw();
        }

        private void AddOpening()
        {
            Openings.Add(new OpeningDef
            {
                Index = Openings.Count,
                XMm = 1000,
                YMm = 900,
                WidthMm = 600,
                HeightMm = 1200,
                Type = "window"
            });
            RequestRedraw();
        }

        private void RemoveOpening(OpeningDef opening)
        {
            if (opening != null)
            {
                Openings.Remove(opening);
                RequestRedraw();
            }
        }

        private void SyncFromConfig()
        {
            Segments = new ObservableCollection<SegmentDef>(_config.Segments);
            Openings = new ObservableCollection<OpeningDef>(_config.Openings);
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Strategy));
            OnPropertyChanged(nameof(PanelWidthMm));
            OnPropertyChanged(nameof(SeparatorWidthMm));
            OnPropertyChanged(nameof(WallHeightMm));
            OnPropertyChanged(nameof(DisableWallJoins));
            OnPropertyChanged(nameof(SplitMethod));
            OnPropertyChanged(nameof(Segments));
            OnPropertyChanged(nameof(Openings));
            RequestRedraw();
        }

        private void RequestRedraw() =>
            RedrawRequested?.Invoke(_config, Segments, Openings);

        private void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
