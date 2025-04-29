using System.Windows.Controls;
using System.Windows;

namespace Market
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (login == "admin" && password == "admin")
            {
                App.CurrentUser = new User { Login = login, Role = "Admin" };

                var mainWindow = new MainWindow();
                mainWindow.Show();


                Close();
            }
            else if (login == "client" && password == "client")
            {
                App.CurrentUser = new User { Login = login, Role = "Client" };


                var mainWindow = new MainWindow();
                mainWindow.Show();


                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class User
        {
            public string Login { get; set; }
            public string Role { get; set; }

            public bool IsAdmin => Role == "Admin";
        }
    }
}