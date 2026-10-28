using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace AnimalShelter
{
    public partial class AnimalEditWindow : Window
    {
        public Animal ResultAnimal { get; private set; }
        private readonly Animal _editingAnimal;

        // временное имя файла фото, которое применится при сохранении
        private string _currentPhotoName;

        public AnimalEditWindow(Animal animal)
        {
            InitializeComponent();
            _editingAnimal = animal;

            LoadSpeciesAndGenders();

            if (animal == null)
            {
                TxtHeader.Text = "Добавление животного";
            }
            else
            {
                TxtHeader.Text = "Редактирование животного";
                FillFields(animal);
            }
        }

        private void LoadSpeciesAndGenders()
        {
            CmbSpecies.ItemsSource = DatabaseHelper.GetSpeciesList();
            CmbGender.ItemsSource = DatabaseHelper.GetGendersList();
        }

        private void FillFields(Animal a)
        {
            TxtNickname.Text = a.Nickname;
            TxtAge.Text = a.Age?.ToString() ?? "";
            TxtHealth.Text = a.HealthStatus;
            DpAdmissionDate.SelectedDate = a.AdmissionDate;
            ChkIsAdopted.IsChecked = a.IsAdopted;
            CmbSpecies.SelectedValue = a.SpeciesID;
            LoadBreeds(a.SpeciesID);
            CmbBreed.SelectedValue = a.BreedID;
            CmbGender.SelectedValue = a.GenderID;

            // если у животного уже есть фото — показываем
            _currentPhotoName = a.PhotoPath;
            ShowPhoto(_currentPhotoName);
        }

        private void CmbSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSpecies.SelectedValue is int speciesId)
            {
                LoadBreeds(speciesId);
            }
        }

        private void LoadBreeds(int speciesId)
        {
            CmbBreed.ItemsSource = DatabaseHelper.GetBreedsBySpecies(speciesId);
        }

        // показ фото в превью
        private void ShowPhoto(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                ImgPhoto.Source = null;
                return;
            }

            string fullPath = Path.Combine(PhotoHelper.PhotosFolder, fileName);
            if (!File.Exists(fullPath))
            {
                ImgPhoto.Source = null;
                return;
            }

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(fullPath);
            bmp.EndInit();
            ImgPhoto.Source = bmp;
        }

        private void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото животного"
            };

            if (dialog.ShowDialog() != true) return;

            // копируем файл в папку Photos
            string nickname = string.IsNullOrWhiteSpace(TxtNickname.Text) ? "animal" : TxtNickname.Text.Trim();
            string imported = PhotoHelper.ImportPhoto(dialog.FileName, nickname);

            if (imported != null)
            {
                // удаляем предыдущий файл, если был
                if (!string.IsNullOrWhiteSpace(_currentPhotoName) && _currentPhotoName != imported)
                    PhotoHelper.DeletePhoto(_currentPhotoName);

                _currentPhotoName = imported;
                ShowPhoto(_currentPhotoName);
            }
        }

        private void BtnRemovePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentPhotoName)) return;

            var result = MessageBox.Show("Удалить фото?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            PhotoHelper.DeletePhoto(_currentPhotoName);
            _currentPhotoName = null;
            ImgPhoto.Source = null;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNickname.Text))
            {
                MessageBox.Show("Введите кличку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbSpecies.SelectedValue == null)
            {
                MessageBox.Show("Выберите вид.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbBreed.SelectedValue == null)
            {
                MessageBox.Show("Выберите породу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbGender.SelectedValue == null)
            {
                MessageBox.Show("Выберите пол.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? age = null;
            if (!string.IsNullOrWhiteSpace(TxtAge.Text))
            {
                if (int.TryParse(TxtAge.Text, out int parsed))
                    age = parsed;
                else
                {
                    MessageBox.Show("Возраст должен быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var result = new Animal
            {
                AnimalID = _editingAnimal?.AnimalID ?? 0,
                Nickname = TxtNickname.Text.Trim(),
                SpeciesID = (int)CmbSpecies.SelectedValue,
                BreedID = (int)CmbBreed.SelectedValue,
                Age = age,
                GenderID = (int)CmbGender.SelectedValue,
                HealthStatus = TxtHealth.Text.Trim(),
                AdmissionDate = DpAdmissionDate.SelectedDate,
                IsAdopted = ChkIsAdopted.IsChecked == true,
                PhotoPath = _currentPhotoName
            };

            try
            {
                if (_editingAnimal == null)
                    DatabaseHelper.AddAnimal(result);
                else
                    DatabaseHelper.UpdateAnimal(result);

                ResultAnimal = result;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}