using MarketApp.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MarketApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }

        public void Navigate(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}