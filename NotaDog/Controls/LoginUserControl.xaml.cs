using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NotaDog.Services;

namespace NotaDog.Controls
{
    /// <summary>
    /// Logique d'interaction pour LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl : System.Windows.Controls.UserControl
    {
        public event RoutedEventHandler? UserLoggedIn;

        public LoginUserControl()
        {
            InitializeComponent();

            // Afficher le nom d'utilisateur s'il est enregistré
            string username = Properties.Settings.Default.Username;
            txtWelcome.Text = $"Bienvenue, {username}";
            txtUsername.Text = username;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            bool stayLoggedIn = chkStayLoggedIn.IsChecked == true;

            if (UserService.ValidateUser(username, password))
            {
                if (stayLoggedIn)
                {
                    Properties.Settings.Default.IsLoggedIn = true;
                    Properties.Settings.Default.Save();
                }

                // Déclencher l'événement pour notifier que l'utilisateur est connecté
                UserLoggedIn?.Invoke(this, new RoutedEventArgs());
            }
            else
            {
                System.Windows.MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
