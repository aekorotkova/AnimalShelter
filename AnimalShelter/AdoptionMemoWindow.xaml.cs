using System;
using System.Windows;

namespace AnimalShelter
{
    public partial class AdoptionMemoWindow : Window
    {
        private readonly Adoption _adoption;

        public AdoptionMemoWindow(Adoption adoption)
        {
            InitializeComponent();
            _adoption = adoption;

            if (adoption == null)
            {
                Close();
                return;
            }

            TxtAnimal.Text = adoption.AnimalNickname;
            TxtVisitor.Text = adoption.VisitorName;
            TxtPhone.Text = string.IsNullOrWhiteSpace(adoption.VisitorPhone) ? "не указан" : adoption.VisitorPhone;
            TxtAdoptionDate.Text = adoption.AdoptionDate.ToString("dd.MM.yyyy");
            TxtNextContact.Text = adoption.NextContactDate.HasValue
                ? adoption.NextContactDate.Value.ToString("dd.MM.yyyy")
                : "не запланирован";
            TxtNotes.Text = string.IsNullOrWhiteSpace(adoption.Notes) ? "нет заметок" : adoption.Notes;
        }

        // копируем телефон владельца в буфер обмена
        private void BtnCopyPhone_Click(object sender, RoutedEventArgs e)
        {
            if (_adoption == null || string.IsNullOrWhiteSpace(_adoption.VisitorPhone))
            {
                MessageBox.Show("Телефон не указан.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                Clipboard.SetText(_adoption.VisitorPhone);
                MessageBox.Show($"Телефон «{_adoption.VisitorPhone}» скопирован в буфер обмена.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка копирования:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}