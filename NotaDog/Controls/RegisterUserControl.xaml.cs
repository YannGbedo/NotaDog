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
    /// Logique d'interaction pour RegisterUserControl.xaml
    /// </summary>
    public partial class RegisterUserControl : System.Windows.Controls.UserControl
    {
        public event RoutedEventHandler? UserRegistered;
        public RegisterUserControl()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                System.Windows.MessageBox.Show("Veuillez entrer un nom d'utilisateur et un mot de passe.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserService.RegisterUser(username, password);
            System.Windows.MessageBox.Show("Utilisateur enregistré avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

            // Déclencher l'événement pour notifier que l'utilisateur est enregistré
            UserRegistered?.Invoke(this, new RoutedEventArgs());
        }
    }
}
