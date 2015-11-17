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

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour CreateIndic.xaml
    /// </summary>
    public partial class CreateIndicWindow : Window
    {
        // Affichage maximum de 4 indicateurs pour des raisons de visibilité
        private const int MAX_INDIC_IN_TAB = 4;

        public static List<int> Positions;
        public static List<OrdreGroupe> Ordres;
        public static List<Groupe> Groupes;
        public static bool ModS;
        public static bool ModPA;

        public static Grid gr;

        private static List<UserControl> indicateurs;
        public static List<UserControl> Indicateurs
        {
            get { return indicateurs; }
            set { indicateurs = value; }
        }

      
        public CreateIndicWindow()
        {
            InitializeComponent();
            Positions = new List<int>();
            Ordres = new List<OrdreGroupe>();
            ModS = false;
            ModPA = false;
            Indicateurs = new List<UserControl>(MAX_INDIC_IN_TAB);
            gr = new Grid();
        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem) cbSelectIndic.SelectedItem;
            string typeIndicateur = typeItem.Content.ToString();

            if (Indicateurs.Count < MAX_INDIC_IN_TAB)
            {
                creerIndicateur(typeIndicateur);
                Console.WriteLine("Il y a {0} indicateurs", Indicateurs.Count);
                gererGrille();
            } 
            else 
            {
                MessageBox.Show("Le nombre d'indicateur est limité à 4 pour un onglet !", "Nombre d'indicateurs trop grand", MessageBoxButton.OK
                    , MessageBoxImage.Exclamation);
            }
        }


        private void creerIndicateur(string typeIndicateur)
        {
            switch (typeIndicateur)
            {
                case "Taux de recouvrement":
                    Indicateurs.Add(new TauxRecouvrement());
                    Console.WriteLine("Ajout du taux de recouvrement");
                    break;
                default: break;
            }
        }

        private void gererGrille()
        {

            gr.Children.Clear();

            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();
            
            switch (Indicateurs.Count)
            {
                case 1 : 
                    gr.ColumnDefinitions.Add(colDef1);
                    gr.RowDefinitions.Add(rowDef1);
                    Grid.SetRow(Indicateurs[0], 0);
                    Grid.SetColumn(Indicateurs[0], 0);
                    Console.WriteLine("Mise en place de la grille pour un indicateur");
                    break;
                case 2 :
                    gr.ColumnDefinitions.Add(colDef1);
                    gr.ColumnDefinitions.Add(colDef2);
                    gr.RowDefinitions.Add(rowDef1);
                    Grid.SetRow(Indicateurs[0], 0);
                    Grid.SetColumn(Indicateurs[0], 0);
                    Grid.SetRow(Indicateurs[1], 0);
                    Grid.SetColumn(Indicateurs[1], 1);
                    Console.WriteLine("Mise en place de la grille pour deux indicateurs");
                    break;
                case 3 :
                    gr.ColumnDefinitions.Add(colDef1);
                    gr.ColumnDefinitions.Add(colDef2);
                    gr.RowDefinitions.Add(rowDef1);
                    gr.RowDefinitions.Add(rowDef2);
                    Grid.SetRow(Indicateurs[0], 0);
                    Grid.SetColumn(Indicateurs[0], 0);
                    Grid.SetRow(Indicateurs[1], 0);
                    Grid.SetColumn(Indicateurs[1], 1);
                    Grid.SetRow(Indicateurs[2], 1);
                    break;
                case 4 :
                    gr.ColumnDefinitions.Add(colDef1);
                    gr.ColumnDefinitions.Add(colDef2);
                    gr.RowDefinitions.Add(rowDef1);
                    gr.RowDefinitions.Add(rowDef2);
                    Grid.SetRow(Indicateurs[0], 0);
                    Grid.SetColumn(Indicateurs[0], 0);
                    Grid.SetRow(Indicateurs[1], 0);
                    Grid.SetColumn(Indicateurs[1], 1);
                    Grid.SetRow(Indicateurs[2], 1);
                    Grid.SetColumn(Indicateurs[2], 0);
                    Grid.SetRow(Indicateurs[3], 1);
                    Grid.SetColumn(Indicateurs[3], 1);
                    break;

                default: break;
            }

            
            for (int i = 0; i < Indicateurs.Count; i++ )
            {
                gr.Children.Add(Indicateurs[i]);
                Console.WriteLine("Ajout de l'indicateur {0}", i);
            }

            MainWindow.SelectedTab.Content = gr;
            Console.WriteLine("Mise en place dans l'onglet");
        }


        #region Gestion des éléments d'interface

        private void cbUser1_Checked(object sender, RoutedEventArgs e)
        {
            Positions.Add(1);
        }

        private void cbUser1_Unchecked(object sender, RoutedEventArgs e)
        {
            Positions.Remove(1);
        }

        private void cbUser2_Checked(object sender, RoutedEventArgs e)
        {
            Positions.Add(2);
        }

        private void cbUser2_Unchecked(object sender, RoutedEventArgs e)
        {
            Positions.Remove(2);
        }

        private void cbUser3_Checked(object sender, RoutedEventArgs e)
        {
            Positions.Add(3);
        }

        private void cbUser3_Unchecked(object sender, RoutedEventArgs e)
        {
            Positions.Remove(3);
        }

        private void cbSPA_Unchecked(object sender, RoutedEventArgs e)
        {
            Ordres.Remove(OrdreGroupe.SPA);
        }

        private void cbSPA_Checked(object sender, RoutedEventArgs e)
        {
            Ordres.Add(OrdreGroupe.SPA);
        }

        private void cbPAS_Checked(object sender, RoutedEventArgs e)
        {
            Ordres.Add(OrdreGroupe.PAS);
        }

        private void cbPAS_Unchecked(object sender, RoutedEventArgs e)
        {
            Ordres.Remove(OrdreGroupe.PAS);
        }

        private void cbS_Checked(object sender, RoutedEventArgs e)
        {
            ModS = true;
        }

        private void cbS_Unchecked(object sender, RoutedEventArgs e)
        {
            ModS = false;
        }

        private void cbPA_Checked(object sender, RoutedEventArgs e)
        {
            ModPA = true;
        }

        private void cbPA_Unchecked(object sender, RoutedEventArgs e)
        {
            ModPA = false;
        }

        #endregion

    }
}
