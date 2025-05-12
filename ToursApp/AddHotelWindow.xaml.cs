using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class AddHotelWindow : Window
    {
        public Hotel NewHotel { get; private set; }

        public AddHotelWindow()
        {
            InitializeComponent();
            LoadCountries();
        }

        private void LoadCountries()
        {
            try
            {
                using (var context = new TourAgentryEntities())
                {
                    CmbCountry.ItemsSource = context.Country.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стран: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtHotelName.Text))
            {
                MessageBox.Show("Введите название отеля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new TourAgentryEntities())
                {
                    var selectedCountry = CmbCountry.SelectedItem as Country;
                    if (selectedCountry == null)
                    {
                        MessageBox.Show("Выберите страну!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    NewHotel = new Hotel
                    {
                        Name = TxtHotelName.Text,
                        CountOfStars = int.Parse((CmbStars.SelectedItem as ComboBoxItem).Content.ToString()),
                        CountryCode = selectedCountry.Code
                    };

                    context.Hotel.Add(NewHotel);
                    context.SaveChanges();
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}