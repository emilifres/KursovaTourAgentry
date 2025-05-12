using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ToursApp
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReports();
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += NavigationService_Navigated;
            }
        }

        void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is ReportsPage)
            {
                LoadReports();
            }
        }

        void LoadReports()
        {
            try
            {
                using (var context = new TourAgentryEntities())
                {
                    // Принудительно очищаем кэш, чтобы получить свежие данные
                    context.Database.ExecuteSqlCommand("DBCC DROPCLEANBUFFERS");
                    var reports = context.Report.ToList();
                    foreach (var report in reports)
                    {
                        System.Diagnostics.Debug.WriteLine($"Id: {report.Id}, EmployeeName: {report.EmployeeName}, Position: {report.Position}, Month: {report.Month?.ToString("MMMM yyyy") ?? "null"}, ToursBooked: {report.ToursBooked}, ClientsServed: {report.ClientsServed}, CreationDate: {report.CreationDate?.ToString("dd/MM/yyyy") ?? "null"}");
                    }
                    DGridHotels.ItemsSource = reports;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new ReportFormationPage());
            }
        }

        void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var reportsForRemoving = DGridHotels.SelectedItems.Cast<Report>().ToList();

            if (reportsForRemoving.Count() == 0)
            {
                MessageBox.Show("Выберите хотя бы один отчёт для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить следующие {reportsForRemoving.Count()} отчетов?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string folderPath = Path.Combine(Environment.CurrentDirectory, "resorcer");
                    using (var context = new TourAgentryEntities())
                    {
                        foreach (var report in reportsForRemoving)
                        {
                            var reportToRemove = context.Report.FirstOrDefault(r => r.Id == report.Id);
                            if (reportToRemove != null)
                            {
                                string fileToDelete = Path.Combine(folderPath, $"Report_{reportToRemove.Id}.txt");
                                if (File.Exists(fileToDelete))
                                    File.Delete(fileToDelete);

                                context.Report.Remove(reportToRemove);
                            }
                        }
                        context.SaveChanges();
                    }

                    LoadReports();
                    MessageBox.Show("Отчёты удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null || btn.Tag == null) return;

                int reportId = Convert.ToInt32(btn.Tag);
                using (var context = new TourAgentryEntities())
                {
                    var report = context.Report.FirstOrDefault(r => r.Id == reportId);
                    if (report == null)
                    {
                        MessageBox.Show("Отчет не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string folderPath = Path.Combine(Environment.CurrentDirectory, "resorcer");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, $"Report_{report.Id}.txt");
                    StringBuilder reportContent = new StringBuilder();
                    reportContent.AppendLine("Отчет по деятельности сотрудника");
                    reportContent.AppendLine($"ID: {report.Id}");
                    reportContent.AppendLine($"Сотрудник: {report.EmployeeName ?? "Не указано"}");
                    reportContent.AppendLine($"Должность: {report.Position ?? "Не указано"}");
                    reportContent.AppendLine($"Месяц: {(report.Month.HasValue ? report.Month.Value.ToString("MMMM yyyy") : "Не указано")}");
                    reportContent.AppendLine($"Забронировано туров: {report.ToursBooked ?? 0}");
                    reportContent.AppendLine($"Клиентов в общем: {report.ClientsServed ?? 0}");
                    reportContent.AppendLine($"Дата создания отчета: {(report.CreationDate.HasValue ? report.CreationDate.Value.ToString("dd/MM/yyyy HH:mm") : "Не указано")}");

                    File.WriteAllText(filePath, reportContent.ToString());
                    MessageBox.Show($"Отчет успешно сохранен в {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}