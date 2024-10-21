using System;
using System.Collections.Generic;
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

namespace NotaDog
{
    /// <summary>
    /// Logique d'interaction pour ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        public bool SkipPrompt { get; private set; } = false;
        public bool Result { get; private set; } = false;

        public ConfirmationDialog(string confirmationType)
        {
            InitializeComponent();

            // Set the message and checkbox based on the confirmation type
            txtMessage.Text = confirmationType switch
            {
                "OpenDocument" => "Do you want to open this document?",
                "CreateDocumentConfirm" => "Are you sure you want to create this document?",
                "CancelConfiguration" => "Are you sure you want to cancel? All unsaved changes will be lost.",
                _ => "Are you sure you want to proceed?",
            };
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SkipPrompt = chkSkipPrompt.IsChecked == true;
            Result = true; // User confirmed
            this.DialogResult = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false; // User canceled
            this.DialogResult = false;
            this.Close();
        }
    }
}
