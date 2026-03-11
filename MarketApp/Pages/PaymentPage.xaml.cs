using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class PaymentPage : Page
    {
        private List<CartItem> cart;
        private Users currentUser;

        public PaymentPage(List<CartItem> cartItems)
        {
            InitializeComponent();
            cart = cartItems;
            currentUser = (Users)App.Current.Properties["CurrentUser"];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lvOrderItems.ItemsSource = cart;
            decimal total = cart.Sum(i => i.Total);
            txtTotal.Text = $"Сумма к оплате: {total:C}";

            rbCard.IsChecked = true;
            cardPanel.Visibility = Visibility.Visible;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (cardPanel != null)
                cardPanel.Visibility = rbCard.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (rbCard.IsChecked == true)
            {
                if (!ValidateCardData())
                    return;
            }

            var order = new Orders
            {
                UserId = currentUser.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(i => i.Total),
                Status = "Paid",
                PaymentMethod = rbCard.IsChecked == true ? "Card" : "Cash",
                PickupAddress = "ул. Рекламная, д.1, офис 101",
                PickupTime = DateTime.Now.AddHours(3)
            };

            Connection.entities.Orders.Add(order);
            Connection.entities.SaveChanges();

            foreach (var item in cart)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.Id,
                    CampaignId = item.CampaignId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                Connection.entities.OrderItems.Add(orderItem);
            }
            Connection.entities.SaveChanges();

            string message = $"Ваш заказ №{order.Id} оплачен.\n" +
                             $"Адрес получения: {order.PickupAddress}\n" +
                             $"Время: {order.PickupTime:dd.MM.yyyy HH:mm}";
            MessageBox.Show(message, "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

            cart.Clear();
            NavigationService.Navigate(new UserMainPage());
        }

        private bool ValidateCardData()
        {
            string number = txtCardNumber.Text.Replace(" ", "").Replace("-", "");
            string expiry = txtExpiry.Text.Trim();
            string cvv = txtCvv.Text.Trim();

            if (!Regex.IsMatch(number, @"^\d{16}$"))
            {
                txtMessage.Text = "Номер карты должен содержать 16 цифр";
                return false;
            }

            if (!Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                txtMessage.Text = "Срок действия должен быть в формате ММ/ГГ";
                return false;
            }

            if (!Regex.IsMatch(cvv, @"^\d{3}$"))
            {
                txtMessage.Text = "CVV должен содержать 3 цифры";
                return false;
            }

            var parts = expiry.Split('/');
            int month = int.Parse(parts[0]);
            int year = int.Parse(parts[1]) + 2000;
            var now = DateTime.Now;
            if (year < now.Year || (year == now.Year && month < now.Month))
            {
                txtMessage.Text = "Срок действия карты истёк";
                return false;
            }

            return true;
        }
    }
}