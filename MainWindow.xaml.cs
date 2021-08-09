using System;
using System.Windows;
using SharedApp.Classes;
using System.IO;
using Microsoft.Win32;

namespace SharedApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataProcessor processor = new DataProcessor();

        public MainWindow()
        {
            InitializeComponent();
        }



        //
        //Обработчик страницы приветствия
        //
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "";
            PasswdTextBox.Password = "";

            HelloPage.Visibility = Visibility.Hidden;
            LoginPage.Visibility = Visibility.Visible;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            LoginRegTextBox.Text = "";
            PasswdRegTextBox.Password = "";

            GetCountriesComboBox();
            HelloPage.Visibility = Visibility.Hidden;
            RegistrationPage.Visibility = Visibility.Visible;
        }



        //
        //Обработчики страницы входа
        //
        private void LogInButtonPage_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text != "" && PasswdTextBox.Password != "")
            {
                switch (processor.LoginFunction(LoginTextBox.Text, PasswdTextBox.Password))
                {
                    case 0:
                        LoginPage.Visibility = Visibility.Hidden;
                        GetUsersFiles();
                        GetUserInformationForMainPage();
                        GetAccessRightsComboBox();
                        GetFileTypeComboBox();
                        
                        MainPage.Visibility = Visibility.Visible;
                        break;

                    case 1:
                        MessageBox.Show("Введенный вами пользователь не существует!");
                        break;

                    case 2:
                        MessageBox.Show("Введен неверный пароль!");
                        break;

                    case 3:
                        MessageBox.Show("БД не отвечает, попробуйте позже =(");
                        LoginPage.Visibility = Visibility.Hidden;
                        HelloPage.Visibility = Visibility.Visible;

                        LoginTextBox.Text = "";
                        PasswdTextBox.Password = "";
                        break;
                }
            }
            else
                MessageBox.Show("Введите логин и пароль");

            if (processor.IsBanned())
            {
                UserMainGrid.Visibility = Visibility.Hidden;
                BanReportGrid.Visibility = Visibility.Visible;
                BanTextBlock.Text = processor.BanReason;
            }
        }
        
        private void BackLoginToMainButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "";
            PasswdTextBox.Password = "";

            LoginPage.Visibility = Visibility.Hidden;
            HelloPage.Visibility = Visibility.Visible;
        }



        //
        //Обработчики страницы регистрации
        //
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginRegTextBox.Text != "" && PasswdRegTextBox.Password != "")
            {   
                if (processor.RegistrationFunction(LoginRegTextBox.Text, PasswdRegTextBox.Password, CountriesComboBox.SelectedItem?.ToString()) == 0)
                {
                    RegistrationPage.Visibility = Visibility.Hidden;
                    GetUsersFiles();
                    GetUserInformationForMainPage();
                    GetAccessRightsComboBox();
                    GetFileTypeComboBox();
                    MainPage.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует");
                    RegistrationPage.Visibility = Visibility.Hidden;
                    HelloPage.Visibility = Visibility.Visible;
                    
                    LoginRegTextBox.Text = "";
                    PasswdRegTextBox.Password = "";
                    CountriesComboBox.SelectedItem = null;
                }
            }
            else
            {
                MessageBox.Show("Введите логин и пароль");
            }
        }

        private void BackRegisterToMainButton_Click(object sender, RoutedEventArgs e)
        {
            LoginRegTextBox.Text = "";
            PasswdRegTextBox.Password = "";
            CountriesComboBox.SelectedItem = null;

            RegistrationPage.Visibility = Visibility.Hidden;
            HelloPage.Visibility = Visibility.Visible;
        }



        //
        //Обработчики главной страницы
        //
        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                string filepath = dlg.FileName;
                processor.UploadFile(filepath, FileTypeComboBox.SelectedIndex, AccessRightComboBox.SelectedIndex);
                TrafficMainTextBox.Text = processor.user.MemorySize.ToString();
            }

            GetUsersFiles();
        }

        private void UserFilesListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (UserFilesListView.SelectedItem != null)
            {
                FilesTbl file = (FilesTbl)UserFilesListView.SelectedItem;

                file = processor.GetFullFileInformation(file);

                string fileName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\" + file.FileNam;
                File.WriteAllBytes(fileName, file.FileBin);

                file = null;
                UserFilesListView.SelectedItem = null; 

                MessageBox.Show("Файл успешно сохранен в папку \"Загрузки\".");
            }
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserFilesListView.SelectedItem != null)
            {
                FilesTbl file = (FilesTbl)UserFilesListView.SelectedItem;
                processor.DeleteFile(file);
            }

            GetUsersFiles();
            GetUserInformationForMainPage();
        }

        private void LookMyFilesButton_Click(object sender, RoutedEventArgs e)
        {
            UploadFileButton.Visibility = Visibility.Visible;
            DeleteFileButton.Visibility = Visibility.Visible;
            FileTypeComboBox.Visibility = Visibility.Visible;
            AccessRightComboBox.Visibility = Visibility.Visible;
            AddFileButton.Visibility = Visibility.Hidden;

            UserNameTextBox.Text = "";

            processor.CheckFiles();
            GetUsersFiles();
        }

        private void SearchUserButton_Click(object sender, RoutedEventArgs e)
        {   
            if (UserNameTextBox.Text != "")
            {
                int opRes = processor.SearchUser(UserNameTextBox.Text);
                if (opRes == 0)
                {
                    UploadFileButton.Visibility = Visibility.Hidden;
                    DeleteFileButton.Visibility = Visibility.Hidden;
                    FileTypeComboBox.Visibility = Visibility.Hidden;
                    AccessRightComboBox.Visibility = Visibility.Hidden;
                    AddFileButton.Visibility = Visibility.Visible;
                    GetUsersFiles();
                }
                else
                {
                    UserNameTextBox.Text = "";
                    MessageBox.Show("Данного пользователя не существует.");
                }
            }
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserFilesListView.SelectedItem != null)
            {
                processor.AddFiles((FilesTbl)UserFilesListView.SelectedItem);
                TrafficMainTextBox.Text = processor.user.MemorySize.ToString();
            }

        }



        //
        //Обработчики функционала администратора
        //
        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            UserMainGrid.Visibility = Visibility.Hidden;
            AdminMainGrid.Visibility = Visibility.Visible;
            GetBanListComboBox();
            GetUsersPlansComboBox();
            GetUsersLevelComboBox();
        }

        private void UserPanelButton_Click(object sender, RoutedEventArgs e)
        {
            AdminMainGrid.Visibility = Visibility.Hidden;
            UserMainGrid.Visibility = Visibility.Visible;
        }

        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserChangedTextBox.Text != "" && BanListComboBox.SelectedItem != null)
            {
                int res = processor.BanUser(UserChangedTextBox.Text, BanListComboBox.SelectedItem.ToString());

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Данный пользователь не существует.");
                        break;

                    case 2:
                        MessageBox.Show("Данный пользователь уже забанен.");
                        break;

                    default:
                        MessageBox.Show($"Пользователь {UserChangedTextBox.Text} забанен!");
                        break;
                }
            }
            else
                MessageBox.Show("Выберите нарушение и введите имя пользователя!");
        }

        private void UnBanButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserChangedTextBox.Text != "")
            {
                int res = processor.UnBanUser(UserChangedTextBox.Text);

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Данный пользователь не существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Пользователь {UserChangedTextBox.Text} разбанен!");
                        break;
                }
            }
            else
                MessageBox.Show("Введите имя пользователя!");
        }

        private void UpgradeUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserTrafficComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тарифный план.");
                return;
            }

            if (UserLevelComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите уровень.");
                return;
            }

            if (UserChangedTextBox.Text != "")
            {
                int res = processor.UpgradeUser(UserChangedTextBox.Text, int.Parse(UserTrafficComboBox.SelectedItem.ToString()), UserLevelComboBox.SelectedItem.ToString());

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Данный пользователь не существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Пользователь {UserChangedTextBox.Text} обновлен!");
                        break;
                }
            }
            else
                MessageBox.Show("Введите имя пользователя!");
        }

        private void AddFileTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileTypeTextBox.Text != "")
            {
                int res = processor.AddFileType(FileTypeTextBox.Text);
                
                switch (res)
                {
                    case 1:
                        MessageBox.Show("Такой тип уже существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Новый тип файла {FileTypeTextBox.Text} добавлен!");
                        GetFileTypeComboBox();
                        break;
                }

                FileTypeTextBox.Text = "";
            }
            else
                MessageBox.Show("Введите новый тип файла!");
        }

        private void AddCountryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CountryTextBox.Text != "")
            {
                int res = processor.AddNewCountry(CountryTextBox.Text);

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Такая страна уже существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Новая страна {CountryTextBox.Text} добавлена!");
                        break;
                }

                CountryTextBox.Text = "";
            }
            else
                MessageBox.Show("Введите новый тип файла!");
        }

        private void AddTrafficButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrafficTextBox.Text != "")
            {
                int res = processor.AddNewTrafficPlan(int.Parse(TrafficTextBox.Text));

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Такой план уже существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Новый план {TrafficTextBox.Text} добавлен!");
                        GetUsersPlansComboBox();
                        break;
                }

                TrafficTextBox.Text = "";
            }
        }

        private void AddBanButton_Click(object sender, RoutedEventArgs e)
        {
            if (BanTextBox.Text != "")
            {
                int res = processor.AddNewBan(BanTextBox.Text);

                switch (res)
                {
                    case 1:
                        MessageBox.Show("Такая причина бана уже существует.");
                        break;

                    case 2:
                        MessageBox.Show("Что-то пошло не так =(");
                        break;

                    default:
                        MessageBox.Show($"Новая причина бана {BanTextBox.Text} добавлена!");
                        GetBanListComboBox();
                        break;
                }

                BanTextBox.Text = "";
            }
        }



        //
        //Служебные функции для работы с интерфейсом
        //
        private void GetCountriesComboBox()
        {
            CountriesComboBox.Items.Clear();
            foreach (CountriesTbl country in processor.GetCountriesList())
            {
                CountriesComboBox.Items.Add(country.CountryName);
            }
        }

        private void GetBanListComboBox()
        {
            BanListComboBox.Items.Clear();
            foreach (BansTbl ban in processor.GetBanList())
            {
                BanListComboBox.Items.Add(ban.BanName);
            }
        }

        private void GetFileTypeComboBox()
        {
            FileTypeComboBox.Items.Clear();
            foreach (FileTypeTbl fileType in processor.GetFileTypes())
            {
                FileTypeComboBox.Items.Add(fileType.TypeName);
            }
        }

        private void GetAccessRightsComboBox()
        {
            AccessRightComboBox.Items.Clear();
            foreach (AccessRightsTbl right in processor.GetAccessRights())
            {
                AccessRightComboBox.Items.Add(right.AccessRight);
            }
        }

        private void GetUserInformationForMainPage()
        {
            FormatedUser user = processor.GetUserInformation();

            LoginMainTextBlock.Text = user.Login;
            LevelMainTextBox.Text = user.Level;
            PlanMainTextBox.Text = user.Plan.ToString();
            TrafficMainTextBox.Text = Math.Round(user.MemorySize, 3).ToString();

            if (user.Level == "Administrator")
            {
                AdminPanelButton.Visibility = Visibility.Visible;
                UserPanelButton.Visibility = Visibility.Visible;
            }
        }

        private void GetUsersFiles()
        {       
            UserFilesListView.ItemsSource = processor.allFiles;
        }

        private void GetUsersPlansComboBox()
        {
            UserTrafficComboBox.Items.Clear();
            foreach (TrafficPlanTbl plan in processor.GetTrafficPlans())
            {
                UserTrafficComboBox.Items.Add(plan.TrafficPlan);
            }
        }

        private void GetUsersLevelComboBox()
        {
            UserLevelComboBox.Items.Clear();
            foreach (UserLevelsTbl level in processor.GetUserLevels())
            {
                UserLevelComboBox.Items.Add(level.LevelName);
            }
        }        
    }
}
