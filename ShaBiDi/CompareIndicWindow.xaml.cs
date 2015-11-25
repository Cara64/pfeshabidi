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
    /// 
    public partial class CompareIndicWindow : Window
    {

        public static List<UserControl> Indicateurs;
        public static List<String> nomIndicateurs = new List<String>();
        public static TypeComp typeComp;
        public static string[] indicateurSelected;

        public CompareIndicWindow()
        {
            InitializeComponent();
            cbSelectIndic1.ItemsSource = nomIndicateurs;
            cbSelectIndic2.ItemsSource = nomIndicateurs;
            typeComp = new TypeComp();
            indicateurSelected = new string[2];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            indicateurSelected[0] = cbSelectIndic1.SelectedItem as string;
            indicateurSelected[1] = cbSelectIndic2.SelectedItem as string;
            typeComp = convert(cbSelectModeComp.SelectedItem as string);
            Console.WriteLine(nomIndicateurs[0]);
            CompTauxRecouvrement comp = new CompTauxRecouvrement();
            ResComparaison res = new ResComparaison();
            res.Content = comp;
            res.Show();
        }

        private TypeComp convert(string strTypeComp)
        {
            TypeComp tmpTypeComp = new TypeComp();
            switch (strTypeComp)
            {
                case "Addition": tmpTypeComp = TypeComp.add;
                    break;
                case "Soustraction": tmpTypeComp = TypeComp.sous;
                    break;
                case "Moyenne": tmpTypeComp = TypeComp.moy;
                    break;
                default: break;
            }

            return tmpTypeComp;
        }
    }
}
