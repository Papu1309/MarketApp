using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Логин и пароль обязательны!";
                return;
            }

            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                txtMessage.Text = "Введите корректный email (например, name@domain.ru)";
                return;
            }

            if (!string.IsNullOrEmpty(phone) && !IsValidPhone(phone))
            {
                txtMessage.Text = "Телефон должен содержать только цифры, возможно с '+' в начале, длина 10-15 символов";
                return;
            }

            if (Connection.entities.Users.Any(u => u.Username == username))
            {
                txtMessage.Text = "Пользователь с таким логином уже существует!";
                return;
            }

            var newUser = new Users
            {
                Username = username,
                Password = password,
                Email = email,
                Phone = phone,
                Role = "User"
            };

            Connection.entities.Users.Add(newUser);
            Connection.entities.SaveChanges();

            MessageBox.Show("Регистрация успешна! Теперь войдите.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new LoginPage());
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string digitsOnly = Regex.Replace(phone, @"\D", "");
            if (digitsOnly.Length < 10 || digitsOnly.Length > 15)
                return false;

            return Regex.IsMatch(phone, @"^\+?\d{10,15}$");
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}