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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.User
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTradePage : Page
    {
        private int _currentUserId;
        private int _selectedUserId;
        private int _tradeId;

        public CreateTradePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Dictionary<string, object> parameters)
            {
                if (parameters.ContainsKey("CurrentUserId"))
                    _currentUserId = (int)parameters["CurrentUserId"];

                if (parameters.ContainsKey("SelectedUserId"))
                    _selectedUserId = (int)parameters["SelectedUserId"];

                if (parameters.ContainsKey("TradeId"))
                    _tradeId = (int)parameters["TradeId"];

                LoadSelectedUserName(_selectedUserId);
                LoadUserItems();


            }
        }

        
        private async Task LoadSelectedUserName(int selectedUserId)
        {
            using (var context = new AppDbContext())
            {
                var selectedUser = await context.Users
                    .Where(u => u.Id == selectedUserId)
                    .FirstOrDefaultAsync();

                SelectedUserTextBlock.Text = selectedUser?.Username;
                
                DataContext = this; 
            }
        }

        private async Task LoadUserItems()
        {
            using (var context = new AppDbContext())
            {
                using (var db = new AppDbContext())
                {

                    var senderItems = db.Inventories
                             .Where(i => i.UserId == _currentUserId)
                             .Select(i => i.Item)
                             .ToList();

                    var receiverItems = db.Inventories
                             .Where(i => i.UserId == _selectedUserId)
                             .Select(i => i.Item)
                             .ToList();
                    SenderItemsList.ItemsSource = senderItems;
                    ReceiverItemsList.ItemsSource = receiverItems;
                }
                 
            }
        }
        private async void SenderItemsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Item;
            if (clickedItem == null) return;

            using (var context = new AppDbContext())
            {
                var existingTradeItem = await context.TradeItems
                    .FirstOrDefaultAsync(ti => ti.TradeId == _tradeId && ti.ItemId == clickedItem.Id && ti.Owner.Id == _currentUserId);

                if (existingTradeItem != null)
                {
                    context.TradeItems.Remove(existingTradeItem);
                }
                else
                {
                    var user = await context.Users.FindAsync(_currentUserId);
                    var tradeItem = new TradeItem
                    {
                        TradeId = _tradeId,
                        ItemId = clickedItem.Id,
                        Quantity = 1,
                        Owner = user,
                    };
                    context.TradeItems.Add(tradeItem);
                }

                await context.SaveChangesAsync();
            }
            UpdateItemBorders(); 
        }





        private async void ReceiverItemsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Item;
            if (clickedItem == null) return;

            using (var context = new AppDbContext())
            {
                var existingTradeItem = await context.TradeItems
                    .FirstOrDefaultAsync(ti => ti.TradeId == _tradeId && ti.ItemId == clickedItem.Id && ti.Owner.Id == _selectedUserId);

                if (existingTradeItem != null)
                {
                    context.TradeItems.Remove(existingTradeItem);
                }
                else
                {
                    var user = await context.Users.FindAsync(_selectedUserId);
                    var tradeItem = new TradeItem
                    {
                        TradeId = _tradeId,
                        ItemId = clickedItem.Id,
                        Quantity = 1,
                        Owner = user,
                    };
                    context.TradeItems.Add(tradeItem);
                }

                await context.SaveChangesAsync();
            }
            UpdateItemBorders(); 
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            
                using (var context = new AppDbContext())
                {
                    var trade = await context.Trades
                                              .Where(t => t.Id == _tradeId)
                                              .FirstOrDefaultAsync();

                    if (trade != null)
                    {
                        trade.IsSend = true;

                        await context.SaveChangesAsync();
                        Frame.GoBack();
                    }
                }
            
        }


        private async void UpdateItemBorders()
        {
            using (var context = new AppDbContext())
            {
                var tradeItems = await context.TradeItems
                    .Where(ti => ti.TradeId == _tradeId)
                    .Select(ti => new { ti.ItemId, OwnerId = ti.Owner.Id }) 
                    .ToListAsync();

                foreach (var item in SenderItemsList.Items)
                {
                    var container = (ListViewItem)SenderItemsList.ContainerFromItem(item);
                    if (container != null)
                    {
                        var border = (Border)container.ContentTemplateRoot;
                        var itemData = item as Item;
                        bool isInTrade = tradeItems.Any(ti => ti.ItemId == itemData.Id && ti.OwnerId == _currentUserId);
                        border.BorderBrush = new SolidColorBrush(isInTrade ? Colors.Green : Colors.Transparent);
                    }
                }

                foreach (var item in ReceiverItemsList.Items)
                {
                    var container = (ListViewItem)ReceiverItemsList.ContainerFromItem(item);
                    if (container != null)
                    {
                        var border = (Border)container.ContentTemplateRoot;
                        var itemData = item as Item;
                        bool isInTrade = tradeItems.Any(ti => ti.ItemId == itemData.Id && ti.OwnerId == _selectedUserId);
                        border.BorderBrush = new SolidColorBrush(isInTrade ? Colors.Green : Colors.Transparent);
                    }
                }
            }
        }

    }
}
