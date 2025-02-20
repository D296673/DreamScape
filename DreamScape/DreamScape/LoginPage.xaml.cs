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
using System.Diagnostics;
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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }
        

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessegeTextBox.Text = "Vul alle velden in.";
                MessegeTextBox.Visibility = Visibility.Visible;
                return;
            }

            bool success = await LoginManager.LoginUserAsync(userName, password);
            if (success)
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Username == userName);
                    if (user != null)
                    {
                        int userId = user.Id; 

                        Frame.Navigate(typeof(mainpage), userId);
                    }
                }
            }
            else
            {
                MessegeTextBox.Text = "Verkeerde gebruikersnaam of wachtwoord.";
                MessegeTextBox.Visibility = Visibility.Visible;
            }
        }

    }
}
