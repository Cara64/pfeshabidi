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
        public static List<int> Positions = new List<int>();
        public static List<OrdreGroupe> Ordres = new List<OrdreGroupe>();
        public static bool ModS = false;
        public static bool ModPA = false;
    

        public CreateIndicWindow()
        {
            InitializeComponent();
        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
           if (cbSelectIndic.Text.Equals("Taux de recouvrement"))
           {
               TauxRecouvrement tr = new TauxRecouvrement();
               Grid gr = new Grid();
               gr.Children.Add(tr);
               MainWindow.SelectedTab.Content = gr;
           }
        }

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

    }
}
