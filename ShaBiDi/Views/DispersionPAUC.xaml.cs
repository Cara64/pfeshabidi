using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShaBiDi.ViewModels;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour DispersionUC.xaml
    /// DispersionPAUC - Contrôle utilisateur pour l'affichage de l'indicateur concernant la dispersion du PA
    /// </summary>
    public partial class DispersionPAUC : UserControl
    {

        #region Attributs et propriétés

        /// <summary>
        /// Modèle pour le graphique OxyPlot
        /// </summary>
        private DispersionPAModel viewModel;
        public DispersionPAModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }

        /// <summary>
        /// Fenêtre de résultat
        /// </summary>
        private ResultWindow res;

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur 
        /// </summary>
        public DispersionPAUC()
        {
            ViewModel = new DispersionPAModel();
            DataContext = ViewModel;
            InitializeComponent();

            res = new ResultWindow();
            res.Title = this.ToString();
            res.Content = this;
            res.Show();
        }

        #endregion


        #region Divers

        public override string ToString()
        {
            return ViewModel.ToString();
        }

        #endregion
    }
}
