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
using ShaBiDi.ViewModels;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour AllerRetourUC.xaml
    /// </summary>
    public partial class AllerRetourUC : UserControl
    {
        private AllerRetourModel viewModel;

        public AllerRetourModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }
        public AllerRetourUC()
        {
            ViewModel = new AllerRetourModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public override string ToString()
        {
            return ViewModel.ToString();
        }
    }
}
