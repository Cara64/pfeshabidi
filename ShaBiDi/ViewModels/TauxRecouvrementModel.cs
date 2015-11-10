using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ShaBiDi.ViewModels
{
    public class TauxRecouvrementModel
    {

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; }
        }

        public TauxRecouvrementModel()
        {
            PlotModel = new PlotModel();
            SetUpModel();
            GetData();
            LoadData();
        }

        private void SetUpModel()
        {
            PlotModel.LegendTitle = "Légende";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            var imageAxis = new LinearAxis 
            { 
                Position = AxisPosition.Bottom, 
                Minimum = 0.0, 
                Maximum = 30.0, 
                Title="N° de l'image" 
            };
            PlotModel.Axes.Add(imageAxis);

            var valueAxis = new LinearAxis 
            { 
                Position = AxisPosition.Left, 
                Minimum = 0.0, 
                Maximum = 100.0, 
                Title = "Taux de recouvrement" 
            };
            PlotModel.Axes.Add(valueAxis);
        }

        private void LoadData()
        {
            var mesures = GetData();
            
            var data = mesures.Keys.ToList();
            data.Sort();

            var lineSerie = new LineSeries
            {
                Title = "Taux de recouvrement",
                StrokeThickness = 1,
                MarkerType = MarkerType.Circle
            };

            foreach (var key in data)
            {
                lineSerie.Points.Add(new DataPoint(key, mesures[key] * 100));
            }

            PlotModel.Series.Add(lineSerie);
        }


        // Normalise les données selon les critères sélectionnés
        private Dictionary<int, double> GetData()
        {
            Indicateur indic = new Indicateur(CreateIndicWindow.Positions, ImportWindow.GroupesExp, CreateIndicWindow.Ordres, CreateIndicWindow.ModS, CreateIndicWindow.ModPA);
            var dicoTauxMoyen = indic.determineTaux();

            Dictionary<int, double> data = new Dictionary<int, double>();
            foreach (Image image in dicoTauxMoyen.Keys)
            {
                data[image.Numero] = dicoTauxMoyen[image];
            }
            return data;

        }

        

    }

}
