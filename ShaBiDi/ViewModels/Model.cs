using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Axes;
using ShaBiDi.Logic;

namespace ShaBiDi.ViewModels
{
    public abstract class Model
    {
        #region Attributs

        protected PlotModel plotModel;
        protected List<int> positions;
        protected List<OrdreGroupe> ordres;
        protected List<Groupe> groupes;
        protected bool modS;
        protected bool modPA;

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


        #region Constructeurs

        public Model()
        {
            PlotModel = new PlotModel();
            
            GetData();
            SetUpModel();
            LoadData();
        }

        #endregion 


        #region Méthodes

        protected abstract void SetUpModel();     // set up UI graph
        protected abstract void LoadData();       // set up data in graph
        protected abstract void GetData();        // retrieve data from computation

        #endregion
    }
}
