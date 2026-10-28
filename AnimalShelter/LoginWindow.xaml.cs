using System;
using System.Windows;

namespace AnimalShelter
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните логин и пароль.",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string role = DatabaseHelper.Authenticate(login, password);

                if (role != null)
                {
                    CurrentUser.Login = login;
                    CurrentUser.Role = role;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль. Попробуйте снова.",
                        "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}