using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDog.Data
{
    public enum Genre
    {
        [Description("Monsieur")]
        Male,
        [Description("Madame")]
        Femelle,
        [Description("Mx")]
        Autre
    }
    public enum TypeID
    {
        [Description("Carte D'identité")]
        CarteIdentite,
        [Description("Passeport")]
        Passeport,
        [Description("Autre")]
        Autre
    }
    public enum IdOrigin
    {
        [Description("Autre")]
        Exemple1,
        [Description("Autre")]
        Exemple2
    }
    public enum MaritalStatus
    {
        [Description("")]
        None = 0,
        [Description("Marié")]
        Married = 1,
        [Description("Célibataire")]
        Celibate = 2,
        [Description("Autre")]
        Other = 3
    }
    public enum ExemptionRegime
    {
        [Description("Entreprise IS")]
        EntrepriseIS,
        [Description("Entreprise IBA")]
        EntrepriseIBA,
        [Description("État")]
        Etat,
        [Description("Établissement Public à caractère administratif")]
        EtablissementPublicAdministratif,
        [Description("Société Nationale")]
        SocieteNationale,
        [Description("Société à participation Publique pour l'habitat")]
        SocieteParticipationPubliqueHabitat,
        [Description("Collectivité Publique")]
        CollectivitePublique,
        [Description("Organisme Public aux acquisitions exonérées")]
        OrganismePublicAcquisitionsExonere,
        [Description("Organisme Privé aux acquisitions exonérées")]
        OrganismePriveAcquisitionsExonere,
        [Description("Pas d'Exemption")]
        NoExemption
    }

    public class GeneralData
    {
        
    }

    public class Person : INotifyPropertyChanged
    {
        public Genre Genre { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Profession { get; set; }
        public string? Adresse { get; set; }
        public string? VilleNaissance { get; set; }
        public string? PaysNaissance { get; set; }
        public DateOnly DateNaissance { get; set; }
        public TypeID TypeID { get; set; }
        public string? IDNumber { get; set; }
        public string? IdOrigin { get; set; }
        public DateOnly IdValidUntil { get; set; }
        public string? Nationalite { get; set; }
        private MaritalStatus _maritalStatus;
        public MaritalStatus MaritalStatus
        {
            get => _maritalStatus;
            set
            {
                if (_maritalStatus != value)
                {
                    _maritalStatus = value;
                    OnPropertyChanged(nameof(MaritalStatus));
                }
            }
        }
        public DateOnly MaritalStatusDate { get; set; }

        public Person()
        {
            // Initialiser les propriétés avec des valeurs par défaut ou vides
            Genre = Genre.Autre;
            Nom = string.Empty;
            Prenom = string.Empty;
            Profession = string.Empty;
            Adresse = string.Empty;
            VilleNaissance = string.Empty;
            PaysNaissance = string.Empty;
            DateNaissance = DateOnly.FromDateTime(DateTime.Now);
            TypeID = TypeID.Autre;
            IDNumber = string.Empty;
            IdOrigin = string.Empty; // Vous pouvez définir la valeur par défaut appropriée
            IdValidUntil = DateOnly.FromDateTime(DateTime.Now);
            Nationalite = string.Empty;
            MaritalStatus = MaritalStatus.None; // Statut marital défini sur Célibataire
            MaritalStatusDate = DateOnly.FromDateTime(DateTime.Now);
        }

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Immeuble : INotifyPropertyChanged
    {
        private bool _isRural;
        public bool IsRural
        {
            get => _isRural;
            set
            {
                if (_isRural != value)
                {
                    _isRural = value;
                    OnPropertyChanged(nameof(IsRural));
                }
            }
        }

        private bool _hasTitreFoncier;
        public bool HasTitreFoncier
        {
            get => _hasTitreFoncier;
            set
            {
                if (_hasTitreFoncier != value)
                {
                    _hasTitreFoncier = value;
                    OnPropertyChanged(nameof(HasTitreFoncier));
                }
            }
        }
        public string NumTitreFoncier { get; set; }

        private bool _isBati;
        public bool IsBati
        {
            get => _isBati;
            set
            {
                if (_isBati != value)
                {
                    _isBati = value;
                    OnPropertyChanged(nameof(IsBati));
                }
            }
        }

        public string? Forme { get; set; }
        public string? Parcelle { get; set; }
        public string? Sise { get; set; }
        public string? Commune { get; set; }
        private double _Contenancetotale;
        public double ContenanceTotale
        {
            get => _Contenancetotale;
            set
            {
                if (_Contenancetotale != value)
                {
                    _Contenancetotale = value;
                    OnPropertyChanged(nameof(ContenanceTotale));
                }
            }
        }
        public string? Objet { get; set; }
        public double? Volume { get; set; }
        public string? Folio { get; set; }

        // Constructeur
        public Immeuble()
        {
            IsRural = false;
            HasTitreFoncier = false;
            NumTitreFoncier = "000000";
            IsBati = false;
            Forme = string.Empty;
            Parcelle = string.Empty;
            Sise = string.Empty;
            Commune = string.Empty;
            ContenanceTotale = 0.0;
            Objet = string.Empty;
            Volume = 0.0;
            Folio = "test";
        }

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OwnershipOrigin
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Objet { get; set; }
        public int ObjetNum { get; set; }
        public string? place {  get; set; }
        public DateOnly Date { get; set; }
        public string? PlaceFiled { get; set; }
        public DateOnly DateFiled { get; set; }
        public string? Folio { get; set; }
        public string? Case { get; set; }

        public OwnershipOrigin()
        {
            Name = string.Empty;
            Price = 0;
            Objet = string.Empty;
            ObjetNum = 0;
            place = string.Empty;
            Date = DateOnly.FromDateTime(DateTime.Now);
            PlaceFiled = string.Empty;
            DateFiled = DateOnly.FromDateTime(DateTime.Now);
            Folio = string.Empty;
            Case = string.Empty;
        }
    }



    public class DesignationEstimation
    {
        public string? Designation { get; set; }
        public double Estimation { get; set; }
    }
}
