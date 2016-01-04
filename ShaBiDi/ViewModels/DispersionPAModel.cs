using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaBiDi.Views;
using ShaBiDi.Logic;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ShaBiDi.ViewModels
{
    /// <summary>
    /// DispersionPAModel - Modèle pour l'indicateur de la dispersion du PA
    /// </summary>
    public class DispersionPAModel : Model
    {

        #region Attributs et propriétés

        /// <summary>
        /// Dictionnaire de donnée pour chaque image
        /// </summary>
        private Dictionary<ImageExp, double> data;
        public Dictionary<ImageExp, double> Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur par défaut pour la classe de calcul du taux de dispersion
        /// </summary>
        public DispersionPAModel()
            : base()
        {
            Data = new Dictionary<ImageExp, double>();
        }

        #endregion


        #region Méthodes surchargées

        /// <summary>
        /// Mise en place de l'UI
        /// </summary>
        protected override void SetUpModel()
        {
            PlotModel.Title = this.ToString();
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
                Maximum = 31.0,
                MinorStep = 1.0,
                MajorStep = 1.0,
                Title = "N° de l'image"
            };
            PlotModel.Axes.Add(imageAxis);

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0.0,
                Maximum = 1000.0,
                Title = "Dispersion du PA en pixels"
            };
            PlotModel.Axes.Add(valueAxis);
        }

        /// <summary>
        /// Mise en place des données
        /// </summary>
        protected override void LoadData()
        {
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();

            var lineSerie = new LineSeries
            {
                Title = "Taux de dispersion du PA",
                StrokeThickness = 1,
                MarkerType = MarkerType.Circle
            };

            foreach (var key in mesures)
            {
                lineSerie.Points.Add(new DataPoint(key.Numero, Data[key]));
            }

            PlotModel.Series.Add(lineSerie);
        }

        /// <summary>
        /// Récupération des données
        /// </summary>
        protected override void GetData()
        {
            Data = AppData.IndicateursDispersionPA.Last().Data;   
        }

        public override string ToString()
        {
            return AppData.IndicateursDispersionPA.Last().ToString();
        }

        #endregion

    }
}
