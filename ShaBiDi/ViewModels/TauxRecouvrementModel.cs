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
        #region Attributs

        private PlotModel plotModel;
        private List<int> positions;
        private List<OrdreGroupe> ordres;
        private List<Groupe> groupes;
        private bool modS;
        private bool modPA;

        #endregion


        #region Propriétés

        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; }
        }

        public List<int> Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        public List<OrdreGroupe> Ordres
        {
            get { return ordres; }
            set { ordres = value; }
        }

        public List<Groupe> Groupes
        {
            get { return groupes; }
            set { groupes = value; }
        }

        public bool ModS
        {
            get { return modS; }
            set { modS = value; }
        }

        public bool ModPA
        {
            get { return modPA; }
            set { modPA = value; }
        }

        #endregion

        public TauxRecouvrementModel()
        {
            PlotModel = new PlotModel();
            GetData();
            SetUpModel();
            LoadData();
        }

        private void SetUpModel()
        {
            PlotModel.Title = setPlotTitle();
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
            Positions = CreateIndicWindow.Positions;
            Groupes = ImportWindow.GroupesExp;
            Ordres = CreateIndicWindow.Ordres;
            ModS = CreateIndicWindow.ModS;
            ModPA = CreateIndicWindow.ModPA;

            I_TauxRecouvrement indic = new I_TauxRecouvrement(Positions, Ordres, ModPA, ModS, Groupes);
            var dicoTauxMoyen = indic.determineTaux();

            Dictionary<int, double> data = new Dictionary<int, double>();
            foreach (Image image in dicoTauxMoyen.Keys)
            {
                data[image.Numero] = dicoTauxMoyen[image];
            }
            return data;
        }

        private string setPlotTitle()
        {
            string res = "IND1_GR";
            foreach (Groupe groupe in Groupes) 
                res += (!groupe.Equals(Groupes.Last())) ? groupe.Identifiant + "-" : groupe.Identifiant + "_U" ;
            foreach (int pos in Positions) 
                res += (!pos.Equals(Positions.Last())) ? pos + "-": pos + "_ORD" ;
            foreach (OrdreGroupe ordre in Ordres)
                res += (!ordre.Equals(Ordres.Last())) ? ordre.ToString() + "-" : ordre.ToString() + "_MOD";
            if (modS && modPA)
                res += "S-PA";
            else
                if (modS) res += "S";
                else res += "PA";

            return res;
        }

          
    }

}
