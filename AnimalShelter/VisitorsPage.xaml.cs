using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class VisitorsPage : UserControl
    {
        private List<Visitor> _allVisitors = new List<Visitor>();

        public VisitorsPage()
        {
            InitializeComponent();
            ApplyRolePermissions();
            LoadData();
        }

        // сотрудник не может удалять посетителей
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
                _allVisitors = DatabaseHelper.GetAllVisitors();
                DgVisitors.ItemsSource = _allVisitors;
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
            var window = new VisitorEditWindow(null) { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgVisitors.SelectedItem is Visitor selected)
            {
                var window = new VisitorEditWindow(selected) { Owner = Window.GetWindow(this) };
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите посетителя для редактирования.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgVisitors.SelectedItem is Visitor selected)
            {
                var result = MessageBox.Show($"Удалить посетителя «{selected.LastName} {selected.FirstName}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteVisitor(selected.VisitorID);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления:\n" + ex.Message,
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите посетителя для удаления.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                DgVisitors.ItemsSource = _allVisitors;
            }
            else
            {
                DgVisitors.ItemsSource = _allVisitors.Where(v =>
                    (v.LastName ?? "").ToLower().Contains(query) ||
                    (v.FirstName ?? "").ToLower().Contains(query) ||
                    (v.Patronymic ?? "").ToLower().Contains(query) ||
                    (v.Phone ?? "").ToLower().Contains(query) ||
                    (v.PurposeName ?? "").ToLower().Contains(query)
                ).ToList();
            }
        }
    }
}