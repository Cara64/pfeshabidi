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

    public partial class MainWindow : Window
    {
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;
        
        public static TabItem SelectedTab;
        public static List<TabItem> TabItems;
        public static List<List<UserControl>> Indicateurs;
        public static List<Grid> Grids;

        public ImportWindow import;
        public CreateIndicWindow createIndic;
        public CompareIndicWindow compareIndic;
        public ExtractWindow extract;

      
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Indicateurs = new List<List<UserControl>>();
                Grids = new List<Grid>();
                TabItems = new List<TabItem>();
                initTab();

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

        #region Gestion des onglets

        private void initTab()
        {
            _tabItems = new List<TabItem>();
            _tabAdd = new TabItem();
            _tabAdd.Header = "+";
            _tabItems.Add(_tabAdd);
            this.addTabItem();
            tabMainWindow.DataContext = _tabItems;
            tabMainWindow.SelectedIndex = 0;
            TabItems = _tabItems;
        }

        private TabItem addTabItem()
        {
            int count = _tabItems.Count;

            TabItem tab  = new TabItem();
            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabMainWindow.FindResource("TabHeader") as DataTemplate;

            tab.MouseDoubleClick += new MouseButtonEventHandler(tabMainWindow_MouseDoubleClick);

            _tabItems.Insert(count - 1, tab);

            Indicateurs.Add(new List<UserControl>());
            Grids.Add(new Grid());

            TabItems = _tabItems;
            return tab;
        }

        private void tabMainWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TabItem tab = sender as TabItem;
            TabNameWindow dlg = new TabNameWindow();

            dlg.txtTitle.Text = tab.Header.ToString();

            if (dlg.ShowDialog() == true)
            {
                tab.Header = dlg.txtTitle.Text.Trim();
            }
           
        }

        private void tabMainWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabMainWindow.SelectedItem as TabItem;
            
            if (tab == null) return;

            if (tab.Equals(_tabAdd))
            {
                tabMainWindow.DataContext = null;
                TabItem newTab = this.addTabItem();
                tabMainWindow.DataContext = _tabItems;
                tabMainWindow.SelectedItem = newTab;
            }

            SelectedTab = tabMainWindow.SelectedItem as TabItem;
            Console.WriteLine(SelectedTab.Header);
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

            TabItems = _tabItems;
        }

        #endregion


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


    }

    
}
