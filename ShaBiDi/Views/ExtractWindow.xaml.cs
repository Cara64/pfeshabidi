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
    /// Logique d'interaction pour ExtractWindow.xaml
    /// ExtractWindow - Fenêtre pour la réalisation d'extraction de données
    /// </summary>
    public partial class ExtractWindow : Window
    {

        #region Attributs

        /// <summary>
        /// Répertoire courant dans lequel on génère le CSV
        /// </summary>
        private string currentDir;
        /// <summary>
        /// Indicateur sélectionné par l'utilisateur
        /// </summary>
        private Indicateur indicSelectionne;

        #endregion


        #region Constructeur

        public ExtractWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region Méthodes de gestion des événements

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbSelectIndicateur.ItemsSource = null;
            cbSelectIndicateur.ItemsSource = AppData.genererListeIndicateurs();
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentDir = tbSelectedPath.Text;
                indicSelectionne = cbSelectIndicateur.SelectedItem as Indicateur;

                if (indicSelectionne is IndicateurTauxRecouvrement)
                {
                    (indicSelectionne as IndicateurTauxRecouvrement).extractOutputTauxToCSV(currentDir);
                }
                else if (indicSelectionne is IndicateurDensiteRecouvrement)
                {
                    (indicSelectionne as IndicateurDensiteRecouvrement).extractOutputDensiteRecouvrementToCSV(currentDir);
                }
                else if (indicSelectionne is IndicateurDispersionPA)
                {
                    (indicSelectionne as IndicateurDispersionPA).extractOutputDispersionToCSV(currentDir);
                }
                else if (indicSelectionne is IndicateurAllerRetour)
                {
                    (indicSelectionne as IndicateurAllerRetour).extractOutputAllerRetourToCSV(currentDir);
                }

                MessageBox.Show("L'extraction a été réalisée avec succès pour l'indicateur " + indicSelectionne.Title + " dans le répertoire " + currentDir, "Extraction terminée", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
                   
        private void btnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            tbSelectedPath.Text = fbd.SelectedPath;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
