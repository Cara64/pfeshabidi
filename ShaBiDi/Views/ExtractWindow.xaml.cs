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
                if (uc is TauxRecouvrementUC)
                {
                    cbSelectIndicateur.Items.Add((uc as TauxRecouvrementUC).ToString());
                }
                if (uc is DensiteRecouvrementUC)
                {
                    cbSelectIndicateur.Items.Add((uc as DensiteRecouvrementUC).ToString());
                }
            }      
        }


        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            int indexSelect = cbSelectIndicateur.SelectedIndex;
            string currentDir = tbSelectedPath.Text;
            StringBuilder csv = new StringBuilder();
            string title = "";
            string filePath = "";
            string delimiter = ";";

            IndicateurSelectionne = Indicateurs.ElementAt(indexSelect);

            if (IndicateurSelectionne is TauxRecouvrementUC)
            {
                TauxRecouvrementUC tr = IndicateurSelectionne as TauxRecouvrementUC;
                Dictionary<ShaBiDi.Logic.Image,double> dataTR = tr.ViewModel.Data;
                
                var mesuresTR = dataTR.Keys.OrderBy(o => o.Numero).ToList();
     
                title = tr.ViewModel.ToString();
                filePath = currentDir + "/" + title + "_OUTPUT.csv";
               
                csv.AppendLine(string.Join(delimiter, "Image", "Taux de recouvrement"));
                
                foreach (var key in mesuresTR)
                {
                    var id = key.Numero.ToString();
                    var value = dataTR[key].ToString();
                    var newLine = string.Join(delimiter, id, value);
                    csv.AppendLine(newLine);
                }

                System.IO.File.WriteAllText(filePath, csv.ToString());

                MessageBox.Show("Extraction output terminé");
            }

            if (IndicateurSelectionne is DensiteRecouvrementUC)
            {
                // TODO: Vérifier la lourdeur en mémoire

                DensiteRecouvrementUC dr = IndicateurSelectionne as DensiteRecouvrementUC;
                Dictionary<ShaBiDi.Logic.Image, double[,]> dataDR = dr.Data;

                var mesuresDR = dataDR.Keys.OrderBy(o => o.Numero).ToList();

                title = dr.ToString();
                filePath = currentDir + "/" + title + "_OUTPUT.csv";

                // Pour déterminer le bandeau de titre
                int sizeCsvTitle = ShaBiDi.Logic.Image.dimensionsImageLignes * ShaBiDi.Logic.Image.dimensionsImageCol + 1;
                string[] csvTitle = new string[sizeCsvTitle];
                csvTitle[0] = "Image";
                int lignes = 0;
                int col = 0;
                for (int i = 1; i < csvTitle.Length; i++)
                {
                    if (lignes > ShaBiDi.Logic.Image.dimensionsImageLignes) lignes = 0;
                    if (col > ShaBiDi.Logic.Image.dimensionsImageCol)
                    { 
                        col = 0;
                        lignes++;
                    }
                    csvTitle[i] = "[" + lignes + "," + col + "]";
                    col++;
                }

                csv.AppendLine(string.Join(delimiter, csvTitle));

                foreach (var key in mesuresDR)
                {
                    int k = 0;
                    string[] value = new string[sizeCsvTitle];
                    value[k] = key.Numero.ToString();
                    
                    for (int i = 0; i < dataDR[key].GetLength(0); i++)
                    {
                        for (int j = 0; j < dataDR[key].GetLength(1); j++)
                        {
                            k++;
                            value[k] = dataDR[key][i,j].ToString();
                        }
                    }

                    var newLine = string.Join(delimiter, value);
                    csv.AppendLine(newLine);

                    System.IO.File.WriteAllText(filePath, csv.ToString());

                    MessageBox.Show("Extraction output terminé");
                }

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
