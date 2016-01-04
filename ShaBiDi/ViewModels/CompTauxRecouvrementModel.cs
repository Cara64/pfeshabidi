using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Axes;
using ShaBiDi.Views;
using ShaBiDi.Logic;

namespace ShaBiDi.ViewModels
{
    /// <summary>
    /// CompTauxRecouvrementModel - Classe pour le modèle de la comparaison du taux de recouvrement
    /// </summary>
    public class CompTauxRecouvrementModel : Model
    {

        #region Attributs et propriétés

        /// <summary>
        /// Ensemble des dictionnaires nécessaires pour les données
        /// </summary>
        private List<Dictionary<ImageExp, double>> data;
        public List<Dictionary<ImageExp, double>> Data
        {
            get { return data; }
            set { data = value;}
        }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur pour la classe CompTauxRecouvrementModel
        /// </summary>
        public CompTauxRecouvrementModel() : base()
        {
        }

        #endregion


        #region Méthodes surchargées

        /// <summary>
        /// Mise en place du graphique
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

        /// <summary>
        /// Mise en place des données
        /// </summary>
        protected override void LoadData()
        {
            int i = 0;
            string indicTitle;
            MarkerType[] markers = { MarkerType.Circle, MarkerType.Cross, MarkerType.Square };
            foreach(Dictionary<ImageExp, double> dico in Data)
            {
                var mesures = dico.Keys.OrderBy(o=>o.Numero).ToList();
                
                indicTitle = (i == 0) ? AppData.ComparateursTauxRecouvrement.Last().ToString() : AppData.ComparateursTauxRecouvrement.Last().IndicCompare.ToString();
            
                var lineSerie = new LineSeries
                {
                    Title = (i != 2) ? indicTitle : "Comparaison",
                    StrokeThickness = 1,
                    MarkerType = markers[i]
                };

                foreach (var key in mesures)
                    lineSerie.Points.Add(new DataPoint(key.Numero, dico[key]));

                PlotModel.Series.Add(lineSerie);
                i++;
            }
        }


        /// <summary>
        /// Récupération des données
        /// </summary>
        protected override void GetData()
        {
            Data = new List<Dictionary<ImageExp, double>>();
            
            // Récupération des données des deux indicateurs
            Data.Add(AppData.ComparateursTauxRecouvrement.Last().DataComparaison[0]);
            Data.Add(AppData.ComparateursTauxRecouvrement.Last().DataComparaison[1]);
            Data.Add(AppData.ComparateursTauxRecouvrement.Last().DataComparaison[2]);
        }

        #endregion


        #region Divers

        public override string ToString()
        {
            string res = "CompTauxRecouvrement_";
            res += AppData.ComparateursTauxRecouvrement.Last().ToString() + "_";
            res += AppData.ComparateursTauxRecouvrement.Last().IndicCompare.ToString();
   
            return res;
        }

        #endregion
    }
}
