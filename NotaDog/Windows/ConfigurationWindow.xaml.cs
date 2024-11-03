using System;
using System.IO;
using System.Windows;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Packaging;
using NotaDog.Controls;
using NotaDog.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Microsoft.Win32;
using NotaDog.Interfaces;

namespace NotaDog.Windows
{
    // Define a class to hold Notary information
    public class NotaryInfo
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? OfficeName { get; set; }
        public string? OfficeAddress { get; set; }
    }
        /// <summary>
        /// Logique d'interaction pour ConfigurationWindow.xaml
        /// </summary>
        public partial class ConfigurationWindow : BaseWindow
    {
        public string documentType;

        public ConfigurationWindow(string documentType)
        {
            InitializeComponent();
            DataContext = this;

            this.documentType = documentType;
            txtDocumentTitle.Text = documentType;

            // Afficher le type de document dans le titre
            this.Title = $"Configuration - {documentType}";

            // Load Notary settings
            LoadNotarySettings();

            LoadConfigurationControls();
        }

        private void LoadNotarySettings()
        {
            txtNotaryFirstName.Text = Properties.Settings.Default.NotaryFirstName;
            txtNotaryLastName.Text = Properties.Settings.Default.NotaryLastName;
            txtNotaryCity.Text = Properties.Settings.Default.NotaryCity;
            txtNotaryCountry.Text = Properties.Settings.Default.NotaryCountry;
            txtNotaryOfficeName.Text = Properties.Settings.Default.NotaryOfficeName;
            txtNotaryOfficeAddress.Text = Properties.Settings.Default.NotaryOfficeAddress;
        }

        private void LoadConfigurationControls()
        {
            // En fonction du type de document, charger les contrôles appropriés
            switch (documentType)
            {
                case "Promesse de Vente":
                    // Charger le contrôle spécifique
                    var venteControl = new PromesseDeVenteControl();
                    MainContent.Content = venteControl;
                    break;

                // Vous pouvez ajouter d'autres cas pour d'autres types de documents

                default:
                    System.Windows.MessageBox.Show("Type de document inconnu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    break;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SkipCreateDocumentConfirmation)
            {
                CreateDocument();
            }
            else
            {
                // Open the confirmation dialog
                ConfirmationDialog dialog = new("CreateDocumentConfirm")
                {
                    Owner = this
                };
                bool? result = dialog.ShowDialog();

                if (result == true && dialog.Result)
                {
                    // Update setting if user chose to skip prompt
                    if (dialog.SkipPrompt)
                    {
                        Properties.Settings.Default.SkipCreateDocumentConfirmation = true;
                        Properties.Settings.Default.Save();
                    }

                    // Enregistrer les informations du notaire si la case est cochée
                    if (chkSaveNotaryInfo.IsChecked == true)
                    {
                        SaveNotaryInfo();
                    }

                    // Proceed with document creation
                    CreateDocument();
                }
                else
                {
                    // User canceled; do nothing
                }
            }
        }

        private void SaveNotaryInfo()
        {
            Properties.Settings.Default.NotaryFirstName = txtNotaryFirstName.Text;
            Properties.Settings.Default.NotaryLastName = txtNotaryLastName.Text;
            Properties.Settings.Default.NotaryCity = txtNotaryCity.Text;
            Properties.Settings.Default.NotaryCountry = txtNotaryCountry.Text;
            Properties.Settings.Default.NotaryOfficeName = txtNotaryOfficeName.Text;
            Properties.Settings.Default.NotaryOfficeAddress = txtNotaryOfficeAddress.Text;

            Properties.Settings.Default.Save();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SkipCancelConfigurationConfirmation)
            {
                // Switch back to the menu window
                MenuWindow menuWindow = new();
                SwitchToWindow(menuWindow);
            }
            else
            {
                // Open the confirmation dialog
                ConfirmationDialog dialog = new("CancelConfiguration")
                {
                    Owner = this
                };
                bool? result = dialog.ShowDialog();

                if (result == true && dialog.Result)
                {
                    // Update setting if user chose to skip prompt
                    if (dialog.SkipPrompt)
                    {
                        Properties.Settings.Default.SkipCancelConfigurationConfirmation = true;
                        Properties.Settings.Default.Save();
                    }

                    // Switch back to the menu window
                    MenuWindow menuWindow = new();
                    SwitchToWindow(menuWindow);
                }
                else
                {
                    // User canceled; do nothing
                }
            }
        }

        private void CreateDocument()
        {
            // Collect Notary information
            var notaryInfo = new NotaryInfo
            {
                FirstName = txtNotaryFirstName.Text,
                LastName = txtNotaryLastName.Text,
                City = txtNotaryCity.Text,
                Country = txtNotaryCountry.Text,
                OfficeName = txtNotaryOfficeName.Text,
                OfficeAddress = txtNotaryOfficeAddress.Text
            };

            // Open a folder choosing dialog
            string? filePath = GetSaveFilePath();
            if (string.IsNullOrEmpty(filePath))
            {
                // User canceled the save dialog
                return;
            }

            // Create the document using DocumentService
            DocumentService.CreateDocument(filePath, documentType: documentType, notaryInfo: notaryInfo, documentControl: MainContent.Content);

            System.Windows.MessageBox.Show("Document created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Ouvrir le document si la case est cochée
            if (chkOpenAfterCreation.IsChecked == true)
            {
                OpenDocumentFile(filePath);
            }

            // Switch back to the menu window or perform other actions as needed
            MenuWindow menuWindow = new();
            SwitchToWindow(menuWindow);
        }

        private string? GetSaveFilePath()
        {
            // Get the default documents folder
            string defaultFolder = DocumentService.GetDefaultDocumentsFolder();

            if (Properties.Settings.Default.AlwaysUseDefaultSaveFolder)
            {
                // Ensure the folder exists
                if (!Directory.Exists(defaultFolder))
                {
                    Directory.CreateDirectory(defaultFolder);
                }

                // Generate a unique file name
                string fileName = $"Document_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                return System.IO.Path.Combine(defaultFolder, fileName);
            }
            else
            {
                // Use SaveFileDialog to allow the user to choose the save location and file name
                Microsoft.Win32.SaveFileDialog saveFileDialog = new()
                {
                    InitialDirectory = defaultFolder,
                    Filter = "Word Documents (*.docx)|*.docx",
                    FileName = $"Document_{DateTime.Now:yyyyMMdd_HHmmss}.docx"
                };

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    return saveFileDialog.FileName;
                }
                else
                {
                    // User canceled the dialog
                    return null;
                }
            }
        }
    }
}
