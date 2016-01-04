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
    /// Logique d'interaction pour CompAllerRetourUC.xaml
    /// CompAllerRetourUC - Contrôle utilisateur pour l'affichage de la comparaison du nombre d'aller retour
    /// </summary>
    public partial class CompAllerRetourUC : UserControl
    {
        #region Attributs et propriétés

        /// <summary>
        /// Modèle pour le graphique
        /// </summary>
        private CompAllerRetourModel viewModel;
        public CompAllerRetourModel ViewModel
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

        public CompAllerRetourUC()
        {
            ViewModel = new CompAllerRetourModel();
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
