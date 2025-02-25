using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.User
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TradePage : Page
    {
        private int _currentUserId; 

        public TradePage()
        {
            this.InitializeComponent();
            LoadTrades();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int userId)
            {
                _currentUserId = userId;
            }
        }

        private async void LoadTrades()
        {
            using (var context = new AppDbContext())
            {
                var trades = await context.Trades
                    .Where(t => t.ReceiverId == _currentUserId && t.Status == "Pending")
                    .Include(t => t.Sender)
                    .ToListAsync();

                TradesList.ItemsSource = trades;
            }
        }

        private void ViewTrade_Click(object sender, RoutedEventArgs e)
        {
            if (TradesList.SelectedItem is Trade selectedTrade)
            {
                // Navigeer naar een pagina om de trade te bekijken (nog te maken)
                //Frame.Navigate(typeof(ViewTradePage), selectedTrade.Id);
            }
        }

        private async void OpenTradeDialog(object sender, RoutedEventArgs e)
        {
            List<Data.User> users = await GetAllUsersExceptCurrentUser();

            ListView userListView = new ListView();
            userListView.ItemsSource = users;
            userListView.DisplayMemberPath = "Username";

            ContentDialog tradeDialog = new ContentDialog
            {
                Title = "Kies een gebruiker om mee te traden",
                Content = userListView,
                PrimaryButtonText = "Annuleren",
                SecondaryButtonText = "Selecteer"
            };

            tradeDialog.XamlRoot = this.Content.XamlRoot;

            ContentDialogResult result = await tradeDialog.ShowAsync();

            if (result == ContentDialogResult.Secondary)
            {
                if (userListView.SelectedItem is Data.User selectedUser)
                {
                    using (var context = new AppDbContext())
                    {
                        var trade = new Trade
                        {
                            SenderId = _currentUserId,
                            ReceiverId = selectedUser.Id,
                            IsSend = false,
                            Status = "Pending",
                            TradeDate = System.DateTime.UtcNow
                            
                        };
                        context.Trades.Add(trade);
                        await context.SaveChangesAsync();

                        var tradeId = trade.Id;

                        Frame.Navigate(typeof(CreateTradePage), new { CurrentUserId = _currentUserId, SelectedUserId = selectedUser.Id, TradeId = tradeId });
                    }
                }
            }
        }


        private async Task<List<Data.User>> GetAllUsersExceptCurrentUser()
        {
            using (var context = new AppDbContext())
            {
                return await context.Users
                    .Where(u => u.Id != _currentUserId)  
                    .ToListAsync();
            }
        }
    }
}
