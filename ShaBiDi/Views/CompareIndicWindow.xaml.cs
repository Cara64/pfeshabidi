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
    /// Logique d'interaction pour CompareIndicWindow.xaml
    /// </summary>

    public partial class CompareIndicWindow : Window
    {
        public List<UserControl> Indicateurs;
        
        public static List<UserControl> IndicateursSelectionnes;
        public static TypeComp TypeComparaison;

        public CompareIndicWindow()
        {
            InitializeComponent();
            Indicateurs = new List<UserControl>();
            IndicateursSelectionnes = new List<UserControl>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (UserControl uc in MainWindow.Indicateurs)
            {
                Indicateurs.Add(uc);

                if (uc is TauxRecouvrementUC)
                {
                    cbSelectIndic1.Items.Add((uc as TauxRecouvrementUC).ToString());
                    cbSelectIndic2.Items.Add((uc as TauxRecouvrementUC).ToString());
                }

                if (uc is DensiteRecouvrementUC)
                {
                    cbSelectIndic1.Items.Add((uc as DensiteRecouvrementUC).ToString());
                    cbSelectIndic2.Items.Add((uc as DensiteRecouvrementUC).ToString());
                }

                if (uc is DispersionPAUC)
                {
                    cbSelectIndic1.Items.Add((uc as DispersionPAUC).ToString());
                    cbSelectIndic2.Items.Add((uc as DispersionPAUC).ToString());
                }

                if (uc is AllerRetourUC)
                {
                    cbSelectIndic1.Items.Add((uc as AllerRetourUC).ToString());
                    cbSelectIndic2.Items.Add((uc as AllerRetourUC).ToString());
                }


            }
        }

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            IndicateursSelectionnes.Clear();

            int indexSelec1 = cbSelectIndic1.SelectedIndex;
            int indexSelec2 = cbSelectIndic2.SelectedIndex;
            
            IndicateursSelectionnes.Add(Indicateurs.ElementAt(indexSelec1));
            IndicateursSelectionnes.Add(Indicateurs.ElementAt(indexSelec2));

            if (IndicateursSelectionnes[0].GetType().Equals(IndicateursSelectionnes[1].GetType()))
            {
                TypeComparaison = convert(cbSelectModeComp.SelectedValue.ToString());
                ResultWindow res = new ResultWindow();

                if (IndicateursSelectionnes[0].GetType().Equals(typeof(TauxRecouvrementUC)))
                {
                    CompTauxRecouvrementUC compTR = new CompTauxRecouvrementUC();
                    MainWindow.Comparateurs.Add(compTR);
                    res.Content = compTR;
                }

                if (IndicateursSelectionnes[0].GetType().Equals(typeof(DensiteRecouvrementUC)))
                {
                    DensiteRecouvrementUC indDR1 = IndicateursSelectionnes[0] as DensiteRecouvrementUC;
                    DensiteRecouvrementUC indDR2 = IndicateursSelectionnes[1] as DensiteRecouvrementUC;
                    if (indDR1.Mode.Equals(indDR2.Mode))
                    {
                        CompDensiteRecouvrementUC compDR = new CompDensiteRecouvrementUC(indDR1.Mode);
                        MainWindow.Comparateurs.Add(compDR);
                        res.Content = compDR;
                    }
                    else
                    {
                        MessageBox.Show("Vous devez comparer deux indicateurs du même type", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    res.Show();
                }

                if (IndicateursSelectionnes[0].GetType().Equals(typeof(DispersionPAUC)))
                {
                    DispersionPAUC indDispPA1 = IndicateursSelectionnes[0] as DispersionPAUC;
                    DispersionPAUC indDispPA2 = IndicateursSelectionnes[1] as DispersionPAUC;

                    CompDispersionPAUC compDispPA = new CompDispersionPAUC();
                    MainWindow.Comparateurs.Add(compDispPA);
                    res.Content = compDispPA;
                }

                if (IndicateursSelectionnes[0].GetType().Equals(typeof(AllerRetourUC)))
                {
                    AllerRetourUC indAR1 = IndicateursSelectionnes[0] as AllerRetourUC;
                    AllerRetourUC indAR2 = IndicateursSelectionnes[1] as AllerRetourUC;

                    CompAllerRetourUC compAR = new CompAllerRetourUC();
                    MainWindow.Comparateurs.Add(compAR);
                    res.Content = compAR;
                }

            } 
            else
            {
                MessageBox.Show("Vous devez comparer deux indicateurs de même type", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
   

        }

        private TypeComp convert(string s)
        {
            TypeComp res;
            switch (s)
            {
                case "Addition": res = TypeComp.add; break;
                case "Soustraction": res = TypeComp.sous; break;
                case "Moyenne": res = TypeComp.moy; break;
                default: throw new Exception(); break;
            }
            return res;
        }
    }
}
