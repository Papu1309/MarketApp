using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class OrderDetailsPage : Page
    {
        private int orderId;

        public OrderDetailsPage(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var order = Connection.entities.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                MessageBox.Show("Заказ не найден");
                NavigationService.GoBack();
                return;
            }

            txtOrderInfo.Text = $"Заказ №{order.Id} от {order.OrderDate:dd.MM.yyyy HH:mm}";
            lvItems.ItemsSource = order.OrderItems.ToList();
            txtTotal.Text = $"Сумма: {order.TotalAmount:C}";
            txtStatus.Text = $"Статус: {order.Status}";
            txtPickup.Text = order.PickupAddress != null
                ? $"Адрес получения: {order.PickupAddress}\nВремя: {order.PickupTime:dd.MM.yyyy HH:mm}"
                : "Заказ ещё не оплачен";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}