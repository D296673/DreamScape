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
        private List<User> Users;
        private List<Item> Items;

        public UserPage()
        {
            this.InitializeComponent();
            LoadUsers();
            LoadItems();  // Laad de items uit de database
        }

        // Laad de gebruikers uit de database
        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                Users = db.Users.ToList();
            }

            // Bind de lijst van gebruikers aan de ListView
            UsersListView.ItemsSource = Users;
        }

        // Laad de beschikbare items uit de database
        private void LoadItems()
        {
            using (var db = new AppDbContext())
            {
                Items = db.Items.ToList();
            }

            // Vul de ComboBox met items
            ItemComboBox.ItemsSource = Items;
            ItemComboBox.DisplayMemberPath = "Name";  // Toon de naam van het item in de ComboBox
            ItemComboBox.SelectedValuePath = "Id";  // Gebruik de Id van het item als waarde
        }

        // Klikken op de knop om het item aan de gebruiker toe te voegen
        private void AddItemToUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is User user)
            {
                // Haal het geselecteerde item op uit de ComboBox
                var selectedItemId = (int)ItemComboBox.SelectedValue;
                var selectedItem = Items.FirstOrDefault(item => item.Id == selectedItemId);

                if (selectedItem != null)
                {
                    int quantityToAdd = 1;  // Voeg bijvoorbeeld 1 van dit item toe

                    // Voeg het item toe aan de inventory van de gebruiker
                    using (var db = new AppDbContext())
                    {
                        // Zoek naar de bestaande inventaris van deze gebruiker
                        var existingInventory = db.Inventories
                            .FirstOrDefault(i => i.UserId == user.Id && i.ItemId == selectedItem.Id);

                        if (existingInventory != null)
                        {
                            // Als het item al bestaat in de inventory, verhoog de hoeveelheid
                            existingInventory.Quantity += quantityToAdd;
                            db.Users.Update(user); // Werk de gebruiker bij als dat nodig is
                        }
                        else
                        {
                            // Als het item nog niet in de inventory van de gebruiker staat, voeg het toe
                            var newInventory = new Inventory
                            {
                                UserId = user.Id,
                                ItemId = selectedItem.Id,
                                Quantity = quantityToAdd
                            };
                            db.Inventories.Add(newInventory);
                        }

                        // Sla de wijzigingen op in de database
                        db.SaveChanges();
                    }

                    // Herlaad de lijst van gebruikers om de wijzigingen weer te geven
                    LoadUsers();
                }
            }
        }

        // Wijzig de rol van de gebruiker (bijvoorbeeld van Speler naar Beheerder)
        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is User user)
            {
                // Verander de rol van de gebruiker
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

                // Herlaad de lijst van gebruikers om de wijziging weer te geven
                LoadUsers();
            }
        }

        // Verwijder de gebruiker
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is User user)
            {
                using (var db = new AppDbContext())
                {
                    // Verwijder de gebruiker uit de database
                    var userToDelete = db.Users.FirstOrDefault(u => u.Id == user.Id);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                    }
                }

                // Herlaad de lijst van gebruikers om de wijziging weer te geven
                LoadUsers();
            }
        }

        // Back Button om naar de vorige pagina te navigeren
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}