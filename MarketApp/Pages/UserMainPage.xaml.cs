using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class UserMainPage : Page
    {
        private Users currentUser;
        private List<CartItem> cart = new List<CartItem>();
        private List<Campaigns> allCampaigns;

        public UserMainPage()
        {
            InitializeComponent();
            currentUser = (Users)App.Current.Properties["CurrentUser"];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserInfo.Text = $"Пользователь: {currentUser.Username} ({currentUser.Role})";
            LoadCampaigns();
            LoadOrders();
        }

        private void LoadCampaigns()
        {
            allCampaigns = Connection.entities.Campaigns
                .Where(c => c.IsActive == true)
                .ToList();
            lvCampaigns.ItemsSource = allCampaigns;
        }

        private void LoadOrders()
        {
            lvOrders.ItemsSource = Connection.entities.Orders
                .Where(o => o.UserId == currentUser.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(search))
                lvCampaigns.ItemsSource = allCampaigns;
            else
                lvCampaigns.ItemsSource = allCampaigns.Where(c => c.Name.ToLower().Contains(search) ||
                                                                  (c.Description != null && c.Description.ToLower().Contains(search))).ToList();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int campaignId = (int)btn.Tag;
            var campaign = Connection.entities.Campaigns.Find(campaignId);
            if (campaign != null)
            {
                var existing = cart.FirstOrDefault(i => i.CampaignId == campaignId);
                if (existing != null)
                    existing.Quantity++;
                else
                    cart.Add(new CartItem { CampaignId = campaign.Id, Name = campaign.Name, Price = campaign.Price, Quantity = 1 });

                UpdateCartView();
                MessageBox.Show("Кампания добавлена в корзину");
            }
        }

        private void BtnDecreaseQuantity(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int campaignId = (int)btn.Tag;
            var item = cart.FirstOrDefault(i => i.CampaignId == campaignId);
            if (item != null)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    cart.Remove(item);
            }
            UpdateCartView();
        }

        private void BtnIncreaseQuantity(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int campaignId = (int)btn.Tag;
            var item = cart.FirstOrDefault(i => i.CampaignId == campaignId);
            if (item != null)
            {
                item.Quantity++;
            }
            UpdateCartView();
        }

        private void UpdateCartView()
        {
            lvCart.ItemsSource = null;
            lvCart.ItemsSource = cart;
            decimal total = cart.Sum(i => i.Total);
            txtCartTotal.Text = $"Итого: {total:C}";
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }
            NavigationService.Navigate(new PaymentPage(cart));
        }

        private void BtnOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int orderId = (int)btn.Tag;
            NavigationService.Navigate(new OrderDetailsPage(orderId));
        }

        private void BtnRefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties.Remove("CurrentUser");
            NavigationService.Navigate(new LoginPage());
        }
    }

    public class CartItem
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}