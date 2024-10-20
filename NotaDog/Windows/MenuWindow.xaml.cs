using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NotaDog.Services;

namespace NotaDog.Windows
{
    /// <summary>
    /// Logique d'interaction pour MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : BaseWindow
    {
        public MenuWindow()
        {
            InitializeComponent();

            // Ensure the documents folder exists
            EnsureDocumentsFolderExists();

            string username = Properties.Settings.Default.Username;
            lblWelcome.Content = $"Bienvenue, {username}!";

            LoadDocumentsList();
            LoadDocumentTypes();

            // Abonner l'événement double-clic
            lstDocuments.MouseDoubleClick += LstDocuments_MouseDoubleClick;
        }

        private void EnsureDocumentsFolderExists()
        {
            // Get the documents folder path
            string documentsFolder = DocumentService.GetDefaultDocumentsFolder();

            // Check if the folder exists
            if (!Directory.Exists(documentsFolder))
            {
                try
                {
                    // Create the folder
                    Directory.CreateDirectory(documentsFolder);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to create the documents folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsLoggedIn = false;
            Properties.Settings.Default.Save();

            // Retour à la fenêtre de connexion
            MainWindow mainWindow = new MainWindow();
            SwitchToWindow(mainWindow);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de paramètres
            SettingsWindow settingsWindow = new SettingsWindow();
            SwitchToWindow(settingsWindow);
        }

        private void LoadDocumentsList()
        {
            List<DocumentInfo> documents = DocumentService.GetDocumentsList();
            lstDocuments.ItemsSource = documents;
        }

        private void LoadDocumentTypes()
        {
            List<string> documentTypes = new List<string>
            {
                "Promesse de Vente"
                // Vous pouvez ajouter d'autres types plus tard
            };
            cmbDocumentTypes.ItemsSource = documentTypes;
            cmbDocumentTypes.SelectedIndex = 0;
        }

        private void CreateDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentTypes.SelectedItem != null)
            {
                string? selectedType = cmbDocumentTypes.SelectedItem.ToString();

                // Ouvrir la fenêtre de configuration en passant le type de document
                if (selectedType != null)
                {
                    ConfigurationWindow configWindow = new ConfigurationWindow(selectedType);
                    SwitchToWindow(configWindow);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Veuillez sélectionner un type de document.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LstDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstDocuments.SelectedItem != null)
            {
                var documentInfo = lstDocuments.SelectedItem as DocumentInfo;
                if (documentInfo != null)
                {
                    OpenDocument(documentInfo);
                }
            }
        }

        private void OpenDocument(DocumentInfo documentInfo)
        {
            if (Properties.Settings.Default.AlwaysOpenWithoutPrompt)
            {
                // Open the document directly
                OpenDocumentFile(documentInfo.FilePath);
            }
            else
            {
                // Show the confirmation dialog
                ConfirmationDialog dialog = new ConfirmationDialog($"Do you want to open the document: {documentInfo.FileName}?");
                dialog.Owner = this; // Optional, sets the owner window
                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true && dialog.Result)
                {
                    // User clicked Yes
                    if (dialog.SkipPrompt)
                    {
                        // Update the setting to skip the prompt in the future
                        Properties.Settings.Default.AlwaysOpenWithoutPrompt = true;
                        Properties.Settings.Default.Save();
                    }
                    OpenDocumentFile(documentInfo.FilePath);
                }
                else
                {
                    // User clicked No or closed the dialog; do nothing
                }
            }
        }

        private void OpenDocumentFile(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to open the document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
