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

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    
    // TODO: Corriger pb gestion des onglets
    // TODO: Gérer le redimensionnement de la fenêtre
    public partial class MainWindow : Window
    {
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;

        public MainWindow()
        {
         
            InitializeComponent();
            initializeTabItems();   
        }


        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportWindow import = new ImportWindow();
            import.Show();
        }

        #region Managing Tab Items

        private void initializeTabItems()
        {
            _tabItems = new List<TabItem>();
            TabItem tabAdd = new TabItem();
            tabAdd.Header = "+";
            _tabItems.Add(tabAdd);
            this.addTabItem("Test");
            tabMainWindow.DataContext = _tabItems;
            tabMainWindow.SelectedIndex = 0;
        }

        private TabItem addTabItem(string nameTab)
        {
            int count = _tabItems.Count;

            TabItem tab  = new TabItem();
            tab.Header = nameTab;
            tab.Name = nameTab;
            tab.HeaderTemplate = tabMainWindow.FindResource("TabHeader") as DataTemplate;

            _tabItems.Insert(count - 1, tab);
            return tab;
        }

        private void tabMainWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabMainWindow.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals(_tabAdd))
                {
                    tabMainWindow.DataContext = null;
                    TabItem newTab = this.addTabItem("");
                    tabMainWindow.DataContext = _tabItems;
                    tabMainWindow.SelectedItem = newTab;
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabMainWindow.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (!tab.Equals(null))
            {
                if (_tabItems.Count < 3)
                {
                    MessageBox.Show("Le dernier onglet ne peut pas être supprimé");
                }
                else if (MessageBox.Show(string.Format("Etes-vous sûr de vouloir supprimer l'onglet '{0}' ?", tab.Header.ToString()), "Suppression d'un onglet", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    TabItem selectedTab = tabMainWindow.SelectedItem as TabItem;
                    tabMainWindow.DataContext = null;
                    _tabItems.Remove(tab);
                    tabMainWindow.DataContext = _tabItems;

                    if (selectedTab.Equals(null) || selectedTab.Equals(tab))
                    {
                        selectedTab = _tabItems[0];
                    }

                    tabMainWindow.SelectedItem = selectedTab;
                }
            }
        }

        #endregion
    }


}
