using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace BlazorLiveCharts2.Pages
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; } = new ISeries[] {
          new LineSeries<double>
                {
                    Values = new List<double>() { 2, 1, 3, 5, 3, 4, 6 },
                    Fill = null,
                    GeometrySize = 20
                }
        };

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };
    }
}
