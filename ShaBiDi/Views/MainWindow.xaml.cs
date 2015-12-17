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
    /// </summary>

    public partial class MainWindow : Window
    {

        public static List<UserControl> Indicateurs;
        public static List<UserControl> Comparateurs;

        public ImportWindow import;
        public CreateIndicWindow createIndic;
        public CompareIndicWindow compareIndic;
        public ExtractWindow extract;


        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Indicateurs = new List<UserControl>();

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

            // TODO: Corriger bug d'affichage
            compareIndic.Show();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            extract.Show();
        }

        #endregion


    }

    
}
