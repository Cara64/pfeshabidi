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
    /// <summary>
    /// Model - Classe abstraite pour la définition des modèles liées aux indicateurs de taux de recouvrement, dispersion, et aller retour
    /// </summary>
    public abstract class Model
    {

        #region Attributs et propriétés

        /// <summary>
        /// Plot Model pour OxyPlot
        /// </summary>
        protected PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; }
        }

        /// <summary>
        /// Positions de l'indicateur
        /// </summary>
        protected List<int> positions;
        public List<int> Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        /// <summary>
        /// Ordres des modalités de l'indicateur
        /// </summary>
        protected List<OrdreGroupe> ordres;
        public List<OrdreGroupe> Ordres
        {
            get { return ordres; }
            set { ordres = value; }
        }

        /// <summary>
        /// Groupes de l'indicateur
        /// </summary>
        protected List<Groupe> groupes;
        public List<Groupe> Groupes
        {
            get { return groupes; }
            set { groupes = value; }
        }

        /// <summary>
        /// Modalité S
        /// </summary>
        protected bool modS;
        public bool ModS
        {
            get { return modS; }
            set { modS = value; }
        }

        /// <summary>
        /// Modalité PA
        /// </summary>
        protected bool modPA;
        public bool ModPA
        {
            get { return modPA; }
            set { modPA = value; }
        }

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut du modèle
        /// </summary>
        public Model()
        {
            PlotModel = new PlotModel();
            
            GetData();
            SetUpModel();
            LoadData();
        }

        #endregion 


        #region Méthodes à surcharger

        /// <summary>
        /// Mise en place de l'UI
        /// </summary>
        protected abstract void SetUpModel();     
        
        /// <summary>
        /// Mise en place des données
        /// </summary>
        protected abstract void LoadData();       // set up data in graph
        
        /// <summary>
        /// Récupération des données
        /// </summary>
        protected abstract void GetData();        // retrieve data from computation

        #endregion

    }
}
