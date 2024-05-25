using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Winery
{
    public partial class ContainerCreationWindow : Window
    {
        private Container NewContainer;
        private bool isEdit;

        public ContainerCreationWindow(Container containerInfo = null)
        {
            InitializeComponent();
            LoadWineIDs();

            if (containerInfo != null)
            {
                isEdit = true;
                NewContainer = containerInfo;
                this.DataContext = NewContainer;

                // Pre-populate fields
                TankIDTextBox.Text = NewContainer.TankID;
                WineIDComboBox.Text = NewContainer.WineID;
                MaxVolumeTextBox.Text = NewContainer.MaxVolume.ToString();
                CurrentVolumeTextBox.Text = NewContainer.CurrentVolume.ToString();
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == NewContainer.Type.ToString());
                StatusComboBox.SelectedItem = StatusComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == NewContainer.Status.ToString());
                LocationComboBox.SelectedItem = LocationComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == NewContainer.Location.ToString());

                this.Title = "Update Container";
                ActionButton.Content = "Save";

                TankIDTextBox.IsReadOnly = true;
            }
            else
            {
                isEdit = false;
                NewContainer = new Container();
                this.DataContext = NewContainer;

                this.Title = "Add Container";
                ActionButton.Content = "Add";

                TankIDTextBox.IsReadOnly = false;
            }
        }

        private void LoadWineIDs()
        {
            var context = WineryContext.Instance;
            var wineIDs = context.Wines.Select(w => w.WineID).ToList();
            WineIDComboBox.ItemsSource = wineIDs;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string message))
            {
                var context = WineryContext.Instance;

                if (isEdit)
                {
                    var updateContainer = context.Containers.FirstOrDefault(c => c.TankID == NewContainer.TankID);
                    if (updateContainer != null)
                    {
                        if (!ValidateContainerVolumes(NewContainer))
                        {
                            MessageBox.Show("Current Volume cannot be greater than Max Volume!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        updateContainer.WineID = WineIDComboBox.Text;
                        updateContainer.MaxVolume = NewContainer.MaxVolume;
                        updateContainer.Type = NewContainer.Type;
                        updateContainer.Status = NewContainer.Status;
                        updateContainer.Location = NewContainer.Location;
                        updateContainer.CurrentVolume = NewContainer.CurrentVolume;
                        updateContainer.LastEditDate = DateTime.Now;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(TankIDTextBox.Text) || TankIDTextBox.Text == "Tank ID (Optional)")
                    {
                        var selectedType = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
                        string prefix = selectedType.Substring(0, 2).ToUpper();
                        NewContainer.TankID = GenerateUniqueTankID(prefix);
                    }
                    else
                    {
                        if (context.Containers.Any(c => c.TankID == TankIDTextBox.Text))
                        {
                            MessageBox.Show("Tank ID must be unique.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        NewContainer.TankID = TankIDTextBox.Text;
                    }

                    if (!ValidateContainerVolumes(NewContainer))
                    {
                        MessageBox.Show("Current Volume cannot be greater than Max Volume!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    NewContainer.WineID = WineIDComboBox.Text;
                    NewContainer.LastEditDate = DateTime.Now;

                    context.Containers.Add(NewContainer);
                }

                try
                {
                    context.SaveChanges();
                    this.Close();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    MessageBox.Show($"Error saving to database: {exceptionMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(message, "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateUniqueTankID(string prefix)
        {
            var context = WineryContext.Instance;
            int count = context.Containers.Count(c => c.TankID.StartsWith(prefix)) + 1;
            string tankID;
            do
            {
                tankID = $"{prefix}{count:0000}";
                count++;
            } while (context.Containers.Any(c => c.TankID == tankID));
            return tankID;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", "Cancel Operation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private bool ValidateContainerVolumes(Container container)
        {
            return container.CurrentVolume <= container.MaxVolume;
        }

        private bool ValidateInput(out string errorMessage)
        {
            bool retVal = true;
            errorMessage = "";

            var context = WineryContext.Instance;

            // Initialize local variables
            int maxVolume = 0;
            int currentVolume = 0;

            if (TypeComboBox.SelectedItem == null)
            {
                retVal = false;
                errorMessage += "Select Type!\n";
                TypeComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                TypeComboBox.ClearValue(BorderBrushProperty);
            }

            if (String.IsNullOrWhiteSpace(MaxVolumeTextBox.Text) || !int.TryParse(MaxVolumeTextBox.Text, out maxVolume) || maxVolume <= 0)
            {
                retVal = false;
                errorMessage += "Enter a valid Max Volume greater than 0!\n";
                MaxVolumeTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                MaxVolumeTextBox.ClearValue(BorderBrushProperty);
            }

            if (StatusComboBox.SelectedItem == null)
            {
                retVal = false;
                errorMessage += "Select Status!\n";
                StatusComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                StatusComboBox.ClearValue(BorderBrushProperty);
            }

            if (LocationComboBox.SelectedItem == null)
            {
                retVal = false;
                errorMessage += "Select Location!\n";
                LocationComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                LocationComboBox.ClearValue(BorderBrushProperty);
            }

            if (String.IsNullOrWhiteSpace(CurrentVolumeTextBox.Text) || !int.TryParse(CurrentVolumeTextBox.Text, out currentVolume))
            {
                currentVolume = 0;
            }

            if (currentVolume > maxVolume)
            {
                retVal = false;
                errorMessage += "Current Volume cannot be greater than Max Volume!\n";
                CurrentVolumeTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                CurrentVolumeTextBox.ClearValue(BorderBrushProperty);
            }

            if (String.IsNullOrWhiteSpace(WineIDComboBox.Text) || !context.Wines.Any(w => w.WineID == WineIDComboBox.Text))
            {
                retVal = false;
                errorMessage += "Enter a valid Wine ID!\n";
                WineIDComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                WineIDComboBox.ClearValue(BorderBrushProperty);
            }

            if (retVal)
            {
                NewContainer.Type = (Container.ContainerType)Enum.Parse(typeof(Container.ContainerType), ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString());
                NewContainer.MaxVolume = maxVolume;
                NewContainer.Status = (Container.ContainerStatus)Enum.Parse(typeof(Container.ContainerStatus), ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString());
                NewContainer.Location = (Container.ContainerLocation)Enum.Parse(typeof(Container.ContainerLocation), ((ComboBoxItem)LocationComboBox.SelectedItem).Content.ToString());
                NewContainer.CurrentVolume = currentVolume;
            }

            return retVal;
        }
    }
}
