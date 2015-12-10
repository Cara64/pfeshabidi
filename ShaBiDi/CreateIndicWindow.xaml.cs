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
        // TODO : Améliorer l'affichage des ancrages
        // TODO : Augmenter le nombre d'indicateurs visibles
        // Affichage maximum de 4 indicateurs pour des raisons de visibilité
        // private const int MAX_INDIC_IN_TAB = 4;

        public static List<int> Positions = new List<int>();
        public static List<OrdreGroupe> Ordres;
        public static List<Groupe> Groupes;
        public static bool ModS;
        public static bool ModPA;
      
        public CreateIndicWindow()
        {
            InitializeComponent();
    
            Positions.Add(1);
            Positions.Add(2);
            Positions.Add(3);

            Ordres = new List<OrdreGroupe>();
            ModS = false;
            ModPA = false;

            Groupes = ImportWindow.GroupesExp;
            lbGroup.ItemsSource = Groupes;
            lbGroup.SelectAll();
           
        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem) cbSelectIndic.SelectedItem;
            string typeIndicateur = typeItem.Content.ToString();
            creerIndicateur(typeIndicateur);
            gererGrille();
        }


        private void creerIndicateur(string typeIndicateur)
        {
            int indexTab = MainWindow.TabItems.IndexOf(MainWindow.SelectedTab);
            switch (typeIndicateur)
            {
                case "Taux de recouvrement":
                    MainWindow.Indicateurs[indexTab].Add(new TauxRecouvrement());
                    break;
                default: break;
            }
        }

        private void gererGrille()
        {
            int indexTab = MainWindow.TabItems.IndexOf(MainWindow.SelectedTab); // index du tab courant
            int ucLength = MainWindow.Indicateurs[indexTab].Count;  // nombre d'UC dans le tab courant

            MainWindow.Grids[indexTab].Children.Clear();
            Console.WriteLine("ucLength = " + ucLength);
            
            MainWindow.Grids[indexTab].ColumnDefinitions.Add(new ColumnDefinition());
            MainWindow.Grids[indexTab].RowDefinitions.Add(new RowDefinition());
            if (ucLength == 2) MainWindow.Grids[indexTab].ColumnDefinitions.Add(new ColumnDefinition());
            if (ucLength == 3) MainWindow.Grids[indexTab].RowDefinitions.Add(new RowDefinition());

            switch (ucLength)
            {
                case 4 :
                    Grid.SetRow(MainWindow.Indicateurs[indexTab][3], 1);
                    Grid.SetColumn(MainWindow.Indicateurs[indexTab][3], 1);
                    goto case 3;
                case 3 :
                    Grid.SetRow(MainWindow.Indicateurs[indexTab][2], 1);
                    // Grid.SetColumn(MainWindow.Indicateurs[indexTab][2], 0);
                    Grid.SetColumnSpan(MainWindow.Indicateurs[indexTab][2], 2);
                    goto case 2;
                case 2 :
                    Grid.SetRow(MainWindow.Indicateurs[indexTab][1], 0);
                    Grid.SetColumn(MainWindow.Indicateurs[indexTab][1], 1);
                    goto case 1;
                case 1 :
                    Grid.SetRow(MainWindow.Indicateurs[indexTab][0], 0);
                    Grid.SetColumn(MainWindow.Indicateurs[indexTab][0], 0);
                    break;
                default: break;
            }

            foreach (UserControl uc in MainWindow.Indicateurs[indexTab])
                MainWindow.Grids[indexTab].Children.Add(uc);

            MainWindow.SelectedTab.Content = MainWindow.Grids[indexTab];     
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

        private void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Groupes.Clear();
            foreach (Groupe groupe in lbGroup.SelectedItems)
            {
                Groupes.Add(groupe);
            }
        }

    }
}
