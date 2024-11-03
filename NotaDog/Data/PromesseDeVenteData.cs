using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDog.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace NotaDog.Data
    {
        public class PromesseDeVenteData : INotifyPropertyChanged
        {
            // Présence ou non de témoins
            private bool _presenceTemoins;
            public bool PresenceTemoins
            {
                get => _presenceTemoins;
                set
                {
                    if (_presenceTemoins != value)
                    {
                        _presenceTemoins = value;
                        OnPropertyChanged(nameof(PresenceTemoins));
                    }
                }
            }

            private int _nombreTemoins;
            public int NombreTemoins
            {
                get => _nombreTemoins;
                set
                {
                    if (_nombreTemoins != value)
                    {
                        _nombreTemoins = value;
                        OnPropertyChanged(nameof(NombreTemoins));
                    }
                }
            }

            // Liste des témoins (Personnes)
            public ObservableCollection<Person> Temoins { get; set; }

            // Promettant et Bénéficiaire
            public Person Promettant { get; set; }
            public Person Beneficiaire { get; set; }

            // Immeuble
            public Immeuble Immeuble { get; set; }

            // Meubles vendus simultanément
            private bool _meublesVenduSimultanement;
            public bool MeublesVenduSimultanement
            {
                get => _meublesVenduSimultanement;
                set
                {
                    if (_meublesVenduSimultanement != value)
                    {
                        _meublesVenduSimultanement = value;
                        OnPropertyChanged(nameof(MeublesVenduSimultanement));
                    }
                }
            }

            private int _nombreMeubles;
            public int NombreMeubles
            {
                get => _nombreMeubles;
                set
                {
                    if (_nombreMeubles != value)
                    {
                        _nombreMeubles = value;
                        OnPropertyChanged(nameof(NombreMeubles));
                    }
                }
            }

            public ObservableCollection<DesignationEstimation> DesignationsEstimations { get; set; }

            // Duree Promesse
            public DateTime DateButoireDemandeVente { get; set; }

            // Faculté de substitution
            public bool FaculteSubstitution { get; set; }

            // Entrée en jouissance (true pour immédiate, false pour différée)
            private bool _entreeJouissanceImmediate;
            public bool EntreeJouissanceImmediate
            {
                get => _entreeJouissanceImmediate;
                set
                {
                    if (_entreeJouissanceImmediate != value)
                    {
                        _entreeJouissanceImmediate = value;
                        OnPropertyChanged(nameof(EntreeJouissanceImmediate));
                    }
                }
            }

            // Champs pour entrée différée
            public DateTime? DateButoireJouissance { get; set; } // Nullable car dépend de EntreeJouissanceImmediate
            public double? Astreinte { get; set; }
            public double? DepositaireSequestre { get; set; }
            public string? ReceveurDepot { get; set; }

            // Prix principal
            public double PrixPrincipal { get; set; }

            // Si Immeuble bâti est true
            public double? PrixTerrainNu { get; set; }
            public double? PrixConstructions { get; set; }

            // Exemption de taxe au régime
            public ExemptionRegime? ExemptionRegime { get; set; } // Nullable si pas d'exemption

            // Servitude connue
            private bool _servitudeConnue;
            public bool ServitudeConnue
            {
                get => _servitudeConnue;
                set
                {
                    if (_servitudeConnue != value)
                    {
                        _servitudeConnue = value;
                        OnPropertyChanged(nameof(ServitudeConnue));
                    }
                }
            }

            public string? ServitudeDe { get; set; } // Si ServitudeConnue est true

            // Obtention d'une note de renseignement d'urbanisme ou certificat d'urbanisme
            public bool NecessiteNoteRenseignementUrbanisme { get; set; }
            public bool NecessiteCertificatUrbanisme { get; set; }

            // Propriétés affectant la visibilité ou l'état des contrôles
            private bool _obtentionPretNecessaire;
            public bool ObtentionPretNecessaire
            {
                get => _obtentionPretNecessaire;
                set
                {
                    if (_obtentionPretNecessaire != value)
                    {
                        _obtentionPretNecessaire = value;
                        OnPropertyChanged(nameof(ObtentionPretNecessaire));
                    }
                }
            }

            // Champs si obtention de prêt est true
            public double? MontantTotalPret { get; set; }
            public double? DureePret { get; set; }
            public double? TauxPret { get; set; }
            public DateTime? DateButoirePret { get; set; }

            // Démarches pour le certificat d'appartenance
            private bool? _promettantFaitDemarcheCertificat;
            public bool? PromettantFaitDemarcheCertificat
            {
                get => _promettantFaitDemarcheCertificat;
                set
                {
                    if (_promettantFaitDemarcheCertificat != value)
                    {
                        _promettantFaitDemarcheCertificat = value;
                        OnPropertyChanged(nameof(PromettantFaitDemarcheCertificat));
                    }
                }
            }

            // Si le Promettant fait les démarches lui-même
            public DateTime? DateButoireCertificat { get; set; }

            // Si le Notaire fait les démarches
            public int? NombreDocumentsCertificat { get; set; }
            public ObservableCollection<string> PiecesCertificat { get; set; }

            // Versement d'une indemnité
            private bool _versementIndemnite;
            public bool VersementIndemnite
            {
                get => _versementIndemnite;
                set
                {
                    if (_versementIndemnite != value)
                    {
                        _versementIndemnite = value;
                        OnPropertyChanged(nameof(VersementIndemnite));
                    }
                }
            }

            // Champs si VersementIndemnite est true
            private bool? _versementSequestre;
            public bool? VersementSequestre
            {
                get => _versementSequestre;
                set
                {
                    if (_versementSequestre != value)
                    {
                        _versementSequestre = value;
                        OnPropertyChanged(nameof(VersementSequestre));
                    }
                }
            }
            public int? NumeroCheque { get; set; }
            public string? Banque { get; set; }
            public double? Somme { get; set; }

            // Si VersementSequestre est true
            public string? NomSequestre { get; set; }
            public bool? PresenceSequestre { get; set; }

            // Somme clause pénale
            public double SommeClausePenale { get; set; }

            // Informations supplémentaires si l'immeuble est bâti
            public bool? PermisConstruireObtenu { get; set; }
            public bool? ConstruitParPromettant { get; set; }
            public string? DelivreurPermis { get; set; }
            public DateTime? DateDelivrancePermis { get; set; }
            public string? LieuOuvertureChantier { get; set; }
            public DateTime? DateOuvertureChantier { get; set; }
            public string? LieuDeclarationAchevement { get; set; }
            public DateTime? DateDeclarationAchevement { get; set; }
            public string? DelivreurCertificatConformite { get; set; }
            public DateTime? DateDelivranceCertificatHabilite { get; set; }

            // Promettant a souscrit à une assurance
            private bool? _promettantSouscritAssurance;
            public bool? PromettantSouscritAssurance
            {
                get => _promettantSouscritAssurance;
                set
                {
                    if (_promettantSouscritAssurance != value)
                    {
                        _promettantSouscritAssurance = value;
                        OnPropertyChanged(nameof(PromettantSouscritAssurance));
                    }
                }
            }

            public bool? AssurancesEntreprisesObtenu { get; set; }
            public bool? ExemptionAssuranceConstruction { get; set; }

            private int? _nombreAttestations;
            public int? NombreAttestations
            {
                get => _nombreAttestations;
                set
                {
                    if (_nombreAttestations != value)
                    {
                        _nombreAttestations = value;
                        OnPropertyChanged(nameof(NombreAttestations));
                    }
                }
            }

            public ObservableCollection<string> Attestations { get; set; }

            // Si PromettantSouscritAssurance est true
            public string? AssuranceTousRisques { get; set; }
            public int? NumeroPoliceAssuranceTousRisques { get; set; }
            public DateTime? DateAssuranceTousRisques { get; set; }
            public string? AssuranceDommage { get; set; }
            public int? NumeroPoliceAssuranceDommage { get; set; }
            public DateTime? DateAssuranceDommage { get; set; }
            public string? AssuranceResponsabiliteCivile { get; set; }
            public int? NumeroPoliceAssuranceResponsabiliteCivile { get; set; }
            public DateTime? DateAssuranceResponsabiliteCivile { get; set; }

            // Renvois et mentions légales
            public int Renvois { get; set; }
            public int MotsRayesNuls { get; set; }
            public int ChiffresRayesNuls { get; set; }
            public int LignesRayeesNulles { get; set; }
            public int BarresTireesBlancs { get; set; }

            // Constructeur
            public PromesseDeVenteData()
            {
                // Initialiser les propriétés avec des valeurs par défaut
                PresenceTemoins = false;
                NombreTemoins = 0;
                Temoins = new ObservableCollection<Person>();

                Promettant = new Person();
                Beneficiaire = new Person();

                Immeuble = new Immeuble();

                MeublesVenduSimultanement = false;
                NombreMeubles = 0;
                DesignationsEstimations = new ObservableCollection<DesignationEstimation>();

                DateButoireDemandeVente = DateTime.Now;

                FaculteSubstitution = false;
                EntreeJouissanceImmediate = true;

                DateButoireJouissance = null;
                Astreinte = null;
                DepositaireSequestre = null;
                ReceveurDepot = null;

                PrixPrincipal = 0.0;
                PrixTerrainNu = null;
                PrixConstructions = null;

                ExemptionRegime = null;

                ServitudeConnue = false;
                ServitudeDe = null;

                NecessiteNoteRenseignementUrbanisme = false;
                NecessiteCertificatUrbanisme = false;

                ObtentionPretNecessaire = false;

                MontantTotalPret = null;
                DureePret = null;
                TauxPret = null;
                DateButoirePret = null;

                PromettantFaitDemarcheCertificat = false;
                DateButoireCertificat = null;
                NombreDocumentsCertificat = 0;
                PiecesCertificat = new ObservableCollection<string>();

                VersementIndemnite = false;
                VersementSequestre = false;
                NumeroCheque = null;
                Banque = null;
                Somme = null;

                NomSequestre = null;
                PresenceSequestre = false;

                SommeClausePenale = 0.0;

                PermisConstruireObtenu = false;
                ConstruitParPromettant = false;
                DelivreurPermis = null;
                DateDelivrancePermis = null;
                LieuOuvertureChantier = null;
                DateOuvertureChantier = null;
                LieuDeclarationAchevement = null;
                DateDeclarationAchevement = null;
                DelivreurCertificatConformite = null;
                DateDelivranceCertificatHabilite = null;
                PromettantSouscritAssurance = false;
                AssurancesEntreprisesObtenu = false;
                ExemptionAssuranceConstruction = false;
                NombreAttestations = 0;
                Attestations = new ObservableCollection<string>();

                AssuranceTousRisques = null;
                NumeroPoliceAssuranceTousRisques = null;
                DateAssuranceTousRisques = null;
                AssuranceDommage = null;
                NumeroPoliceAssuranceDommage = null;
                DateAssuranceDommage = null;
                AssuranceResponsabiliteCivile = null;
                NumeroPoliceAssuranceResponsabiliteCivile = null;
                DateAssuranceResponsabiliteCivile = null;

                Renvois = 0;
                MotsRayesNuls = 0;
                ChiffresRayesNuls = 0;
                LignesRayeesNulles = 0;
                BarresTireesBlancs = 0;
            }

            // Implémentation de INotifyPropertyChanged
            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
