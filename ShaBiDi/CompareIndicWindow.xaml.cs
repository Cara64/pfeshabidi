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
        // TODO : Corriger bug affichage fenêtre
        public List<UserControl> Indicateurs;
        
        public static List<UserControl> IndicateursSelectionnes;
        public static TypeComp TypeComparaison;

        public CompareIndicWindow()
        {
            InitializeComponent();
            Indicateurs = new List<UserControl>();  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CountUC = " + MainWindow.Indicateurs.Count());
            foreach (List<UserControl> listUC in MainWindow.Indicateurs)
            {
                Console.WriteLine(listUC.Count());
                foreach (UserControl uc in listUC)
                {
                    Console.WriteLine(uc.GetType());
                    if (uc is TauxRecouvrement)
                    {
                        cbSelectIndic1.Items.Add(uc as TauxRecouvrement);
                        cbSelectIndic2.Items.Add(uc as TauxRecouvrement);
                    }
                }
            }
        }

        

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            IndicateursSelectionnes.Add(cbSelectIndic1.SelectedItem as UserControl);
            IndicateursSelectionnes.Add(cbSelectIndic2.SelectedItem as UserControl);
            TypeComparaison = convert(cbSelectModeComp.SelectedItem as string);

            CompTauxRecouvrement comp = new CompTauxRecouvrement();
            ResComparaison res = new ResComparaison();
            res.Content = comp;
            res.Show();
        }

        private TypeComp convert(string s)
        {
            switch (s)
            {
                case "Addition" : return TypeComp.add;
                case "Soustraction" : return TypeComp.sous;
                case "Moyenne": return TypeComp.moy;
                default: throw new Exception();
            }
        }
    }
}
