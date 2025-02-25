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

            dynamic parameters = e.Parameter;

            if (parameters != null)
            {
                _currentUserId = parameters.CurrentUserId;
                _selectedUserId = parameters.SelectedUserId;

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
        private void SenderItemsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Item; 

            if (clickedItem != null)
            {
                using (var context = new AppDbContext())
                {
                    var user = context.Users.Where(u => u.Id == _currentUserId).FirstOrDefault();
                    var tradeItem = new TradeItem
                    {
                        TradeId = _tradeId,
                        ItemId = clickedItem.Id,
                        Quantity = 1,
                        OwnerId = user,

                    };
                    context.TradeItems.Add(tradeItem);
                    context.SaveChangesAsync();
                }
                
            }
        }

        private void ReceiverItemsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Item; 

            if (clickedItem != null)
            {
                
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
        }



    }
}
