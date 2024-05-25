using System.ComponentModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace Winery
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Container selectedContainer;
        private Wine selectedWine;
        private int selectedTab;
        private bool isMainPageVisible = true;

        public Container SelectedContainer
        {
            get => selectedContainer;
            set
            {
                selectedContainer = value;
                OnPropertyChanged(nameof(SelectedContainer));
            }
        }

        public Wine SelectedWine
        {
            get => selectedWine;
            set
            {
                selectedWine = value;
                OnPropertyChanged(nameof(SelectedWine));
            }
        }

        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public bool IsMainPageVisible
        {
            get => isMainPageVisible;
            set
            {
                isMainPageVisible = value;
                OnPropertyChanged(nameof(IsMainPageVisible));
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

        private void AddWineButton_Click(object sender, RoutedEventArgs e)
        {
            var wineCreationWindow = new WineCreationWindow();
            wineCreationWindow.ShowDialog();
            dataGridWines.ItemsSource = null;
            dataGridWines.ItemsSource = WineryContext.Instance.Wines.Local;
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

        private void EditWine_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridWines.SelectedItem is Wine selectedWine)
            {
                var wineCreationWindow = new WineCreationWindow(selectedWine);
                wineCreationWindow.ShowDialog();
                dataGridWines.ItemsSource = null;
                dataGridWines.ItemsSource = WineryContext.Instance.Wines.Local;
            }
            else
            {
                MessageBox.Show("Please select a wine to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteWine_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWine != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this wine?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    WineryContext.Instance.Wines.Remove(SelectedWine);
                    WineryContext.Instance.SaveChanges();
                    dataGridWines.ItemsSource = null;
                    dataGridWines.ItemsSource = WineryContext.Instance.Wines.Local;
                }
            }
            else
            {
                MessageBox.Show("Please select a wine to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DataGridContainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Intentionally left empty
        }

        private void DataGridWines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Intentionally left empty
        }

        private void DataGridRow_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.Item is Container selectedContainer)
                {
                    IsMainPageVisible = false;
                    MainFrame.Visibility = Visibility.Visible;
                    MainFrame.Navigate(new ContainerDetailsPage(selectedContainer, this));
                }
                else if (row.Item is Wine selectedWine)
                {
                    IsMainPageVisible = false;
                    MainFrame.Visibility = Visibility.Visible;
                    MainFrame.Navigate(new WineDetailsPage(selectedWine, this));
                }
            }
        }

        public void ShowMainView()
        {
            IsMainPageVisible = true;
            MainFrame.Visibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
