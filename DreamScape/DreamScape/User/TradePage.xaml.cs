using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
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
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int userId)
            {
                _currentUserId = userId;
                LoadTrades();
            }
        }

        private async void LoadTrades()
        {
            using (var context = new AppDbContext())
            {
                var trades = await context.Trades
                    .Where(t => t.ReceiverId == _currentUserId && t.Status == "Pending" && t.IsSend == true)
                    .Include(t => t.Sender)
                    .ToListAsync();

                TradesList.ItemsSource = trades;
            }
        }


        private async void ViewTrade_Click(object sender, RoutedEventArgs e)
        {
            if (TradesList.SelectedItem is Trade selectedTrade)
            {
                if (selectedTrade.Status == "Pending")
                {
                    using (var context = new AppDbContext())
                    {
                        var trade = await context.Trades.FindAsync(selectedTrade.Id);
                        if (trade == null || trade.Status != "Pending") return; 

                        var tradeItems = await context.TradeItems
                            .Where(ti => ti.TradeId == trade.Id)
                            .Include(ti => ti.Item)
                            .Include(ti => ti.Owner)
                            .ToListAsync();

                        var Sender = await context.Users.FindAsync(trade.SenderId);
                        var receiver = await context.Users.FindAsync(trade.ReceiverId);

                        if (Sender == null || receiver == null) return;

                        var itemsToReceive = tradeItems
                            .Where(ti => ti.Owner.Id != _currentUserId)
                            .Select(ti => ti.Item.Name)
                            .ToList();

                        var itemsToGive = tradeItems
                            .Where(ti => ti.Owner.Id == _currentUserId)
                            .Select(ti => ti.Item.Name)
                            .ToList();

                        StackPanel contentPanel = new StackPanel { Spacing = 10 };

                        Grid tradeGrid = new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                            }
                        };

                        StackPanel senderPanel = new StackPanel();
                        senderPanel.Children.Add(new TextBlock { Text = "Jij", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) });

                        ListView senderList = new ListView
                        {
                            ItemsSource = itemsToGive,
                            Height = 100
                        };
                        senderPanel.Children.Add(senderList);
                        Grid.SetColumn(senderPanel, 0);
                        tradeGrid.Children.Add(senderPanel);

                        StackPanel receiverPanel = new StackPanel();
                        receiverPanel.Children.Add(new TextBlock { Text = "Ontvanger", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) });

                        ListView receiverList = new ListView
                        {
                            ItemsSource = itemsToReceive,
                            Height = 100
                        };
                        receiverPanel.Children.Add(receiverList);
                        Grid.SetColumn(receiverPanel, 1);
                        tradeGrid.Children.Add(receiverPanel);

                        contentPanel.Children.Add(tradeGrid);

                        ContentDialog tradeDialog = new ContentDialog
                        {
                            Title = "Trade Details",
                            Content = contentPanel,
                            PrimaryButtonText = "Accept",
                            CloseButtonText = "Decline"
                        };

                        tradeDialog.XamlRoot = this.Content.XamlRoot;
                        ContentDialogResult result = await tradeDialog.ShowAsync();


                        if (result == ContentDialogResult.Primary)
                        {
                            foreach (var tradeItem in tradeItems)
                            {
                                var inventoryItem = await context.Inventories
                                    .FirstOrDefaultAsync(inv => inv.ItemId == tradeItem.Item.Id && inv.UserId == tradeItem.Owner.Id);

                                if (inventoryItem != null)
                                {
                                    inventoryItem.UserId = tradeItem.Owner.Id == Sender.Id ? receiver.Id : Sender.Id;
                                }
                                else
                                {
                                    context.Inventories.Add(new Inventory
                                    {
                                        UserId = tradeItem.Owner.Id == Sender.Id ? receiver.Id : Sender.Id,
                                        ItemId = tradeItem.Item.Id,
                                        Quantity = 1
                                    });
                                }
                            }

                            trade.Status = "Completed";
                        }
                        else
                        {
                            trade.Status = "Declined";
                        }

                        context.Trades.Update(trade);
                        await context.SaveChangesAsync();

                        selectedTrade.Status = trade.Status;
                    }

                    ContentDialog tradeDialog1 = new ContentDialog
                    {
                        Title = "Trade Details",
                        Content = $"This trade has been {selectedTrade.Status}",
                        CloseButtonText = "OK"
                    };

                    tradeDialog1.XamlRoot = this.Content.XamlRoot;
                    await tradeDialog1.ShowAsync(); 
                }
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

                        Frame.Navigate(typeof(CreateTradePage), new Dictionary<string, object>
                        {
                            { "CurrentUserId", _currentUserId },
                            { "SelectedUserId", selectedUser.Id },
                            { "TradeId", tradeId }
                        });

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
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
