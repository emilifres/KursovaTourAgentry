using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System;
using ToursApp;
using System.Text;
using System.Collections.Generic;

namespace ToursApp
{
    public partial class LoginWindow : Window
    {
        private string _generatedCaptcha;
        private readonly Random _random = new Random();

   

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        // Генерация случайной капчи
        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=,.?";

            int captchaLength = 4; // Длина капчи
            StringBuilder captchaBuilder = new StringBuilder();

            for (int i = 0; i < captchaLength; i++)
            {
                int index = _random.Next(chars.Length);
                captchaBuilder.Append(chars[index]);
            }

            _generatedCaptcha = captchaBuilder.ToString();
            CaptchaText.Text = _generatedCaptcha;
        }

        // Обработчик кнопки "Обновить"
        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            CaptchaInput.Text = string.Empty;
            ErrorText.Visibility = Visibility.Collapsed;
        }

        // Обработчик кнопки "Войти"
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                ShowError("Введите логин.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Введите пароль.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CaptchaInput.Text))
            {
                ShowError("Введите капчу.");
                return;
            }

            if (!CaptchaInput.Text.Equals(_generatedCaptcha))
            {
                ShowError("Капча введена неверно.");
                GenerateCaptcha(); // Обновить капчу при ошибке
                CaptchaInput.Text = string.Empty;
                return;
            }

            // Проверка логина и пароля
            var users = new Dictionary<string, (string Password, string Role)>
            {
                { "admin", ("123", "admin") },
                { "manager", ("345", "manager") },
                { "operator", ("567", "operator") }
            };

            if (users.ContainsKey(LoginBox.Text) && users[LoginBox.Text].Password == PasswordBox.Password)
            {
                MessageBox.Show("Успешный вход!", "Добро пожаловать", MessageBoxButton.OK, MessageBoxImage.Information);
                string role = users[LoginBox.Text].Role; // Извлекаем роль пользователя
                MainWindow mainWindow = new MainWindow(role); // Передаем роль в конструктор MainWindow
                mainWindow.Show();
                Close();
            }
            else
            {
                ShowError("Неверный логин или пароль.");
            }
        }

        // Отображение сообщения об ошибке
        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        //события фокуса для TextBox
        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.BorderBrush = System.Windows.Media.Brushes.Blue;
        }

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D1D1D1"));
        }
    }
}