using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AnimalShelter
{
    public partial class AnimalCardWindow : Window
    {
        public AnimalCardWindow(Animal animal)
        {
            InitializeComponent();

            if (animal == null)
            {
                Close();
                return;
            }

            TxtHeader.Text = $"Карточка животного — {animal.Nickname}";
            TxtNickname.Text = animal.Nickname;
            TxtSubtitle.Text = $"{animal.SpeciesName} • {animal.BreedName}";

            TxtSpecies.Text = animal.SpeciesName;
            TxtBreed.Text = animal.BreedName;
            TxtAge.Text = animal.Age.HasValue ? $"{animal.Age} лет" : "не указан";
            TxtGender.Text = animal.GenderName;
            TxtAdmission.Text = animal.AdmissionDate?.ToString("dd.MM.yyyy") ?? "не указана";
            TxtStatus.Text = animal.IsAdopted ? "Усыновлён" : "В приюте";
            TxtHealth.Text = string.IsNullOrWhiteSpace(animal.HealthStatus) ? "нет данных" : animal.HealthStatus;

            LoadPhoto(animal);
        }

        // загрузка фото
        private void LoadPhoto(Animal animal)
        {
            if (string.IsNullOrWhiteSpace(animal.PhotoPath)) return;

            string path = Path.Combine(PhotoHelper.PhotosFolder, animal.PhotoPath);
            if (!File.Exists(path)) return;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(path);
            bmp.EndInit();
            ImgPhoto.Source = bmp;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}