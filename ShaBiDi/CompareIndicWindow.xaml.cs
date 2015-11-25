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
    /// Logique d'interaction pour CompareIndicWindow.xaml
    /// </summary>

    public partial class CompareIndicWindow : Window
    {

        public static List<UserControl> Indicateurs;

        public static List<String> nomIndicateurs = new List<String>();

        public static bool compAdd;
        public static bool compSous;
        public static bool compMoy;

        public static string[] indicateurSelected;

        public CompareIndicWindow()
        {
            InitializeComponent();
            cbSelectIndic1.ItemsSource = nomIndicateurs;
            cbSelectIndic2.ItemsSource = nomIndicateurs;
            compAdd = compSous = compMoy = false;
            indicateurSelected = new string[2];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void cbSous_Checked(object sender, RoutedEventArgs e)
        {
            compSous = true;
        }

        private void cbAdd_Checked(object sender, RoutedEventArgs e)
        {
            compAdd = true;
        }

        private void cbMoy_Checked(object sender, RoutedEventArgs e)
        {
            compMoy = true;
        }

        private void cbAdd_Unchecked(object sender, RoutedEventArgs e)
        {
            compAdd = false;
        }

        private void cbSous_Unchecked(object sender, RoutedEventArgs e)
        {
            compSous = false;
        }

        private void cbMoy_Unchecked(object sender, RoutedEventArgs e)
        {
            compMoy = false;
        }

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            indicateurSelected[0] = cbSelectIndic1.SelectedItem as string;
            indicateurSelected[1] = cbSelectIndic2.SelectedItem as string;

            Console.WriteLine(nomIndicateurs[0]);
            CompTauxRecouvrement comp = new CompTauxRecouvrement();
            ResComparaison res = new ResComparaison();
            res.Content = comp;
            res.Show();
        }
    }
}
