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
using NotaDog.Controls;

namespace NotaDog.Windows
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : BaseWindow
    {
        private string documentType;

        public ConfigurationWindow(string documentType)
        {
            InitializeComponent();
            this.documentType = documentType;

            // Afficher le type de document dans le titre
            this.Title = $"Configuration - {documentType}";

            LoadConfigurationControls();
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
    }
}
