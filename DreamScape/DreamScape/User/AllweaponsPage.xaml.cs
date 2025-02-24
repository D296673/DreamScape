using DreamScape.Data;
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
    public sealed partial class AllweaponsPage : Page
    {
        private List<Item> Items;
        public AllweaponsPage()
        {

            this.InitializeComponent();
            using (var db = new AppDbContext())
            {
                Items = db.Items.ToList();
            }
            WeaponsListView.ItemsSource = Items;
        }

        private void WeaponsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = e.ClickedItem as Item;
            WeaponNameText.Text = selectedItem.Name;
            WeaponDescriptionText.Text = selectedItem.Description;
            WeaponTypeText.Text = $"Type: {selectedItem.Type}";
            WeaponRarityText.Text = $"Rarity{selectedItem.Rarity}";
            WeaponPowerText.Text = $"Power: {selectedItem.Power}";
            WeaponSpeedText.Text = $"speed: {selectedItem.Speed}";
            WeaponDescriptionText.Text = $"Durability: {selectedItem.Durability}";
            WeaponMagicText.Text = $"MagicProperties: {selectedItem.MagicProperties}";
        }

        private void Backbutton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
