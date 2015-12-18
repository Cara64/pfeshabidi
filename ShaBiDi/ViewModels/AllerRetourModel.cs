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
    public class AllerRetourModel : Model
    {
        private Dictionary<Image, double> data;
        private I_AllerRetour indic;

        public Dictionary<Image, double> Data
        {
            get { return data; }
            set { data = value; }
        }

        public I_AllerRetour Indic
        {
            get { return indic; }
            set { indic = value; }
        }
       
        public AllerRetourModel()
            : base()
        {}

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
                Title = "Aller-retour bandeau / image"
            };
            PlotModel.Axes.Add(valueAxis);
        }

        protected override void LoadData()
        {
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();

            var lineSerie = new LineSeries
            {
                Title = "Nombre d'aller-retour bandeau/image",
                StrokeThickness = 1,
                MarkerType = MarkerType.Circle
            };

            foreach (var key in mesures)
            {
                lineSerie.Points.Add(new DataPoint(key.Numero, Data[key]));
            }

            PlotModel.Series.Add(lineSerie);
        }


        // Normalise les données selon les critères sélectionnés
        protected override void GetData()
        {
            Data = new Dictionary<Image, double>();

            Positions = CreateIndicWindow.Positions;
            Groupes = CreateIndicWindow.Groupes;
            Ordres = CreateIndicWindow.Ordres;
            ModS = CreateIndicWindow.ModS;
            ModPA = CreateIndicWindow.ModPA;

            indic = new I_AllerRetour(Positions, Ordres, ModPA, ModS, Groupes);
            Data = indic.determineAllerRetour();
            
        }

        public override string ToString()
        {
            string res = "AllerRetour_GR";
            foreach (Groupe groupe in Groupes)
                res += (!groupe.Equals(Groupes.Last())) ? groupe.Identifiant + "-" : groupe.Identifiant + "_U";
            foreach (int pos in Positions)
                res += (!pos.Equals(Positions.Last())) ? pos + "-" : pos + "_ORD";
            foreach (OrdreGroupe ordre in Ordres)
                res += (!ordre.Equals(Ordres.Last())) ? ordre.ToString() + "-" : ordre.ToString() + "_MOD";
            if (ModS && ModPA)
                res += "S-PA";
            else
                if (ModS) res += "S";
                else res += "PA";

            return res;
        }
    }
    }
}
