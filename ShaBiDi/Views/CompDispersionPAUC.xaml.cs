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
    /// Logique d'interaction pour CompDispersionPAUC.xaml
    /// CompDispersionPAUC - Contrôle utilisateur pour l'affichage de la comparaison de la dispersion en PA
    /// </summary>
    public partial class CompDispersionPAUC : UserControl
    {
        #region Attributs et propriétés

        /// <summary>
        /// Modèle pour le graphique
        /// </summary>
        private CompDispersionPAModel viewModel;
        public CompDispersionPAModel ViewModel
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

        public CompDispersionPAUC()
        {
            ViewModel = new CompDispersionPAModel();
            DataContext = ViewModel;
            InitializeComponent();

            res = new ResultWindow();
            res.Title = this.ToString();
            res.Content = this;
            res.Show();
        }

        #endregion
    }
}
