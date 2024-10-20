using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            txtDocumentsFolder.Text = Properties.Settings.Default.DocumentsFolder;
            chkAlwaysOpenWithoutPrompt.IsChecked = Properties.Settings.Default.AlwaysOpenWithoutPrompt;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the settings
            Properties.Settings.Default.DocumentsFolder = txtDocumentsFolder.Text;
            Properties.Settings.Default.AlwaysOpenWithoutPrompt = chkAlwaysOpenWithoutPrompt.IsChecked == true;
            Properties.Settings.Default.Save();

            // Switch back to the menu window
            MenuWindow menuWindow = new MenuWindow();
            SwitchToWindow(menuWindow);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Discard changes and return to the menu window
            MenuWindow menuWindow = new MenuWindow();
            SwitchToWindow(menuWindow);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a folder browser dialog to select a new documents folder
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = txtDocumentsFolder.Text;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtDocumentsFolder.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
