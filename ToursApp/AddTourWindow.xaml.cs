using System;
using System.Linq;
using System.Windows;

namespace ToursApp
{
    public partial class AddTourWindow : Window
    {
        private readonly TourAgentryEntities _context = new TourAgentryEntities();
        public Tour NewTour { get; private set; }

        public AddTourWindow()
        {
            InitializeComponent();
            LoadComboBoxes();
            // Устанавливаем даты по умолчанию
            StartDatePicker.SelectedDate = DateTime.Now;
            EndDatePicker.SelectedDate = DateTime.Now.AddDays(7);
        }

        private void LoadComboBoxes()
        {
            // Загружаем данные из базы
            HotelComboBox.ItemsSource = _context.Hotel.ToList();
            TypeComboBox.ItemsSource = _context.Type.ToList();
            TourOperatorComboBox.ItemsSource = _context.TourOperator.ToList();

            // Устанавливаем значения по умолчанию (первый элемент, если есть)
            if (HotelComboBox.Items.Count > 0) HotelComboBox.SelectedIndex = 0;
            if (TypeComboBox.Items.Count > 0) TypeComboBox.SelectedIndex = 0;
            if (TourOperatorComboBox.Items.Count > 0) TourOperatorComboBox.SelectedIndex = 0;
        }

        private void ClearFields()
        {
            NameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            TicketsCountTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = DateTime.Now;
            EndDatePicker.SelectedDate = DateTime.Now.AddDays(7);
            if (HotelComboBox.Items.Count > 0) HotelComboBox.SelectedIndex = 0;
            if (TypeComboBox.Items.Count > 0) TypeComboBox.SelectedIndex = 0;
            if (TourOperatorComboBox.Items.Count > 0) TourOperatorComboBox.SelectedIndex = 0;
        }

        private bool ValidateAndCreateTour(out Tour tour)
        {
            tour = null;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название тура.");
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return false;
            }

            if (!int.TryParse(TicketsCountTextBox.Text, out int ticketsCount) || ticketsCount <= 0)
            {
                MessageBox.Show("Введите корректное количество билетов.");
                return false;
            }

            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату начала тура.");
                return false;
            }

            if (!EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату окончания тура.");
                return false;
            }

            if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.");
                return false;
            }

            if (HotelComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите отель.");
                return false;
            }

            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип тура.");
                return false;
            }

            if (TourOperatorComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите туроператора.");
                return false;
            }

            tour = new Tour
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text,
                Price = price,
                TicketsCount = ticketsCount,
                StartDate = StartDatePicker.SelectedDate.Value,
                EndDate = EndDatePicker.SelectedDate.Value,
                IsActual = true,
                HotelId = (int)HotelComboBox.SelectedValue,
                TypeId = (int)TypeComboBox.SelectedValue,
                TourOperatorId = (int)TourOperatorComboBox.SelectedValue
            };

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateAndCreateTour(out Tour tour))
                {
                    _context.Tour.Add(tour);
                    _context.SaveChanges();
                    ClearFields();
                    MessageBox.Show("Тур добавлен. Можете добавить следующий.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateAndCreateTour(out Tour tour))
                {
                    NewTour = tour;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}