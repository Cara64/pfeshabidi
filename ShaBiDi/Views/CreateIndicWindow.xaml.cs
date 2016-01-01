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
    /// Logique d'interaction pour CreateIndic.xaml
    /// </summary>
    public partial class CreateIndicWindow : Window
    { 
        public static List<int> Positions = new List<int>();
        public static List<OrdreGroupe> Ordres;
        public static List<Groupe> Groupes;
        public static bool ModS;
        public static bool ModPA;
      
        public CreateIndicWindow()
        {
            InitializeComponent();
            
            // Ajout de toutes les positions par défauts
            Positions.Add(1);
            Positions.Add(2);
            Positions.Add(3);

            Ordres = new List<OrdreGroupe>();
            ModS = false;
            ModPA = false;

            Groupes = AppData.GroupesExp;
            lbGroup.ItemsSource = Groupes;

            foreach (ListBoxItem item in lbGroup.Items)
            {
                item.IsSelected = true;
            }
            

        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem) cbSelectIndic.SelectedItem;
            string typeIndicateur = typeItem.Content.ToString();
            creerIndicateur(typeIndicateur);
        }



        private void creerIndicateur(string typeIndicateur)
        {
            ResultWindow res = new ResultWindow();

            switch (typeIndicateur)
            {
                case "Taux de recouvrement":
                    TauxRecouvrementUC tr = new TauxRecouvrementUC();
                    AppData.Indicateurs.Add(tr);
                    res.Title = tr.ViewModel.ToString();
                    res.Content = tr;
                    break;
                case "Densité de recouvrement (transparence)":
                    DensiteRecouvrementUC drTransparent = new DensiteRecouvrementUC("gris");
                    AppData.Indicateurs.Add(drTransparent);
                    res.Title = drTransparent.ToString();
                    res.Content = drTransparent;
                    break;
                case "Densité de recouvrement (couleur)":
                    DensiteRecouvrementUC drCouleur = new DensiteRecouvrementUC("couleur");
                    AppData.Indicateurs.Add(drCouleur);
                    res.Title = drCouleur.ToString();
                    res.Content = drCouleur;
                    break;
                case "Dispersion PA":
                    DispersionPAUC dispPA = new DispersionPAUC();
                    AppData.Indicateurs.Add(dispPA);
                    res.Title = dispPA.ToString();
                    res.Content = dispPA; 
                    break;
                case "Nombre d'allers retours bandeau / image":
                    AllerRetourUC ar = new AllerRetourUC();
                    AppData.Indicateurs.Add(ar);
                    res.Title = ar.ToString();
                    res.Content = ar;
                    break;
                default: break;
            }

            res.Show();
                   
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
