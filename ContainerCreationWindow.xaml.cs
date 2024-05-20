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

            if (containerInfo != null)
            {
                isEdit = true;
                NewContainer = containerInfo;
                this.DataContext = NewContainer;

                this.Title = "Update Container";
            }
            else
            {
                isEdit = false;
                NewContainer = new Container();
                this.DataContext = NewContainer;

                this.Title = "Add Container";
            }
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
                        updateContainer.WineID = NewContainer.WineID;
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

        private bool ValidateInput(out string errorMessage)
        {
            bool retVal = true;
            errorMessage = "";

            if (TypeComboBox.SelectedItem == null || ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString() == "Select Type")
            {
                retVal = false;
                errorMessage += "Select Type!\n";
                TypeComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                TypeComboBox.ClearValue(BorderBrushProperty);
            }

            if (String.IsNullOrWhiteSpace(MaxVolumeTextBox.Text) || !int.TryParse(MaxVolumeTextBox.Text, out int maxVolume) || maxVolume <= 0)
            {
                retVal = false;
                errorMessage += "Enter a valid Max Volume greater than 0!\n";
                MaxVolumeTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                MaxVolumeTextBox.ClearValue(BorderBrushProperty);
                NewContainer.MaxVolume = maxVolume; // Ensure the parsed value is set to the container
            }

            if (StatusComboBox.SelectedItem == null || ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString() == "Select Status")
            {
                retVal = false;
                errorMessage += "Select Status!\n";
                StatusComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                StatusComboBox.ClearValue(BorderBrushProperty);
            }

            if (LocationComboBox.SelectedItem == null || ((ComboBoxItem)LocationComboBox.SelectedItem).Content.ToString() == "Select Location")
            {
                retVal = false;
                errorMessage += "Select Location!\n";
                LocationComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                LocationComboBox.ClearValue(BorderBrushProperty);
            }

            if (String.IsNullOrWhiteSpace(CurrentVolumeTextBox.Text) || !int.TryParse(CurrentVolumeTextBox.Text, out int currentVolume))
            {
                NewContainer.CurrentVolume = 0; // Default value
            }
            else
            {
                NewContainer.CurrentVolume = currentVolume;
            }

            return retVal;
        }
    }
}
