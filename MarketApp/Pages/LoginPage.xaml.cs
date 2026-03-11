using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Заполните все поля!";
                return;
            }

            var user = Connection.entities.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                txtMessage.Text = "Неверный логин или пароль!";
                return;
            }

            App.Current.Properties["CurrentUser"] = user;

            if (user.Role == "Admin")
                NavigationService.Navigate(new AdminMainPage());
            else
                NavigationService.Navigate(new UserMainPage());
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}