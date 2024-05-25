using System.Windows;
using System.Windows.Controls;

namespace Winery
{
    public partial class WineDetailsPage : Page
    {
        private MainWindow mainWindow;

        public WineDetailsPage(Wine wine, MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = wine;
            this.mainWindow = mainWindow;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowMainView();
        }
    }
}
