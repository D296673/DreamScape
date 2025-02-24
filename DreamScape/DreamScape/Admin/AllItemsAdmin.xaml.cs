using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Data;

namespace DreamScape.Admin
{
    public sealed partial class AllItemsAdmin : Page
    {
        private List<Item> Items;
        private Item selectedItem; // Store selected item

        public AllItemsAdmin()
        {
            this.InitializeComponent();
            LoadItems();
        }

        private void LoadItems()
        {
            using (var db = new AppDbContext())
            {
                Items = db.Items.ToList();
            }
            WeaponsListView.ItemsSource = Items;
        }

        private void WeaponsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectedItem = e.ClickedItem as Item;
            if (selectedItem != null)
            {
                WeaponNameTextBox.Text = selectedItem.Name;
                WeaponDescriptionTextBox.Text = selectedItem.Description;
                WeaponTypeTextBox.Text = selectedItem.Type;
                WeaponRarityTextBox.Text = selectedItem.Rarity.ToString();
                WeaponPowerTextBox.Text = selectedItem.Power.ToString();
                WeaponSpeedTextBox.Text = selectedItem.Speed.ToString();
                WeaponDurabilityTextBox.Text = selectedItem.Durability.ToString();
                WeaponMagicTextBox.Text = selectedItem.MagicProperties;
            }
        }

        private void SaveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                using (var db = new AppDbContext())
                {
                    var itemToUpdate = db.Items.FirstOrDefault(i => i.Id == selectedItem.Id);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Name = WeaponNameTextBox.Text;
                        itemToUpdate.Description = WeaponDescriptionTextBox.Text;
                        itemToUpdate.Type = WeaponTypeTextBox.Text;
                        itemToUpdate.Rarity = WeaponRarityTextBox.Text;
                        itemToUpdate.Power = int.Parse(WeaponPowerTextBox.Text);
                        itemToUpdate.Speed = int.Parse(WeaponSpeedTextBox.Text);
                        itemToUpdate.Durability = int.Parse(WeaponDurabilityTextBox.Text);
                        itemToUpdate.MagicProperties = WeaponMagicTextBox.Text;

                        db.SaveChanges();
                    }
                }

                LoadItems(); 
            }
        }

        private void Backbutton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void CreateItemButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateItem));
        }
    }
}
