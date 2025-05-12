using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; // Для Include

namespace ToursApp
{
    public partial class HotelsPage : Page
    {
        private readonly string _userRole; // Добавляем поле для хранения роли

        public HotelsPage(string role)
        {
            InitializeComponent();
            _userRole = role;
            ApplyRoleBasedAccess();
            LoadHotels(); // Добавляем вызов загрузки отелей
        }

        private void ApplyRoleBasedAccess()
        {
            if (_userRole == "operator")
            {
                // Отключаем кнопки для оператора
                BtnAdd.IsEnabled = false; // Исправляем имя кнопки
                BtnDelete.IsEnabled = false; // Используем существующую кнопку удаления
                // BtnEditHotel отсутствует в XAML, убираем или добавляем в XAML, если нужна
            }
        }

        private void LoadHotels()
        {
            try
            {
                using (var context = new TourAgentryEntities())
                {
                    var hotels = context.Hotel.Include("Country").ToList();
                    DGridHotels.ItemsSource = hotels;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addHotelWindow = new AddHotelWindow();
            addHotelWindow.Owner = Application.Current.MainWindow;
            addHotelWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (addHotelWindow.ShowDialog() == true)
            {
                LoadHotels();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var hotelsForRemoving = DGridHotels.SelectedItems.Cast<Hotel>().ToList();

            if (hotelsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один отель для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить следующие {hotelsForRemoving.Count} элементов?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new TourAgentryEntities())
                    {
                        foreach (var hotel in hotelsForRemoving)
                        {
                            var hotelToRemove = context.Hotel.FirstOrDefault(h => h.Id == hotel.Id);
                            if (hotelToRemove != null)
                            {
                                context.Hotel.Remove(hotelToRemove);
                            }
                        }
                        context.SaveChanges();
                        MessageBox.Show("Данные удалены!");
                        LoadHotels();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}