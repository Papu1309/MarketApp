using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class AdminMainPage : Page
    {
        private Users currentUser;

        public AdminMainPage()
        {
            InitializeComponent();
            currentUser = (Users)App.Current.Properties["CurrentUser"];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserInfo.Text = $"Администратор: {currentUser.Username}";
            LoadCampaigns();
            LoadUsers();
            LoadOrders();
            LoadStatistics();
        }

        private void LoadCampaigns()
        {
            lvCampaigns.ItemsSource = Connection.entities.Campaigns.ToList();
        }

        private void LoadUsers()
        {
            lvUsers.ItemsSource = Connection.entities.Users.ToList();
        }

        private void LoadOrders()
        {
            lvOrders.ItemsSource = Connection.entities.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        private void LoadStatistics()
        {
            var orders = Connection.entities.Orders.ToList();
            txtTotalOrders.Text = $"Всего заказов: {orders.Count}";
            txtTotalRevenue.Text = $"Общая выручка: {orders.Sum(o => o.TotalAmount):C}";

            var top = Connection.entities.OrderItems
                .GroupBy(i => i.Campaigns.Name)
                .Select(g => new { Name = g.Key, Count = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .ToList();

            lstTopCampaigns.ItemsSource = top.Select(x => new { Display = $"{x.Name} — {x.Count} шт." }).ToList();
        }

        private void BtnAddCampaign_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditCampaignPage(null));
        }

        private void BtnEditCampaign_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvCampaigns.SelectedItem as Campaigns;
            if (selected == null)
            {
                MessageBox.Show("Выберите кампанию");
                return;
            }
            NavigationService.Navigate(new EditCampaignPage(selected));
        }

        private void BtnDeleteCampaign_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvCampaigns.SelectedItem as Campaigns;
            if (selected == null)
            {
                MessageBox.Show("Выберите кампанию");
                return;
            }

            var result = MessageBox.Show($"Удалить кампанию \"{selected.Name}\"?", "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Connection.entities.Campaigns.Remove(selected);
                Connection.entities.SaveChanges();
                LoadCampaigns();
            }
        }

        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            if (selected.Role == "Admin")
                selected.Role = "User";
            else
                selected.Role = "Admin";

            Connection.entities.SaveChanges();
            LoadUsers();
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            if (selected.Id == currentUser.Id)
            {
                MessageBox.Show("Нельзя удалить себя");
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя {selected.Username}?", "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Connection.entities.Users.Remove(selected);
                Connection.entities.SaveChanges();
                LoadUsers();
            }
        }

        private void BtnChangeOrderStatus_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvOrders.SelectedItem as Orders;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            if (selected.Status == "Paid")
                selected.Status = "Готов к выдаче";
            else if (selected.Status == "Готов к выдаче")
                selected.Status = "Выполнен";
            else if (selected.Status == "Выполнен")
                selected.Status = "Paid";
            else
                selected.Status = "Paid";

            Connection.entities.SaveChanges();
            LoadOrders();
            LoadStatistics();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties.Remove("CurrentUser");
            NavigationService.Navigate(new LoginPage());
        }
    }
}