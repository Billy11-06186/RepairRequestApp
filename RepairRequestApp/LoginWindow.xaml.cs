using System;
using System.Windows;
using System.Windows.Input;

namespace RepairRequestApp
{
    public partial class LoginWindow : Window
    {
        private const string VALID_LOGIN = "user";
        private const string VALID_PASSWORD = "12345";

        private int loginAttempts = 0;
        private const int MAX_ATTEMPTS = 3;

        public LoginWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) => txtLogin.Focus();

            txtLogin.KeyDown += TxtLogin_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login))
            {
                ShowError("Введите логин!");
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль!");
                txtPassword.Focus();
                return;
            }

            if (login == VALID_LOGIN && password == VALID_PASSWORD)
            {
                LoginSuccess();
            }
            else
            {
                loginAttempts++;
                int remainingAttempts = MAX_ATTEMPTS - loginAttempts;

                MessageBox.Show("Вы неправильно ввели логин или пароль!",
                                "Ошибка авторизации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                if (loginAttempts >= MAX_ATTEMPTS)
                {
                    MessageBox.Show("Превышено количество попыток входа. Приложение будет закрыто.",
                                    "Доступ заблокирован",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                else
                {
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
        }

        private void LoginSuccess()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            errorBorder.Visibility = Visibility.Visible;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                errorBorder.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}