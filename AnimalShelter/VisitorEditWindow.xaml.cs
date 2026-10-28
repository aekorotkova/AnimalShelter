using System;
using System.Windows;

namespace AnimalShelter
{
    public partial class VisitorEditWindow : Window
    {
        public Visitor ResultVisitor { get; private set; }
        private readonly Visitor _editingVisitor;

        public VisitorEditWindow(Visitor visitor)
        {
            InitializeComponent();
            _editingVisitor = visitor;

            CmbPurpose.ItemsSource = DatabaseHelper.GetPurposesList();

            if (visitor == null)
            {
                TxtHeader.Text = "Добавление посетителя";
            }
            else
            {
                TxtHeader.Text = "Редактирование посетителя";
                FillFields(visitor);
            }
        }

        private void FillFields(Visitor v)
        {
            TxtLastName.Text = v.LastName;
            TxtFirstName.Text = v.FirstName;
            TxtPatronymic.Text = v.Patronymic;
            TxtPhone.Text = v.Phone;
            CmbPurpose.SelectedValue = v.PurposeID;
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
            if (CmbPurpose.SelectedValue == null)
            {
                MessageBox.Show("Выберите цель визита.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = new Visitor
            {
                VisitorID = _editingVisitor?.VisitorID ?? 0,
                LastName = TxtLastName.Text.Trim(),
                FirstName = TxtFirstName.Text.Trim(),
                Patronymic = TxtPatronymic.Text.Trim(),
                Phone = TxtPhone.Text.Trim(),
                PurposeID = (int)CmbPurpose.SelectedValue
            };

            try
            {
                if (_editingVisitor == null)
                    DatabaseHelper.AddVisitor(result);
                else
                    DatabaseHelper.UpdateVisitor(result);

                ResultVisitor = result;
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