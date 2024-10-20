using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace NotaDog.Windows
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : BaseWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // Load current settings
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load the settings into the UI elements
            string documentsFolder = Properties.Settings.Default.DocumentsFolder;
            if (string.IsNullOrEmpty(documentsFolder))
            {
                documentsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NotaDogDocs");
            }
            txtDocumentsFolder.Text = documentsFolder;
            chkAlwaysOpenWithoutPrompt.IsChecked = Properties.Settings.Default.AlwaysOpenWithoutPrompt;

            // Load Notary Information
            txtNotaryFirstName.Text = Properties.Settings.Default.NotaryFirstName;
            txtNotaryLastName.Text = Properties.Settings.Default.NotaryLastName;
            txtNotaryCity.Text = Properties.Settings.Default.NotaryCity;
            txtNotaryCountry.Text = Properties.Settings.Default.NotaryCountry;
            txtNotaryOfficeName.Text = Properties.Settings.Default.NotaryOfficeName;
            txtNotaryOfficeAddress.Text = Properties.Settings.Default.NotaryOfficeAddress;
            txtNotaryAuthorityCity.Text = Properties.Settings.Default.NotaryAuthorityCity;
            txtNotaryAuthorityCountry.Text = Properties.Settings.Default.NotaryAuthorityCountry;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate the documents folder
            string folderPath = txtDocumentsFolder.Text;
            if (!Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("The specified documents folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save the settings
            Properties.Settings.Default.DocumentsFolder = folderPath;
            Properties.Settings.Default.AlwaysOpenWithoutPrompt = chkAlwaysOpenWithoutPrompt.IsChecked == true;

            // Save Notary Information
            Properties.Settings.Default.NotaryFirstName = txtNotaryFirstName.Text;
            Properties.Settings.Default.NotaryLastName = txtNotaryLastName.Text;
            Properties.Settings.Default.NotaryCity = txtNotaryCity.Text;
            Properties.Settings.Default.NotaryCountry = txtNotaryCountry.Text;
            Properties.Settings.Default.NotaryOfficeName = txtNotaryOfficeName.Text;
            Properties.Settings.Default.NotaryOfficeAddress = txtNotaryOfficeAddress.Text;
            Properties.Settings.Default.NotaryAuthorityCity = txtNotaryAuthorityCity.Text;
            Properties.Settings.Default.NotaryAuthorityCountry = txtNotaryAuthorityCountry.Text;

            Properties.Settings.Default.Save();

            // Switch back to the menu window
            MenuWindow menuWindow = new();
            SwitchToWindow(menuWindow);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Discard changes and return to the menu window
            MenuWindow menuWindow = new();
            SwitchToWindow(menuWindow);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select the default documents folder",
                ShowNewFolderButton = true,
                SelectedPath = txtDocumentsFolder.Text
            };

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtDocumentsFolder.Text = dialog.SelectedPath;
            }
        }
    }
}
