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
    /// Logique d'interaction pour DispersionUC.xaml
    /// </summary>
    public partial class DispersionPAUC : UserControl
    {
        private DispersionPAModel viewModel;

        public DispersionPAModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }
        public DispersionPAUC()
        {
            ViewModel = new DispersionPAModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public override string ToString()
        {
            return ViewModel.ToString();
        }
    }
}
