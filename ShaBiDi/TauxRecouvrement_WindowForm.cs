using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace ShaBiDi
{
    public partial class TauxRecouvrement_WindowForm : Form
    {
        public TauxRecouvrement_WindowForm()
        {
            InitializeComponent();
            PlotView Plot = new PlotView();
            Plot.Model = new PlotModel();
            Plot.Dock = DockStyle.Fill;
            this.Controls.Add(Plot);

            Plot.Model.PlotType = PlotType.XY;
            Plot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            Plot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);

            // Create Line series
            var s1 = new LineSeries { Title = "LineSeries", StrokeThickness = 1 };
            s1.Points.Add(new DataPoint(2, 7));
            s1.Points.Add(new DataPoint(7, 9));
            s1.Points.Add(new DataPoint(9, 4));

            // add Series and Axis to plot model
            Plot.Model.Series.Add(s1);
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = 0.0, Maximum = 10.0});
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = 0.0, Maximum = 10.0 });
        }
    }
}
