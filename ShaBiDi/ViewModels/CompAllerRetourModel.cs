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
    public class CompAllerRetourModel : Model
    {
        private List<Dictionary<ImageExp, double>> data;
        private List<AllerRetourUC> indicSelect;
       
        public List<Dictionary<ImageExp, double>> Data
        {
            get { return data; }
            set { data = value;}
        }

        public List<AllerRetourUC> IndicSelect
        {
            get { return indicSelect; }
            set { indicSelect = value; }
        }

        public CompAllerRetourModel() : base()
        {
            IndicSelect = new List<AllerRetourUC>();
            foreach (System.Windows.Controls.UserControl uc in CompareIndicWindow.IndicateursSelectionnes)
                IndicSelect.Add(uc as AllerRetourUC);
        }
            
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
            AllerRetourUC indic = new AllerRetourUC();
            MarkerType[] markers = { MarkerType.Circle, MarkerType.Cross, MarkerType.Square };
            foreach(Dictionary<ImageExp, double> dico in Data)
            {
                var mesures = dico.Keys.OrderBy(o=>o.Numero).ToList();
                
                if (i != 2) indic = CompareIndicWindow.IndicateursSelectionnes[i] as AllerRetourUC;
                
                var lineSerie = new LineSeries
                {
                    Title = (i != 2) ? indic.ToString() : "Comparaison",
                    StrokeThickness = 1,
                    MarkerType = markers[i]
                };

                foreach (var key in mesures)
                    lineSerie.Points.Add(new DataPoint(key.Numero, dico[key]*100));

                PlotModel.Series.Add(lineSerie);
                i++;
            }
        }


        // Normalise les données selon les critères sélectionnés
        protected override void GetData()
        {
            Data = new List<Dictionary<ImageExp, double>>();
            
            // Récupération des données des deux indicateurs
          
            AllerRetourModel indic1Model = IndicSelect[0].ViewModel;
            AllerRetourModel indic2Model = IndicSelect[1].ViewModel;

            Dictionary<ImageExp, double> dataIndic1 = indic1Model.Data;
            Dictionary<ImageExp, double> dataIndic2 = indic2Model.Data;
            
            Data.Add(dataIndic1);
            Data.Add(dataIndic2);

            I_AllerRetour indic1 = new I_AllerRetour(indic1Model.Positions, indic1Model.Ordres, indic1Model.ModPA, indic1Model.ModS, indic1Model.Groupes);
            I_AllerRetour indic2 = new I_AllerRetour(indic2Model.Positions, indic2Model.Ordres, indic2Model.ModPA, indic2Model.ModS, indic2Model.Groupes);
           
            Dictionary<ImageExp, double> dataRes = indic1.compareAllerRetour(CompareIndicWindow.TypeComparaison, indic2);
            
            Data.Add(dataRes);
        }

        public override string ToString()
        {
           
            string res = "CompAllerRetour_";
            res += IndicSelect[0].ViewModel.ToString() + "_";
            res += IndicSelect[1].ViewModel.ToString();
   
            return res;
        }
    }
}
