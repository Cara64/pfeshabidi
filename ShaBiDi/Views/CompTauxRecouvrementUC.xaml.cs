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
    /// Logique d'interaction pour CompTauxRecouvrementUC.xaml
    /// CompTauxRecouvrementUC - Contrôle utilisateur pour l'affichage de la comparaison du taux de recouvrement
    /// </summary>
    public partial class CompTauxRecouvrementUC : UserControl
    {

        #region Attribut et propriétés

        /// <summary>
        /// Modèle pour le graphique
        /// </summary>
        private CompTauxRecouvrementModel viewModel;
        public CompTauxRecouvrementModel ViewModel
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

        public CompTauxRecouvrementUC()
        {
            ViewModel = new CompTauxRecouvrementModel();
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