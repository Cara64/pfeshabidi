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

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour ExtractionDonnees.xaml
    /// </summary>
    public partial class ExtractWindow : Window
    {
        public List<UserControl> Indicateurs;

        public static UserControl IndicateurSelectionne;
        public ExtractWindow()
        {
            InitializeComponent();
            Indicateurs = new List<UserControl>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {    
            foreach (UserControl uc in MainWindow.Indicateurs)
            {
                Indicateurs.Add(uc);
                if (uc is TauxRecouvrement)
                {
                    cbSelectIndicateur.Items.Add((uc as TauxRecouvrement).ToString());
                }
            }      
        }


        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            int indexSelect = cbSelectIndicateur.SelectedIndex;

            IndicateurSelectionne = Indicateurs.ElementAt(indexSelect);

            if (IndicateurSelectionne is TauxRecouvrement)
            {
                TauxRecouvrement tr = IndicateurSelectionne as TauxRecouvrement;
                Dictionary<ShaBiDi.Logic.Image,double> data = tr.ViewModel.Data;
                
                var mesures = data.Keys.OrderBy(o => o.Numero).ToList();
                var csv = new StringBuilder();

                string filePath = System.IO.Directory.GetCurrentDirectory() + "/testoutput.csv";
                string delimiter = ";";

                csv.AppendLine(string.Join(delimiter, "Image", "Taux de recouvrement"));
                
                foreach (var key in mesures)
                {
                    var id = key.Numero.ToString();
                    var value = data[key].ToString();
                    var newLine = string.Join(delimiter, id, value);
                    csv.AppendLine(newLine);
                }

                System.IO.File.WriteAllText(filePath, csv.ToString());

                MessageBox.Show("Extraction output terminé");

            }
        }

        private void btnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            tbSelectedPath.Text = fbd.SelectedPath;       
        }
    }
}
