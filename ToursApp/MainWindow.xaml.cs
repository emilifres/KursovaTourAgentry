using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class MainWindow : Window
    {
        private readonly string _userRole;

        public MainWindow(string role)
        {
            InitializeComponent();
            _userRole = role;
            Manager.MainFrame = MainFrame;
            // Устанавливаем начальную страницу
            MainFrame.Navigate(new ToursPage(_userRole));
            ConfigureAccess(); // Настраиваем видимость кнопок при инициализации
        }

        // Метод для настройки видимости кнопок на основе роли
        private void ConfigureAccess()
        {
            switch (_userRole.ToLower())
            {
                case "admin":
                    // Админ имеет доступ ко всем кнопкам
                    ToursButton.Visibility = Visibility.Visible;
                    ClientButton.Visibility = Visibility.Visible;
                    HotelButton.Visibility = Visibility.Visible;
                    ReportsButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Visible;
                    LogoutButton.Visibility = Visibility.Visible;
                    break;

                case "manager":
                    // Менеджер: Туры, Клиенты, Отели, Отчеты, Выход
                    ToursButton.Visibility = Visibility.Visible;
                    ClientButton.Visibility = Visibility.Visible;
                    HotelButton.Visibility = Visibility.Visible; // Исправлено: добавлен доступ к Отели
                    ReportsButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Collapsed; // Нет доступа
                    LogoutButton.Visibility = Visibility.Visible;
                    break;

                case "operator":
                    // Туроператор: Туры, Клиенты, Отели, Отчеты, Выход
                    ToursButton.Visibility = Visibility.Visible;
                    ClientButton.Visibility = Visibility.Visible;
                    HotelButton.Visibility = Visibility.Visible;
                    ReportsButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Collapsed; // Нет доступа
                    LogoutButton.Visibility = Visibility.Visible;
                    break;

                default:
                    MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    break;
            }
        }


        private void Tours_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ToursPage(_userRole));
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void Hotel_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HotelsPage(_userRole));
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole == "admin")
            {
                MainFrame.Navigate(new EmployeePage());
            }
            else
            {
                MessageBox.Show("Доступ запрещён для вашей роли!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}