using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotaDog.Services;
using MessageBox = System.Windows.MessageBox;

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

            // Load the setting value
            chkAlwaysUseDefaultSaveFolder.IsChecked = Properties.Settings.Default.AlwaysUseDefaultSaveFolder;

            string username = Properties.Settings.Default.Username;
            lblWelcome.Content = $"Bienvenue, {username}!";

            LoadDocumentsList();
            LoadDocumentTypes();

            // Abonner l'événement double-clic
            lstDocuments.MouseDoubleClick += LstDocuments_MouseDoubleClick;
        }

        private static void EnsureDocumentsFolderExists()
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
            MainWindow mainWindow = new();
            SwitchToWindow(mainWindow);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de paramètres
            SettingsWindow settingsWindow = new();
            SwitchToWindow(settingsWindow);
        }

        private void LoadDocumentsList()
        {
            List<DocumentInfo> documents = DocumentService.GetDocumentsList();
            lstDocuments.ItemsSource = documents;
        }

        private void LoadDocumentTypes()
        {
            List<string> documentTypes =
            [
                "Promesse de Vente"
                // Vous pouvez ajouter d'autres types plus tard
            ];
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
                    ConfigurationWindow configWindow = new(selectedType);
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
                var documentInfo = (DocumentInfo)lstDocuments.SelectedItem;
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
                ConfirmationDialog dialog = new("OpenDocument")
                {
                    Owner = this // Optional, sets the owner window
                };
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

        private void ChkAlwaysUseDefaultSaveFolder_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysUseDefaultSaveFolder = true;
            Properties.Settings.Default.Save();
        }

        private void ChkAlwaysUseDefaultSaveFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysUseDefaultSaveFolder = false;
            Properties.Settings.Default.Save();
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            if (GetSelectedDocument(sender) is DocumentInfo selectedDocument)
            {
                // Appeler votre méthode pour ouvrir le document
                OpenDocumentFile(selectedDocument.FilePath);
            }
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (GetSelectedDocument(sender) is DocumentInfo selectedDocument)
            {
                // Demander une confirmation avant de supprimer
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le document '{selectedDocument.FileName}' ?", "Confirmer la suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(selectedDocument.FilePath);
                        // Rafraîchir la liste des documents
                        LoadDocumentsList();
                        LoadDocumentTypes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression du document : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private DocumentInfo? GetSelectedDocument(object sender)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.DataContext is DocumentInfo documentInfo)
                {
                    return documentInfo;
                }
            }
            return null;
        }
    }
}
