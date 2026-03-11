using System;
using System.Windows;
using System.Windows.Controls;
using MarketApp.Connect;

namespace MarketApp.Pages
{
    public partial class EditCampaignPage : Page
    {
        private Campaigns editingCampaign;

        public EditCampaignPage(Campaigns campaign)
        {
            InitializeComponent();
            editingCampaign = campaign;

            if (campaign != null)
            {
                txtTitle.Text = "Редактирование кампании";
                txtName.Text = campaign.Name;
                txtDescription.Text = campaign.Description;
                txtPrice.Text = campaign.Price.ToString();
                txtDuration.Text = campaign.Duration.ToString();
                chkIsActive.IsChecked = campaign.IsActive;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtDuration.Text))
            {
                txtMessage.Text = "Заполните все поля!";
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                txtMessage.Text = "Некорректная цена";
                return;
            }

            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                txtMessage.Text = "Длительность должна быть положительным числом";
                return;
            }

            if (editingCampaign == null)
            {
                var newCamp = new Campaigns
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = price,
                    Duration = duration,
                    IsActive = chkIsActive.IsChecked ?? true
                };
                Connection.entities.Campaigns.Add(newCamp);
            }
            else
            {
                editingCampaign.Name = txtName.Text.Trim();
                editingCampaign.Description = txtDescription.Text.Trim();
                editingCampaign.Price = price;
                editingCampaign.Duration = duration;
                editingCampaign.IsActive = chkIsActive.IsChecked ?? true;
            }

            Connection.entities.SaveChanges();
            MessageBox.Show("Данные сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.GoBack();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}