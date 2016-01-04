using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ShaBiDi.Views;
using ShaBiDi.Logic;

namespace ShaBiDi.ViewModels
{
    /// <summary>
    /// TauxRecouvrementModel - Modèle pour le taux de recouvrement
    /// </summary>
    public class TauxRecouvrementModel : Model
    {

        #region Attributs et propriétés

        /// <summary>
        /// Dictionnaires de données pour chaque image
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
        /// Constructeur par défaut pour la classe de taux de recouvrement modèle
        /// </summary>
        public TauxRecouvrementModel()
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
                Maximum = 100.0,
                Title = "Taux de recouvrement"
            };
            PlotModel.Axes.Add(valueAxis);
        }

        /// <summary>
        /// Mise en place des données
        /// </summary>
        protected override void LoadData()
        {
            var mesures = data.Keys.OrderBy(o => o.Numero).ToList();

            var lineSerie = new LineSeries
            {
                Title = "Taux de recouvrement",
                StrokeThickness = 1,
                MarkerType = MarkerType.Circle
            };

            foreach (var key in mesures)
            {
                lineSerie.Points.Add(new DataPoint(key.Numero, data[key]));
            }

            PlotModel.Series.Add(lineSerie);
        }

        /// <summary>
        /// Récupération des données
        /// </summary>
        protected override void GetData()
        {
            Data = AppData.IndicateursTauxRecouvrement.Last().Data;
        }

        /// <summary>
        /// Nom de l'indicateur
        /// </summary>
        /// <returns>Nom de l'indicateur</returns>
        public override string ToString()
        {
            return AppData.IndicateursTauxRecouvrement.Last().ToString();
        }

        #endregion

    }

          
 

}
