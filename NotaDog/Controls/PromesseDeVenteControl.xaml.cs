using System;
using System.Windows;
using DocumentFormat.OpenXml.Wordprocessing;
using NotaDog.Services;
using DS = NotaDog.Services.DocumentService;
using NotaDog.Interfaces;
using System.Collections.ObjectModel;
using NotaDog.Data.NotaDog.Data;
using NotaDog.Converters;
using System.Windows.Controls;
using System.ComponentModel;
using NotaDog.Data;
using TextBox = System.Windows.Controls.TextBox;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using ListPoints = NotaDog.Services.ListPoints;
using System.Printing;
using System.Windows.Documents;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;

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

            if (promesseDeVenteData == null)
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

        public void BuildDocumentPart(MainDocumentPart mainPart)
        {
            // Vérifier que PromesseDeVenteData n'est pas null
            if (PromesseDeVenteData == null)
            {
                throw new InvalidOperationException("PromesseDeVenteData ne peut pas être null.");
            }

            if (mainPart.Document.Body != null)
            {
                Body body = mainPart.Document.Body;

                // Create some default styles

                TextStyle GreenBoldStyle = new()
                {
                    Bold = true,
                    Color = "Green"
                };

                TextStyle UnderlineStyle = new()
                {
                    Underline = true
                };

                TextStyle BoldStyle = new()
                {
                    Bold = true
                };


                // Initializing the Person Counter
                int personCounter = 0;

                // temoins
                if (PromesseDeVenteData.Temoins.Count > 0)
                {
                    body.Append(DS.CreateParagraph("En présence réelle de :", JustificationValues.Right, ParagraphType.Special));
                    for (int i = 0; i < PromesseDeVenteData.Temoins.Count; i++)
                    {
                        personCounter++;
                        var temoin = PromesseDeVenteData.Temoins[i];
                        string prefix = (i == 0) ? "" : "Et ";
                        string TexteTemoin = $"{personCounter}. {prefix}{DS.GetEnumDescription(temoin.Genre)} {temoin.Nom + " " + temoin.Prenom}, {temoin.Profession}, demeurant à {temoin.Adresse};";
                        string TexteTemoin2 = $"Né à {temoin.VilleNaissance} ({temoin.PaysNaissance}) le {temoin.DateNaissance} ; Titulaire du {DS.GetEnumDescription(temoin.TypeID)} numéro {temoin.IDNumber} délivrée par {temoin.IdOrigin} et valable jusqu’au {temoin.IdValidUntil} ; De nationalité {temoin.Nationalite} ;";
                        body.Append(DS.CreateParagraph(TexteTemoin, JustificationValues.Both, ParagraphType.Special));
                        body.Append(DS.CreateParagraph(TexteTemoin2, JustificationValues.Both, ParagraphType.Special));
                    }
                    string TexteTemoinEnd = "Témoins certificateurs ayant les qualités requises par la Loi ainsi qu’ils le déclarent ;";
                    string TexteTemoinEnd2 = $"Lesquels déclarent, sous leur responsabilité, parfaitement connaître Monsieur {PromesseDeVenteData.Promettant.Nom}, Vendeur ci-après nommé, et certifient sa présence à l’acte. Ils attestent également que, Monsieur {PromesseDeVenteData.Beneficiaire.Nom} est vivant et qu’il est bien le propriétaire de l’immeuble ci-dessous désigné ; ";
                    body.Append(DS.CreateParagraph(TexteTemoinEnd, JustificationValues.Both, ParagraphType.Special));
                    body.Append(DS.CreateParagraph(TexteTemoinEnd2, JustificationValues.Both, ParagraphType.Special));
                }

                // Add the people involved
                body.Append(DS.CreateParagraph("A COMPARU :", JustificationValues.Right, ParagraphType.Special, underlineFrom: "A ", underlineUntil: "RU"));
                var promettant = PromesseDeVenteData.Promettant;
                var beneficiaire = PromesseDeVenteData.Beneficiaire;

                // Add Promettant
                personCounter++;
                string TextePromettant = $"{personCounter}. {DS.GetEnumDescription(promettant.Genre)} {promettant.Nom + " " + promettant.Prenom}, {promettant.Profession}, demeurant à {promettant.Adresse};";
                string TextePromettant2 = $"Né à {promettant.VilleNaissance} ({promettant.PaysNaissance}) le {promettant.DateNaissance} ; Titulaire du {DS.GetEnumDescription(promettant.TypeID)} numéro {promettant.IDNumber} délivrée par {promettant.IdOrigin} et valable jusqu’au {promettant.IdValidUntil} ; De nationalité {promettant.Nationalite} ;";
                string TextePromettant3 = string.Empty;
                string TextePromettant4 = "Ci-après dénommé le « PROMETTANT »,";
                string TextePromettant5 = "D’UNE PART ;";

                switch (promettant.MaritalStatus)
                {
                    case MaritalStatus.None:
                        break;
                    case MaritalStatus.Married:
                        break;
                    case MaritalStatus.Celibate:
                        TextePromettant3 = $"Lequel, déclare être célibataire pour n’avoir jamais été marié conformément au certificat de coutume et de célibat délivré par l’Agence Nationale d’Identification des Personnes (ANIP), en date à COTONOU, du {promettant.MaritalStatusDate}, et dont l’original est ci-après annexé aux présentes ;";
                        break;
                    case MaritalStatus.Other:
                        break;
                    default:
                        break;
                }

                body.Append(DS.CreateParagraph(TextePromettant, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TextePromettant2, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TextePromettant3, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TextePromettant4, JustificationValues.Right, ParagraphType.Special, boldFrom: "PRO", boldUntil: "ANT"));
                body.Append(DS.CreateParagraph(TextePromettant5, JustificationValues.Right, ParagraphType.Special, underlineFrom: "D’", underlineUntil: "RT"));
                body.Append(DS.SParagraph());

                // Add Beneficiaire
                personCounter++;
                string TexteBeneficiaire = $"{personCounter}. {DS.GetEnumDescription(beneficiaire.Genre)} {beneficiaire.Nom + " " + beneficiaire.Prenom}, {beneficiaire.Profession}, demeurant à {beneficiaire.Adresse};";
                string TexteBeneficiaire2 = $"Né à {beneficiaire.VilleNaissance} ({beneficiaire.PaysNaissance}) le {beneficiaire.DateNaissance} ; Titulaire du {DS.GetEnumDescription(beneficiaire.TypeID)} numéro {beneficiaire.IDNumber} délivrée par {beneficiaire.IdOrigin} et valable jusqu’au {beneficiaire.IdValidUntil} ; De nationalité {beneficiaire.Nationalite} ;";
                string TexteBeneficiaire3 = "Ci-après dénommés le « BENEFICIAIRE »,";
                string TexteBeneficiaire4 = "D’AUTRE PART.";
                string TexteBeneficiaire5 = "Lesquels ès qualités ont, par ces présentes, requis le notaire soussigné de constater par acte authentique les conventions suivantes :";

                body.Append(DS.CreateParagraph(TexteBeneficiaire, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TexteBeneficiaire2, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TexteBeneficiaire3, JustificationValues.Right, ParagraphType.Special, boldFrom: "BEN", boldUntil: "IRE"));
                body.Append(DS.CreateParagraph(TexteBeneficiaire4, JustificationValues.Right, ParagraphType.Special, underlineFrom: "D’", underlineUntil: "RT"));
                body.Append(DS.SParagraph());
                body.Append(DS.CreateParagraph(TexteBeneficiaire5, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.SParagraph());

                // Clause Etat CAPACITE
                body.Append(DS.CreateHeadingParagraph("ÉTAT. CAPACITE", 2, ParagraphType.Special, UnderlineStyle));
                string TexteEtatCapacite = "Les parties confirment l'exactitude des indications les concernant respectivement telles qu'elles figurent ci-dessus.";
                string TexteEtatCapacite2 = "Ils déclarent en outre qu'ils ne font l'objet d'aucune mesure ou procédure susceptible de restreindre leur capacité ou de mettre obstacle à la libre disposition de leurs biens.";
                string TexteEtatCapacite3 = "Ils déclarent qu’ils ne sont pas en état de cessation de paiement, redressement ou liquidation judiciaire, ni concernés par aucune mesure de protection telle que la tutelle ou la curatelle.";

                body.Append(DS.CreateParagraph(TexteEtatCapacite, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TexteEtatCapacite2, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.CreateParagraph(TexteEtatCapacite3, JustificationValues.Both, ParagraphType.Special));
                body.Append(DS.SParagraph());

                // Clause Promesse De Vente
                body.Append(DS.CreateHeadingParagraph("PROMESSE DE VENTE", 2, ParagraphType.General, UnderlineStyle));
                string TextePromesseVente = "Le PROMETTANT confère au BENEFICIAIRE la faculté d'acquérir, le BIEN ci-dessous identifié.";
                string TextePromesseVente2 = "Le PROMETTANT prend cet engagement pour lui-même ou ses ayants droit même protégés.";
                string TextePromesseVente3 = "Le BENEFICIAIRE accepte la présente promesse de vente en tant que promesse, mais se réserve la faculté d'en demander ou non la réalisation.";
                string TextePromesseVente4 = "Cette promesse de vente est établie conformément aux dispositions du Code foncier et domanial et du Code civil.";

                body.Append(DS.CreateParagraph(TextePromesseVente, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePromesseVente2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePromesseVente3, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePromesseVente4, JustificationValues.Both, ParagraphType.General));

                // Clause Designation
                body.Append(DS.CreateHeadingParagraph("DESIGNATION", 2, ParagraphType.General, UnderlineStyle));

                var immeuble = PromesseDeVenteData.Immeuble;
                string TexteDesignation = string.Empty;
                string TexteDesignation2 = string.Empty;
                string TexteDesignation3 = string.Empty;
                string TexteDesignation4 = string.Empty;
                string TexteDesignation5 = string.Empty;
                
                List<ListPoints> items =
                [
                    new ListPoints("Premier élément", 0),
                    new ListPoints("Deuxième élément", 0)
                ];

                switch (immeuble.IsRural, immeuble.HasTitreFoncier, immeuble.IsBati)
                {
                    case (true, false, false):
                        // Code for: Immeuble is rural, no titre foncier, and not bâti.
                        TexteDesignation = $"Un immeuble rural non bâti, de forme {immeuble.Forme}, constituant la parcelle {immeuble.Parcelle}, sise à {immeuble.Sise}, Commune {immeuble.Commune}, d’une contenance totale de {NumberToFrenchWordsConverter.ConvertToFrenchWords(immeuble.ContenanceTotale)} ({immeuble.ContenanceTotale}), faisant l’objet de {immeuble.Objet} ;";
                        TexteDesignation2 = "Ledit immeuble est limité :";
                        TexteDesignation3 = $"Ledit immeuble fait l’objet du {immeuble.Objet}.";
                        TexteDesignation4 = "Tel au surplus que ledit immeuble existe, s'étend, se poursuit et se comporte sans aucune exception ni réserve, l’ACQUEREUR déclarant le bien connaître pour l'avoir visité à l'effet des présentes.";
                        TexteDesignation5 = "Demeure ci-annexée une copie du levé topographique avec plan de situation du BIEN objet des présentes.";

                        body.Append(DS.CreateParagraph(TexteDesignation, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation2, JustificationValues.Both, ParagraphType.General));
                        DS.CreateBulletList(mainPart, items, ParagraphType.General);
                        body.Append(DS.CreateParagraph(TexteDesignation3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation4, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation5, JustificationValues.Both, ParagraphType.General));

                        break;

                    case (true, false, true):
                        // Code for: Immeuble is rural, no titre foncier, but is bâti.
                        TexteDesignation = $"Un immeuble rural bâti, de forme {immeuble.Forme}, constituant la parcelle {immeuble.Parcelle}, sise à {immeuble.Sise}, Commune {immeuble.Commune}, d’une contenance totale de {NumberToFrenchWordsConverter.ConvertToFrenchWords(immeuble.ContenanceTotale)} ({immeuble.ContenanceTotale}), faisant l’objet de {immeuble.Objet} ;";
                        TexteDesignation2 = "Ledit immeuble est limité :";
                        TexteDesignation3 = $"Ledit immeuble fait l’objet du {immeuble.Objet}.";
                        TexteDesignation4 = "Tel au surplus que ledit immeuble existe, s'étend, se poursuit et se comporte sans aucune exception ni réserve, l’ACQUEREUR déclarant le bien connaître pour l'avoir visité à l'effet des présentes.";
                        TexteDesignation5 = "Demeure ci-annexée une copie du levé topographique avec plan de situation du BIEN objet des présentes.";

                        body.Append(DS.CreateParagraph(TexteDesignation, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation2, JustificationValues.Both, ParagraphType.General));
                        DS.CreateBulletList(mainPart, items, ParagraphType.General);
                        body.Append(DS.CreateParagraph(TexteDesignation3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation4, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteDesignation5, JustificationValues.Both, ParagraphType.General));
                        break;

                    case (false, true, false):
                        // Code for: Immeuble is not rural, has titre foncier, and not bâti.
                        TexteDesignation = $"Un immeuble urbain non bâti, de forme {immeuble.Forme}, constituant la parcelle {immeuble.Parcelle}, sise à {immeuble.Sise}, Commune {immeuble.Commune}, d’une contenance totale de {NumberToFrenchWordsConverter.ConvertToFrenchWords(immeuble.ContenanceTotale)} ({immeuble.ContenanceTotale}), faisant l’objet du Titre Foncier numéro {immeuble.NumTitreFoncier} inséré au Registre Foncier de la Commune de {immeuble.Commune}, volume {immeuble.Volume}, folio {immeuble.Folio} ;";
                        TexteDesignation2 = "Limité :";
                        TexteDesignation3 = "Tel au surplus que ledit immeuble existe, s'étend, se poursuit et se comporte sans aucune exception ni réserve, l’ACQUEREUR déclarant le bien connaître pour l'avoir visité à l'effet des présentes.";

                        body.Append(DS.CreateParagraph(TexteDesignation, JustificationValues.Both, ParagraphType.General, boldFrom: "Titre", boldUntil: immeuble.Folio));
                        body.Append(DS.CreateParagraph(TexteDesignation2, JustificationValues.Both, ParagraphType.General));
                        DS.CreateBulletList(mainPart, items, ParagraphType.General);
                        body.Append(DS.CreateParagraph(TexteDesignation3, JustificationValues.Both, ParagraphType.General));
                        break;

                    case (false, true, true):
                        // Code for: Immeuble is not rural, has titre foncier, and is bâti.
                        TexteDesignation = $"Un immeuble urbain bâti, de forme {immeuble.Forme}, constituant la parcelle {immeuble.Parcelle}, sise à {immeuble.Sise}, Commune {immeuble.Commune}, d’une contenance totale de {NumberToFrenchWordsConverter.ConvertToFrenchWords(immeuble.ContenanceTotale)} ({immeuble.ContenanceTotale}), faisant l’objet du Titre Foncier numéro {immeuble.NumTitreFoncier} inséré au Registre Foncier de la Commune de {immeuble.Commune}, volume {immeuble.Volume}, folio {immeuble.Folio} ;";
                        TexteDesignation2 = "Limité :";
                        TexteDesignation3 = "Tel au surplus que ledit immeuble existe, s'étend, se poursuit et se comporte sans aucune exception ni réserve, l’ACQUEREUR déclarant le bien connaître pour l'avoir visité à l'effet des présentes.";

                        body.Append(DS.CreateParagraph(TexteDesignation, JustificationValues.Both, ParagraphType.General, boldFrom: "Titre", boldUntil: immeuble.Folio));
                        body.Append(DS.CreateParagraph(TexteDesignation2, JustificationValues.Both, ParagraphType.General));
                        DS.CreateBulletList(mainPart, items, ParagraphType.General);
                        body.Append(DS.CreateParagraph(TexteDesignation3, JustificationValues.Both, ParagraphType.General));
                        break;

                    default:
                        // Code for any other scenario, if needed.
                        break;
                }
                body.Append(DS.SParagraph());

                // Clause Origine Propriete
                body.Append(DS.CreateHeadingParagraph("ORIGINE DE PROPRIETE", 2, ParagraphType.General, UnderlineStyle));

                var ownershipOrigin = PromesseDeVenteData.OwnershipOrigin;

                string TexteOriginePropriete = $"L'immeuble ci-dessus désigné, objet de la présente cession, appartient en propre au VENDEUR pour l’avoir acquis par voie d’achat auprès de {ownershipOrigin.Name}, suivant une convention de vente, moyennant un prix de {NumberToFrenchWordsConverter.ConvertToFrenchWords(ownershipOrigin.Price)} ({ownershipOrigin.Price}) francs CFA, et a fait l’objet d’un {ownershipOrigin.Objet} portant le numéro {ownershipOrigin.ObjetNum} en date à {ownershipOrigin.place} du {ownershipOrigin.Date}. Ledit acte a été enregistré à {ownershipOrigin.PlaceFiled} le {ownershipOrigin.DateFiled}, folio {ownershipOrigin.Folio} case {ownershipOrigin.Case}.";
                body.Append(DS.CreateParagraph(TexteOriginePropriete, JustificationValues.Both, ParagraphType.General));

                // Clause Identification Meubles
                body.Append(DS.CreateHeadingParagraph("IDENTIFICATION DES MEUBLES", 2, ParagraphType.General, UnderlineStyle));

                if (PromesseDeVenteData.MeublesVenduSimultanement)
                {
                    var designations = PromesseDeVenteData.DesignationsEstimations;

                    string TexteMeubles = "La promesse de vente comprend les meubles meublants ci-désignés :";
                    body.Append(DS.CreateParagraph(TexteMeubles, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.SParagraph());
                    CreateDesignationEstimationTable(body, designations);
                }
                else
                {
                    string TexteNoMeubles = "La promesse de vente ne comprend pas de meuble.";

                    body.Append(DS.CreateParagraph(TexteNoMeubles, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause DUREE DE LA PROMESSE 
                body.Append(DS.CreateHeadingParagraph("DUREE DE LA PROMESSE", 2, ParagraphType.General, UnderlineStyle));

                string TexteDureePromesse = $"La réalisation de la vente promise pourra être demandée par le BENEFICIAIRE jusqu'au {PromesseDeVenteData.DateButoireDemandeVente} inclusivement.";
                string TexteDureePromesse2 = "Pendant ce délai, le PROMETTANT ne pourra pas révoquer son engagement.";
                string TexteDureePromesse3 = "En cas de carence du PROMETTANT pour la réalisation de la vente, ce dernier ne saurait se prévaloir à l’encontre du BENEFICIAIRE de l’expiration du délai ci-dessus fixé.";
                string TexteDureePromesse4 = "Si le PROMETTANT vend le bien promis, en violation de la promesse de vente, à un tiers qui en connaissait l’existence, l’acte est nul et le BENEFICIAIRE peut être substitué au tiers acquéreur.";

                body.Append(DS.CreateParagraph(TexteDureePromesse, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteDureePromesse2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteDureePromesse3, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteDureePromesse4, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause MODALITES DE REALISATION DE LA VENTE
                body.Append(DS.CreateHeadingParagraph("MODALITES DE REALISATION DE LA VENTE", 2, ParagraphType.General, UnderlineStyle));

                body.Append(DS.CreateParagraph("La réalisation de la promesse aura lieu :", JustificationValues.Both, ParagraphType.General));

                List<ListPoints> modaliteItemsList1 = new List<ListPoints>
                {
                    new ListPoints("Soit par la signature de l'acte authentique constatant le caractère définitif de la vente, accompagnée par la remise d’un chèque de banque ou du versement par virement sur le compte du notaire chargé de recevoir l’acte authentique de vente d’une somme correspondant :", 0),
                    new ListPoints("au prix stipulé payable comptant déduction faite de l’indemnité d’immobilisation éventuellement versée en exécution des présentes,", 1),
                    new ListPoints("à la provision sur frais d’acte de vente et de prêt éventuel,", 1),
                    new ListPoints("à l’éventuelle commission d’intermédiaire,", 1),
                    new ListPoints("et de manière générale de tous comptes et proratas.", 1),
                    new ListPoints("Soit par la levée d'option faite par le BÉNÉFICIAIRE à l'intérieur de ce délai, suivie de la signature de l’acte authentique de vente dans le délai visé ci-dessus. Si la levée d’option a lieu alors que des conditions suspensives sont encore pendantes, elle n’impliquera pas renonciation à celles-ci, sauf volonté contraire exprimée par le BÉNÉFICIAIRE. Cette levée d'option sera effectuée par le BÉNÉFICIAIRE auprès du notaire rédacteur de l’acte de vente par tous moyens et toutes formes ; elle devra être accompagnée, pour être recevable, par la remise d’un chèque de banque ou du versement par virement sur le compte dudit notaire d’une somme correspondant :", 0),
                    new ListPoints("au prix stipulé payable comptant déduction faite de l’indemnité d’immobilisation éventuellement versée en exécution des présentes (étant précisé que, pour la partie du prix payé au moyen d'un emprunt, il convient de justifier de la disponibilité des fonds ou d’une offre de prêt acceptée),", 1),
                    new ListPoints("à la provision sur frais d’acte de vente et de prêt éventuel,", 1),
                    new ListPoints("à l’éventuelle commission d’intermédiaire,", 0),
                    new ListPoints("et de manière générale de tous comptes et proratas.", 1)
                };
                DS.CreateBulletList(mainPart, modaliteItemsList1, ParagraphType.General);

                body.Append(DS.CreateParagraph("L’attention du BÉNÉFICIAIRE est particulièrement attirée sur les points suivants :", JustificationValues.Both, ParagraphType.General));

                List<ListPoints> modaliteItemsList2 = new List<ListPoints>
                {
                    new ListPoints("Il lui sera imposé de fournir une attestation émanant de la banque qui aura émis le virement et justifiant de l’origine des fonds sauf si ces fonds résultent d'un ou plusieurs prêts constatés dans l'acte authentique de vente ou dans un acte authentique séparé.", 0)
                };
                DS.CreateBulletList(mainPart, modaliteItemsList2, ParagraphType.General);

                body.Append(DS.SParagraph());

                // Clause FORCE EXÉCUTOIRE DE LA PROMESSE
                body.Append(DS.CreateHeadingParagraph("FORCE EXÉCUTOIRE DE LA PROMESSE", 2, ParagraphType.General, UnderlineStyle));

                string TexteExecPromesse1 = "Il est entendu entre les PARTIES qu’en raison de l’acceptation par le BENEFICIAIRE de la promesse faite par le PROMETTANT, en tant que simple promesse, il s’est formé entre elles un contrat dans les termes de l'article 1103 du Code civil. En conséquence, et pendant toute la durée du contrat, celui-ci ne pourra être révoqué que par leur consentement mutuel. Il en résulte notamment que :";
                body.Append(DS.CreateParagraph(TexteExecPromesse1, JustificationValues.Both, ParagraphType.General));

                List<ListPoints> execPromesseItemsList = new List<ListPoints>
                {
                    new ListPoints("Le PROMETTANT a, pour sa part, définitivement consenti à la vente et qu’il est d’ores et déjà débiteur de l’obligation de transférer la propriété au profit du BENEFICIAIRE aux conditions des présentes. Le PROMETTANT ne peut plus, par suite, pendant toute la durée de la présente promesse, conférer une autre promesse à un tiers ni aucun droit réel ni charge quelconque sur le BIEN, consentir aucun bail, location ou prorogation de bail. Il ne pourra non plus apporter aucune modification matérielle, si ce n'est avec le consentement du BENEFICIAIRE, ni détérioration au BIEN. Il en ira de même si la charge ou la détérioration n'était pas le fait direct du PROMETTANT. Le non-respect de cette obligation entraînera l'extinction des présentes si bon semble au BENEFICIAIRE.", 0),
                    new ListPoints("Par le présent contrat de promesse, les PARTIES conviennent que la formation du contrat de vente est exclusivement subordonnée au consentement du BENEFICIAIRE, indépendamment du comportement du PROMETTANT.", 0),
                    new ListPoints("Toute révocation ou rétractation unilatérale de la volonté du PROMETTANT sera de plein droit dépourvue de tout effet sur le contrat promis du fait de l'acceptation de la présente promesse en tant que telle par le BENEFICIAIRE. En outre, le PROMETTANT ne pourra pas se prévaloir des dispositions de l’article 1590 du Code civil en offrant de restituer le double de la somme le cas échéant versée au titre de l’indemnité d’immobilisation.", 0),
                    new ListPoints("En tant que de besoin, le PROMETTANT se soumet à l'exécution en nature prévue par les articles 1142 et 1184 alinéa 2 du Code civil.", 0)
                };
                DS.CreateBulletList(mainPart, execPromesseItemsList, ParagraphType.General);

                body.Append(DS.SParagraph());

                // Clause CESSION DE LA PROMESSE OU SUBSTITUTION D’ACQUEREUR
                body.Append(DS.CreateHeadingParagraph("CESSION DE LA PROMESSE OU SUBSTITUTION D’ACQUEREUR", 2, ParagraphType.General, UnderlineStyle));

                if (PromesseDeVenteData.FaculteSubstitution)
                {
                    string TexteSubstitution1 = "La réalisation de la promesse pourra avoir lieu, soit au profit du BÉNÉFICIAIRE soit au profit de ses héritiers, cessionnaires, ayants droit ou commands par lui désignés.";
                    string TexteSubstitution2 = "Il est bien entendu que dans le cas de réalisation au profit d'une personne autre que le BÉNÉFICIAIRE, celui-ci restera tenu de toutes les obligations contractées, solidairement avec l'acquéreur qu'il se sera substitué.";

                    body.Append(DS.CreateParagraph(TexteSubstitution1, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteSubstitution2, JustificationValues.Both, ParagraphType.General));
                }
                else
                {
                    string TexteCession = "La réalisation de la promesse ne pourra avoir lieu qu'au profit du BÉNÉFICIAIRE qui s’interdit formellement le droit de céder à qui que ce soit le bénéfice de la présente promesse.";
                    
                    body.Append(DS.CreateParagraph(TexteCession, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause PROPRIETE - JOUISSANCE
                body.Append(DS.CreateHeadingParagraph("PROPRIETE - JOUISSANCE", 2, ParagraphType.General, UnderlineStyle));

                string TexteJouissance1 = "Le transfert de propriété aura lieu le jour de la signature de l'acte authentique constatant la réalisation de la vente, comme il est dit ci-dessus.";
                body.Append(DS.CreateParagraph(TexteJouissance1, JustificationValues.Both, ParagraphType.General));

                if (PromesseDeVenteData.EntreeJouissanceImmediate)
                {
                    string TexteJouissance2 = "L'entrée en jouissance s'effectuera le même jour par la prise de possession réelle, le PROMETTANT s'obligeant, pour cette date à rendre l'immeuble libre de toute location ou occupation.";
                    body.Append(DS.CreateParagraph(TexteJouissance2, JustificationValues.Both, ParagraphType.General));
                }
                else
                {
                    string TexteJouissance2 = $"L'entrée en jouissance aura lieu au plus tard le {PromesseDeVenteData.DateButoireJouissance}, le PROMETTANT s'obligeant à rendre le bien libre de toute occupation à compter de cette date, à peine d'une astreinte de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.Astreinte)} ({PromesseDeVenteData.Astreinte}) francs CFA par jour de retard, cette astreinte ne faisant pas obstacle au droit pour l'acquéreur d'exiger la libération à la date convenue et d'obtenir tous dommages et intérêts.";
                    string TexteJouissance3 = $"En vue de garantir le paiement éventuel de l'astreinte convenue, la somme de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.DepositaireSequestre)} ({PromesseDeVenteData.DepositaireSequestre}) francs CFA sera prélevée sur le prix et remise à M. {PromesseDeVenteData.ReceveurDepot}, dépositaire-séquestre. ";

                    body.Append(DS.CreateParagraph(TexteJouissance2, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteJouissance3, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause PRIX
                body.Append(DS.CreateHeadingParagraph("PRIX", 2, ParagraphType.General, UnderlineStyle));

                string TextePrix1 = string.Empty;
                string TextePrix2 = "Ce prix sera payable comptant le jour de la signature de l'acte authentique constatant la réalisation de la vente.";

                if (immeuble.IsBati)
                {
                    TextePrix1 = $"La vente, si elle se réalise, aura lieu moyennant le prix principal de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.PrixPrincipal)} ({PromesseDeVenteData.PrixPrincipal}) francs CFA. Lequel prix s’applique :";

                    List<ListPoints> prixItemsList =
                    [
                        new ListPoints($"au terrain nu, à concurrence de la somme de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.PrixTerrainNu)} ({PromesseDeVenteData.PrixTerrainNu}) francs CFA ;", 0),
                        new ListPoints($"aux constructions, à concurrence de la somme de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.PrixConstructions)} ({PromesseDeVenteData.PrixConstructions}) de francs CFA.", 0)
                    ];

                    body.Append(DS.CreateParagraph(TextePrix1, JustificationValues.Both, ParagraphType.General));
                    DS.CreateBulletList(mainPart, prixItemsList, ParagraphType.General);
                }
                else
                {
                    TextePrix1 = $"La vente, si elle se réalise, aura lieu moyennant le prix principal de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.PrixPrincipal)} ({PromesseDeVenteData.PrixPrincipal}) francs CFA. Lequel prix s’applique :";

                    body.Append(DS.CreateParagraph(TextePrix1, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.CreateParagraph(TextePrix2, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause TAXE SUR LA PLUS-VALUE IMMOBILIERE
                body.Append(DS.CreateHeadingParagraph("TAXE SUR LA PLUS-VALUE IMMOBILIERE", 2, ParagraphType.General, UnderlineStyle));

                string TextePlusValue1 = "Le notaire soussigné a informé les parties des dispositions des articles 108 et suivants du Code Général des Impôts, relatives à l’imposition des plus-values immobilières.";
                string TextePlusValue2 = string.Empty;
                string TextePlusValue3 = string.Empty;

                body.Append(DS.CreateParagraph(TextePlusValue1, JustificationValues.Both, ParagraphType.General));

                switch (PromesseDeVenteData.ExemptionRegime)
                {
                    case ExemptionRegime.NoExemption:
                        TextePlusValue2 = "Le PROMETTANT déclare ne pas pouvoir bénéficier des exemptions édictées par l’article 110 du Code Général des Impôts, comme n’étant ni un des établissements publics visés par ledit article, ni une entreprise soumise à l’impôt sur les bénéfices d’affaires ou à l’impôt sur les sociétés.";
                        TextePlusValue3 = "En conséquence, la présente vente donne lieu à taxation de la plus-value immobilière. Cet impôt sera acquitté concomitamment à l’enregistrement des présentes, conformément aux dispositions de l’article 116 du Code précité. A cet effet, le PROMETTANT s’engage à payer le montant de l’impôt sur la plus-value immobilière.";
                        break;
                    case ExemptionRegime.EntrepriseIBA:
                    case ExemptionRegime.EntrepriseIS:
                        TextePlusValue2 = "Le PROMETTANT déclare être une entre soumise à l’impôt sur les bénéfices d’affaires ou à l’impôt sur les sociétés, et que l’immeuble objet des présentes est inscrit à l’actif de son bilan.";
                        TextePlusValue3 = "En conséquence, la présente vente est exemptée de taxation sur la plus-value immobilière.";
                        break;
                    case ExemptionRegime.EtablissementPublicAdministratif:
                    case ExemptionRegime.SocieteParticipationPubliqueHabitat:
                    case ExemptionRegime.CollectivitePublique:
                    case ExemptionRegime.SocieteNationale:
                    case ExemptionRegime.OrganismePublicAcquisitionsExonere:
                    case ExemptionRegime.Etat:
                    case ExemptionRegime.OrganismePriveAcquisitionsExonere:
                        TextePlusValue2 = "Le PROMETTANT déclare être exempté de la taxe sur la plus-value immobilière comme étant l’une des entités exemptées de ladite taxe aux termes de l’article 110 1) du Code Général des Impôts.";
                        TextePlusValue3 = "En conséquence, la présente vente est exemptée de taxation sur la plus-value immobilière.";
                        break;
                    default:
                        break;
                }

                body.Append(DS.CreateParagraph(TextePlusValue2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePlusValue3, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause CONDITIONS DE LA VENTE EVENTUELLE
                body.Append(DS.CreateHeadingParagraph("CONDITIONS DE LA VENTE EVENTUELLE", 2, ParagraphType.General, UnderlineStyle));

                string TexteCondition1 = "La vente, si elle se réalise, aura lieu notamment aux conditions suivantes que l'acquéreur sera tenu d'exécuter :";

                body.Append(DS.CreateParagraph(TexteCondition1, JustificationValues.Both, ParagraphType.General));

                List<ListPoints> conditionItemsList1 =
                [
                    new ListPoints("l'acquéreur prendra l'immeuble dans l'état où il se trouvera le jour de l'entrée en jouissance, sans recours contre le vendeur pour quelque cause que ce soit, notamment son bon ou mauvais état, vices cachés ou différences de contenance excédant même un vingtième (1/20ème) ; ", 0),
                    new ListPoints("il profitera des servitudes actives et supportera celles passives, apparentes ou occultes, continues ou discontinues, grevant l'immeuble, le tout à ses risques et périls, sans recours contre le vendeur.", 0)
                ];

                DS.CreateBulletList(mainPart, conditionItemsList1, ParagraphType.General, ["●"]);

                if (PromesseDeVenteData.ServitudeConnue)
                {
                    string TexteCondition2 = "À cet égard, le PROMETTANT déclare que ledit immeuble n'est à sa connaissance grevé d'aucune autre servitude que :";
                    string TexteCondition3 = $"\ta) la servitude de {PromesseDeVenteData.ServitudeDe} ; ";
                    string TexteCondition4 = "\tb) celles pouvant résulter de la situation naturelle des lieux, de la loi ou de l'urbanisme.";

                    body.Append(DS.CreateParagraph(TexteCondition2, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteCondition3, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteCondition4, JustificationValues.Both, ParagraphType.General));
                }
                else
                {
                    string TexteCondition2 = "À cet égard, le PROMETTANT déclare que ledit immeuble n'est à sa connaissance grevé d'aucune autre servitude que celles pouvant résulter de la situation naturelle des lieux, de la loi, de l'urbanisme et de tous règlements le régissant.";

                    body.Append(DS.CreateParagraph(TexteCondition2, JustificationValues.Both, ParagraphType.General));
                }

                List<ListPoints> conditionItemsList2 =
                [
                    new ListPoints("il paiera, à compter de l'entrée en jouissance, une fraction des impôts foncier et de tous impôts auxquels est assujetti ledit immeuble ; cette fraction sera déterminée au prorata de la jouissance respective du vendeur et de l'acquéreur. Les parties se régleront entre elles les prorata ainsi déterminés ;", 0),
                    new ListPoints("il continuera ou résiliera, selon qu'il avisera, à compter de la date d'entrée en jouissance, tous contrats d'abonnement pour la distribution de l'eau, du gaz et de l'électricité.", 0),
                    new ListPoints("il fera son affaire personnelle de la continuation ou de la résiliation de l'assurance contre l'incendie et autres risques souscrite par le vendeur ; ", 0),
                    new ListPoints("il acquittera tous les frais, droits et émoluments de l'acte qui constatera la réalisation de la vente.", 0)
                ];

                DS.CreateBulletList(mainPart, conditionItemsList2, ParagraphType.General, ["●"]);

                body.Append(DS.SParagraph());

                // Clause CONDITIONS SUSPENSIVES
                body.Append(DS.CreateHeadingParagraph("CONDITIONS SUSPENSIVES", 5, ParagraphType.General, UnderlineStyle));
                body.Append(DS.CreateParagraph("1°) Conditions suspensives générales", JustificationValues.Both, ParagraphType.General, UnderlineStyle));

                string TexteConditionSus;
                if (PromesseDeVenteData.NecessiteNoteRenseignementUrbanisme)
                {
                    TexteConditionSus = "que la note de renseignement d'urbanisme ne révèle pas de servitudes ou autres prescriptions administratives de nature à mettre en cause, à plus ou moins long terme, même partiellement, le droit de propriété ou de jouissance de l'acquéreur et à diminuer sensiblement la valeur du bien ;";
                }
                else if (PromesseDeVenteData.NecessiteCertificatUrbanisme)
                {
                    TexteConditionSus = "que le certificat d'urbanisme ne révèle pas de servitudes ou autres prescriptions administratives de nature à mettre en cause, à plus ou moins long terme, même partiellement, le droit de propriété ou de jouissance de l'acquéreur et à diminuer sensiblement la valeur du bien ;";
                }
                else
                {
                    TexteConditionSus = string.Empty;
                }

                List<ListPoints> conditionItemsList3 =
                [
                    new ListPoints(TexteConditionSus, 0),
                    new ListPoints("que l’attestation de situation géographique délivrée par l’Institut National Géographique (IGN) ne révèle pas que le BIEN est situé dans une zone déclarée d’utilité publique ;", 0),
                    new ListPoints("que les titres présomptifs de propriété ne révèlent aucune charge réelle ou servitude susceptible de nuire au droit de propriété ou de jouissance de l'acquéreur ; ", 0),
                    new ListPoints("que l'origine de propriété ne révèle aucune cause susceptible d'entraîner l'éviction de l'acquéreur ;", 0),
                    new ListPoints("que les états délivrés en vue de la réalisation des présentes conventions ne révèlent pas d'obstacle à la vente ou d'inscription de privilège ou d'hypothèque garantissant des créances dont le solde, en capital et intérêts et accessoires, ne pourra être remboursé à l'aide du prix de vente, sauf si les créanciers inscrits donnent leur consentement à une procédure de purge amiable.", 0)
                ];

                DS.CreateBulletList(mainPart, conditionItemsList3, ParagraphType.General, ["●"]);

                string TexteConditionSus2 = "Ces conditions suspensives sont stipulées dans l'intérêt exclusif du BENEFICIAIRE. En conséquence, en cas de défaillance de ces conditions ou de l'une d'entre elles, au jour fixé pour la signature de l'acte authentique de vente, il aura seul qualité pour s'en prévaloir et, s'il le désire, se trouver délié de tout engagement. Dans cette hypothèse, la présente convention sera considérée comme caduque, sans indemnité de part ni d'autre, et la somme versée lui sera restituée.";
                string TexteConditionSus3 = "Le BENEFICIAIRE pourra néanmoins demander la réalisation des présentes conventions en faisant son affaire personnelle des conséquences de la défaillance de la condition.";

                body.Append(DS.CreateParagraph(TexteConditionSus2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteConditionSus3, JustificationValues.Both, ParagraphType.General));

                if (PromesseDeVenteData.ObtentionPretNecessaire)
                {
                    body.Append(DS.CreateParagraph("2°) Condition suspensive particulière – obtention d’un prêt", JustificationValues.Both, ParagraphType.General, UnderlineStyle));

                    string TextePret1 = $"Le BENEFICIAIRE déclare qu'il paiera le prix de la vente avec l'aide d'un ou plusieurs prêts d'un montant total de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.MontantTotalPret)} ({PromesseDeVenteData.MontantTotalPret}) francs CFA, d'une durée d'au moins {PromesseDeVenteData.DureePret} ans et productif d'intérêts au taux maximum de {PromesseDeVenteData.TauxPret} pour cent l'an.";
                    string TextePret2 = $"Par suite, la présente convention est soumise à la condition suspensive d'obtention de ces prêts à ces conditions, dans un délai de deux (02) mois, soit au plus tard le {PromesseDeVenteData.DateButoirePret} et selon les modalités ci-après définies, faute de quoi la condition suspensive sera considérée comme non réalisée.";
                    string TextePret3 = "Le ou les prêts seront considérés comme obtenus par la réception par le BENEFICIAIRE des offres de prêts établies conformément aux dispositions légales en vigueur et répondant aux conditions ci-dessus.";
                    string TextePret4 = "Le BENEFICIAIRE devra notifier, au notaire désigné pour la rédaction de l'acte authentique, dans les huit (08) jours de leur remise ou de leur réception, les offres à lui faites ou le refus opposé aux demandes de prêt.";
                    string TextePret5 = "Le BENEFICIAIRE s'oblige à déposer ses demandes de prêt dans un délai de dix (10) jours à compter des présentes et à en justifier aussitôt audit notaire en lui en adressant le double.";
                    string TextePret6 = "Si le BENEFICIAIRE ne respecte pas ces obligations, le PROMETTANT pourra se libérer du présent engagement en lui restituant la somme versée ci-dessous.";
                    string TextePret7 = "Si le BENEFICIAIRE veut renoncer à la condition suspensive ci-dessus stipulée, il devra notifier au notaire, dans les formes et délais sus-indiqués, qu'il dispose désormais des sommes nécessaires pour payer le prix sans l'aide d'un prêt.";

                    body.Append(DS.CreateParagraph(TextePret1, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret2, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret3, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret4, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret5, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret6, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TextePret7, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause CERTIFICAT D’APPARTENANCE
                if (!immeuble.HasTitreFoncier)
                {
                    body.Append(DS.CreateHeadingParagraph("CERTIFICAT D’APPARTENANCE", 5, ParagraphType.General, UnderlineStyle));
                    
                    if (PromesseDeVenteData.PromettantFaitDemarcheCertificat)
                    {
                        string TexteAppart1 = "Conformément aux dispositions de l’article 17 du Code Foncier et Domanial, toute vente immobilière doit être précédée de la délivrance d’un titre foncier sur le bien vendu ou, par exception, par la délivrance d’un certificat d’appartenance.";
                        string TexteAppart2 = "L’immeuble ci-dessus désigné ne disposant pas de titre foncier, les présentes sont soumises à la condition suspensive de l’obtention, par le PROMETTANT, d’un certificat d’appartenance auprès de l’Agence Nationale du Domaine et du Foncier (ANDF), dans un délai de trois (03) mois à compter de la signature des présentes.";
                        string TexteAppart3 = "Si, passé ce délai le notaire soussigné n'a pas reçu, de la part du PROMETTANT, le certificat d’appartenance relatif à l'immeuble ci-dessus désigné, la présente promesse sera caduque de plein droit sans que le BENEFICIAIRE ait à notifier de mise en demeure, ou de remplir une quelconque formalité judiciaire. Il lui sera alors restituer, sans délai, par le PROMETTANT toute(s) somme(s) qu’il aurait pu avoir versé au PROMETTANT dans le cadre de cette opération immobilière.";
                        string TexteAppart4 = $"Le PROMETTANT s’engage à déposer la demande délivrance du certificat d’appartenance dans un délai de quinze (15) jours à compter de la signature des présentes, soit au plus tard le {PromesseDeVenteData.DateButoireCertificat}.";

                        body.Append(DS.CreateParagraph(TexteAppart1, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart2, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart4, JustificationValues.Both, ParagraphType.General));
                    }
                    else
                    {
                        string TexteAppart1 = "Le PROMETTANT donne par les présentes tous pouvoirs au notaire soussigné à l’effet de procéder aux formalités d’obtention du certificat d’appartenance. Pour assurer la bonne exécution du présent mandat, le PROMETTANT renonce, jusqu’à l’obtention dudit certificat par le notaire soussigné, au pouvoir que lui reconnaît l’article 2004 du Code Civil de révoquer librement le mandat.";
                        string TexteAppart2 = "Le PROMETTANT déclare avoir obtenu les pièces nécessaires à l’obtention dudit certificat à savoir :";
                        string TexteAppart3 = "Une copie de l’ensemble de ces documents demeure ci-annexée.";
                        string TexteAppart4 = "Les frais relatifs à l’établissement du certificat d’appartenance sont à la charge exclusive du PROMETTANT qui s’y oblige. ";
                        string TexteAppart5 = "Le BENEFICIAIRE déclare avoir pris connaissance de l’ensemble de ces documents dès avant la signature des présentes.  ";

                        List<ListPoints> listPoints = ListPoints.GetListFromStrings(PromesseDeVenteData.PiecesCertificat, 0);

                        body.Append(DS.CreateParagraph(TexteAppart1, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart2, JustificationValues.Both, ParagraphType.General));
                        DS.CreateBulletList(mainPart, listPoints, ParagraphType.General);
                        body.Append(DS.CreateParagraph(TexteAppart3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart4, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteAppart5, JustificationValues.Both, ParagraphType.General));
                        
                    }

                    body.Append(DS.SParagraph());
                }

                // Clause DROIT DE PREEMPTION
                body.Append(DS.CreateHeadingParagraph("DROIT DE PREEMPTION", 5, ParagraphType.General, UnderlineStyle));

                string TextePreemption1 = "La vente ne pourra se réaliser que si les droits de préemption, dont l'immeuble vendu peut faire l'objet ne sont pas exercés par leurs titulaires respectifs. En cas d'exercice du droit de préemption, la présente convention sera caduque sans indemnité de part ni d'autre ; la somme versée par le BENEFICIAIRE lui sera restituée.";
                string TextePreemption2 = "Il est convenu entre les parties que la saisine d'une juridiction en fixation du prix et éventuellement en vue de modifier les conditions de la vente entraînera la caducité des présentes conventions, au même titre que l'exercice pur et simple du droit de préemption.";
                string TextePreemption3 = "Tous pouvoirs sont donnés au notaire soussigné à l'effet de procéder aux formalités nécessaires à la purge de ce(s) droit(s) de préemption.";

                body.Append(DS.CreateParagraph(TextePreemption1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePreemption2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePreemption3, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause INDEMNITE D'IMMOBILISATION
                body.Append(DS.CreateHeadingParagraph("INDEMNITE D'IMMOBILISATION", 5, ParagraphType.General, UnderlineStyle));

                if (PromesseDeVenteData.VersementIndemnite)
                {
                    string TexteIndemniteSomme = $"par chèque n°{PromesseDeVenteData.NumeroCheque} tiré sur la banque {PromesseDeVenteData.Banque} au compte de l'office, la somme de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.Somme)} ({PromesseDeVenteData.Somme}) francs CFA.";
                    string TexteIndemnite1 = "À titre d'indemnité d'immobilisation de l'immeuble objet de la présente promesse de vente, le BENEFICIAIRE verse au PROMETTANT " + TexteIndemniteSomme;
                    string TexteIndemnite2 = "Cette somme qui ne sera pas productive d'intérêts est versée à titre d'acompte sur le prix de la vente. Lors de la passation de l'acte authentique, elle sera imputée sur le montant du prix stipulé payable comptant.";
                    string TexteIndemnite3 = "En aucun cas cette somme ne pourra être considérée comme un versement d'arrhes permettant aux parties de se dédire, ou comme une clause pénale. Elle constitue une indemnisation forfaitaire destinée à compenser le préjudice causé au vendeur par la non-réalisation de la vente.";
                    string TexteIndemnite4 = "Si l'une des conditions suspensives ci-dessus stipulées ne se réalise pas selon les modalités sus-indiquées, elle sera restituée au BENEFICIAIRE.";
                    string TexteIndemnite5 = "Si toutes les conditions suspensives se réalisent, mais que le BENEFICIAIRE ne lève pas l'option en respectant les modalités de validité et de délais ci-après stipulées, elle sera acquise de plein droit au PROMETTANT et remise à celui-ci par le notaire soussigné.";
                    string TexteIndemnite6 = "L'indemnité sera totalement acquise au PROMETTANT quelle que soit la date de la renonciation du BENEFICIAIRE, son montant n'étant pas fixé en considération de la durée de l'immobilisation.";

                    if (PromesseDeVenteData.VersementSequestre)
                    {
                        if (PromesseDeVenteData.NotaireSequestre)
                        {
                            TexteIndemnite1 = "À titre d'indemnité d'immobilisation de l'immeuble objet de la présente promesse de vente, le BENEFICIAIRE verse au notaire soussigné, " + TexteIndemniteSomme;
                        }
                        else
                        {
                            TexteIndemnite1 = "À titre d'indemnité d'immobilisation de l'immeuble objet de la présente promesse de vente, le BENEFICIAIRE verse au séquestre désigné comme il est dit ci-après, " + TexteIndemniteSomme;
                        }
                        TexteIndemnite5 = "Si toutes les conditions suspensives se réalisent, mais que le BENEFICIAIRE ne lève pas l'option en respectant les modalités de validité et de délais ci-après stipulées, elle sera acquise de plein droit au PROMETTANT et remise à celui-ci par le séquestre.";

                        body.Append(DS.CreateParagraph(TexteIndemnite1, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite2, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite4, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite5, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite6, JustificationValues.Both, ParagraphType.General));

                        body.Append(DS.SParagraph());

                        // Clause SEQUESTRE
                        body.Append(DS.CreateHeadingParagraph("SEQUESTRE", 5, ParagraphType.General, UnderlineStyle));

                        string TexteSequestre1 = $"D'un commun accord, les parties choisissent comme séquestre M. {(PromesseDeVenteData.NotaireSequestre ? PromesseDeVenteData.NomSequestre : Properties.Settings.Default.NotaryLastName)} {(PromesseDeVenteData.PresenceSequestre ? "ici présent et qui accepte" : "")}. Le séquestre, mandataire commun des parties, détiendra cette somme pour le compte de qui il appartiendra, étant stipulé qu'elle sera indisponible tant que sa nature d'acompte ou d'indemnité d'immobilisation ne sera pas déterminée en vertu des stipulations du contrat.";
                        string TexteSequestre2 = "La mission du séquestre sera de remettre la somme, versée comme il est dit ci-dessus, au PROMETTANT ou au BENEFICIAIRE selon ce qui est convenu aux termes du présent acte.";
                        string TexteSequestre3 = "Le séquestre opérera le versement prévu avec l'accord des deux parties ou en vertu d'une décision judiciaire devenue exécutoire.";
                        string TexteSequestre4 = "À défaut d'accord, la partie la plus diligente pourra se pourvoir en justice afin qu'il soit statué sur le sort de la somme détenue par le séquestre. Le séquestre est dès à présent autorisé par les PARTIES à consigner l'indemnité d'immobilisation à la caisse des dépôts et consignations en cas de difficultés.";

                        body.Append(DS.CreateParagraph(TexteSequestre1, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteSequestre2, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteSequestre3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteSequestre4, JustificationValues.Both, ParagraphType.General));
                    }
                    else
                    {
                        body.Append(DS.CreateParagraph(TexteIndemnite1, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite2, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite3, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite4, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite5, JustificationValues.Both, ParagraphType.General));
                        body.Append(DS.CreateParagraph(TexteIndemnite6, JustificationValues.Both, ParagraphType.General));
                    }
                }
                else
                {
                    body.Append(DS.CreateParagraph("La présente promesse de vente est conclue sans indemnité d’immobilisation ni dépôt de garantie.", JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause CLAUSE PENALE
                body.Append(DS.CreateHeadingParagraph("CLAUSE PENALE", 5, ParagraphType.General, UnderlineStyle));

                string[] TexteClausePenale =
                    [
                    $"Au cas où l'une quelconque des parties, après avoir été mise en demeure, ne régulariserait pas l'acte authentique et ne satisferait pas ainsi aux obligations alors exigibles, elle devra verser à l'autre partie la somme de {NumberToFrenchWordsConverter.ConvertToFrenchWords(PromesseDeVenteData.SommeClausePenale)} ({PromesseDeVenteData.SommeClausePenale}) francs CFA à titre de clause pénale.",
                    "Il est précisé que la présente clause ne peut être assimilée à une stipulation d'arrhes et n'emporte pas novation. Ainsi chacune des parties aura la possibilité de poursuivre l'autre en exécution de la vente."
                    ];

                DS.AddParagraphTexts(body, TexteClausePenale, ParagraphType.General);

                body.Append(DS.SParagraph());

                if (immeuble.IsBati)
                {
                    // Clause DISPOSITIONS RELATIVES À LA CONSTRUCTION  
                    body.Append(DS.CreateHeadingParagraph("DISPOSITIONS RELATIVES À LA CONSTRUCTION", 5, ParagraphType.General, UnderlineStyle));
                    body.Append(DS.CreateParagraph("a) Permis de construire", JustificationValues.Both, ParagraphType.General, UnderlineStyle));

                    string TexteConstruction1 = $"Le PROMETTANT déclare qu’il a édifié un bâtiment de type R+{PromesseDeVenteData.BatimentType} sur le BIEN objet des présentes. Il déclare que cette construction a fait l’objet :";
                    string TexteConstruction2 = $"- d’un permis de construire délivré par {PromesseDeVenteData.DelivreurPermis} le {PromesseDeVenteData.DateDelivrancePermis} ;";
                    string TexteConstruction3 = $"- d’une déclaration d’ouverture de chantier en date à {PromesseDeVenteData.LieuOuvertureChantier} du {PromesseDeVenteData.DateOuvertureChantier} ;";
                    string TexteConstruction4 = $"- d’une déclaration d’achèvement mentionnant la conformité des travaux avec le permis de construire en date à {PromesseDeVenteData.LieuDeclarationAchevement} du {PromesseDeVenteData.DateDeclarationAchevement} ;";
                    string TexteConstruction5 = $"- d’un certificat de conformité et d’habitabilité délivré par {PromesseDeVenteData.DelivreurCertificatConformite} le {PromesseDeVenteData.DateDelivranceCertificatHabilite} ;";
                    string TexteConstruction6 = "Demeurent ci-annexées les copies de ces documents, dont les originaux ont été remis par le PROMETTANT à le BENEFICIAIRE qui le reconnaît.";
                    string TexteConstruction7 = "Le PROMETTANT déclare ne pas avoir fait, dans le BIEN, d’autres travaux nécessitant l’obtention au préalable d’un permis de construire ou d’un permis de démolir conformément à la législation en vigueur.";
                    string TexteConstruction8 = string.Empty;
                    string TexteConstruction9 = string.Empty;
                    string TexteConstruction10 = string.Empty;
                    string TexteConstruction11 = string.Empty;
                    string TexteConstruction12 = string.Empty;
                    string[] TexteConstruction = [];

                    switch (PromesseDeVenteData.ConstruitParPromettant, PromesseDeVenteData.PermisConstruireObtenu)
                    {
                        case (true, true) :
                            TexteConstruction =
                                [
                                TexteConstruction1, TexteConstruction2, TexteConstruction3, TexteConstruction4, TexteConstruction5, TexteConstruction6, TexteConstruction7
                                ];
                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            break;
                        case (false, true) :
                            TexteConstruction1 = $"Le PROMETTANT déclare qu’un bâtiment de type R+{PromesseDeVenteData.BatimentType} a été construit sur le BIEN objet des présentes par l’un des précédents propriétaires. Il déclare que cette construction a fait l’objet :";
                            TexteConstruction =
                                [
                                TexteConstruction1, TexteConstruction2, TexteConstruction3, TexteConstruction4, TexteConstruction5, TexteConstruction6, TexteConstruction7
                                ];
                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            break;
                        case (true, false) :
                            TexteConstruction1 = $"Le PROMETTANT déclare qu’il a édifié un bâtiment de type R+{PromesseDeVenteData.BatimentType} sur le BIEN objet des présentes. Il déclare que cette construction a fait l’objet :";
                            body.Append(DS.CreateParagraph(TexteConstruction1, JustificationValues.Both, ParagraphType.General));

                            TexteConstruction2 = "Le notaire soussigné rappelle aux parties que, sauf les cas de dispense prévues par l’article 9 du décret numéro 2020-056 du 5 février 2020 portant réglementation du permis de construire et du permis de démolir en République du Bénin, tel que modifié par le décret numéro 2022-418 du 20 juillet 2022, toute construction, ou modification d’immeuble existant, doit faire l’objet d’un permis de construire préalable. La réalisation d’une construction non autorisée par un permis de construire constitue une violation du décret numéro 2020-056 du 5 février 2020 qui peut entraîner : une amende allant de trois cent mille (300.000) à trois millions (3.000.000) de francs CFA ; ainsi que la démolition des ouvrages ordonner par la juridiction compétente en vertu des dispositions encore en vigueur du décret numéro 2014-205 du 13 mars 2014.";
                            body.Append(DS.CreateParagraph(TexteConstruction2, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "; a", colorUntil: "14."));

                            TexteConstruction3 = "Constitue aussi notamment des infractions aux dispositions du décret numéro 2020-056 du 5 février 2020 :";
                            TexteConstruction4 = "- la non-conformité des constructions ou travaux au permis de construire délivré. Laquelle infraction est sanctionnée, sans préjudice de sanctions pénales, d’une obligation de mise en conformité ou de la démolition des ouvrages, ainsi que d’une amende allant de cinq cent mille (500.000) à deux millions (2.000.000) de francs CFA ;";
                            TexteConstruction5 = "- l’occupation ou l’exploitation d’un ouvrage ou d’une construction sans l’obtention préalable du certificat de conformité et d’habitabilité. Laquelle infraction est sanctionnée par une amende de cinq cent mille (500.000) francs CFA par jour de retard d’obtention dudit certificat ;";
                            TexteConstruction6 = "- le non-respect de l’obligation de souscription d’une assurance obligatoire sur les risques de la construction avant l’ouverture du chantier. Laquelle infraction est sanctionnée par une amende allant de cinq cent mille (500.000) à cinq millions (5.000.000) de francs CFA.";
                            TexteConstruction7 = "De même, le notaire soussigné informe les parties que le décret précité fait obligation, jusqu’au 5 février 2025, à toute personne ayant entamé l’édification d’une construction sans permis de construire, achevée ou non à la date du 5 février 2015, de demander un certificat de régularisation. Ce certificat a pour effet de suspendre l’application des sanctions relatives aux infractions visées par le décret susvisé pendant une durée de deux (02) ans à compter de la date d’entrée en vigueur dudit décret, soit jusqu’au 5 février 2022, afin de permettre au demandeur d’accomplir les formalités nécessaires à l’obtention d’un permis de construire. L’obtention du certificat de régularisation pour un immeuble achevé vaut certificat de conformité et d’habitabilité.";
                            TexteConstruction =
                                [
                                TexteConstruction3, TexteConstruction4, TexteConstruction5, TexteConstruction6, TexteConstruction7
                                ];

                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            body.Append(DS.SParagraph());

                            TexteConstruction8 = "Le PROMETTANT déclare ne pas avoir été mis en demeure par les services compétents de procéder à une régularisation de la situation des constructions édifiées. Il déclare en outre ne pas avoir fait, dans le BIEN, d’autres travaux nécessitant l’obtention au préalable d’un permis de construire ou d’un permis de démolir conformément à la législation en vigueur.\r\n\r\n";
                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            body.Append(DS.CreateParagraph(TexteConstruction8, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "; a", colorUntil: "14."));

                            TexteConstruction9 = "Le notaire soussigné informe le BENEFICIAIRE de la nécessité d’obtenir un permis de construire et/ou un certificat de régularisation, afin de régulariser la ou les constructions édifiées sans permis de construire, sous peine de se voir infliger l’une des sanctions prévues par le décret susvisé.";
                            TexteConstruction10 = "LE BENEFICIAIRE déclare avoir reçu du notaire soussigné toutes les explications utiles à sa bonne compréhension des obligations et des sanctions édictées par la règlementation en vigueur. Il déclare vouloir faire son affaire personnelle de l’obtention ou non d’un permis de construire et d’un certificat de régularisation des constructions édifiées sur le BIEN objet des présentes, et décharge le notaire soussigné de toute responsabilité à cet égard.";
                            TexteConstruction11 = "En conséquence, les parties, parfaitement informés de tous les inconvénients de cette situation et des conséquences pouvant en résulter, déclarent vouloir passer outre aux recommandations et avertissements donnés par le notaire susnommé, et le requièrent de recevoir la promesse de vente dont s'agit.";
                            body.Append(DS.CreateParagraph(TexteConstruction9, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.CreateParagraph(TexteConstruction10, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "par", colorUntil: "vigueur"));
                            body.Append(DS.CreateParagraph(TexteConstruction8, JustificationValues.Both, ParagraphType.General, textStyle: GreenBoldStyle));
                            break;
                        case (false, false) :
                            TexteConstruction1 = $"Le PROMETTANT déclare qu’il a édifié un bâtiment de type R+{PromesseDeVenteData.BatimentType} sur le BIEN objet des présentes. Il déclare que cette construction a fait l’objet :";
                            body.Append(DS.CreateParagraph(TexteConstruction1, JustificationValues.Both, ParagraphType.General));

                            if (PromesseDeVenteData.PermisConstruireExist)
                            {
                                TexteConstruction2 = "Il déclare qu’à sa connaissance cette construction a fait l’objet d’un permis de construire, mais qu’il ne dispose pas de la copie du permis de construire ni des copies des déclarations d’ouverture et d’achèvement de chantier, de la déclaration d’achèvement du chantier ou du certificat de conformité et d’habitabilité.";
                            }
                            else
                            {
                                TexteConstruction2 = "Il déclare qu’à sa connaissance cette construction n’a pas fait l’objet d’un permis de construire.";
                            }
                            body.Append(DS.CreateParagraph(TexteConstruction2, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.SParagraph());

                            TexteConstruction3 = "Le notaire soussigné rappelle aux parties que, sauf les cas de dispense prévues par l’article 9 du décret numéro 2020-056 du 5 février 2020 portant réglementation du permis de construire et du permis de démolir en République du Bénin, tel que modifié par le décret numéro 2022-418 du 20 juillet 2022, toute construction, ou modification d’immeuble existant, doit faire l’objet d’un permis de construire préalable. La réalisation d’une construction non autorisée par un permis de construire constitue une violation du décret numéro 2020-056 du 5 février 2020 qui peut entraîner : une amende allant de trois cent mille (300.000) à trois millions (3.000.000) de francs CFA ; ainsi que la démolition des ouvrages ordonner par la juridiction compétente en vertu des dispositions encore en vigueur du décret numéro 2014-205 du 13 mars 2014.";
                            body.Append(DS.CreateParagraph(TexteConstruction3, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "; a", colorUntil: "14."));

                            TexteConstruction4 = "Constitue aussi notamment des infractions aux dispositions du décret numéro 2020-056 du 5 février 2020 :";
                            TexteConstruction5 = "- la non-conformité des constructions ou travaux au permis de construire délivré. Laquelle infraction est sanctionnée, sans préjudice de sanctions pénales, d’une obligation de mise en conformité ou de la démolition des ouvrages, ainsi que d’une amende allant de cinq cent mille (500.000) à deux millions (2.000.000) de francs CFA ;";
                            TexteConstruction6 = "- l’occupation ou l’exploitation d’un ouvrage ou d’une construction sans l’obtention préalable du certificat de conformité et d’habitabilité. Laquelle infraction est sanctionnée par une amende de cinq cent mille (500.000) francs CFA par jour de retard d’obtention dudit certificat ;";
                            TexteConstruction7 = "- le non-respect de l’obligation de souscription d’une assurance obligatoire sur les risques de la construction avant l’ouverture du chantier. Laquelle infraction est sanctionnée par une amende allant de cinq cent mille (500.000) à cinq millions (5.000.000) de francs CFA.";
                            TexteConstruction8 = "De même, le notaire soussigné informe les parties que le décret précité fait obligation, jusqu’au 5 février 2025, à toute personne ayant entamé l’édification d’une construction sans permis de construire, achevée ou non à la date du 5 février 2015, de demander un certificat de régularisation. Ce certificat a pour effet de suspendre l’application des sanctions relatives aux infractions visées par le décret susvisé pendant une durée de deux (02) ans à compter de la date d’entrée en vigueur dudit décret, soit jusqu’au 5 février 2022, afin de permettre au demandeur d’accomplir les formalités nécessaires à l’obtention d’un permis de construire. L’obtention du certificat de régularisation pour un immeuble achevé vaut certificat de conformité et d’habitabilité.";
                            TexteConstruction =
                                [
                                TexteConstruction4, TexteConstruction5, TexteConstruction6, TexteConstruction7, TexteConstruction8
                                ];

                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            body.Append(DS.SParagraph());

                            TexteConstruction9 = "Le PROMETTANT déclare ne pas avoir été mis en demeure par les services compétents de procéder à une régularisation de la situation des constructions édifiées. Il déclare en outre ne pas avoir fait, dans le BIEN, d’autres travaux nécessitant l’obtention au préalable d’un permis de construire ou d’un permis de démolir conformément à la législation en vigueur.\r\n\r\n";
                            DS.AddParagraphTexts(body, TexteConstruction, ParagraphType.General);
                            body.Append(DS.CreateParagraph(TexteConstruction9, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "; a", colorUntil: "14."));

                            TexteConstruction10 = "Le notaire soussigné informe le BENEFICIAIRE de la nécessité d’obtenir un permis de construire et/ou un certificat de régularisation, afin de régulariser la ou les constructions édifiées sans permis de construire, sous peine de se voir infliger l’une des sanctions prévues par le décret susvisé.";
                            TexteConstruction11 = "LE BENEFICIAIRE déclare avoir reçu du notaire soussigné toutes les explications utiles à sa bonne compréhension des obligations et des sanctions édictées par la règlementation en vigueur. Il déclare vouloir faire son affaire personnelle de l’obtention ou non d’un permis de construire et d’un certificat de régularisation des constructions édifiées sur le BIEN objet des présentes, et décharge le notaire soussigné de toute responsabilité à cet égard.";
                            TexteConstruction12 = "En conséquence, les parties, parfaitement informés de tous les inconvénients de cette situation et des conséquences pouvant en résulter, déclarent vouloir passer outre aux recommandations et avertissements donnés par le notaire susnommé, et le requièrent de recevoir la promesse de vente dont s'agit.";
                            body.Append(DS.CreateParagraph(TexteConstruction10, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.CreateParagraph(TexteConstruction11, JustificationValues.Both, ParagraphType.General, color: "green", colorFrom: "par", colorUntil: "vigueur"));
                            body.Append(DS.CreateParagraph(TexteConstruction12, JustificationValues.Both, ParagraphType.General, textStyle: GreenBoldStyle));
                            break;
                    }

                    body.Append(DS.CreateParagraph("b) Assurances des risques de la construction", JustificationValues.Both, ParagraphType.General, UnderlineStyle));

                    string TexteAssuranceConstruction1 = string.Empty;
                    string TexteAssuranceConstruction2 = string.Empty;
                    string TexteAssuranceConstruction3 = string.Empty;

                    string[] TexteAssuranceConstruction = [];
                    List<ListPoints> listAttestations = ListPoints.GetDumbListFromStrings(PromesseDeVenteData.Attestations, 0);

                    switch (PromesseDeVenteData.PromettantSouscritAssurance, PromesseDeVenteData.AssurancesEntreprisesObtenu, PromesseDeVenteData.ExemptionAssuranceConstruction)
                    {
                        case (true, true, false):
                            TexteAssuranceConstruction1 = $"Le PROMETTANT atteste également qu’il a souscrit à une assurance tous risques chantier auprès de {PromesseDeVenteData.AssuranceTousRisques}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceTousRisques}, en date du {PromesseDeVenteData.DateAssuranceTousRisques}, ainsi qu’une assurance dommage à l’ouvrage auprès de {PromesseDeVenteData.AssuranceDommage}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceDommage}, en date du {PromesseDeVenteData.DateAssuranceDommage}, et une assurance responsabilité civile auprès de {PromesseDeVenteData.AssuranceResponsabiliteCivile}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceResponsabiliteCivile}, en date du {PromesseDeVenteData.DateAssuranceResponsabiliteCivile}, avant l’ouverture du chantier, conformément aux dispositions du décret n°2016-054 du 10 mars 2016 portant obligation d’assurances des risques de la construction en République du Bénin.";
                            TexteAssuranceConstruction2 = "Il déclare par ailleurs avoir obtenu la copie des attestations d’assurances responsabilité civile, responsabilité civile décennale et/ou dommages à l’ouvrage souscrites par les constructeurs de l’ouvrage, à savoir :";
                            TexteAssuranceConstruction =
                                [
                                TexteAssuranceConstruction1, TexteAssuranceConstruction2
                                ];
                            DS.AddParagraphTexts(body, TexteAssuranceConstruction, ParagraphType.General);

                            DS.CreateBulletList(mainPart, listAttestations, ParagraphType.General);
                            TexteAssuranceConstruction3 = "Demeurent ci-annexées les copies des contrats d’assurance et les quittances de paiement des primes d’assurance payés par le PROMETTANT et les copies des attestations d’assurances des constructeurs, dont les originaux ont été remis par le PROMETTANT à le BENEFICIAIRE qui le reconnaît.";
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction3, JustificationValues.Both, ParagraphType.General));
                            break;
                        case (true, false, false):
                            TexteAssuranceConstruction1 = $"Le PROMETTANT atteste également qu’il a souscrit à une assurance tous risques chantier auprès de {PromesseDeVenteData.AssuranceTousRisques}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceTousRisques}, en date du {PromesseDeVenteData.DateAssuranceTousRisques}, ainsi qu’une assurance dommage à l’ouvrage auprès de {PromesseDeVenteData.AssuranceDommage}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceDommage}, en date du {PromesseDeVenteData.DateAssuranceDommage}, et une assurance responsabilité civile auprès de {PromesseDeVenteData.AssuranceResponsabiliteCivile}, numéro de police {PromesseDeVenteData.NumeroPoliceAssuranceResponsabiliteCivile}, en date du {PromesseDeVenteData.DateAssuranceResponsabiliteCivile}, avant l’ouverture du chantier, conformément aux dispositions du décret n°2016-054 du 10 mars 2016 portant obligation d’assurances des risques de la construction en République du Bénin.";
                            TexteAssuranceConstruction2 = "Il déclare toutefois ne pas avoir obtenue la copie des attestations d’assurances responsabilité civile, responsabilité civile décennale et/ou dommages à l’ouvrages souscrites par les constructeurs de l’ouvrage, à savoir :";
                            TexteAssuranceConstruction =
                                [
                                TexteAssuranceConstruction1, TexteAssuranceConstruction2
                                ];
                            DS.AddParagraphTexts(body, TexteAssuranceConstruction, ParagraphType.General);

                            DS.CreateBulletList(mainPart, listAttestations, ParagraphType.General);
                            TexteAssuranceConstruction3 = "Demeurent ci-annexées les copies des contrats d’assurance et les quittances de paiement des primes d’assurance payés par le PROMETTANT, dont les originaux ont été remis par le PROMETTANT à le BENEFICIAIRE qui le reconnaît.";
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction3, JustificationValues.Both, ParagraphType.General));
                            break;
                        case (false, false, true):
                            TexteAssuranceConstruction1 = "Le PROMETTANT déclare que le coût de réalisation des ouvrages bâtis est inférieur à cent millions (100.000.000) de francs CFA, et qu’il ne s’agit pas de bâtiments ou ouvrages socio communautaires. En conséquence il n’est pas soumis à l’obligation de souscrire une assurance garantissant les dommages subis par l’ouvrage dans la phase de construction ou causés à autrui par l’activité du chantier, conformément aux dispositions de l’article 6 du décret n°2016-054 du 10 mars 2016 portant obligation d’assurances des risques de la construction en République du Bénin.";
                            TexteAssuranceConstruction2 = "Le PROMETTANT déclare en conséquence ne pas avoir souscrit à l’une des assurances mentionnées par ledit décret.";
                            TexteAssuranceConstruction =
                                [
                                TexteAssuranceConstruction1, TexteAssuranceConstruction2
                                ];
                            DS.AddParagraphTexts(body, TexteAssuranceConstruction, ParagraphType.General);
                            break;
                        case (false, true, true):
                            TexteAssuranceConstruction1 = "Le PROMETTANT déclare que le coût de réalisation des ouvrages bâtis est inférieur à cent millions (100.000.000) de francs CFA, et qu’il ne s’agit pas de bâtiments ou ouvrages socio communautaires. En conséquence il n’est pas soumis à l’obligation de souscrire une assurance garantissant les dommages subis par l’ouvrage dans la phase de construction ou causés à autrui par l’activité du chantier, conformément aux dispositions de l’article 6 du décret n°2016-054 du 10 mars 2016 portant obligation d’assurances des risques de la construction en République du Bénin.";
                            TexteAssuranceConstruction2 = "Le PROMETTANT déclare en conséquence ne pas avoir souscrit à l’une des assurances mentionnées par ledit décret.";
                            TexteAssuranceConstruction =
                                [
                                TexteAssuranceConstruction1, TexteAssuranceConstruction2
                                ];
                            DS.AddParagraphTexts(body, TexteAssuranceConstruction, ParagraphType.General);

                            DS.CreateBulletList(mainPart, listAttestations, ParagraphType.General);
                            TexteAssuranceConstruction3 = "Demeurent ci-annexées les copies des contrats d’assurance et les quittances de paiement des primes d’assurance payés par le PROMETTANT, dont les originaux ont été remis par le PROMETTANT à le BENEFICIAIRE qui le reconnaît.";
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction3, JustificationValues.Both, ParagraphType.General));
                            break;
                        case (false, false, false):
                            TexteAssuranceConstruction1 = "Le PROMETTANT déclare que le coût de la réalisation des ouvrages bâtis est supérieur à cent millions (100.000.000) de francs CFA, mais qu’il n’a pas souscrit, avant l’ouverture du chantier, les assurances obligatoires prescrites par le décret n°2016-054 du 10 mars 2016.";
                            TexteAssuranceConstruction2 = "Il déclare également ne pas avoir obtenue la copie des attestations d’assurances responsabilité civile, responsabilité civile décennale et/ou dommages à l’ouvrages souscrites par les constructeurs de l’ouvrage, à savoir :";
                            TexteAssuranceConstruction =
                                [
                                TexteAssuranceConstruction1, TexteAssuranceConstruction2
                                ];
                            DS.AddParagraphTexts(body, TexteAssuranceConstruction, ParagraphType.General);

                            DS.CreateBulletList(mainPart, listAttestations, ParagraphType.General);
                            TexteAssuranceConstruction3 = "Le notaire soussigné rappelle aux parties que le non respect des dispositions du décret susvisé peut entraîner l’application des sanctions édictées aux articles 20 et 21 dudit décret dont notamment : l’infliction d’une amende égale à cent pour cent (100%) du montant total des primes que le contrevenant aurait du payer si les assurances avaient été normalement souscrites ; laquelle amende est réhausser à cent cinquante pour cent (150%) du montant desdites primes si aucune souscription n’est possible à la date où il est constaté la méconnaissance des dispositions du décret. A cette sanction, ce rajoute celle édictée par le décret numéro 2020-056 du 5 février 2020 portant réglementation du permis de construire et du permis de démolir en République du Bénin, tel que modifié par le décret numéro 2022-418 du 20 juillet 2022, qui dispose que le non-respect de l’obligation de souscription d’une assurance obligatoire sur les risques de la construction avant l’ouverture du chantier, constitue une infraction sanctionnée par une amende allant de cinq cent mille (500.000) à cinq millions (5.000.000) de francs CFA.";
                            string TexteAssuranceConstruction4 = $"M. {promettant.Nom}, PROMETTANT, reconnaît avoir été informé par le notaire soussigné de ce qu'en l'absence d'assurance dommages-ouvrages, il devient débiteur des garanties imposées aux constructeurs. LE BENEFICIAIRE pourra donc en cas de dommages, se retourner contre lui sans qu'une clause exonératoire puisse être insérée dans l'acte.";
                            string TexteAssuranceConstruction5 = "Le PROMETTANT reconnaît que le notaire soussigné, lui a fait la lecture des sanctions encourues, prévues par le décret n°2016-054 du 10 mars 2016 portant obligation d’assurances des risques de la construction en République du Bénin, et par le décret numéro 2020-056 du 5 février 2020 portant réglementation du permis de construire et du permis de démolir en République du Bénin, tel que modifié par le décret numéro 2022-418 du 20 juillet 2022, et lui a rappelé l'impossibilité dans laquelle il se trouvait de se soustraire aux responsabilités ci-dessus mentionnées, celles-ci étant d'ordre public.";
                            string TexteAssuranceConstruction6 = string.Empty;
                            if (PromesseDeVenteData.AssuranceResponsabiliteDecenaleObtenu)
                            {
                                TexteAssuranceConstruction6 = $"M. {beneficiaire.Nom}, ACQUEREUR, reconnaît avoir été averti par le notaire susnommé des inconvénients résultant du fait que seule l'assurance responsabilité décennale a été souscrite par le PROMETTANT, à l'exclusion de l'assurance dommages-ouvrages et des risques encourus liés à l'acquisition d'un immeuble insuffisamment assuré. Son attention a été en particulier attirée sur le fait qu'en cas de dommages à l'immeuble il n'aura d'autre solution que d'agir contre son PROMETTANT, qui peut être introuvable ou insolvable, ou contre les constructeurs ou leurs assureurs, mais il devra alors faire les frais du/des procès.";
                            }
                            else
                            {
                                TexteAssuranceConstruction6 = $"M. {beneficiaire.Nom}, ACQUEREUR, reconnaît avoir été averti par le notaire susnommé des inconvénients résultant du fait qu'il n'existe pas d'assurance responsabilité civile décennale ni d'assurance dommages à l’ouvrage et des risques encourus liés à l'acquisition d'un immeuble non assuré. Son attention a été en particulier attirée sur le fait qu'en cas de dommages à l'immeuble il n'aura d'autre solution que d'agir contre son PROMETTANT, qui peut être introuvable ou insolvable, ou contre les constructeurs ou leurs assureurs, mais il devra alors faire les frais du/des procès.";
                            }
                            string TexteAssuranceConstruction7 = $"M. {beneficiaire.Nom}, ACQUEREUR, déclare confirmer expressément son intention d'acquérir néanmoins l'immeuble dont s'agit.";
                            string TexteAssuranceConstruction8 = "En conséquence, les parties, parfaitement informés de tous les inconvénients de cette situation et des conséquences pouvant en résulter, déclarent vouloir passer outre aux recommandations et avertissements donnés par le notaire susnommé, et le requièrent de recevoir la promesse de vente dont s'agit.";

                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction3, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.SParagraph());
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction4, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction5, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.SParagraph());
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction6, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction7, JustificationValues.Both, ParagraphType.General));
                            body.Append(DS.SParagraph());
                            body.Append(DS.CreateParagraph(TexteAssuranceConstruction8, JustificationValues.Both, ParagraphType.General, textStyle: BoldStyle));
                            break;
                    }

                    body.Append(DS.SParagraph());
                    // Clause ASSAINISSEMENT
                    body.Append(DS.CreateHeadingParagraph("ASSAINISSEMENT", 5, ParagraphType.General, UnderlineStyle));

                    string TexteAssainissement1 = "Le PROMETTANT déclare que l’immeuble n’est pas raccordé à un réseau d’assainissement collectif des eaux usées à usage domestique, mais dispose d’une fosse septique. Il déclare qu’à sa connaissance ladite fosse septique ne souffre d’aucun défaut ou vice.";
                    string TexteAssainissement2 = "Le BENEFICIAIRE fera son affaire personnelle de la vidange de ladite fosse septique à compter de la date de signature de l’acte de vente.";
                    string[] TexteAssainissement =
                        [
                        TexteAssainissement1, TexteAssainissement2
                        ];
                    DS.AddParagraphTexts(body, TexteAssainissement, ParagraphType.General);

                    body.Append(DS.SParagraph());
                }

                // Clause OBLIGATIONS GENERALES DU PROMETTANT
                body.Append(DS.CreateHeadingParagraph("OBLIGATIONS GENERALES DU PROMETTANT", 5, ParagraphType.General, UnderlineStyle));

                string TexteObligationGeneral1 = "Le PROMETTANT oblige solidairement et indivisiblement entre eux, ses héritiers et ayants cause, fussent-ils mineurs ou majeurs protégés.";
                string TexteObligationGeneral2 = "Il s'interdit, à compter d'aujourd'hui, tout acte susceptible de porter atteinte au droit de propriété et aux conditions de jouissance promises au BENEFICIAIRE.";
                string TexteObligationGeneral3 = "Il s'oblige à fournir au notaire chargé de dresser l'acte de vente tous les documents qui lui seront demandés concernant son état civil, sa capacité et l'immeuble promis, notamment les titres présomptifs de propriété, les polices d'assurances contre l'incendie et autres dommages, ainsi que les copies exécutoires ou les originaux des titres locatifs ou d'occupation.";
                string TexteObligationGeneral4 = "Il s'engage à rapporter les mainlevées et certificats de radiation de toutes les inscriptions qui seraient révélées par l'état à requérir sur la publication de la vente au fichier immobilier.";
                string TexteObligationGeneral5 = "Il s'engage à entretenir l'immeuble et à ne pas en modifier l'usage et la consistance.";
                string TexteObligationGeneral6 = "Le PROMETTANT déclare par ailleurs qu'à sa connaissance, l'immeuble n'a pas fait l'objet d'une exploitation classée susceptible d'entraîner un risque de pollution.";

                string[] TexteObligationGeneral =
                    [
                    TexteObligationGeneral1, TexteObligationGeneral2, TexteObligationGeneral3, TexteObligationGeneral4, TexteObligationGeneral5, TexteObligationGeneral6
                    ];
                DS.AddParagraphTexts(body, TexteObligationGeneral, ParagraphType.General);

                body.Append(DS.SParagraph());

                // Clause OBLIGATION DE GARDE DU PROMETTANT
                body.Append(DS.CreateHeadingParagraph("OBLIGATION DE GARDE DU PROMETTANT", 5, ParagraphType.General, UnderlineStyle));

                string TexteObligationGarde1 = string.Empty;
                string TexteObligationGarde2 = string.Empty;
                string TexteObligationGarde3 = string.Empty;
                string TexteObligationGarde4 = string.Empty;
                string TexteObligationGarde5 = string.Empty;
                string TexteObligationGarde6 = string.Empty;
                string TexteObligationGarde7 = string.Empty;
                string TexteObligationGarde8 = string.Empty;
                string[] TexteObligationGarde = [];

                if (immeuble.IsBati)
                {
                    TexteObligationGarde1 = "Entre la date des présentes et la date d’entrée en jouissance du BENEFICIAIRE, le BIEN, et le cas échéant les MEUBLES, tels qu’ils sont sus-désignés demeureront sous la garde et possession du PROMETTANT qui s’y oblige.";
                    TexteObligationGarde2 = "En conséquence, il est convenu ce qui suit :";
                    TexteObligationGarde3 = "Eléments d’équipement";
                    TexteObligationGarde4 = "Le PROMETTANT s’engage à laisser dans le BIEN tout ce qui est immeuble par destination ainsi que, sans que cette liste soit limitative et sous la seule réserve que les éléments ci-après désignés existent :";
                    TexteObligationGarde5 = "Le BENEFICIAIRE pourra visiter les lieux juste avant la prise de jouissance du BIEN, et s’assurer du respect de l’engagement qui précède.";
                    TexteObligationGarde6 = "Entretien, réparation";
                    TexteObligationGarde7 = "Jusqu'à l’entrée en jouissance du BENEFICIAIRE, le PROMETTANT s’engage à :";
                    TexteObligationGarde8 = "Les PARTIES se rapprocheront directement entre elles afin d'effectuer une visite préalablement à la signature de l'acte authentique de vente dans le but de vérifier l'état général par rapport à ce qu'il est à ce jour et de procéder au relevé des compteurs. ";

                    string[] listeEquipment =
                        [
                        "les supports de tringles à rideau, s’ils sont scellés dans le mur ;",
                        "les moquettes ;",
                        "les poignées de porte telles qu’elles existaient lors de la visite ;",
                        "les pommeaux ou boules d'escalier ;",
                        "les portes, planches et équipements de rangement des placards ;",
                        "les arbres, arbustes, rosiers, plantes et fleurs en terre si jardin privatif ;",
                        "l’équipement sanitaire et de conditionnement d’air ;",
                        "les éléments d’éclairage fixés au mur et/ou plafonds, à l'exception des appliques et luminaires ;",
                        "l’équipement électrique ;",
                        "le câblage et les prises informatiques ;",
                        "tous les carreaux et vitrages sans cassures ni fêlures ;",
                        "les volets, persiennes, stores-bannes et leurs motorisations."
                        ];
                    List<ListPoints> listeEquipmentItems = ListPoints.GetListFromStrings(listeEquipment, 0);
                    string[] listeEntretien =
                        [
                        "ne pas apporter de modification quelconque ;",
                        "délivrer le BIEN dans son état actuel ;",
                        "conserver ses assurances ;",
                        "maintenir en bon état de fonctionnement les équipements du BIEN : chauffe-eau, électricité, climatisation, VMC, sanitaire ;",
                        "laisser les fils électriques d’éclairage suffisamment longs et équipés de leurs douilles et ampoules ou spots ou néons ;",
                        "entretenir le BIEN et ses abords ;",
                        "réparer les dégâts survenus depuis la visite."
                        ];
                    List<ListPoints> listeEntretienItems = ListPoints.GetListFromStrings(listeEntretien, 0);

                    body.Append(DS.CreateParagraph(TexteObligationGarde1, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteObligationGarde2, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.SParagraph());
                    body.Append(DS.CreateParagraph(TexteObligationGarde3, JustificationValues.Both, ParagraphType.General, textStyle: UnderlineStyle));
                    body.Append(DS.CreateParagraph(TexteObligationGarde4, JustificationValues.Both, ParagraphType.General));
                    DS.CreateBulletList(mainPart, listeEquipmentItems, ParagraphType.General);
                    body.Append(DS.SParagraph());
                    body.Append(DS.CreateParagraph(TexteObligationGarde5, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteObligationGarde6, JustificationValues.Both, ParagraphType.General, textStyle: UnderlineStyle));
                    body.Append(DS.CreateParagraph(TexteObligationGarde7, JustificationValues.Both, ParagraphType.General));
                    DS.CreateBulletList(mainPart, listeEntretienItems, ParagraphType.General);
                    body.Append(DS.CreateParagraph(TexteObligationGarde8, JustificationValues.Both, ParagraphType.General));
                }
                else
                {
                    TexteObligationGarde1 = "Entre la date des présentes et la date d’entrée en jouissance du BENEFICIAIRE, le BIEN demeure sous la garde et possession du PROMETTANT qui s’y oblige.";
                    TexteObligationGarde2 = "En conséquence, il est convenu ce qui suit :";
                    TexteObligationGarde3 = "Eléments d’équipement";
                    TexteObligationGarde4 = "Le PROMETTANT s’engage à laisser dans le BIEN tout ce qui est immeuble par destination ainsi que, sans que cette liste soit limitative et sous la seule réserve que les éléments ci-après désignés existent :";
                    TexteObligationGarde5 = "Le BENEFICIAIRE pourra visiter les lieux juste avant la prise de jouissance du BIEN, et s’assurer du respect de l’engagement qui précède.";
                    TexteObligationGarde6 = "Entretien, réparation";
                    TexteObligationGarde7 = "Jusqu'à l’entrée en jouissance du BENEFICIAIRE, le PROMETTANT s’engage à :";
                    TexteObligationGarde8 = "Les PARTIES se rapprocheront directement entre elles afin d'effectuer une visite préalablement à la signature de l'acte authentique de vente dans le but de vérifier l'état général par rapport à ce qu'il est à ce jour. ";

                    string TexteEquipment = "- les arbres, arbustes, rosiers, plantes et fleurs en terre si jardin privatif ;";
                    
                    string[] listeEntretien =
                        [
                        "ne pas apporter de modification quelconque ;",
                        "délivrer le BIEN dans son état actuel ;",
                        "entretenir le BIEN et ses abords ;",
                        "réparer les dégâts survenus depuis la visite."
                        ];
                    List<ListPoints> listeEntretienItems = ListPoints.GetListFromStrings(listeEntretien, 0);

                    body.Append(DS.CreateParagraph(TexteObligationGarde1, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteObligationGarde2, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.SParagraph());
                    body.Append(DS.CreateParagraph(TexteObligationGarde3, JustificationValues.Both, ParagraphType.General, textStyle: UnderlineStyle));
                    body.Append(DS.CreateParagraph(TexteObligationGarde4, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteEquipment, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.SParagraph());
                    body.Append(DS.CreateParagraph(TexteObligationGarde5, JustificationValues.Both, ParagraphType.General));
                    body.Append(DS.CreateParagraph(TexteObligationGarde6, JustificationValues.Both, ParagraphType.General, textStyle: UnderlineStyle));
                    body.Append(DS.CreateParagraph(TexteObligationGarde7, JustificationValues.Both, ParagraphType.General));
                    DS.CreateBulletList(mainPart, listeEntretienItems, ParagraphType.General);
                    body.Append(DS.CreateParagraph(TexteObligationGarde8, JustificationValues.Both, ParagraphType.General));
                }

                body.Append(DS.SParagraph());

                // Clause SINISTRE PENDANT LA DURÉE DE VALIDITÉ DE LA PROMESSE
                body.Append(DS.CreateHeadingParagraph("SINISTRE PENDANT LA DURÉE DE VALIDITÉ DE LA PROMESSE", 5, ParagraphType.General, UnderlineStyle));

                string TexteSinistre1 = "Si un sinistre quelconque frappait le BIEN durant la durée de validité des présentes, les PARTIES conviennent que le BENEFICIAIRE aura la faculté :";
                string TexteSinistre2 = "- Soit de renoncer purement et simplement à la vente et de se voir immédiatement remboursé de toute somme avancée par lui le cas échéant.";
                string TexteSinistre3 = "- Soit de maintenir l’acquisition du BIEN alors sinistré totalement ou partiellement et de se voir attribuer les indemnités susceptibles d’être versées par la ou les compagnies d’assurances concernées, sans limitation de ces indemnités fussent-elles supérieures au prix convenu aux présentes. Le PROMETTANT entend que dans cette hypothèse le BENEFICIAIRE soit purement subrogé dans tous ses droits à l’égard desdites compagnies d’assurances.";
                string TexteSinistre4 = "Il est précisé que l’existence des présentes ne pourrait alors être remise en cause que par un sinistre de nature à rendre le BIEN inhabitable ou impropre à son exploitation.";
                string TexteSinistre5 = "Le PROMETTANT indique que le BIEN est assuré, qu’il est à jour du paiement des primes et qu’il n’existe aucun contentieux en cours entre lui et la compagnie assurant le BIEN.";
                string[] TexteSinistre =
                    [
                    TexteSinistre1, TexteSinistre2, TexteSinistre3, TexteSinistre4, TexteSinistre5
                    ];

                DS.AddParagraphTexts(body, TexteSinistre, ParagraphType.General);

                body.Append(DS.SParagraph());

                // Clause CONDITION DE SURVIE DU BÉNÉFICIAIRE
                body.Append(DS.CreateHeadingParagraph("CONDITION DE SURVIE DU BÉNÉFICIAIRE", 5, ParagraphType.General, UnderlineStyle));

                string TexteSurvieBeneficiaire1 = "Au cas de décès du BENEFICIAIRE s’il s’agit d’une personne physique, ou de dissolution judiciaire dudit BENEFICIAIRE s’il s’agit d’une personne morale, avant la constatation authentique de la réalisation des présentes, les présentes seront caduques.";
                string TexteSurvieBeneficiaire2 = "Pour ce qui concerne l’indemnité d’immobilisation, elle ne sera pas due et celle versée devra être restituée, et ce même si le décès ou la dissolution judiciaire survient après la réalisation des conditions suspensives.";

                body.Append(DS.CreateParagraph(TexteSurvieBeneficiaire1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteSurvieBeneficiaire2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.SParagraph());

                // Clause FRAIS
                body.Append(DS.CreateHeadingParagraph("FRAIS", 5, ParagraphType.General, UnderlineStyle));

                string TexteFrais1 = "Les frais, droits et émoluments du présent acte sont supportés par le BENEFICIAIRE, qui s'oblige à leur paiement.";
                body.Append(DS.CreateParagraph(TexteFrais1, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause DOMICILE
                body.Append(DS.CreateHeadingParagraph("DOMICILE", 5, ParagraphType.General, UnderlineStyle));

                string TexteDomicile1 = "Pour l'exécution du présent acte, les parties font élection de domicile en leurs demeures respectives.";
                string TexteDomicile2 = "Toutefois, les notifications relatives à la condition suspensive d'obtention d'un prêt et à la levée d'option seront valablement faites au domicile du PROMETTANT.";

                body.Append(DS.CreateParagraph(TexteDomicile1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteDomicile2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.SParagraph());

                // Clause POUVOIRS
                body.Append(DS.CreateHeadingParagraph("POUVOIRS", 5, ParagraphType.General, UnderlineStyle));

                string TextePouvoir1 = "Les PARTIES confèrent à tout clerc ou collaborateur de l’office notarial dénommé en tête des présentes, ainsi qu'à ceux le cas échéant du notaire en participation ou en concours, avec faculté d'agir ensemble ou séparément, tous pouvoirs nécessaires à l’effet :";
                string TextePouvoir2 = "- de signer toutes demandes de pièces, demandes de renseignements, et lettres de purge de droit de préemption préalables à la vente,";
                string TextePouvoir3 = "- de dresser et signer tous actes qui se révéleraient nécessaires en vue de l’accomplissement des formalités de publicité foncière des présentes dans l’éventualité où l’une des parties demanderait la publication du présent acte au service de la publicité foncière, d’effectuer toutes précisions pour mettre les présentes en conformité avec la réglementation sur la publicité foncière.";

                body.Append(DS.CreateParagraph(TextePouvoir1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePouvoir2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TextePouvoir3, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.SParagraph());

                // Clause AFFIRMATIONS DE SINCERITE
                body.Append(DS.CreateHeadingParagraph("AFFIRMATIONS DE SINCERITE", 5, ParagraphType.General, UnderlineStyle));

                string TexteSincerite1 = "Avant de clore, le notaire soussigné a informé les parties des dispositions des articles 486 et 504 du Code Général des Impôts ; lesquelles prévoient, en cas d’inexactitude ou d’omission dans un acte, une sanction fiscale consistant dans une pénalité d’assiette allant de vingt pour cent (20%) à quatre-vingt pour cent (80%) des sommes imposables non déclarées ; et, en cas de dissimulation volontaire de tout ou partie de sommes sujettes à l’impôt, une sanction pénale consistant dans une amende de cent mille (100.000) à deux millions (2.000.000) de francs CFA, et un emprisonnement de un (1) à cinq (5) ans.";
                string TexteSincerite2 = "Chacune des parties, interpellées séparément, a affirmé, sous lesdites peines, que le présent acte exprime l’intégralité du prix convenu. ";
                string TexteSincerite3 = $"Maître {Properties.Settings.Default.NotaryLastName}, notaire soussigné, affirme qu'à sa connaissance, le présent acte n'est modifié ni contredit par aucune contre-lettre contenant augmentation du prix.";

                body.Append(DS.CreateParagraph(TexteSincerite1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteSincerite2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteSincerite3, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.SParagraph());

                // Clause MENTION
                body.Append(DS.CreateHeadingParagraph("MENTION", 1, ParagraphType.General, UnderlineStyle));

                string TexteMention1 = "Mention des présentes est consentie partout où besoin sera.";

                body.Append(DS.CreateParagraph(TexteMention1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.SParagraph());

                // Clause MENTION SUR LA PROTECTION DES DONNÉES PERSONNELLES
                body.Append(DS.CreateHeadingParagraph("MENTION SUR LA PROTECTION DES DONNÉES PERSONNELLES", 1, ParagraphType.General, UnderlineStyle));

                string TexteMentionProtectionData1 = "L’Office notarial traite des données personnelles concernant les personnes mentionnées aux présentes, pour l’accomplissement des activités notariales, notamment de formalités d’actes.";
                string TexteMentionProtectionData2 = "Ce traitement est fondé sur le respect d’une obligation légale et l’exécution d’une mission relevant de l’exercice de l’autorité publique déléguée par l’Etat dont sont investis les notaires, officiers publics, conformément à la loi numéro 2002-015 du 30 décembre 2002 portant Statut du Notariat en République du Bénin.";
                string TexteMentionProtectionData3 = "Ces données seront susceptibles d’être transférées aux destinataires suivants :";
                string TexteMentionProtectionData4 = "-\tles administrations ou partenaires légalement habilités tels que la Direction Générale des Impôts, ou, le cas échéant, l’Agence Nationale du Domaine et du Foncier (ANDF), les instances notariales, les organismes du notariat, ";
                string TexteMentionProtectionData5 = "-\tles offices notariaux participant ou concourant à l’acte,";
                string TexteMentionProtectionData6 = "-\tles établissements financiers concernés,";
                string TexteMentionProtectionData7 = "-\tles organismes de conseils spécialisés pour la gestion des activités notariales,";
                string TexteMentionProtectionData8 = "L’acte authentique et ses annexes sont conservés cent (100) ans. Les vérifications liées aux personnalités politiquement exposées, au blanchiment des capitaux et au financement du terrorisme sont conservées dix (10) ans après la fin de la relation d’affaires.";
                string TexteMentionProtectionData9 = "Conformément à la réglementation en vigueur relative à la protection des données personnelles, les intéressés peuvent demander l’accès aux données les concernant. Le cas échéant, ils peuvent demander la rectification ou l’effacement de celles-ci, obtenir la limitation du traitement de ces données ou vous y opposer pour des raisons tenant à votre situation particulière.";
                string TexteMentionProtectionData10 = "Si ces personnes estiment, après avoir contacté l’Office notarial, que leurs droits ne sont pas respectés, elles peuvent introduire une réclamation auprès d’une autorité de contrôle, l’Autorité de Protection des Données à caractère Personnel (APDP).";
                string[] TexteMentionProtectionData =
                    [
                    TexteMentionProtectionData4, TexteMentionProtectionData5, TexteMentionProtectionData6, TexteMentionProtectionData7
                    ];
                List<ListPoints> MentionProtectionItems = ListPoints.GetListFromStrings(TexteMentionProtectionData, 0);

                body.Append(DS.CreateParagraph(TexteMentionProtectionData1, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteMentionProtectionData2, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteMentionProtectionData3, JustificationValues.Both, ParagraphType.General));
                DS.CreateBulletList(mainPart, items, ParagraphType.General);
                body.Append(DS.CreateParagraph(TexteMentionProtectionData8, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteMentionProtectionData9, JustificationValues.Both, ParagraphType.General));
                body.Append(DS.CreateParagraph(TexteMentionProtectionData10, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause FORMALISME LIE AUX ANNEXES
                body.Append(DS.CreateHeadingParagraph("FORMALISME LIE AUX ANNEXES", 1, ParagraphType.General, UnderlineStyle));

                string TexteFormalisme1 = "Les annexes, s'il en existe, font partie intégrante de l'acte.";
                body.Append(DS.CreateParagraph(TexteFormalisme1, JustificationValues.Both, ParagraphType.General));

                body.Append(DS.SParagraph());

                // Clause DONT ACTE
                body.Append(DS.CreateHeadingParagraph("DONT ACTE", 5, ParagraphType.General, UnderlineStyle, JustificationValues.End));

                string TexteDontActe1 = "Sur ------- pages.";
                string TexteDontActe2 = $"Fait et passé à {Properties.Settings.Default.NotaryCountry} {Properties.Settings.Default.NotaryOfficeAddress} ;";
                string TexteDontActe3 = "En l'Office Notarial ;";
                string TexteDontActe4 = $"L'AN {NumberToFrenchWordsConverter.ConvertToFrenchWords((double)DateTime.Now.Year)}";
                string TexteDontActe5 = "";
                string TexteDontActe6 = "";
                string TexteDontActe7 = "";
                string TexteDontActe8 = "";
                string TexteDontActe9 = "";
                string TexteDontActe10 = "";
                string TexteDontActe11 = "";


                body.Append(DS.SParagraph());
            }


        }

        public static void CreateDesignationEstimationTable(
            Body body,
            ObservableCollection<DesignationEstimation> designationsEstimations)
        {
            // Define shading for grey background
            Shading headerShading = new Shading()
            {
                Color = "auto",          // Text color (auto uses the default)
                Fill = "D9D9D9",         // Background color in hex (light grey)
                Val = ShadingPatternValues.Clear
            };

            // Définir les largeurs de colonnes en dxa
            int[] columnWidths = { 5353, 2835 };

            // Définir la police Tahoma 9
            RunProperties defaultRunProperties = new RunProperties(
                new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" },
                new FontSize { Val = "18" } // Taille 9 points * 2
            );

            // Créer l'en-tête
            List<TableCellData> headerCells = new List<TableCellData>
            {
                new TableCellData("Désignation")
                {
                    RunProperties = new RunProperties(
                        new Bold(),
                        new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" },
                        new FontSize { Val = "18" } // Taille 9 points * 2
                    ),
                    CellProperties = new TableCellProperties(headerShading)
                },
                new TableCellData("Estimation")
                {
                    RunProperties = new RunProperties(
                        new Bold(),
                        new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" },
                        new FontSize { Val = "18" } // Taille 9 points * 2
                    ),
                    CellProperties = new TableCellProperties(headerShading)
                }
            };

            // Créer les lignes de la table
            List<List<TableCellData>> rows = new List<List<TableCellData>>();

            double totalEstimation = 0;

            foreach (var item in designationsEstimations)
            {
                totalEstimation += item.Estimation;

                var designationCell = new TableCellData(item.Designation ?? "")
                {
                    ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Both }
                )
                };

                var estimationCell = new TableCellData(item.Estimation.ToString("N2"))
                {
                    ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Both }
                )
                };

                rows.Add(new List<TableCellData> { designationCell, estimationCell });
            }

            // Ajouter la ligne de total
            var totalDesignationCell = new TableCellData("Total")
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Right }
                )
            };

            var totalEstimationCell = new TableCellData(totalEstimation.ToString("N2"));

            rows.Add(new List<TableCellData> { totalDesignationCell, totalEstimationCell });

            // Créer la table
            DS.CreateTable(body, columnWidths, headerCells, rows);
        }

    }
}
