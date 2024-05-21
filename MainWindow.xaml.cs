using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace Winery
{
    public partial class MainWindow : Window
    {
        private Container selectedContainer;
        private Wine selectedWine;
        private int selectedTab;

        public Container SelectedContainer
        {
            get => selectedContainer;
            set
            {
                selectedContainer = value;
            }
        }

        public Wine SelectedWine
        {
            get => selectedWine;
            set
            {
                selectedWine = value;
            }
        }

        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Load data for Wines
            WineryContext.Instance.Wines.Load();
            dataGridWines.ItemsSource = WineryContext.Instance.Wines.Local;

            // Load data for Containers
            WineryContext.Instance.Containers.Load();
            dataGridContainers.ItemsSource = WineryContext.Instance.Containers.Local;

            this.DataContext = this;
        }

        private void AddContainerButton_Click(object sender, RoutedEventArgs e)
        {
            var containerCreationWindow = new ContainerCreationWindow();
            containerCreationWindow.ShowDialog();
            dataGridContainers.ItemsSource = null;
            dataGridContainers.ItemsSource = WineryContext.Instance.Containers.Local;
        }

        private void EditContainer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridContainers.SelectedItem is Container selectedContainer)
            {
                var containerCreationWindow = new ContainerCreationWindow(selectedContainer);
                containerCreationWindow.ShowDialog();
                dataGridContainers.ItemsSource = null;
                dataGridContainers.ItemsSource = WineryContext.Instance.Containers.Local;
            }
            else
            {
                MessageBox.Show("Please select a container to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteContainer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this container?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    WineryContext.Instance.Containers.Remove(SelectedContainer);
                    WineryContext.Instance.SaveChanges();
                    dataGridContainers.ItemsSource = null;
                    dataGridContainers.ItemsSource = WineryContext.Instance.Containers.Local;
                }
            }
            else
            {
                MessageBox.Show("Please select a container to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DataGridContainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Intentionally left empty
        }

        private void DataGridRow_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Container selectedContainer)
            {
                MainTabControl.Visibility = Visibility.Collapsed;
                MainFrame.Visibility = Visibility.Visible;
                MainFrame.Navigate(new ContainerDetailsPage(selectedContainer, this));
            }
        }

        public void ShowMainView()
        {
            MainTabControl.Visibility = Visibility.Visible;
            MainFrame.Visibility = Visibility.Collapsed;
        }
    }
}
