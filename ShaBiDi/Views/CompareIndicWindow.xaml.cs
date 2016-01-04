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

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour CompareIndicWindow.xaml
    /// CompareIndicWindow - Fenêtre pour créer une comparaison entre les indicateurs
    /// TODO: Rafraichir la liste à la création d'un nouvel indicateur
    /// </summary>
    public partial class CompareIndicWindow : Window
    {

        #region Attributs

        /// <summary>
        /// Type de comparaison sélectionné
        /// </summary>
        private TypeComp typeComparaison;

        #endregion


        #region Constructeur

        public CompareIndicWindow()
        {
            InitializeComponent();

            typeComparaison = TypeComp.Add;
            cbSelectModeComp.Items.Add("Addition");
            cbSelectModeComp.Items.Add("Soustraction");
            cbSelectModeComp.Items.Add("Moyenne");
            cbSelectModeComp.SelectedIndex = 0;
        }

        #endregion


        #region Méthodes événementielles

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Génération de la liste d'indicateur
            cbSelectIndic1.ItemsSource = null;
            cbSelectIndic1.ItemsSource = AppData.genererListeIndicateurs();
        }

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            Indicateur indSel1 = cbSelectIndic1.SelectedItem as Indicateur;
            Indicateur indSel2 = cbSelectIndic2.SelectedItem as Indicateur;
            typeComparaison = AppData.convertStringToTypeComp(cbSelectModeComp.SelectedItem as string);

            /* Méthodologie de la création de comparaison :
             * On cast les deux indicateurs vers le type d'indicateur voulu
             * On définit l'indicateur comparé au second indicateur pour le premier indicateur
             * On appelle la méthode de comparaison du premier indicateur
             * On ajoute l'indicateur 1 à la liste des indicateurs comparateurs
             * On génère l'UC de l'indicateur comparateur
             */

            if (indSel1 != null && indSel2 != null)
            {
                if (indSel1 is IndicateurTauxRecouvrement)  // Taux recouvrement
                {
                    Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();
                    IndicateurTauxRecouvrement indTR1 = indSel1 as IndicateurTauxRecouvrement;
                    IndicateurTauxRecouvrement indTR2 = indSel2 as IndicateurTauxRecouvrement;

                    indTR1.IndicCompare = indTR2;
                    indTR1.compareTaux(typeComparaison);
                    AppData.ComparateursTauxRecouvrement.Add(indTR1);
                    CompTauxRecouvrementUC compTauxRecouvrement = new CompTauxRecouvrementUC();

                }
                else if (indSel1 is IndicateurDensiteRecouvrement)      // Densité de recouvrement
                {
                    Dictionary<ImageExp, double[,]> dicoCompare = new Dictionary<ImageExp, double[,]>();
                    IndicateurDensiteRecouvrement indTR1 = indSel1 as IndicateurDensiteRecouvrement;
                    IndicateurDensiteRecouvrement indTR2 = indSel2 as IndicateurDensiteRecouvrement;

                    indTR1.IndicCompare = indTR2;
                    indTR1.compareDensite(typeComparaison);
                    AppData.ComparateursDensiteRecouvrement.Add(indTR1);
                    CompDensiteRecouvrementUC compDispersionPA = new CompDensiteRecouvrementUC("gris");
                }
                else if (indSel1 is IndicateurDispersionPA)             // Dispersion PA
                {
                    Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();
                    IndicateurDispersionPA indTR1 = indSel1 as IndicateurDispersionPA;
                    IndicateurDispersionPA indTR2 = indSel2 as IndicateurDispersionPA;

                    indTR1.IndicCompare = indTR2;
                    indTR1.compareDispersion(typeComparaison);
                    AppData.ComparateursDispersionPA.Add(indTR1);
                    CompDispersionPAUC compDispersionPA = new CompDispersionPAUC();
                }

                else if (indSel1 is IndicateurAllerRetour)          // Aller Retour
                {
                    Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();
                    IndicateurAllerRetour indTR1 = indSel1 as IndicateurAllerRetour;
                    IndicateurAllerRetour indTR2 = indSel2 as IndicateurAllerRetour;

                    indTR1.IndicCompare = indTR2;
                    indTR1.compareAllerRetour(typeComparaison);
                    AppData.ComparateursAllerRetour.Add(indTR1);
                    CompAllerRetourUC compDispersionPA = new CompAllerRetourUC();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner correctement les indicateurs", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void cbSelectIndic1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbSelectIndic2.ItemsSource = null;
            cbSelectIndic2.ItemsSource = AppData.genererListeIndicateursDetermines(e.AddedItems[0] as Indicateur); 
        }
        
        #endregion



    }
}
