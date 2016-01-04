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
    /// Logique d'interaction pour TauxRecouvrementUC.xaml
    /// TauxRecouvrementUC - Contrôle utilisateur pour l'affichage de l'indicateur de taux de recouvrement
    /// </summary>
    public partial class TauxRecouvrementUC : UserControl
    {

        #region Attributs et propriétés

        /// <summary>
        /// Modèle associé au graphique
        /// </summary>
        private TauxRecouvrementModel viewModel;
        public TauxRecouvrementModel ViewModel
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
        /// Constructeur de la classe TauxRecouvrementUC
        /// </summary>
        public TauxRecouvrementUC()
        {
            // On définit le modèle
            ViewModel = new TauxRecouvrementModel();
            DataContext = ViewModel;
            InitializeComponent();

            // On affiche le résultat
            res = new ResultWindow();
            res.Title = this.ToString();
            res.Content = this;
            res.Show();
        }

        #endregion


        #region Divers

        /// <summary>
        /// Retourne le nom de l'indicateur
        /// </summary>
        /// <returns>Nom de l'indicateur</returns>
        public override string ToString()
        {
            return ViewModel.ToString();
        }

        #endregion
    }
}
