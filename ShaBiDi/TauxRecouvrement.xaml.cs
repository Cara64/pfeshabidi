using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot.Axes;

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour TauxRecouvrement.xaml
    /// </summary>
    public partial class TauxRecouvrement : Window
    {
        public TauxRecouvrement()
        {
            InitializeComponent();
            
            PlotModel pm = new PlotModel();

            pm.PlotType = PlotType.XY;
            pm.Background = OxyColor.FromRgb(255, 255, 255);
            pm.TextColor = OxyColor.FromRgb(255, 255, 255);

            var s1 = new OxyPlot.Series.LineSeries { Title = "Taux de recouvrement", StrokeThickness = 1 };
            s1.Points.Add(new DataPoint(2, 7));
            s1.Points.Add(new DataPoint(7, 9));
            s1.Points.Add(new DataPoint(9, 4));

            pm.Series.Add(s1);
          
            pm.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 30 });
            pm.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 50 });

            Toto.Model = pm;

     
    
           
            
            
            
        }
    }
}
