using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class ToursPage : Page
    {
        private readonly TourAgentryEntities _context = new TourAgentryEntities();
        private readonly string _userRole;

        public ToursPage(string role)
        {
            InitializeComponent();
            _userRole = role;
            ApplyRoleBasedAccess();
            LoadTours();
        }

        private void ApplyRoleBasedAccess()
        {
            if (_userRole == "operator")
            {
                // Отключаем кнопки для оператора
                AddButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
            // Для manager и admin кнопки остаются включёнными
        }

        private void LoadTours()
        {
            ToursDataGrid.ItemsSource = _context.Tour.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addTourWindow = new AddTourWindow();
                if (addTourWindow.ShowDialog() == true)
                {
                    var newTour = addTourWindow.NewTour;
                    _context.Tour.Add(newTour);
                    _context.SaveChanges();
                    LoadTours();
                    MessageBox.Show("Тур добавлен.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ToursDataGrid.SelectedItem is Tour selectedTour)
                {
                    var hasReservations = _context.Reservation.Any(r => r.TourId == selectedTour.Id);
                    if (hasReservations)
                    {
                        MessageBox.Show("Нельзя удалить тур, так как на него есть бронирования.");
                        return;
                    }

                    _context.Tour.Remove(selectedTour);
                    _context.SaveChanges();
                    LoadTours();
                    MessageBox.Show("Тур удален.");
                }
                else
                {
                    MessageBox.Show("Выберите тур для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }
    }
}