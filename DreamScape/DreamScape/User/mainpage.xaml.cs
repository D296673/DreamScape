using DreamScape.Data;
using DreamScape.User;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class mainpage : Page
    {
        private int userId;
        private DreamScape.Data.User user;
        public mainpage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                if (int.TryParse(e.Parameter.ToString(), out int parsedUserId))
                {
                    userId = parsedUserId; 

                    using (var db = new AppDbContext())
                    {
                        user = db.Users.FirstOrDefault(u => u.Id == userId);
                        NameTextBlock.Text = user.Username;
                        RoleTextBlock.Text = $"Role: {user.Role}";
                    }
                }
            }
        }
        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout",
                Content = "Are you sure you want to log out?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot 
            };

            ContentDialogResult result = await logoutDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(LoginPage)); 
            }
        }

        private void UserProfileButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InventoryPage), userId);
        }

        private void WeaponsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AllweaponsPage));
        }

        private void TradingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TradePage), userId);
        }

    }
}
