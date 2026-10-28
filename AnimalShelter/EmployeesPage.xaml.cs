using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class EmployeesPage : UserControl
    {
        private List<Employee> _allEmployees = new List<Employee>();

        public EmployeesPage()
        {
            InitializeComponent();
            ApplyRolePermissions();
            LoadData();
        }

        // сотрудник не может удалять сотрудников
        private void ApplyRolePermissions()
        {
            if (!CurrentUser.IsAdmin)
            {
                BtnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            try
            {
                _allEmployees = DatabaseHelper.GetAllEmployees();
                DgEmployees.ItemsSource = _allEmployees;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            TxtSearch.Clear();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new EmployeeEditWindow(null) { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee selected)
            {
                var window = new EmployeeEditWindow(selected) { Owner = Window.GetWindow(this) };
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee selected)
            {
                var result = MessageBox.Show($"Удалить сотрудника «{selected.LastName} {selected.FirstName}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteEmployee(selected.EmployeeID);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления. Возможно, у сотрудника есть связанные записи.\n\n" + ex.Message,
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                DgEmployees.ItemsSource = _allEmployees;
            }
            else
            {
                DgEmployees.ItemsSource = _allEmployees.Where(emp =>
                    (emp.LastName ?? "").ToLower().Contains(query) ||
                    (emp.FirstName ?? "").ToLower().Contains(query) ||
                    (emp.Patronymic ?? "").ToLower().Contains(query) ||
                    (emp.Position ?? "").ToLower().Contains(query) ||
                    (emp.Phone ?? "").ToLower().Contains(query)
                ).ToList();
            }
        }
    }
}