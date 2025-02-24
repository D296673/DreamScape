using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateItem : Page
    {
        public CreateItem()
        {
            this.InitializeComponent();
        }

        private void CreateItemButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var newItem = new Item
                {
                    Name = WeaponNameTextBox.Text,
                    Description = WeaponDescriptionTextBox.Text,
                    Type = WeaponTypeTextBox.Text,
                    Rarity = WeaponRarityTextBox.Text,
                    Power = int.TryParse(WeaponPowerTextBox.Text, out int power) ? power : 0,
                    Speed = int.TryParse(WeaponSpeedTextBox.Text, out int speed) ? speed : 0,
                    Durability = int.TryParse(WeaponDurabilityTextBox.Text, out int durability) ? durability : 0,
                    MagicProperties = WeaponMagicTextBox.Text
                };

                db.Items.Add(newItem);
                db.SaveChanges();
            }

            Frame.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
