using System.Windows.Controls;
using System.Windows;
using ToursApp;
using System.Linq;
using System.Windows.Media;

namespace ToursApp
{
    public partial class EmployeePage : Page
    {
        private TourAgentryEntities _context;
        public EmployeePage()
        {
            InitializeComponent();
            _context = new TourAgentryEntities();
            LoadEmployees();
        }

        public string Role { get; set; }

        private void LoadEmployees()
        {
            EmployeeDataGrid.ItemsSource = _context.Employee.ToList();
        }

        public void BtnAdd_Click(object sender, RoutedEventArgs args)
        {
            Window addWindow = new Window
            {
                Title = "Добавить сотрудника",
                Width = 600,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            Grid grid = new Grid
            {
                Margin = new Thickness(1)
            };

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Increased height for buttons
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBox txtEmployeeName = new TextBox { Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            TextBox txtRole = new TextBox { Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            TextBox txtDateofbirth = new TextBox { Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            TextBox txtPhone = new TextBox { Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            TextBox txtEmail = new TextBox { Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };

            Label lblEmployeeName = new Label
            {
                Content = "ФИО:",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Label lblRole = new Label
            {
                Content = "Роль:",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Label lblDateofbirth = new Label
            {
                Content = "Дата рождения:",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Label lblPhone = new Label
            {
                Content = "Телефон:",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Label lblEmail = new Label
            {
                Content = "Email:",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            Grid.SetRow(lblEmployeeName, 1);
            Grid.SetColumn(lblEmployeeName, 0);
            Grid.SetRow(txtEmployeeName, 1);
            Grid.SetColumn(txtEmployeeName, 1);
            Grid.SetRow(lblRole, 2);
            Grid.SetColumn(lblRole, 0);
            Grid.SetRow(txtRole, 2);
            Grid.SetColumn(txtRole, 1);
            Grid.SetRow(lblDateofbirth, 3);
            Grid.SetColumn(lblDateofbirth, 0);
            Grid.SetRow(txtDateofbirth, 3);
            Grid.SetColumn(txtDateofbirth, 1);
            Grid.SetRow(lblPhone, 4);
            Grid.SetColumn(lblPhone, 0);
            Grid.SetRow(txtPhone, 4);
            Grid.SetColumn(txtPhone, 1);
            Grid.SetRow(lblEmail, 5);
            Grid.SetColumn(lblEmail, 0);
            Grid.SetRow(txtEmail, 5);
            Grid.SetColumn(txtEmail, 1);


            grid.Children.Add(lblEmployeeName);
            grid.Children.Add(txtEmployeeName);
            grid.Children.Add(lblRole);
            grid.Children.Add(txtRole);
            grid.Children.Add(lblDateofbirth);
            grid.Children.Add(txtDateofbirth);
            grid.Children.Add(lblPhone);
            grid.Children.Add(txtPhone);
            grid.Children.Add(lblEmail);
            grid.Children.Add(txtEmail);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 20, 10, 10)
            };
            Button btnSave = new Button
            {
                Content = "Сохранить",
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(255, 204, 0)),
                MinWidth = 200,
                Height = 50,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Black,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10)
            };
            Button btnCancel = new Button
            {
                Content = "Отмена",
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(255, 204, 0)),
                MinWidth = 200,
                Height = 50,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Black,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10)
            };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtEmployeeName.Text) || string.IsNullOrEmpty(txtEmployeeName.Text) ||
                    string.IsNullOrEmpty(txtRole.Text) || string.IsNullOrEmpty(txtDateofbirth.Text) ||
                    string.IsNullOrEmpty(txtPhone.Text) || string.IsNullOrEmpty(txtEmail.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                var newEmployee = new Employee
                {
                    EmployeeName = txtEmployeeName.Text,
                    Role = txtRole.Text,
                    Dateofbirth = txtDateofbirth.Text,
                    Phone = txtPhone.Text,
                    Email = txtEmail.Text
                };

                _context.Employee.Add(newEmployee);
                _context.SaveChanges();
                LoadEmployees();
                addWindow.Close();
            };

            btnCancel.Click += (s, e) => addWindow.Close();

            buttonPanel.Children.Add(btnSave);
            buttonPanel.Children.Add(btnCancel);
            Grid.SetRow(buttonPanel, 6);
            Grid.SetColumnSpan(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            addWindow.Content = grid;
            addWindow.ShowDialog();
        }

        public void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {selectedEmployee.EmployeeName}?",
                                             "Подтверждение удаления", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Employee.Remove(selectedEmployee);
                    _context.SaveChanges();
                    LoadEmployees();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.");
            }
        }
    }
}