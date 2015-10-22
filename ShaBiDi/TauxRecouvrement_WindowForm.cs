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
        Random r = new Random();
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
            var s1 = new LineSeries { Title = "LineSeries", StrokeThickness = 1, MarkerType = MarkerType.Circle };
            for (int i = 1; i < 31; i++)
            {
                s1.Points.Add(new DataPoint(i, r.NextDouble()*100.0));
            }

            // add Series and Axis to plot mode
            Plot.Model.Series.Add(s1);
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = 0.0, Maximum = 30.0});
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = 0.0, Maximum = 100.0 });
        }
    }
}
