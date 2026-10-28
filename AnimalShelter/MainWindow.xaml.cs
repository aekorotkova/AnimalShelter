using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (CurrentUser.Role != null)
            {
                TxtUserName.Text = $"{CurrentUser.Login} ({CurrentUser.Role})";
            }

            ApplyRolePermissions();

            MainContent.Content = new DashboardPage();
        }

        // разделы, недоступные сотруднику
        private void ApplyRolePermissions()
        {
            if (!CurrentUser.IsAdmin)
            {
                BtnEmployees.Visibility = Visibility.Collapsed;
                BtnSettings.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardPage();
        }

        private void BtnAnimals_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AnimalsPage();
        }

        private void BtnVisitors_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VisitorsPage();
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EmployeesPage();
        }

        private void BtnAdoptions_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AdoptionsPage();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReportsPage();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsPage();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        public void NavigateToReports()
        {
            MainContent.Content = new ReportsPage();
        }

        public void NavigateToAdoptions()
        {
            MainContent.Content = new AdoptionsPage();
        }
    }
}