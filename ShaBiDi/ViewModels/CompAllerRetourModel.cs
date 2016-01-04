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
    /// CompAllerRetourModel - Classe pour le modèle de comparaison du nombre aller-retour bandeau / image
    /// </summary>
    public class CompAllerRetourModel : Model
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
        /// Constructeur pour la classe CompAllerRetourModel
        /// </summary>
        public CompAllerRetourModel() : base()
        {
        }

        #endregion


        #region Méthodes surchargées

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
                Title = "Nb A/R bandeau image" 
            };
            PlotModel.Axes.Add(valueAxis);
        }

        protected override void LoadData()
        {
            int i = 0;
            string indicTitle;
            MarkerType[] markers = { MarkerType.Circle, MarkerType.Cross, MarkerType.Square };
            foreach(Dictionary<ImageExp, double> dico in Data)
            {
                var mesures = dico.Keys.OrderBy(o=>o.Numero).ToList();

                indicTitle = (i == 0) ? AppData.ComparateursAllerRetour.Last().ToString() : AppData.ComparateursAllerRetour.Last().IndicCompare.ToString();
                
                var lineSerie = new LineSeries
                {
                    Title = (i != 2) ? indicTitle : "Comparaison",
                    StrokeThickness = 1,
                    MarkerType = markers[i]
                };

                foreach (var key in mesures)
                    lineSerie.Points.Add(new DataPoint(key.Numero, dico[key]*100));

                PlotModel.Series.Add(lineSerie);
                i++;
            }
        }

        protected override void GetData()
        {
            Data = new List<Dictionary<ImageExp, double>>();
            
            // Récupération des données des deux indicateurs
            Data.Add(AppData.ComparateursAllerRetour.Last().DataComparaison[0]);
            Data.Add(AppData.ComparateursAllerRetour.Last().DataComparaison[1]);
            Data.Add(AppData.ComparateursAllerRetour.Last().DataComparaison[2]);
           
        }

        public override string ToString()
        {
            string res = "CompAllerRetour_";
            res += AppData.ComparateursAllerRetour.Last().ToString() + "_";
            res += AppData.ComparateursAllerRetour.Last().IndicCompare.ToString();
   
            return res;
        }

        #endregion
    }
}
