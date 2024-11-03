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
        Male,
        Femelle,
        Autre
    }
    public enum TypeID
    {
        CarteIdentite,
        Passeport,
        Autre
    }
    public enum IdOrigin
    {
        Exemple1,
        Exemple2
    }
    public enum MaritalStatus
    {
        Married,
        Celibate,
        Other
    }
    public enum ExemptionRegime
    {
        EntrepriseIS,
        EntrepriseIBA,
        Etat,
        EtablissementPublicAdministratif,
        SocieteNationale,
        SocieteParticipationPubliqueHabitat,
        CollectivitePublique,
        OrganismePublicAcquisitionsExonere,
        OrganismePriveAcquisitionsExonere
    }

    public class GeneralData
    {
        
    }

    public class Person
    {
        public Genre Genre { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Profession { get; set; }
        public string? Adresse { get; set; }
        public string? VilleNaissance { get; set; }
        public string? PaysNaissance { get; set; }
        public DateTime DateNaissance { get; set; }
        public TypeID TypeID { get; set; }
        public string? IDNumber { get; set; }
        public IdOrigin IdOrigin { get; set; }
        public DateTime IdValidUntil { get; set; }
        public string? Nationalite { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
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
        public double ContenanceTotale { get; set; }
        public string? Objet { get; set; }

        // Constructeur
        public Immeuble()
        {
            IsRural = false;
            HasTitreFoncier = false;
            IsBati = false;
            Forme = string.Empty;
            Parcelle = string.Empty;
            Sise = string.Empty;
            Commune = string.Empty;
            ContenanceTotale = 0.0;
            Objet = string.Empty;
        }

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DesignationEstimation
    {
        public string? Designation { get; set; }
        public double Estimation { get; set; }
    }
}
