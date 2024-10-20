using System.Text;
using System.Windows;
using NotaDog.Controls;
using NotaDog.Services;

namespace NotaDog.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Vérifier si l'utilisateur est resté connecté
            if (Properties.Settings.Default.IsLoggedIn)
            {
                // Ouvrir directement la fenêtre du menu
                MenuWindow menuWindow = new();
                SwitchToWindow(menuWindow);
            }
            else
            {
                LoadAppropriateControl();
            }
        }

        private void LoadAppropriateControl()
        {
            if (!UserService.IsUserRegistered())
            {
                // Aucun utilisateur enregistré, afficher le contrôle d'inscription
                var registerControl = new RegisterUserControl();
                registerControl.UserRegistered += RegisterControl_UserRegistered;
                MainContent.Content = registerControl;
            }
            else
            {
                // Utilisateur enregistré, afficher le contrôle de connexion
                var loginControl = new LoginUserControl();
                loginControl.UserLoggedIn += LoginControl_UserLoggedIn;
                MainContent.Content = loginControl;
            }
        }

        private void RegisterControl_UserRegistered(object sender, RoutedEventArgs e)
        {
            // Après l'inscription, charger le contrôle de connexion
            LoadAppropriateControl();
        }

        private void LoginControl_UserLoggedIn(object sender, RoutedEventArgs e)
        {
            // Après la connexion, ouvrir la fenêtre du menu
            MenuWindow menuWindow = new();
            SwitchToWindow(menuWindow);
        }

        /*
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            bool stayLoggedIn = chkStayLoggedIn.IsChecked == true;

            // Pour le moment, nous n'implémentons pas la vérification des identifiants
            // À l'avenir, vous pourrez ajouter la logique de vérification ici

            // Si l'option "Rester connecté" est cochée, enregistrer cet état
            if (stayLoggedIn)
            {
                Properties.Settings.Default.IsLoggedIn = true;
                Properties.Settings.Default.Save();
            }

            // Ouvrir la fenêtre du menu
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();
            this.Close();
        }
        */
    }
}