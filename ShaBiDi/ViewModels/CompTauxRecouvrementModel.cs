using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Axes;


namespace ShaBiDi.ViewModels
{
    public class CompTauxRecouvrementModel : Model
    {
       
        private List<Dictionary<Image, double>> data;
       
        public List<Dictionary<Image, double>> Data
        {
            get { return data; }
            set { data = value;}
        }

        public CompTauxRecouvrementModel() : base()
        {
 
            Data = new List<Dictionary<Image, double>>();
            

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
                Title = "Taux de recouvrement" 
            };
            PlotModel.Axes.Add(valueAxis);
        }

        protected override void LoadData()
        {
            int i = 0;
            TauxRecouvrement indic = new TauxRecouvrement();
            MarkerType[] markers = { MarkerType.Circle, MarkerType.Cross, MarkerType.Square };
            foreach(Dictionary<Image, double> dico in Data)
            {
                var mesures = dico.Keys.OrderBy(o=>o.Numero).ToList();
                
                if (i != 2) indic = CompareIndicWindow.IndicateursSelectionnes[i] as TauxRecouvrement;
                
                var lineSerie = new LineSeries
                {
                    Title = (i != 2) ? "Comparaison" : indic.ToString(),
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
            TauxRecouvrement indic1 = CompareIndicWindow.IndicateursSelectionnes[0] as TauxRecouvrement;
            TauxRecouvrement indic2 = CompareIndicWindow.IndicateursSelectionnes[1] as TauxRecouvrement;

            Dictionary<Image, double> dataIndic1 = indic1.ViewModel.Data;
            Dictionary<Image, double> dataIndic2 = indic2.ViewModel.Data;
            Data.Add(dataIndic1);
            Data.Add(dataIndic2);

            I_TauxRecouvrement indic3 = new I_TauxRecouvrement(Positions, Ordres, ModPA, ModS, Groupes);
            indic3 = indic3.compareTaux(CompareIndicWindow.TypeComparaison,
                new I_TauxRecouvrement(indic1.ViewModel.Positions, indic1.ViewModel.Ordres, indic1.ViewModel.ModPA, indic1.ViewModel.ModS, indic2.ViewModel.Groupes),
                new I_TauxRecouvrement(indic2.ViewModel.Positions, indic2.ViewModel.Ordres, indic2.ViewModel.ModPA, indic2.ViewModel.ModS, indic2.ViewModel.Groupes));
            
            Dictionary<Image, double> dataRes = indic3._monDico;
            Data.Add(dataRes);
        }

        public override string ToString()
        {
            string res = "CompTauxRecouvrement_";
            //res+=NomsIndicateurs[0] + "_";
            //res+=NomsIndicateurs[1];

            return res;
        }
    }
}
