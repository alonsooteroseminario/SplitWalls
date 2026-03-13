using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SplitWalls.Models;

namespace SplitWalls.UI
{
    public static class ProfileCanvasRenderer
    {
        private const double Margin = 40.0;

        public static void Render(Canvas canvas,
                                   WallProfileConfig config,
                                   ObservableCollection<SegmentDef> segments,
                                   ObservableCollection<OpeningDef> openings)
        {
            canvas.Children.Clear();

            if (segments == null || segments.Count == 0) return;

            double canvasW = canvas.ActualWidth;
            double canvasH = canvas.ActualHeight;
            if (canvasW < 1 || canvasH < 1) return;

            double wallLength = 0;
            foreach (var s in segments)
                if (s.EndMm > wallLength) wallLength = s.EndMm;
            if (wallLength <= 0) return;

            double wallHeight = config?.Defaults?.WallHeightMm > 0
                ? config.Defaults.WallHeightMm
                : 2440.0;

            double scaleX = (canvasW - 2 * Margin) / wallLength;
            double scaleY = (canvasH - 2 * Margin) / wallHeight;
            double scale = Math.Min(scaleX, scaleY);

            double drawW = wallLength * scale;
            double drawH = wallHeight * scale;
            double originX = Margin;
            double originY = Margin;

            // Wall outline
            AddRect(canvas, originX, originY, drawW, drawH,
                    Brushes.LightGray, Brushes.Black, 2);

            // Segments
            foreach (var seg in segments)
            {
                double x = originX + seg.StartMm * scale;
                double w = seg.WidthMm * scale;
                if (w < 1) continue;

                var fill = GetProfileBrush(seg.Profile);
                AddRect(canvas, x, originY, w, drawH, fill, Brushes.DarkGray, 1);

                // Width label above segment
                AddText(canvas, seg.WidthMm.ToString("0") + "mm",
                        x + w / 2, originY - 14, 9, HorizontalAlignment.Center);

                // Profile label inside segment
                if (w > 20)
                    AddText(canvas, seg.Profile ?? "",
                            x + w / 2, originY + drawH / 2 - 7, 9, HorizontalAlignment.Center);
            }

            // Openings
            if (openings != null)
            {
                foreach (var op in openings)
                {
                    double ox = originX + op.XMm * scale;
                    double oy = originY + (wallHeight - op.YMm - op.HeightMm) * scale;
                    double ow = op.WidthMm * scale;
                    double oh = op.HeightMm * scale;
                    if (ow < 1 || oh < 1) continue;
                    AddRect(canvas, ox, oy, ow, oh,
                            new SolidColorBrush(Color.FromArgb(180, 173, 216, 230)),
                            Brushes.SteelBlue, 1);
                    AddText(canvas, op.Type ?? "",
                            ox + ow / 2, oy + oh / 2 - 7, 8, HorizontalAlignment.Center);
                }
            }

            // Total length label below wall
            AddText(canvas, wallLength.ToString("0") + "mm total",
                    originX + drawW / 2, originY + drawH + 6, 9, HorizontalAlignment.Center);
        }

        private static Brush GetProfileBrush(string profile)
        {
            switch (profile)
            {
                case "U":       return new SolidColorBrush(Color.FromRgb(255, 230, 180));
                case "L_left":  return new SolidColorBrush(Color.FromRgb(180, 210, 255));
                case "L_right": return new SolidColorBrush(Color.FromRgb(180, 255, 210));
                case "T":       return new SolidColorBrush(Color.FromRgb(255, 200, 200));
                case "I":       return new SolidColorBrush(Color.FromRgb(220, 200, 255));
                case "borde":   return new SolidColorBrush(Color.FromRgb(255, 255, 180));
                default:        return new SolidColorBrush(Color.FromRgb(240, 240, 240));
            }
        }

        private static void AddRect(Canvas canvas,
                                     double x, double y, double w, double h,
                                     Brush fill, Brush stroke, double thickness)
        {
            var rect = new Rectangle
            {
                Width = w,
                Height = h,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            canvas.Children.Add(rect);
        }

        private static void AddText(Canvas canvas, string text,
                                     double cx, double y, double fontSize,
                                     HorizontalAlignment align)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                Foreground = Brushes.Black,
                HorizontalAlignment = align
            };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double x = align == HorizontalAlignment.Center
                ? cx - tb.DesiredSize.Width / 2
                : cx;
            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);
            canvas.Children.Add(tb);
        }
    }
}
