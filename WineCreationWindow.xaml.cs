using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Winery
{
    public partial class WineCreationWindow : Window
    {
        private Wine NewWine;
        private bool isEdit;

        public WineCreationWindow(Wine wineInfo = null)
        {
            InitializeComponent();

            if (wineInfo != null)
            {
                isEdit = true;
                NewWine = wineInfo;
                this.DataContext = NewWine;

                // Pre-populate fields
                WineIDTextBox.Text = NewWine.WineID;
                SweetnessTextBox.Text = NewWine.Sweetness.ToString();
                SulfurLevelTextBox.Text = NewWine.SulfurLevel.ToString();
                PressureTextBox.Text = NewWine.Pressure.ToString();
                VineyardTextBox.Text = NewWine.Vineyard;
                RegionTextBox.Text = NewWine.Region;
                AlcoholContentTextBox.Text = NewWine.AlcoholContent.ToString();
                NotesTextBox.Text = NewWine.Notes;
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == NewWine.Type.ToString());

                this.Title = "Update Wine";
                ActionButton.Content = "Save";
            }
            else
            {
                isEdit = false;
                NewWine = new Wine();
                this.DataContext = NewWine;

                this.Title = "Add Wine";
                ActionButton.Content = "Add";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string message))
            {
                var context = WineryContext.Instance;

                if (isEdit)
                {
                    var updateWine = context.Wines.FirstOrDefault(w => w.WineID == NewWine.WineID);
                    if (updateWine != null)
                    {
                        updateWine.Type = NewWine.Type;
                        updateWine.Sweetness = NewWine.Sweetness;
                        updateWine.SulfurLevel = NewWine.SulfurLevel;
                        updateWine.Pressure = NewWine.Pressure;
                        updateWine.Vineyard = NewWine.Vineyard;
                        updateWine.Region = NewWine.Region;
                        updateWine.AlcoholContent = NewWine.AlcoholContent;
                        updateWine.Notes = NewWine.Notes;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(WineIDTextBox.Text) || WineIDTextBox.Text == "Wine ID (Optional)")
                    {
                        var selectedType = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
                        string prefix = selectedType.Substring(0, 2).ToUpper();
                        NewWine.WineID = GenerateUniqueWineID(prefix);
                    }
                    else
                    {
                        if (context.Wines.Any(w => w.WineID == WineIDTextBox.Text))
                        {
                            MessageBox.Show("Wine ID must be unique.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        NewWine.WineID = WineIDTextBox.Text;
                    }

                    context.Wines.Add(NewWine);
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

        private string GenerateUniqueWineID(string prefix)
        {
            var context = WineryContext.Instance;
            int count = context.Wines.Count(w => w.WineID.StartsWith(prefix)) + 1;
            string wineID;
            do
            {
                wineID = $"{prefix}{count:0000}";
                count++;
            } while (context.Wines.Any(w => w.WineID == wineID));
            return wineID;
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

            if (TypeComboBox.SelectedItem == null)
            {
                retVal = false;
                errorMessage += "Select Type!\n";
                TypeComboBox.BorderBrush = Brushes.Red;
            }
            else
            {
                try
                {
                    NewWine.Type = (Wine.WineType)Enum.Parse(typeof(Wine.WineType), ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString());
                    TypeComboBox.ClearValue(BorderBrushProperty);
                }
                catch (Exception)
                {
                    retVal = false;
                    errorMessage += "Invalid Type selection!\n";
                    TypeComboBox.BorderBrush = Brushes.Red;
                }
            }

            if (String.IsNullOrWhiteSpace(SweetnessTextBox.Text) || !int.TryParse(SweetnessTextBox.Text, out int sweetness) || sweetness < 0 || sweetness > 100)
            {
                retVal = false;
                errorMessage += "Enter a valid Sweetness between 0 and 100!\n";
                SweetnessTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                SweetnessTextBox.ClearValue(BorderBrushProperty);
                NewWine.Sweetness = sweetness;
            }

            if (String.IsNullOrWhiteSpace(SulfurLevelTextBox.Text) || !int.TryParse(SulfurLevelTextBox.Text, out int sulfurLevel) || sulfurLevel < 0 || sulfurLevel > 200)
            {
                retVal = false;
                errorMessage += "Enter a valid Sulfur Level between 0 and 200!\n";
                SulfurLevelTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                SulfurLevelTextBox.ClearValue(BorderBrushProperty);
                NewWine.SulfurLevel = sulfurLevel;
            }

            int pressure = 0;
            if (NewWine.Type == Wine.WineType.Sparkling)
            {
                if (!String.IsNullOrWhiteSpace(PressureTextBox.Text) && (!int.TryParse(PressureTextBox.Text, out pressure) || pressure < 0 || pressure > 10))
                {
                    retVal = false;
                    errorMessage += "Enter a valid Pressure between 0 and 10!\n";
                    PressureTextBox.BorderBrush = Brushes.Red;
                }
                else
                {
                    PressureTextBox.ClearValue(BorderBrushProperty);
                    NewWine.Pressure = String.IsNullOrWhiteSpace(PressureTextBox.Text) ? 0 : pressure;
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(PressureTextBox.Text))
                {
                    MessageBox.Show("Pressure can only be set for Sparkling wine. It will be set to null.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PressureTextBox.ClearValue(BorderBrushProperty);
                    NewWine.Pressure = 0;
                }
                else
                {
                    NewWine.Pressure = 0;
                }
            }

            if (String.IsNullOrWhiteSpace(VineyardTextBox.Text))
            {
                retVal = false;
                errorMessage += "Enter a Vineyard!\n";
                VineyardTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                VineyardTextBox.ClearValue(BorderBrushProperty);
                NewWine.Vineyard = VineyardTextBox.Text;
            }

            if (String.IsNullOrWhiteSpace(RegionTextBox.Text))
            {
                retVal = false;
                errorMessage += "Enter a Region!\n";
                RegionTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                RegionTextBox.ClearValue(BorderBrushProperty);
                NewWine.Region = RegionTextBox.Text;
            }

            int alcoholContent = 0;
            if (!String.IsNullOrWhiteSpace(AlcoholContentTextBox.Text) && (!int.TryParse(AlcoholContentTextBox.Text, out alcoholContent) || alcoholContent < 0 || alcoholContent > 100))
            {
                retVal = false;
                errorMessage += "Enter a valid Alcohol Content between 0 and 100!\n";
                AlcoholContentTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                AlcoholContentTextBox.ClearValue(BorderBrushProperty);
                NewWine.AlcoholContent = alcoholContent;
            }

            NewWine.Notes = NotesTextBox.Text;

            return retVal;
        }

    }
}
