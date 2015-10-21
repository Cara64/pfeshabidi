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
using Microsoft.Win32;

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour ImportWindow.xaml
    /// </summary>
    /// TODO : Implémenter affichage des infos fichiers (sélection, num groupe et ordre)
    public partial class ImportWindow : Window
    {
        private List<String> _importFiles;

        public ImportWindow()
        {
            InitializeComponent();

            _importFiles = new List<String>();
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {

            lbImportedFiles.ItemsSource = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Document texte|*.txt|Fichier CSV|*.csv";
            
            if (ofd.ShowDialog() == true)
            {         
                foreach(String file in ofd.FileNames)
                {
                    if (!_importFiles.Contains(file)) _importFiles.Add(file);
                }
            }

            lbImportedFiles.ItemsSource = _importFiles;            
      }

        private void btnDeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Implémenter suppression des fichiers
        }
    }
}
