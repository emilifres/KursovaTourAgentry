using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                // Проверяем, что DGridClients инициализирован
                if (DGridClients == null)
                {
                    MessageBox.Show("DataGrid DGridClients не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new TourAgentryEntities())
                {
                    // Проверяем, что контекст создан
                    if (context == null)
                    {
                        MessageBox.Show("Контекст базы данных не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Загружаем клиентов
                    var clients = context.Client.ToList();
                    if (clients == null || !clients.Any())
                    {
                        MessageBox.Show("Список клиентов пуст или не удалось загрузить данные!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    DGridClients.ItemsSource = clients;
                }
            }
            catch (Exception ex)
            {
                // Подробное сообщение об ошибке
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Manager.MainFrame != null)
                {
                    // Переходим на страницу ClientFormationPage
                    Manager.MainFrame.Navigate(new ClientFormationPage());
                }
                else
                {
                    MessageBox.Show("MainFrame не инициализирован! Убедитесь, что Manager.MainFrame настроен корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переходе на страницу ClientFormationPage: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}