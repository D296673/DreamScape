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
    public sealed partial class UserPage : Page
    {
        private List<DreamScape.Data.User> Users;
        private List<Item> Items;

        public UserPage()
        {
            this.InitializeComponent();
            LoadUsers();
            LoadItems(); 
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                Users = db.Users.ToList();
            }

            UsersListView.ItemsSource = Users;
        }

        private void LoadItems()
        {
            using (var db = new AppDbContext())
            {
                Items = db.Items.ToList();
            }

            ItemComboBox.ItemsSource = Items;
            ItemComboBox.DisplayMemberPath = "Name";  
            ItemComboBox.SelectedValuePath = "Id";  
        }

        private void AddItemToUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DreamScape.Data.User user)
            {
                var selectedItemId = (int)ItemComboBox.SelectedValue;
                var selectedItem = Items.FirstOrDefault(item => item.Id == selectedItemId);

                if (selectedItem != null)
                {
                    int quantityToAdd = 1; 

                    using (var db = new AppDbContext())
                    {
                        var existingInventory = db.Inventories
                            .FirstOrDefault(i => i.UserId == user.Id && i.ItemId == selectedItem.Id);

                        if (existingInventory != null)
                        {
                            existingInventory.Quantity += quantityToAdd;
                            db.Users.Update(user);
                        }
                        else
                        {
                            var newInventory = new Inventory
                            {
                                UserId = user.Id,
                                ItemId = selectedItem.Id,
                                Quantity = quantityToAdd
                            };
                            db.Inventories.Add(newInventory);
                        }

                        db.SaveChanges();
                    }

                    LoadUsers();
                }
            }
        }

        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DreamScape.Data.User user)
            {
                using (var db = new AppDbContext())
                {
                    if (user.Role == "Beheerder")
                    {
                        user.Role = "Speler";
                        user.IsAdmin = false;
                    }
                    else
                    {
                        user.Role = "Beheerder";
                        user.IsAdmin = true;
                    }

                    db.Users.Update(user);
                    db.SaveChanges();
                }

                LoadUsers();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DreamScape.Data.User user)
            {
                using (var db = new AppDbContext())
                {
                    var userToDelete = db.Users.FirstOrDefault(u => u.Id == user.Id);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                    }
                }

                LoadUsers();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
