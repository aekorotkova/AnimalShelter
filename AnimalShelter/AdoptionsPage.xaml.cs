using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class AdoptionsPage : UserControl
    {
        private List<Adoption> _allAdoptions = new List<Adoption>();

        public AdoptionsPage()
        {
            InitializeComponent();
            ApplyRolePermissions();
            LoadData();
        }

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
                _allAdoptions = DatabaseHelper.GetAllAdoptions();
                DgAdoptions.ItemsSource = _allAdoptions;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            TxtSearch.Clear();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AdoptionEditWindow { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgAdoptions.SelectedItem is Adoption selected)
            {
                var result = MessageBox.Show(
                    $"Удалить запись об усыновлении животного «{selected.AnimalNickname}»?\n\n" +
                    "Животное вернётся в статус «не усыновлён».", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteAdoption(selected.AdoptionID, selected.AnimalID);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                DgAdoptions.ItemsSource = _allAdoptions;
            }
            else
            {
                DgAdoptions.ItemsSource = _allAdoptions.Where(a =>
                    (a.AnimalNickname ?? "").ToLower().Contains(query) ||
                    (a.VisitorName ?? "").ToLower().Contains(query) ||
                    (a.EmployeeName ?? "").ToLower().Contains(query)
                ).ToList();
            }
        }

        // кнопка «Памятка» — открывает окно с данными о владельце
        private void BtnMemo_Click(object sender, RoutedEventArgs e)
        {
            if (DgAdoptions.SelectedItem is Adoption selected)
            {
                var memo = new AdoptionMemoWindow(selected) { Owner = Window.GetWindow(this) };
                memo.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите запись об усыновлении.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}