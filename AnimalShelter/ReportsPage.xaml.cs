using System;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class ReportsPage : UserControl
    {
        public ReportsPage()
        {
            InitializeComponent();

            DpFrom.SelectedDate = DateTime.Today.AddDays(-30);
            DpTo.SelectedDate = DateTime.Today;

            LoadAllReports();
        }

        private void LoadAllReports()
        {
            LoadInShelterReport();
            LoadAdoptedReport();
            LoadSpeciesReport();
        }

        private void LoadInShelterReport()
        {
            try
            {
                var data = DatabaseHelper.GetAnimalsInShelterReport();
                DgInShelter.ItemsSource = data;
                TxtInShelterCount.Text = $"Всего в приюте: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчёта «Животные в приюте»:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAdoptedReport()
        {
            try
            {
                var data = DatabaseHelper.GetAdoptedAnimalsReport();
                DgAdopted.ItemsSource = data;
                TxtAdoptedCount.Text = $"Всего усыновлено: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчёта «Усыновлённые»:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSpeciesReport()
        {
            try
            {
                var data = DatabaseHelper.GetSpeciesStatistics();
                DgSpecies.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчёта «По видам»:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefreshInShelter_Click(object sender, RoutedEventArgs e) => LoadInShelterReport();
        private void BtnRefreshAdopted_Click(object sender, RoutedEventArgs e) => LoadAdoptedReport();
        private void BtnRefreshSpecies_Click(object sender, RoutedEventArgs e) => LoadSpeciesReport();

        private void BtnBuildPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (DpFrom.SelectedDate == null || DpTo.SelectedDate == null)
            {
                MessageBox.Show("Выберите обе даты.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpFrom.SelectedDate > DpTo.SelectedDate)
            {
                MessageBox.Show("Дата «С» не может быть позже даты «По».",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var data = DatabaseHelper.GetAdoptionsByPeriod(
                    DpFrom.SelectedDate.Value, DpTo.SelectedDate.Value);
                DgPeriod.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчёта «За период»:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnExportInShelter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = DatabaseHelper.GetAnimalsInShelterReport();
                string path = ExcelExporter.ExportToCsv(data, "Животные_в_приюте");
                MessageBox.Show($"Отчёт сохранён на рабочий стол:\n{path}",
                    "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportAdopted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = DatabaseHelper.GetAdoptedAnimalsReport();
                string path = ExcelExporter.ExportToCsv(data, "Усыновлённые_животные");
                MessageBox.Show($"Отчёт сохранён на рабочий стол:\n{path}",
                    "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportSpecies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = DatabaseHelper.GetSpeciesStatistics();
                string path = ExcelExporter.ExportToCsv(data, "Статистика_по_видам");
                MessageBox.Show($"Отчёт сохранён на рабочий стол:\n{path}",
                    "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (DpFrom.SelectedDate == null || DpTo.SelectedDate == null)
            {
                MessageBox.Show("Выберите обе даты.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var data = DatabaseHelper.GetAdoptionsByPeriod(
                    DpFrom.SelectedDate.Value, DpTo.SelectedDate.Value);
                string path = ExcelExporter.ExportToCsv(data, "Усыновления_за_период");
                MessageBox.Show($"Отчёт сохранён на рабочий стол:\n{path}",
                    "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // строим напоминания о контактах за выбранное число дней
        private void BtnBuildReminders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int days = GetSelectedDays();
                var data = DatabaseHelper.GetUpcomingContacts(days);
                DgReminders.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчёта «Напоминания»:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // экспорт напоминаний в excel
        private void BtnExportReminders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int days = GetSelectedDays();
                var data = DatabaseHelper.GetUpcomingContacts(days);
                string path = ExcelExporter.ExportToCsv(data, "Напоминания_о_контакте");
                MessageBox.Show($"Отчёт сохранён на рабочий стол:\n{path}",
                    "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // считываем выбранный период
        private int GetSelectedDays()
        {
            switch (CmbContactDays.SelectedIndex)
            {
                case 0: return 7;
                case 1: return 14;
                case 2: return 30;
                case 3: return 90;
                default: return 30;
            }
        }
    }
}