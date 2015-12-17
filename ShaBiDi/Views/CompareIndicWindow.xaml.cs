﻿using System;
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
                if (uc is TauxRecouvrement)
                {
                    cbSelectIndic1.Items.Add((uc as TauxRecouvrement).ToString());
                    cbSelectIndic2.Items.Add((uc as TauxRecouvrement).ToString());
                }
            }
        }

        private void btnCreateCompareIndic_Click(object sender, RoutedEventArgs e)
        {
            int indexSelec1 = cbSelectIndic1.SelectedIndex;
            int indexSelec2 = cbSelectIndic2.SelectedIndex;
            
            IndicateursSelectionnes.Add(Indicateurs.ElementAt(indexSelec1));
            IndicateursSelectionnes.Add(Indicateurs.ElementAt(indexSelec2));
            Console.WriteLine(cbSelectModeComp.SelectedItem);
            TypeComparaison = convert(cbSelectModeComp.SelectedValue.ToString());

            CompTauxRecouvrement comp = new CompTauxRecouvrement();
            ResultWindow res = new ResultWindow();
            res.Content = comp;
            res.Show();
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