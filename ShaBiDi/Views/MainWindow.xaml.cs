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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// MainWindow - Ecran principal de l'application
    /// </summary>

    public partial class MainWindow : Window
    {

        private ImportWindow import;
        private CreateIndicWindow createIndic;
        private CompareIndicWindow compareIndic;
        private ExtractWindow extract;


        public MainWindow()
        {
            try
            {
                InitializeComponent();
               
                import = new ImportWindow();
                createIndic = new CreateIndicWindow();
                compareIndic = new CompareIndicWindow();
                extract = new ExtractWindow();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        


        #region Bouton menu principal

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            createIndic.Show();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            import.Show();
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            compareIndic.Show();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            extract.Show();
        }

        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cette fonctionnalité n'est pas disponible");
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cette fonctionnalité n'est pas disponible");
        }


    }

    
}
