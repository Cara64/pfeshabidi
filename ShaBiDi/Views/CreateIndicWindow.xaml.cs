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
using System.Windows.Shapes;
using ShaBiDi.Logic;
using System.Threading;
using System.ComponentModel;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour CreateIndic.xaml
    /// CreateIndicWindow - Fenêtre pour la création d'indicateur
    /// TODO: Corriger pb de highlight de la sélection des groupes
    /// </summary>
    public partial class CreateIndicWindow : Window
    {

        #region Attributs

        /// <summary>
        /// Indicateur sélectionné 
        /// </summary>
        private string typeIndicateur;

        /// <summary>
        /// Liste contenant les positions des joueurs filtrées
        /// </summary>
        private List<int> positions = new List<int>();
        /// <summary>
        /// Liste contenant les ordres de modalité filtrées
        /// </summary>
        private List<OrdreGroupe> ordres;
        /// <summary>
        /// Liste contenant les groupes filtrés
        /// </summary>
        private List<Groupe> groupes;
        /// <summary>
        /// Booléen pour le filtrage des groupes en modalité sans point d'attention
        /// </summary>
        private bool modS;
        /// <summary>
        /// Booléen pour le filtrage des groupes en modalité avec point d'attention
        /// </summary>
        private bool modPA;


        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la  classe CreateIndicWindow
        /// </summary>
        public CreateIndicWindow()
        {
            InitializeComponent();
            
            // Ajout de toutes les positions par défauts
            positions.Add(1);
            positions.Add(2);
            positions.Add(3);

            // Les ordres et les modalités sont initialisés à 0
            ordres = new List<OrdreGroupe>();
            modS = false;
            modPA = false;

            // Sélection de l'ensemble des groupes
            groupes = AppData.GroupesExp;
            lbGroup.ItemsSource = groupes;
            lbGroup.SelectAll();

            typeIndicateur = "Taux de recouvrement";
        }

        #endregion


        #region Méthodes indicateurs

        /// <summary>
        /// Méthode qui permet de créer l'indicateur et l'ajouter en fonction du type d'indicateur
        /// </summary>
        private void creerIndicateur()
        {

            switch (typeIndicateur)
            {
                case "Taux de recouvrement":
                    IndicateurTauxRecouvrement tauxRecouvrement = new IndicateurTauxRecouvrement(positions, ordres, modPA, modS, groupes);
                    AppData.IndicateursTauxRecouvrement.Add(tauxRecouvrement);
                    AppData.IndicateursTauxRecouvrement.Last().determineTaux();
                    break;
                case "Densité de recouvrement":
                    IndicateurDensiteRecouvrement densiteRecouvrement = new IndicateurDensiteRecouvrement(positions, ordres, modPA, modS, groupes);
                    AppData.IndicateursDensiteRecouvrement.Add(densiteRecouvrement);
                    AppData.IndicateursDensiteRecouvrement.Last().determineDensite();
                    break;
                case "Dispersion PA":
                    IndicateurDispersionPA dispersionPA = new IndicateurDispersionPA(positions, ordres, modPA, modS, groupes);
                    AppData.IndicateursDispersionPA.Add(dispersionPA);
                    AppData.IndicateursDispersionPA.Last().determineDispersion();
                    break;
                case "Nombre d'allers retours bandeau / image":
                    IndicateurAllerRetour allerRetour = new IndicateurAllerRetour(positions, ordres, modPA, modS, groupes);
                    AppData.IndicateursAllerRetour.Add(allerRetour);
                    AppData.IndicateursAllerRetour.Last().determineAllerRetour();
                    break;
                default: break;
            }
             
        }

        #endregion


        #region Gestion des éléments d'interface

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
            creerIndicateur();
        }

        private void cbUser1_Checked(object sender, RoutedEventArgs e)
        {
            positions.Add(1);
        }

        private void cbUser1_Unchecked(object sender, RoutedEventArgs e)
        {
            positions.Remove(1);
        }

        private void cbUser2_Checked(object sender, RoutedEventArgs e)
        {
            positions.Add(2);
        }

        private void cbUser2_Unchecked(object sender, RoutedEventArgs e)
        {
            positions.Remove(2);
        }

        private void cbUser3_Checked(object sender, RoutedEventArgs e)
        {
            positions.Add(3);
        }

        private void cbUser3_Unchecked(object sender, RoutedEventArgs e)
        {
            positions.Remove(3);
        }

        private void cbSPA_Unchecked(object sender, RoutedEventArgs e)
        {
            ordres.Remove(OrdreGroupe.SPA);
        }

        private void cbSPA_Checked(object sender, RoutedEventArgs e)
        {
            ordres.Add(OrdreGroupe.SPA);
        }

        private void cbPAS_Checked(object sender, RoutedEventArgs e)
        {
            ordres.Add(OrdreGroupe.PAS);
        }

        private void cbPAS_Unchecked(object sender, RoutedEventArgs e)
        {
            ordres.Remove(OrdreGroupe.PAS);
        }

        private void cbS_Checked(object sender, RoutedEventArgs e)
        {
            modS = true;
        }

        private void cbS_Unchecked(object sender, RoutedEventArgs e)
        {
            modS = false;
        }

        private void cbPA_Checked(object sender, RoutedEventArgs e)
        {
            modPA = true;
        }

        private void cbPA_Unchecked(object sender, RoutedEventArgs e)
        {
            modPA = false;
        }

        private void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            groupes.Clear();
            foreach (Groupe groupe in lbGroup.SelectedItems)
            {
                groupes.Add(groupe);
            }
        }

        private void cbSelectIndic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeIndicateurItem = (sender as ComboBox).SelectedItem as ComboBoxItem;
            typeIndicateur = typeIndicateurItem.Content as string;
        }

        /// <summary>
        /// Méthode déclenchée lors de la fermeture de fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        #endregion

    }
}
