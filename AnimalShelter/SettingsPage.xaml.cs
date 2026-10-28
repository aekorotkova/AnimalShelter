using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AnimalShelter
{
    /// <summary>
    /// Вспомогательный класс для отображения строки справочника в DataGrid.
    /// </summary>
    public class DictionaryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }  // для пород — название вида
        public int? ParentId { get; set; }      // для пород — SpeciesID
    }

    public partial class SettingsPage : UserControl
    {
        // Текущий выбранный тип справочника: species / breeds / genders / purposes
        private string _currentDict = "species";

        public SettingsPage()
        {
            InitializeComponent();
            TxtCurrentUser.Text = $"Пользователь: {CurrentUser.Login} ({CurrentUser.Role})";

            // Загружаем данные только когда контрол полностью загружен
            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDictionary();
        }

        // СПРАВОЧНИКИ
        private void CmbDictType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgDictionary == null) return;   // ← защита
            if (CmbDictType.SelectedIndex < 0) return;

            switch (CmbDictType.SelectedIndex)
            {
                case 0: _currentDict = "species"; break;
                case 1: _currentDict = "breeds"; break;
                case 2: _currentDict = "genders"; break;
                case 3: _currentDict = "purposes"; break;
            }

            LoadDictionary();
        }

        private void LoadDictionary()
        {
            if (DgDictionary == null) return;

            try
            {
                var items = new List<DictionaryItem>();

                switch (_currentDict)
                {
                    case "species":
                        foreach (var s in DatabaseHelper.GetSpeciesList())
                        {
                            items.Add(new DictionaryItem { Id = s.Item1, Name = s.Item2, ParentName = "" });
                        }
                        break;

                    case "genders":
                        foreach (var g in DatabaseHelper.GetGendersList())
                        {
                            items.Add(new DictionaryItem { Id = g.Item1, Name = g.Item2, ParentName = "" });
                        }
                        break;

                    case "purposes":
                        foreach (var p in DatabaseHelper.GetPurposesList())
                        {
                            items.Add(new DictionaryItem { Id = p.Item1, Name = p.Item2, ParentName = "" });
                        }
                        break;

                    case "breeds":
                        var species = DatabaseHelper.GetSpeciesList();
                        var speciesDict = new Dictionary<int, string>();
                        foreach (var s in species)
                            speciesDict[s.Item1] = s.Item2;

                        foreach (var b in DatabaseHelper.GetAllBreeds())
                        {
                            items.Add(new DictionaryItem
                            {
                                Id = b.Item1,
                                Name = b.Item2,
                                ParentId = b.Item3,
                                ParentName = speciesDict.ContainsKey(b.Item3) ? speciesDict[b.Item3] : ""
                            });
                        }
                        break;
                }

                DgDictionary.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки справочника:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDictRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDictionary();
        }

        private void BtnDictAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = GetDictTitle();

            if (_currentDict == "breeds")
            {
                OpenBreedEditWindow(null);
                return;
            }

            string name = PromptForName($"Добавление в «{title}»", "Название:");
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                switch (_currentDict)
                {
                    case "species": DatabaseHelper.AddSpecies(name); break;
                    case "genders": DatabaseHelper.AddGender(name); break;
                    case "purposes": DatabaseHelper.AddPurpose(name); break;
                }
                LoadDictionary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDictEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(DgDictionary.SelectedItem is DictionaryItem selected))
            {
                MessageBox.Show("Выберите запись для редактирования.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_currentDict == "breeds")
            {
                OpenBreedEditWindow(selected);
                return;
            }

            string title = GetDictTitle();
            string name = PromptForName($"Редактирование в «{title}»", "Новое название:", selected.Name);
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                switch (_currentDict)
                {
                    case "species": DatabaseHelper.UpdateSpecies(selected.Id, name); break;
                    case "genders": DatabaseHelper.UpdateGender(selected.Id, name); break;
                    case "purposes": DatabaseHelper.UpdatePurpose(selected.Id, name); break;
                }
                LoadDictionary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка редактирования:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDictDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(DgDictionary.SelectedItem is DictionaryItem selected))
            {
                MessageBox.Show("Выберите запись для удаления.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить запись «{selected.Name}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                switch (_currentDict)
                {
                    case "species": DatabaseHelper.DeleteSpecies(selected.Id); break;
                    case "genders": DatabaseHelper.DeleteGender(selected.Id); break;
                    case "purposes": DatabaseHelper.DeletePurpose(selected.Id); break;
                    case "breeds": DatabaseHelper.DeleteBreed(selected.Id); break;
                }
                LoadDictionary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления. Возможно, запись используется в других таблицах.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDictTitle()
        {
            switch (_currentDict)
            {
                case "species": return "Виды животных";
                case "breeds": return "Породы";
                case "genders": return "Пол";
                case "purposes": return "Цели визита";
                default: return "";
            }
        }

        private string PromptForName(string title, string label, string defaultValue = "")
        {
            var window = new Window
            {
                Title = title,
                Width = 400,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.WhiteSmoke
            };

            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 6)
            });

            var textBox = new TextBox
            {
                Text = defaultValue,
                Height = 35,
                FontSize = 14,
                Padding = new Thickness(8, 0, 8, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1)
            };
            panel.Children.Add(textBox);

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };

            var btnOk = new Button
            {
                Content = "OK",
                Width = 100,
                Height = 32,
                Margin = new Thickness(0, 0, 10, 0),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A6FA5")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            var btnCancel = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 32,
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B0BEC5")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            string result = null;
            btnOk.Click += (s, args) => { result = textBox.Text.Trim(); window.Close(); };
            btnCancel.Click += (s, args) => { window.Close(); };

            buttons.Children.Add(btnOk);
            buttons.Children.Add(btnCancel);
            panel.Children.Add(buttons);

            window.Content = panel;
            window.ShowDialog();

            return result;
        }

        /// <summary>
        /// Открывает окно редактирования породы (с выбором вида).
        /// </summary>
        private void OpenBreedEditWindow(DictionaryItem existing)
        {
            var window = new Window
            {
                Title = existing == null ? "Добавление породы" : "Редактирование породы",
                Width = 420,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.WhiteSmoke
            };

            var panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new TextBlock
            {
                Text = "Вид животного *",
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 6)
            });

            var cmbSpecies = new ComboBox
            {
                Height = 32,
                FontSize = 13,
                DisplayMemberPath = "Item2",
                SelectedValuePath = "Item1",
                VerticalContentAlignment = VerticalAlignment.Center,
                ItemsSource = DatabaseHelper.GetSpeciesList()
            };
            if (existing != null && existing.ParentId.HasValue)
                cmbSpecies.SelectedValue = existing.ParentId.Value;
            panel.Children.Add(cmbSpecies);

            panel.Children.Add(new TextBlock
            {
                Text = "Название породы *",
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 15, 0, 6)
            });

            var textBox = new TextBox
            {
                Text = existing?.Name ?? "",
                Height = 35,
                FontSize = 14,
                Padding = new Thickness(8, 0, 8, 0),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            panel.Children.Add(textBox);

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var btnOk = new Button
            {
                Content = "Сохранить",
                Width = 110,
                Height = 32,
                Margin = new Thickness(0, 0, 10, 0),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A6FA5")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            var btnCancel = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 32,
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B0BEC5")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnOk.Click += (s, args) =>
            {
                if (cmbSpecies.SelectedValue == null)
                {
                    MessageBox.Show("Выберите вид.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MessageBox.Show("Введите название породы.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    int speciesId = (int)cmbSpecies.SelectedValue;
                    string name = textBox.Text.Trim();

                    if (existing == null)
                        DatabaseHelper.AddBreed(name, speciesId);
                    else
                        DatabaseHelper.UpdateBreed(existing.Id, name, speciesId);

                    window.Close();
                    LoadDictionary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения:\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            btnCancel.Click += (s, args) => window.Close();

            buttons.Children.Add(btnOk);
            buttons.Children.Add(btnCancel);
            panel.Children.Add(buttons);

            window.Content = panel;
            window.ShowDialog();
        }

        // ==============================================
        // СМЕНА ПАРОЛЯ
        // ==============================================

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = TxtOldPassword.Password;
            string newPass = TxtNewPassword.Password;
            string confirm = TxtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(oldPass) ||
                string.IsNullOrWhiteSpace(newPass) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPass.Length < 4)
            {
                MessageBox.Show("Новый пароль должен быть не короче 4 символов.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPass != confirm)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = DatabaseHelper.ChangePassword(CurrentUser.Login, oldPass, newPass);

                if (success)
                {
                    MessageBox.Show("Пароль успешно изменён.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtOldPassword.Clear();
                    TxtNewPassword.Clear();
                    TxtConfirmPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Старый пароль неверный.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка смены пароля:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}