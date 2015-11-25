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
    public class CompTauxRecouvrementModel
    {
        #region Attributs

        private PlotModel plotModel;
        private List<int> positions;
        private List<OrdreGroupe> ordres;
        private List<Groupe> groupes;
        private bool modS;
        private bool modPA;
        private string[] nomsIndicateurs = new string[3];

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

        public string[] NomsIndicateurs
        {
            get { return nomsIndicateurs; }
            set { nomsIndicateurs = value;}
        }

        #endregion

        public CompTauxRecouvrementModel()
        {
 
            for (int i = 0; i < CompareIndicWindow.indicateurSelected.Count(); i++) NomsIndicateurs[i] = CompareIndicWindow.indicateurSelected[i];
            PlotModel = new PlotModel();
            GetData();
            SetUpModel();
            LoadData();
            CompareIndicWindow.nomIndicateurs.Add(setPlotTitle());

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
            int i = 0;
            var listMesures = GetData();
            string[] titleData = { NomsIndicateurs[0], NomsIndicateurs[1], "Comparaison" };
            MarkerType[] markers = { MarkerType.Circle, MarkerType.Cross, MarkerType.Square };
            foreach (Dictionary<int, double> mesures in listMesures)
            {
                var data = mesures.Keys.ToList();
                data.Sort();
                var lineSerie = new LineSeries
                {
                    Title = titleData[i],
                    StrokeThickness = 1,
                    MarkerType = markers[i]
                };

                foreach (var key in data)
                    lineSerie.Points.Add(new DataPoint(key, mesures[key] * 100));

                PlotModel.Series.Add(lineSerie);
                i++;
            }    
        }


        // Normalise les données selon les critères sélectionnés
        private List<Dictionary<int, double>> GetData()
        {
            // Pour démonstration.
            // TODO : Modifier de sorte à ce que les indicateurs correspondent réellement à ceux sélectionnés

            // Recalcul des deux indicateurs
            TauxRecouvrement indic;
            I_TauxRecouvrement[] indicTR = new I_TauxRecouvrement[3];
            Dictionary<Image, double> dicoTauxMoyen;

            Dictionary<int, double> dataInd1  = new Dictionary<int,double>();
            Dictionary<int, double> dataInd2  = new Dictionary<int,double>();
            Dictionary<int, double> dataComp  = new Dictionary<int,double>();

            List<Dictionary<int, double>> data = new List<Dictionary<int, double>>();

            for (int i = 0; i < CreateIndicWindow.Indicateurs.Count; i++)
            {
                indic = CreateIndicWindow.Indicateurs[i] as TauxRecouvrement;

                Positions = indic.ViewModel.Positions;
                Groupes = indic.ViewModel.Groupes;
                Ordres = indic.ViewModel.Ordres;
                ModS = indic.ViewModel.ModS;
                ModPA = indic.ViewModel.ModPA;

                indicTR[i] = new I_TauxRecouvrement(Positions, Ordres, ModPA, ModS, Groupes);
                dicoTauxMoyen = indicTR[i].determineTaux();
                foreach (Image image in dicoTauxMoyen.Keys)
                {
                    if (i == 0) dataInd1[image.Numero] = dicoTauxMoyen[image];
                    else dataInd2[image.Numero] = dicoTauxMoyen[image];
                }
            }

            // Calcul du résultat
            TypeComp typeComp;
            if (CompareIndicWindow.compAdd) typeComp = TypeComp.add;
            else
            {
                if (CompareIndicWindow.compSous) typeComp = TypeComp.sous;
                else typeComp = TypeComp.moy;
            }

            indicTR[2] = new I_TauxRecouvrement(Positions, Ordres, ModPA, ModS, Groupes);
            indicTR[2] = indicTR[2].compareTaux(typeComp, indicTR[0], indicTR[1]);
            dicoTauxMoyen = indicTR[2]._monDico;
            foreach (Image image in dicoTauxMoyen.Keys)
            {
                dataComp[image.Numero]= dicoTauxMoyen[image];
            }

            data.Add(dataInd1);
            data.Add(dataInd2);
            data.Add(dataComp);
            return data;
        }

        private string setPlotTitle()
        {
            string res = "CompTauxRecouvrement_";
            res+=NomsIndicateurs[0] + "_";
            res+=NomsIndicateurs[1];

            return res;
        }
    }
}
