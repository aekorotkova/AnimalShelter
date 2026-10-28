using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();

            // приветствие
            TxtWelcome.Text = $"Добро пожаловать, {CurrentUser.Login}!";

            LoadCounters();
            LoadReminders();
            LoadOldestAnimals();
        }

        // загружаем счётчики
        private void LoadCounters()
        {
            try
            {
                var counters = DatabaseHelper.GetDashboardCounters();
                TxtAnimalsInShelter.Text = counters.AnimalsInShelter.ToString();
                TxtAdoptedLastMonth.Text = counters.AdoptedLastMonth.ToString();
                TxtVisitors.Text = counters.TotalVisitors.ToString();
                TxtEmployees.Text = counters.TotalEmployees.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки счётчиков:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // напоминания о контакте на 7 дней вперёд
        private void LoadReminders()
        {
            try
            {
                var reminders = DatabaseHelper.GetUpcomingContacts(7);

                if (reminders.Count == 0)
                {
                    ListReminders.Visibility = Visibility.Collapsed;
                    TxtNoReminders.Visibility = Visibility.Visible;
                }
                else
                {
                    ListReminders.ItemsSource = reminders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки напоминаний:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // животные, которые давно в приюте (последние 5)
        private void LoadOldestAnimals()
        {
            try
            {
                var animals = DatabaseHelper.GetOldestAnimals(5);
                ListOldestAnimals.ItemsSource = animals;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка животных:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // быстрые действия
        private void BtnQuickAddAnimal_Click(object sender, RoutedEventArgs e)
        {
            var window = new AnimalEditWindow(null) { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadCounters();
                LoadOldestAnimals();
            }
        }

        private void BtnQuickAddVisitor_Click(object sender, RoutedEventArgs e)
        {
            var window = new VisitorEditWindow(null) { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadCounters();
            }
        }

        private void BtnGoReports_Click(object sender, RoutedEventArgs e)
        {
            // находим главное окно и переключаемся на отчёты
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigateToReports();
        }

        private void BtnGoAdoptions_Click(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigateToAdoptions();
        }
    }
}