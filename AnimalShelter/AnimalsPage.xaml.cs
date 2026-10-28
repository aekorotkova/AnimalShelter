using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    public partial class AnimalsPage : UserControl
    {
        private List<Animal> _allAnimals = new List<Animal>();

        public AnimalsPage()
        {
            InitializeComponent();
            ApplyRolePermissions();
            LoadData();
        }

        // сотрудник не может удалять и менять статус усыновления
        private void ApplyRolePermissions()
        {
            if (!CurrentUser.IsAdmin)
            {
                BtnDelete.Visibility = Visibility.Collapsed;
                BtnToggleAdoption.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            try
            {
                _allAnimals = DatabaseHelper.GetAllAnimals();
                DgAnimals.ItemsSource = _allAnimals;
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
            var window = new AnimalEditWindow(null) { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnimals.SelectedItem is Animal selected)
            {
                var window = new AnimalEditWindow(selected) { Owner = Window.GetWindow(this) };
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите животное для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnimals.SelectedItem is Animal selected)
            {
                var result = MessageBox.Show($"Удалить животное «{selected.Nickname}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteAnimal(selected.AnimalID);
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
                MessageBox.Show("Выберите животное для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                DgAnimals.ItemsSource = _allAnimals;
            }
            else
            {
                DgAnimals.ItemsSource = _allAnimals.Where(a =>
                    (a.Nickname ?? "").ToLower().Contains(query) ||
                    (a.SpeciesName ?? "").ToLower().Contains(query) ||
                    (a.BreedName ?? "").ToLower().Contains(query) ||
                    (a.GenderName ?? "").ToLower().Contains(query) ||
                    (a.HealthStatus ?? "").ToLower().Contains(query)
                ).ToList();
            }
        }

        private void BtnToggleAdoption_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnimals.SelectedItem is Animal selected)
            {
                string newStatus = selected.IsAdopted ? "не усыновлён" : "усыновлён";
                var result = MessageBox.Show($"Изменить статус животного «{selected.Nickname}» на «{newStatus}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        selected.IsAdopted = !selected.IsAdopted;
                        DatabaseHelper.UpdateAnimal(selected);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите животное.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DgAnimals_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgAnimals.SelectedItem is Animal selected)
            {
                var card = new AnimalCardWindow(selected) { Owner = Window.GetWindow(this) };
                card.ShowDialog();
            }
        }
    }
}