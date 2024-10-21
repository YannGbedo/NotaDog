using System;
using System.Collections.Generic;
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
using DocumentFormat.OpenXml.Packaging;
using NotaDog.Controls;
using NotaDog.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Microsoft.Win32;

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
        public string? AuthorityCity { get; set; }
        public string? AuthorityCountry { get; set; }
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

                    // Proceed with document creation
                    CreateDocument();
                }
                else
                {
                    // User canceled; do nothing
                }
            }
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
                OfficeAddress = txtNotaryOfficeAddress.Text,
                AuthorityCity = txtNotaryCity.Text,
                AuthorityCountry = txtNotaryCountry.Text
            };

            // Collect document-specific data
            PromesseDeVenteData? documentData = null;
            if (MainContent.Content is PromesseDeVenteControl venteControl)
            {
                documentData = venteControl.GetData();
                // Use notaryInfo and documentData to create the document
            }

            // Open a folder choosing dialog
            string? filePath = GetSaveFilePath();
            if (string.IsNullOrEmpty(filePath))
            {
                // User canceled the save dialog
                return;
            }

            // Create a Word document for testing
            // Create the document using DocumentService
            DocumentService.CreateDocument(filePath, this.documentType, notaryInfo, documentData);

            System.Windows.MessageBox.Show("Document created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
