using System;
using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class ReportFormationPage : Page
    {
        public ReportFormationPage()
        {
            InitializeComponent();
            TbCreationDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(TbEmployeeName.Text))
                {
                    MessageBox.Show("Введите имя сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (CbPosition.SelectedItem == null)
                {
                    MessageBox.Show("Выберите должность!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (CbMonth.SelectedItem == null)
                {
                    MessageBox.Show("Выберите месяц!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(TbToursBooked.Text, out int toursBooked))
                {
                    MessageBox.Show("Количество забронированных туров должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(TbClientsServed.Text, out int clientsServed))
                {
                    MessageBox.Show("Количество клиентов должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получение выбранной должности и месяца
                string position = ((ComboBoxItem)CbPosition.SelectedItem).Content.ToString();
                string selectedMonth = ((ComboBoxItem)CbMonth.SelectedItem).Content.ToString();

                // Преобразование месяца в DateTime
                int monthNumber = Array.IndexOf(new[]
                {
                    "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                    "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
                }, selectedMonth) + 1;

                DateTime? monthDate = monthNumber > 0
                    ? new DateTime(DateTime.Now.Year, monthNumber, 1)
                    : (DateTime?)null;

                using (var context = new TourAgentryEntities())
                {
                    var report = new Report
                    {
                        EmployeeName = TbEmployeeName.Text,
                        Position = position,
                        Month = monthDate,
                        ToursBooked = toursBooked,
                        ClientsServed = clientsServed,
                        CreationDate = DateTime.TryParse(TbCreationDate.Text, out DateTime creationDate)
                            ? creationDate
                            : (DateTime?)null,
                        Name = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}",
                        Download = null
                    };

                    context.Report.Add(report);
                    context.SaveChanges();
                }

                MessageBox.Show("Отчет успешно сохранен! Обновите список отчетов вручную, если необходимо.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
