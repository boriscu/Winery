using System.Windows;
using System.Windows.Controls;

namespace Winery
{
    public partial class ContainerDetailsPage : Page
    {
        private MainWindow mainWindow;

        public ContainerDetailsPage(Container container, MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = container;
            this.mainWindow = mainWindow;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowMainView();
        }
    }
}
