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

        private List<UserControl> indicateurs;
        public List<UserControl> Indicateurs
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
        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
            creerIndicateur(cbSelectIndic.SelectedItem.ToString());
        }


        private void creerIndicateur(string typeIndicateur)
        {
            switch (typeIndicateur)
            {
                case "Taux de recouvrement":
                    indicateurs.Add(new TauxRecouvrement());
                    break;
                default: break;
            }
        }

        private void gestionGrille()
        {

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
