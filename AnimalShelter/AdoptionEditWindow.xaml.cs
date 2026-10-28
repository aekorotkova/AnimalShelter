using System;
using System.Windows;

namespace AnimalShelter
{
    public partial class AdoptionEditWindow : Window
    {
        public AdoptionEditWindow()
        {
            InitializeComponent();

            DpAdoptionDate.SelectedDate = DateTime.Today;
            // по умолчанию напомнить через 2 недели
            DpNextContact.SelectedDate = DateTime.Today.AddDays(14);

            try
            {
                CmbAnimal.ItemsSource = DatabaseHelper.GetAvailableAnimals();
                CmbVisitor.ItemsSource = DatabaseHelper.GetVisitorsForCombo();
                CmbEmployee.ItemsSource = DatabaseHelper.GetEmployeesForCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки справочников:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbAnimal.SelectedValue == null)
            {
                MessageBox.Show("Выберите животное.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbVisitor.SelectedValue == null)
            {
                MessageBox.Show("Выберите посетителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (DpAdoptionDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату усыновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var adoption = new Adoption
            {
                AnimalID = (int)CmbAnimal.SelectedValue,
                VisitorID = (int)CmbVisitor.SelectedValue,
                EmployeeID = (int)CmbEmployee.SelectedValue,
                AdoptionDate = DpAdoptionDate.SelectedDate.Value,
                NextContactDate = DpNextContact.SelectedDate,
                Notes = TxtNotes.Text.Trim()
            };

            try
            {
                DatabaseHelper.AddAdoption(adoption);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка оформления:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}