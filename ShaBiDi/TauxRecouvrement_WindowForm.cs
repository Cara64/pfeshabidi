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
       
        public TauxRecouvrement_WindowForm(Dictionary<Image, double> dicoTauxMoyen)
        {
            InitializeComponent();

            double[] images = new double[30];
            double[] tauxMoyenParImage = new double[30];

            int i = 0;
            foreach (Image image in dicoTauxMoyen.Keys)
            {
                images[i] = image.Numero;
                tauxMoyenParImage[i] = dicoTauxMoyen[image];
                i++;
            }

            PlotView Plot = new PlotView();
            Plot.Model = new PlotModel();
            Plot.Dock = DockStyle.Fill;
            this.Controls.Add(Plot);

            Plot.Model.PlotType = PlotType.XY;
            Plot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            Plot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);
          
            // Create Line series
            var s1 = new LineSeries { Title = "Taux de recouvrement", StrokeThickness = 1, MarkerType = MarkerType.Circle };
            for (int j = 0; j < images.Length; j++)
            {
                s1.Points.Add(new DataPoint(images[j], tauxMoyenParImage[j]));
                Console.WriteLine(images[j]);
                Console.WriteLine(tauxMoyenParImage[j]);
            }

            // add Series and Axis to plot mode
            Plot.Model.Series.Add(s1);
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = 0.0, Maximum = 30.0});
            Plot.Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = 0.0, Maximum = 100.0 });
        }
    }
}
