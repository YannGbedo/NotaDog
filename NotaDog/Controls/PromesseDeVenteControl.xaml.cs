using System;
using System.Windows;
using DocumentFormat.OpenXml.Wordprocessing;
using NotaDog.Services;
using NotaDog.Interfaces;
using System.Collections.ObjectModel;
using NotaDog.Data.NotaDog.Data;
using NotaDog.Converters;
using System.Windows.Controls;
using System.ComponentModel;
using NotaDog.Data;
using TextBox = System.Windows.Controls.TextBox;

namespace NotaDog.Controls
{

    public partial class PromesseDeVenteControl : System.Windows.Controls.UserControl, INotifyPropertyChanged, IDocumentBuilder
    {
        public PromesseDeVenteData PromesseDeVenteData { get; set; }

        // Constructeur sans paramètre
        public PromesseDeVenteControl() : this(null)
        {
        }

        public PromesseDeVenteControl(PromesseDeVenteData? promesseDeVenteData = null)
        {
            InitializeComponent();

            if (promesseDeVenteData == null )
            {
                // Initialiser les données
                PromesseDeVenteData = new PromesseDeVenteData();
            }
            else
            {
                // Utiliser l'instance fournie
                PromesseDeVenteData = promesseDeVenteData;
            }


            DataContext = PromesseDeVenteData;
        }

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Gestion du changement du nombre de témoins
        private void NombreTemoinsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int nombreTemoins))
            {
                AdjustTemoinsList(nombreTemoins);
            }
        }

        private void AdjustTemoinsList(int nombreTemoins)
        {
            var temoins = PromesseDeVenteData.Temoins;

            while (temoins.Count < nombreTemoins)
            {
                temoins.Add(new Person());
            }

            while (temoins.Count > nombreTemoins)
            {
                temoins.RemoveAt(temoins.Count - 1);
            }
        }

        private void NombreMeublesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int nombreMeubles))
            {
                AdjustDesignationsEstimationsList(nombreMeubles);
            }
            else
            {
                // Si la saisie n'est pas un nombre valide, on vide la liste
                PromesseDeVenteData.DesignationsEstimations.Clear();
            }
        }

        private void AdjustDesignationsEstimationsList(int nombreMeubles)
        {
            var list = PromesseDeVenteData.DesignationsEstimations;

            while (list.Count < nombreMeubles)
            {
                list.Add(new DesignationEstimation());
            }

            while (list.Count > nombreMeubles)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        private void NombreDocumentsCertificat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int nombreDocuments))
            {
                AdjustPiecesCertificatList(nombreDocuments);
            }
            else
            {
                // Si la saisie n'est pas un nombre valide, on vide la liste
                PromesseDeVenteData.PiecesCertificat.Clear();
            }
        }

        private void AdjustPiecesCertificatList(int nombreDocuments)
        {
            var list = PromesseDeVenteData.PiecesCertificat;

            while (list.Count < nombreDocuments)
            {
                list.Add(string.Empty);
            }

            while (list.Count > nombreDocuments)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        private void NombreAttestations_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int nombreAttestations))
            {
                AdjustAttestationsList(nombreAttestations);
            }
            else
            {
                // Si la saisie n'est pas un nombre valide, on vide la liste
                PromesseDeVenteData.Attestations.Clear();
            }
        }

        private void AdjustAttestationsList(int nombreAttestations)
        {
            var list = PromesseDeVenteData.Attestations;

            while (list.Count < nombreAttestations)
            {
                list.Add(string.Empty);
            }

            while (list.Count > nombreAttestations)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public void BuildDocumentPart(Body body)
        {
            
        }
    }
}
