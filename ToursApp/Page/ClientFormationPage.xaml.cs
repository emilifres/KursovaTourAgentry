using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Word;

namespace ToursApp
{
    public partial class ClientFormationPage : System.Windows.Controls.Page
    {
        public ClientFormationPage()
        {
            InitializeComponent();
        }

        private void SaveClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что обязательные поля для клиента заполнены
                if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ClientPhoneTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ClientEmailTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля клиента (ФИО, телефон, email)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем, что обязательные поля для тура заполнены
                if (string.IsNullOrWhiteSpace(TourNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TicketsCountTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TourCostTextBox.Text) ||
                    string.IsNullOrWhiteSpace(NightsCountTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля тура (название, количество билетов, стоимость, количество ночей)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка числовых полей для тура
                if (!int.TryParse(TicketsCountTextBox.Text, out int ticketsCount))
                {
                    MessageBox.Show("Количество билетов должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!decimal.TryParse(TourCostTextBox.Text, out decimal tourCost))
                {
                    MessageBox.Show("Стоимость должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(NightsCountTextBox.Text, out int nightsCount))
                {
                    MessageBox.Show("Количество ночей должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Разделяем ФИО на имя и фамилию
                var fullName = ClientNameTextBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (fullName.Length < 2)
                {
                    MessageBox.Show("Введите имя и фамилию клиента через пробел!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new TourAgentryEntities())
                {
                    // Проверяем существование тура
                    var tour = context.Tour.FirstOrDefault(t => t.Name == TourNameTextBox.Text);
                    if (tour == null)
                    {
                        MessageBox.Show("Указанный тур не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Проверяем существование сотрудника (можно заменить на текущего пользователя)
                    var employee = context.Employee.FirstOrDefault();
                    if (employee == null)
                    {
                        MessageBox.Show("Сотрудник не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Проверяем существование отеля (можно сделать выбор отеля в UI)
                    var hotel = context.Hotel.FirstOrDefault();
                    if (hotel == null)
                    {
                        MessageBox.Show("Отель не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Создаем нового клиента
                    var newClient = new Client
                    {
                        Name = ClientNameTextBox.Text,
                        Phone = ClientPhoneTextBox.Text,
                        Email = ClientEmailTextBox.Text
                    };

                    context.Client.Add(newClient);
                    context.SaveChanges(); // Сохраняем клиента, чтобы получить его Id

                    // Создаем новую запись бронирования
                    var newReservation = new Reservation
                    {
                        ReservationDate = DateTime.Now,
                        TourId = tour.Id, // Используем Id найденного тура
                        ClientId = newClient.Id,
                        EmployeeId = employee.Id, // Используем Id найденного сотрудника
                        HotelId = hotel.Id // Используем Id найденного отеля
                    };

                    context.Reservation.Add(newReservation);
                    context.SaveChanges(); // Сохраняем бронирование

                    MessageBox.Show("Клиент и бронирование успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                // Указываем полный путь к шаблону
                string templatePath = @"C:\Users\Эмилия\Desktop\ToursApp\ToursApp\Templates\Оформления_тура.docx";

                if (!System.IO.File.Exists(templatePath))
                {
                    MessageBox.Show("Шаблон документа 'Оформления_тура.docx' не найден по пути: " + templatePath, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(EmployeeNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmployeePositionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ClientNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ClientPhoneTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ClientEmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TourNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TicketsCountTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TourCostTextBox.Text) ||
                    string.IsNullOrWhiteSpace(NightsCountTextBox.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка числовых полей
                if (!int.TryParse(TicketsCountTextBox.Text, out _))
                {
                    MessageBox.Show("Количество билетов должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!decimal.TryParse(TourCostTextBox.Text, out _))
                {
                    MessageBox.Show("Стоимость должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(NightsCountTextBox.Text, out _))
                {
                    MessageBox.Show("Количество ночей должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = false;

                doc = wordApp.Documents.Open(templatePath);

                // Замена текста в документе
                ReplaceTextInDocument(doc, "ФИО сотрудника: ____________________________", $"ФИО сотрудника: {EmployeeNameTextBox.Text}");
                ReplaceTextInDocument(doc, "Должность: ________________________________", $"Должность: {EmployeePositionTextBox.Text}");
                ReplaceTextInDocument(doc, "ФИО заказчика: ____________________________", $"ФИО заказчика: {ClientNameTextBox.Text}");
                ReplaceTextInDocument(doc, "Телефон: _________________________________", $"Телефон: {ClientPhoneTextBox.Text}");
                ReplaceTextInDocument(doc, "Электронная почта: ________________________", $"Электронная почта: {ClientEmailTextBox.Text}");
                ReplaceTextInDocument(doc, "Название тура: ____________________________", $"Название тура: {TourNameTextBox.Text}");
                ReplaceTextInDocument(doc, "Количество билетов: _______________________", $"Количество билетов: {TicketsCountTextBox.Text}");
                ReplaceTextInDocument(doc, "Стоимость (за всё): _______________________", $"Стоимость (за всё): {TourCostTextBox.Text}");
                ReplaceTextInDocument(doc, "Количество ночей: _________________________", $"Количество ночей: {NightsCountTextBox.Text}");
                ReplaceTextInDocument(doc, "Подпись сотрудника: ________________________", $"Подпись сотрудника: {EmployeeNameTextBox.Text}");
                ReplaceTextInDocument(doc, "Подпись заказчика: _________________________", $"Подпись заказчика: {ClientNameTextBox.Text}");
                ReplaceTextInDocument(doc, "Дата оформления: ___ «____» ___________________ 20___ г.",
                    $"Дата оформления: {DateTime.Now:dd «MMMM» yyyy} г.");
                doc.Save();
                doc.Close();

                MessageBox.Show("Отчёт успешно сформирован! Изменения сохранены в: " + templatePath, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (doc != null)
                {
                    try { doc.Close(false); System.Runtime.InteropServices.Marshal.ReleaseComObject(doc); }
                    catch { }
                }
                if (wordApp != null)
                {
                    try { System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp); }
                    catch { }
                }
                doc = null;
                wordApp = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void ReplaceTextInDocument(Microsoft.Office.Interop.Word.Document doc, string oldText, string newText)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: oldText, ReplaceWith: newText, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}