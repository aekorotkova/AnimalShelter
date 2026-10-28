using System;
using System.Windows;

namespace AnimalShelter
{
    public partial class EmployeeEditWindow : Window
    {
        public Employee ResultEmployee { get; private set; }
        private readonly Employee _editingEmployee;

        public EmployeeEditWindow(Employee employee)
        {
            InitializeComponent();
            _editingEmployee = employee;

            if (employee == null)
            {
                TxtHeader.Text = "Добавление сотрудника";
            }
            else
            {
                TxtHeader.Text = "Редактирование сотрудника";
                FillFields(employee);
            }
        }

        private void FillFields(Employee emp)
        {
            TxtLastName.Text = emp.LastName;
            TxtFirstName.Text = emp.FirstName;
            TxtPatronymic.Text = emp.Patronymic;
            TxtPosition.Text = emp.Position;
            TxtPhone.Text = emp.Phone;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                MessageBox.Show("Введите фамилию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text))
            {
                MessageBox.Show("Введите имя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = new Employee
            {
                EmployeeID = _editingEmployee?.EmployeeID ?? 0,
                LastName = TxtLastName.Text.Trim(),
                FirstName = TxtFirstName.Text.Trim(),
                Patronymic = TxtPatronymic.Text.Trim(),
                Position = TxtPosition.Text.Trim(),
                Phone = TxtPhone.Text.Trim()
            };

            try
            {
                if (_editingEmployee == null)
                    DatabaseHelper.AddEmployee(result);
                else
                    DatabaseHelper.UpdateEmployee(result);

                ResultEmployee = result;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message,
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