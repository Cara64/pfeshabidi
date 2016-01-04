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
    /// Logique d'interaction pour AllerRetourUC.xaml
    /// AllerRetourUC - Contrôle utilisateur pour l'affichage de l'indicateur concernant le nombre d'aller-retour bandeau/image
    /// </summary>
    public partial class AllerRetourUC : UserControl
    {

        #region Attributs et propriétés

        /// <summary>
        /// Modèle associé au graphique
        /// </summary>
        private AllerRetourModel viewModel;
        public AllerRetourModel ViewModel
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
        /// Constructeur de la classe AllerRetourUC
        /// </summary>
        public AllerRetourUC()
        {
            ViewModel = new AllerRetourModel();
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
