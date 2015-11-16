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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour TauxRecouvrement.xaml
    /// </summary>
    public partial class TauxRecouvrement : UserControl
    {
        private ViewModels.TauxRecouvrementModel viewModel;

        public TauxRecouvrement()
        {
            viewModel = new ViewModels.TauxRecouvrementModel();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
