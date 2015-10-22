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
        public List<int> positions = new List<int>();
        public List<OrdreGroupe> ordres = new List<OrdreGroupe>();
        public bool modS = false;
        public bool modPA = false;

        public CreateIndicWindow()
        {
            InitializeComponent();
        }

        private void btnCreateIndic_Click(object sender, RoutedEventArgs e)
        {
           if (cbSelectIndic.Text.Equals("Taux de recouvrement"))
           {
               TauxRecouvrement_WindowForm tr = new TauxRecouvrement_WindowForm();
               tr.Show();
           }
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








    }
}
