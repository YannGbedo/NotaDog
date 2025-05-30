using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using NotaDog.Windows;
using DocumentFormat.OpenXml;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using NotaDog.Interfaces;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using DocumentFormat.OpenXml.Math;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Justification = DocumentFormat.OpenXml.Wordprocessing.Justification;
using JustificationValues = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;
using StyleValues = DocumentFormat.OpenXml.Wordprocessing.StyleValues;
using System.Collections.ObjectModel;

namespace NotaDog.Services
{
    public enum ParagraphType
    {
        General,
        Special
    }

    public class TextStyle
    {
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Underline { get; set; } = false;
        public string? Color { get; set; } = null; // Hexadecimal color code, e.g., "FF0000" for red
    }

    public class TableCellData(string text)
    {
        public string Text { get; set; } = text;
        public RunProperties RunProperties { get; set; } = new RunProperties(
                new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" },
                new FontSize { Val = "18" } // Taille 9 points * 2
            );
        public ParagraphProperties ParagraphProperties { get; set; } = new ParagraphProperties(
                        new Justification { Val = JustificationValues.Both }
                    );
        public TableCellProperties CellProperties { get; set; } = new TableCellProperties();
        public int RowSpan { get; set; } = 1; // Nombre de lignes à fusionner verticalement
        public int ColSpan { get; set; } = 1; // Nombre de colonnes à fusionner horizontalement
    }

    public enum BulletType
    {
        Dash,      // Tiret '-'
        Circle,    // Cercle '●'
        Square,    // Carré '■'
        Custom     // Permet d'utiliser un symbole personnalisé
    }

    public class ListPoints
    {
        public string Text { get; set; }
        public int Level { get; set; } // Niveau de la liste (0 pour le niveau principal, 1 pour les sous-listes)

        public ListPoints(string text, int level)
        {
            Text = text;
            Level = level;
        }

        public static List<ListPoints> GetListFromStrings(ObservableCollection<string> items, int level)
        {
            List<ListPoints> listPoints = [];

            foreach (string item in items)
            {
                ListPoints point = new(item, level);
                listPoints.Add(point);
            }

            return listPoints;
        }

        public static List<ListPoints> GetListFromStrings(string[] items, int level)
        {
            List<ListPoints> listPoints = [];

            foreach (string item in items)
            {
                ListPoints point = new(item, level);
                listPoints.Add(point);
            }

            return listPoints;
        }

        public static List<ListPoints> GetDumbListFromStrings(ObservableCollection<string> items, int level)
        {
            List<ListPoints> listPoints = [];

            for (int i = 0; i < items.Count; i++)
            {
                string item = string.Empty;
                if (i != items.Count - 1)
                {
                    item = items[i] + " ;";
                }
                else
                {
                    item = items[i] + ".";
                }

                ListPoints point = new(item, level);
                listPoints.Add(point);
            }

            return listPoints;
        }
    }

    public static class NumberToFrenchWordsConverter
    {
        private static readonly string[] Units = { "zéro", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "dix",
        "onze", "douze", "treize", "quatorze", "quinze", "seize",
        "dix-sept", "dix-huit", "dix-neuf" };

        private static readonly string[] Tens = { "", "dix", "vingt", "trente", "quarante", "cinquante", "soixante",
        "soixante", "quatre-vingt", "quatre-vingt" };

        public static string ConvertToFrenchWords(double number)
        {
            // Convertir le nombre en chaîne avec deux décimales
            var numberString = Math.Abs(number).ToString("F2", CultureInfo.InvariantCulture);
            var parts = numberString.Split('.');
            long integerPart = long.Parse(parts[0]);
            string decimalPart = parts.Length > 1 ? parts[1] : "";

            string result = "";

            if (number < 0)
                result += "moins ";

            // Convertir la partie entière
            result += ConvertIntegerToWords(integerPart);

            // Convertir la partie décimale si elle existe
            if (!string.IsNullOrEmpty(decimalPart))
            {
                result += " virgule " + ConvertDecimalPartToWords(decimalPart);
            }

            return result.Trim();
        }

        private static string ConvertIntegerToWords(long number)
        {
            if (number == 0)
                return "zéro";

            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000000000) + " milliard";
                if ((number / 1000000000) > 1)
                    words += "s";
                words += " ";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000000) + " million";
                if ((number / 1000000) > 1)
                    words += "s";
                words += " ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                if ((number / 1000) == 1)
                    words += "mille ";
                else
                    words += ConvertIntegerToWords(number / 1000) + " mille ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                if ((number / 100) == 1)
                    words += "cent";
                else
                    words += Units[number / 100] + " cent";

                if (number % 100 == 0 && (number / 100) > 1)
                    words += "s";

                words += " ";
                number %= 100;
            }

            if (number > 0)
            {
                if (number <= 19)
                {
                    words += Units[number];
                }
                else
                {
                    int tens = (int)(number / 10);
                    int units = (int)(number % 10);

                    if (tens == 7 || tens == 9)
                    {
                        tens--;
                        units += 10;
                    }

                    words += Tens[tens];

                    if (units == 1 && tens != 8)
                    {
                        words += " et un";
                    }
                    else if (units > 0)
                    {
                        words += "-" + Units[units];
                    }
                    else if (tens == 8)
                    {
                        words += "s";
                    }
                }
            }
            else
            {
                // Supprimer le dernier espace inutile si number == 0
                words = words.TrimEnd();
            }

            return words.Trim();
        }

        private static string ConvertDecimalPartToWords(string decimalPart)
        {
            var words = "";
            foreach (char digit in decimalPart)
            {
                int digitValue = int.Parse(digit.ToString());
                words += Units[digitValue] + " ";
            }

            return words.Trim();
        }
    }

    public static class DocumentService
    {
        /*DOC BROWSING METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // Get the default saving folder
        public static string GetDefaultDocumentsFolder()
        {
            // Vérifier si un dossier personnalisé est défini
            string customFolder = Properties.Settings.Default.DocumentsFolder;
            if (!string.IsNullOrEmpty(customFolder) && Directory.Exists(customFolder))
            {
                return customFolder;
            }
            else
            {
                // Retourner le dossier par défaut
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NotaDogDocs");
            }
        }

        // List the docs
        public static List<DocumentInfo> GetDocumentsList()
        {
            string folderPath = GetDefaultDocumentsFolder();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var documents = new List<DocumentInfo>();
            foreach (var file in Directory.GetFiles(folderPath, "*.docx", SearchOption.AllDirectories))
            {
                if (IsNotaDogDocument(file))
                {
                    var docInfo = GetDocumentInfo(file);
                    if (docInfo != null)
                    {
                        documents.Add(docInfo);
                    }
                }
            }
            return documents;
        }

        // Check if created by NotaDog
        private static bool IsNotaDogDocument(string filePath)
        {
            try
            {
                using WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false);
                var customProps = wordDoc.CustomFilePropertiesPart;
                if (customProps != null)
                {
                    var props = customProps.Properties;
                    var generatedByProp = props.Elements<CustomDocumentProperty>().FirstOrDefault(p => p.Name?.Value == "GeneratedByNotaDog");
                    if (generatedByProp != null && generatedByProp.VTLPWSTR != null && generatedByProp.VTLPWSTR.Text == "True")
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Handle exceptions if needed
            }
            return false;
        }

        // Get one doc's info
        private static DocumentInfo? GetDocumentInfo(string filePath)
        {
            try
            {
                using WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false);
                var customProps = wordDoc.CustomFilePropertiesPart;
                if (customProps != null)
                {
                    var props = customProps.Properties;
                    var generatedByProp = props.Elements<CustomDocumentProperty>().FirstOrDefault(p => p.Name?.Value == "GeneratedByNotaDog");
                    if (generatedByProp != null && generatedByProp.VTLPWSTR != null && generatedByProp.VTLPWSTR.Text == "True")
                    {
                        var docTypeProp = props.Elements<CustomDocumentProperty>().FirstOrDefault(p => p.Name?.Value == "DocumentType");
                        string documentType;
                        if (docTypeProp != null && docTypeProp.VTLPWSTR != null)
                        {
                            documentType = docTypeProp.VTLPWSTR.Text;
                        }
                        else
                        {
                            documentType = "Unknown";
                        }

                        return new DocumentInfo
                        {
                            FileName = Path.GetFileName(filePath),
                            DocumentType = documentType,
                            FilePath = filePath
                        };
                    }
                }
            }
            catch
            {
                // Handle exceptions if needed
            }
            return null;
        }

        /*DOC BUILDING METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // Document creation
        public static void CreateDocument(string filePath, string documentType, NotaryInfo notaryInfo, object documentControl)
        {
            // Create the Word document
            using WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
            
            // Add main document part
            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new Document
            {
                // Initialiser le Body du document
                Body = new Body()
            };

            // Définir les styles par défaut
            SetDefaultStyles(mainPart);

            // Build the document content
            BuildDocumentContent(mainPart, notaryInfo, documentControl);

            // Ajouter l'en-tête
            AddHeader(mainPart);

            // Add custom properties
            AddCustomProperty(wordDoc, "GeneratedByNotaDog", "True");
            AddCustomProperty(wordDoc, "DocumentType", documentType);
        }

        // Build a document given its type
        private static void BuildDocumentContent(MainDocumentPart mainPart, NotaryInfo notaryInfo, object documentControl)
        {
            //Body body = mainPart.Document.AppendChild(new Body());
            Body body;
            if (mainPart.Document.Body != null)
            {
                body = mainPart.Document.Body;
            }
            else
            {
                body = new();
            }

            // Set up page settings
            SetPageSettings(body);

            // Intro Common Text
            AddCommonIntroduction(body, notaryInfo);

            // Le contrôle du document construit sa partie du document
            if (documentControl is IDocumentBuilder documentBuilder)
            {
                documentBuilder.BuildDocumentPart(mainPart);
            }
            else
            {
                throw new InvalidOperationException("Le contrôle du document doit implémenter IDocumentBuilder.");
            }
        }

        // Add the general Intro paragraph
        private static void AddCommonIntroduction(Body body, NotaryInfo notaryInfo)
        {
            string textTemplate = "PARDEVANT Maître [notaryLastName] [notaryFirstName] soussigné, Notaire à la résidence de [notaryCity], membre de la Société Civile Professionnelle dénommée « [notaryOfficeName] » titulaire d’un office notarial, dont le siège est à [notaryCity] (République du [notaryCountry]) ;";

            // Remplacer les placeholders par les valeurs réelles
            string introductionText = textTemplate
                .Replace("[notaryLastName]", notaryInfo.LastName)
                .Replace("[notaryFirstName]", notaryInfo.FirstName)
                .Replace("[notaryCity]", notaryInfo.City)
                .Replace("[notaryOfficeName]", notaryInfo.OfficeName)
                .Replace("[notaryCountry]", notaryInfo.Country);

            // Déterminer les parties du texte à mettre en gras
            string boldFrom = "PARDEVANT";
            string boldUntil = notaryInfo.City + ","; // Par exemple, jusqu'à "COTONOU,"

            // Créer le paragraphe avec les styles spécifiés
            Paragraph introParagraph = CreateParagraph(
                introductionText,
                JustificationValues.Both,
                ParagraphType.Special,
                null, // Pas de textStyle par défaut
                boldFrom: boldFrom,
                boldUntil: boldUntil);

            // Ajouter le paragraphe au corps du document
            body.AppendChild(introParagraph);
        }

        public static void AddParagraphTexts(Body body, string[] strings, ParagraphType paragraphType)
        {
            foreach (string str in strings)
            {
                body.AppendChild(CreateParagraph(str, JustificationValues.Both, paragraphType));
            }
        }

        public static Paragraph CreateParagraph(
            string text,
            JustificationValues justification,
            ParagraphType paragraphType,
            TextStyle? textStyle = null,
            string? boldFrom = null,
            string? boldUntil = null,
            string? italicFrom = null,
            string? italicUntil = null,
            string? underlineFrom = null,
            string? underlineUntil = null,
            string? color = null,
            string? colorFrom = null,
            string? colorUntil = null)
        {
            // Définir les propriétés du paragraphe en fonction du type
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = justification },
                Indentation = new Indentation
                {
                    Left = (paragraphType == ParagraphType.Special) ? InchesToTwipsInt32(1.77).ToString() : "0",
                    Right = "0",
                    FirstLine = InchesToTwipsInt32(0.2).ToString()
                },
                SpacingBetweenLines = new SpacingBetweenLines
                {
                    Before = "0",
                    After = "0"
                },
                OutlineLevel = new OutlineLevel { Val = 9 } // Texte de corps
            };

            // Créer une liste de runs
            List<Run> runs = new List<Run>();

            // Index actuel dans le texte
            int currentIndex = 0;
            int textLength = text.Length;

            // Liste des styles à appliquer
            var styleRanges = new List<(int start, int end, RunProperties properties)>();

            // Fonction pour trouver les indices de début et de fin d'une sous-chaîne
            int FindIndex(string substring, int startIndex)
            {
                if (string.IsNullOrEmpty(substring))
                    return -1;
                return text.IndexOf(substring, startIndex, StringComparison.Ordinal);
            }

            // Ajouter les styles spécifiques
            if (boldFrom != null && boldUntil != null)
            {
                int start = FindIndex(boldFrom, currentIndex);
                int end = FindIndex(boldUntil, start) + boldUntil.Length;
                if (start >= 0 && end > start)
                {
                    styleRanges.Add((start, end, new RunProperties(new Bold())));
                }
            }

            if (italicFrom != null && italicUntil != null)
            {
                int start = FindIndex(italicFrom, currentIndex);
                int end = FindIndex(italicUntil, start) + italicUntil.Length;
                if (start >= 0 && end > start)
                {
                    styleRanges.Add((start, end, new RunProperties(new Italic())));
                }
            }

            if (underlineFrom != null && underlineUntil != null)
            {
                int start = FindIndex(underlineFrom, currentIndex);
                int end = FindIndex(underlineUntil, start) + underlineUntil.Length;
                if (start >= 0 && end > start)
                {
                    styleRanges.Add((start, end, new RunProperties(new Underline { Val = UnderlineValues.Single })));
                }
            }

            if (color != null && colorFrom != null && colorUntil != null)
            {
                int start = FindIndex(colorFrom, currentIndex);
                int end = FindIndex(colorUntil, start) + colorUntil.Length;
                if (start >= 0 && end > start)
                {
                    RunProperties colorProperties = new RunProperties(new Color { Val = color });
                    styleRanges.Add((start, end, colorProperties));
                }
            }

            // Trier les styles par ordre de début
            styleRanges = styleRanges.OrderBy(r => r.start).ToList();

            // Ajouter le style général si défini
            RunProperties generalRunProperties = new RunProperties();
            if (textStyle != null)
            {
                if (textStyle.Bold)
                    generalRunProperties.Bold = new Bold();

                if (textStyle.Italic)
                    generalRunProperties.Italic = new Italic();

                if (textStyle.Underline)
                    generalRunProperties.Underline = new Underline { Val = UnderlineValues.Single };

                if (!string.IsNullOrEmpty(textStyle.Color))
                    generalRunProperties.Color = new Color { Val = textStyle.Color };
            }

            // Parcourir le texte et créer les runs
            int position = 0;
            foreach (var range in styleRanges)
            {
                if (position < range.start)
                {
                    // Texte avant le style
                    string plainText = text.Substring(position, range.start - position);
                    Run run = new Run(generalRunProperties.CloneNode(true), new Text(plainText) { Space = SpaceProcessingModeValues.Preserve });
                    runs.Add(run);
                }

                // Texte avec le style spécifique
                string styledText = text.Substring(range.start, range.end - range.start);
                // Combiner les propriétés générales et spécifiques
                RunProperties combinedProperties = (RunProperties)generalRunProperties.CloneNode(true);
                combinedProperties.Append(range.properties);
                Run styledRun = new Run(combinedProperties, new Text(styledText) { Space = SpaceProcessingModeValues.Preserve });
                runs.Add(styledRun);

                position = range.end;
            }

            if (position < textLength)
            {
                // Texte restant après le dernier style
                string remainingText = text.Substring(position);
                Run run = new Run(generalRunProperties.CloneNode(true), new Text(remainingText) { Space = SpaceProcessingModeValues.Preserve });
                runs.Add(run);
            }

            // Créer le paragraphe et y ajouter les runs
            Paragraph paragraph = new Paragraph(paragraphProperties);
            foreach (var run in runs)
            {
                paragraph.Append(run);
            }

            return paragraph;
        }

        public static Paragraph CreateHeadingParagraph(
            string text,
            int headingLevel,
            ParagraphType paragraphType,
            TextStyle? textStyle = null,
            JustificationValues? justification = null)
        {
            if (justification == null)
            {
                justification = JustificationValues.Center;
            };
            // Définir les propriétés du paragraphe en fonction du type
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                // Justification par défaut pour les titres (peut être ajustée)
                Justification = new Justification { Val = justification },
                // Indentation en fonction du type de paragraphe
                Indentation = new Indentation
                {
                    Left = (paragraphType == ParagraphType.Special) ? InchesToTwipsInt32(1.77).ToString() : "0",
                    Right = "0",
                    FirstLine = InchesToTwipsInt32(0.2).ToString()
                },
                SpacingBetweenLines = new SpacingBetweenLines
                {
                    Before = "0",
                    After = "0"
                },
                OutlineLevel = new OutlineLevel { Val = headingLevel - 1 }, // Niveau du titre
                ParagraphStyleId = new ParagraphStyleId { Val = $"Heading{headingLevel}" } // Style du titre
            };

            // Créer les propriétés du run en fonction des styles de texte
            RunProperties runProperties = new RunProperties();

            if (textStyle != null)
            {
                if (textStyle.Bold)
                    runProperties.Bold = new Bold();

                if (textStyle.Italic)
                    runProperties.Italic = new Italic();

                if (textStyle.Underline)
                    runProperties.Underline = new Underline { Val = UnderlineValues.Single };

                if (!string.IsNullOrEmpty(textStyle.Color))
                    runProperties.Color = new Color { Val = textStyle.Color };
            }

            /* Définir la police et la taille par défaut pour les titres
            runProperties.RunFonts = new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" };
            runProperties.FontSize = new FontSize { Val = GetFontSizeForHeading(headingLevel) };
            runProperties.FontSizeComplexScript = new FontSizeComplexScript { Val = GetFontSizeForHeading(headingLevel) };*/

            // Créer le run avec le texte
            Run run = new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve });

            // Créer le paragraphe
            Paragraph paragraph = new Paragraph(paragraphProperties, run);

            return paragraph;
        }

        public static Paragraph SParagraph()
        {
            // Créer un nouvel objet Paragraph
            Paragraph paragraph = new Paragraph();

            // Définir les propriétés du paragraphe
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            // Créer un Run avec un espace non insécable
            Run run = new Run();

            // Définir les propriétés du Run pour la taille de police
            RunProperties runProperties = new RunProperties();
            // Taille de police en demi-points (5 points * 2)
            FontSize fontSize = new FontSize() { Val = "10" };
            runProperties.Append(fontSize);

            // Ajouter les propriétés du Run
            run.Append(runProperties);
            // Ajouter un espace non insécable pour que le paragraphe ne soit pas supprimé
            run.Append(new Text("\u00A0"));

            // Ajouter les propriétés du paragraphe
            paragraph.Append(paragraphProperties);
            // Ajouter le Run au paragraphe
            paragraph.Append(run);

            return paragraph;
        }

        // Add Notadog cutom Property
        private static void AddCustomProperty(WordprocessingDocument doc, string propName, string propValue)
        {
            CustomFilePropertiesPart customPropsPart;
            if (doc.CustomFilePropertiesPart == null)
            {
                customPropsPart = doc.AddCustomFilePropertiesPart();
                customPropsPart.Properties = new DocumentFormat.OpenXml.CustomProperties.Properties();
            }
            else
            {
                customPropsPart = doc.CustomFilePropertiesPart;
            }

            var props = customPropsPart.Properties;

            // Check if the property already exists.
            var existingProp = props.Elements<CustomDocumentProperty>()
                .FirstOrDefault(p => p.Name.Value == propName);

            if (existingProp != null)
            {
                // Update the existing property value
                existingProp.VTLPWSTR = new DocumentFormat.OpenXml.VariantTypes.VTLPWSTR(propValue);
            }
            else
            {
                // Get a unique PropertyId
                int pid = 2; // Property IDs must start from 2
                var existingPids = props.Elements<CustomDocumentProperty>()
                    .Select(p => p.PropertyId.Value)
                    .ToArray();

                while (existingPids.Contains(pid))
                {
                    pid++;
                }

                // Create the new custom property
                CustomDocumentProperty newProp = new()
                {
                    Name = propName,
                    PropertyId = pid,
                    VTLPWSTR = new DocumentFormat.OpenXml.VariantTypes.VTLPWSTR(propValue),
                    FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" // Text format
                };

                props.AppendChild(newProp);
            }

            props.Save();
        }

        /*BULLET LIST METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        public static void CreateBulletList(MainDocumentPart mainPart, IEnumerable<ListPoints> items, ParagraphType paragraphType, string[]? specialBullets = null)
        {
            // Générer un NumId unique pour la liste
            var numId = GenerateNewNumberingId();

            // Ajouter les définitions de numérotation au document
            Body body;
            if (mainPart.Document.Body != null)
            {
                body = mainPart.Document.Body;

                AddNumberingDefinitions(mainPart, numId, specialBullets);

                // Créer les paragraphes pour chaque élément de la liste
                foreach (var item in items)
                {
                    if (item.Text != string.Empty)
                    {
                        var paragraph = new Paragraph();

                        // Créer les propriétés du paragraphe
                        ParagraphProperties paragraphProperties = new ParagraphProperties
                        {
                            Justification = new Justification { Val = JustificationValues.Left },
                            Indentation = new Indentation
                            {
                                Left = InchesToTwipsInt32(0.5 * item.Level).ToString(),
                                Hanging = InchesToTwipsInt32(0.25).ToString()
                            },
                            SpacingBetweenLines = new SpacingBetweenLines
                            {
                                Before = "0",
                                After = "0"
                            }
                        };

                        // Définir les propriétés de numérotation
                        var numberingProperties = new NumberingProperties(
                            new NumberingLevelReference() { Val = item.Level },
                            new NumberingId() { Val = numId }
                        );

                        paragraphProperties.Append(numberingProperties);
                        paragraph.Append(paragraphProperties);

                        // Ajouter le texte de l'élément
                        var run = new Run();
                        run.Append(new Text(item.Text));
                        paragraph.Append(run);

                        // Ajouter le paragraphe au corps du document
                        body.Append(paragraph);
                    }
                }
            }
        }

        private static int numberingCounter = 1;

        private static int GenerateNewNumberingId()
        {
            return numberingCounter++;
        }

        private static void AddNumberingDefinitions(MainDocumentPart mainPart, int numId, string[]? specialBullets)
        {
            // Obtenir ou créer l'élément NumberingDefinitions
            NumberingDefinitionsPart numberingPart;

            if (mainPart.NumberingDefinitionsPart == null)
            {
                numberingPart = mainPart.AddNewPart<NumberingDefinitionsPart>();
                numberingPart.Numbering = new Numbering();
            }
            else
            {
                numberingPart = mainPart.NumberingDefinitionsPart;
            }

            // Créer un abstractNumId unique
            int abstractNumId = numId;

            // Créer l'AbstractNum avec plusieurs niveaux
            AbstractNum abstractNum = new AbstractNum() { AbstractNumberId = abstractNumId };

            // Niveau 0 - Tiret
            if (specialBullets != null)
            {
                Level level0 = new Level(
                new StartNumberingValue() { Val = 1 },
                new NumberingFormat() { Val = NumberFormatValues.Bullet },
                new LevelText() { Val = specialBullets[0] },
                //new LevelJustification() { Val = LevelJustificationValues.Left },
                new ParagraphProperties(
                    new Indentation() { Left = "0", Hanging = "360" },
                    new Justification { Val = JustificationValues.Both }
                    )
                )
                { LevelIndex = 0 };

                abstractNum.Append(level0);

                if (specialBullets.Length > 1)
                {
                    // Niveau 1 - Cercle
                    Level level1 = new Level(
                        new StartNumberingValue() { Val = 1 },
                        new NumberingFormat() { Val = NumberFormatValues.Bullet },
                        new LevelText() { Val = specialBullets[1] },
                        //new LevelJustification() { Val = LevelJustificationValues.Left },
                        new ParagraphProperties(
                            new Indentation() { Left = "720", Hanging = "360" },
                            new Justification { Val = JustificationValues.Both }
                        )
                    )
                    { LevelIndex = 1 };

                    abstractNum.Append(level1);
                }
            }
            else
            {
                Level level0 = new Level(
                new StartNumberingValue() { Val = 1 },
                new NumberingFormat() { Val = NumberFormatValues.Bullet },
                new LevelText() { Val = "-" },
                //new LevelJustification() { Val = LevelJustificationValues.Left },
                new ParagraphProperties(
                    new Indentation() { Left = "◦", Hanging = "360" },
                    new Justification { Val = JustificationValues.Both }
                    )
                )
                { LevelIndex = 0 };

                // Niveau 1 - Cercle
                Level level1 = new Level(
                    new StartNumberingValue() { Val = 1 },
                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                    new LevelText() { Val = "o" },
                    //new LevelJustification() { Val = LevelJustificationValues.Left },
                    new ParagraphProperties(
                        new Indentation() { Left = "720", Hanging = "360" },
                        new Justification { Val = JustificationValues.Both }
                    )
                )
                { LevelIndex = 1 };

                abstractNum.Append(level0);
                abstractNum.Append(level1);
            }
            

            

            numberingPart.Numbering.Append(abstractNum);

            // Créer le NumberingInstance
            NumberingInstance numberingInstance = new NumberingInstance(
                new AbstractNumId() { Val = abstractNumId }
            )
            { NumberID = numId };

            numberingPart.Numbering.Append(numberingInstance);
        }

        private static string GetBulletCharacter(BulletType bulletType, string customBullet)
        {
            switch (bulletType)
            {
                case BulletType.Dash:
                    return "-"; // Tiret
                case BulletType.Circle:
                    return "●"; // Cercle plein
                case BulletType.Square:
                    return "■"; // Carré
                case BulletType.Custom:
                    return customBullet ?? "-"; // Utiliser le symbole personnalisé ou un tiret par défaut
                default:
                    return "-"; // Par défaut, utiliser un tiret
            }
        }

        /*TABLE METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // Table Creation
        public static void CreateTable(
            Body body,
            int[] columnWidths,
            List<TableCellData> headerCells,
            List<List<TableCellData>> rows)
        {
            // Créer la table
            Table table = new Table();

            // Définir les propriétés de la table
            TableProperties tblProperties = new TableProperties(
                new TableWidth { Width = (columnWidths.Sum()).ToString(), Type = TableWidthUnitValues.Dxa },
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new RightBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
                ),
                new TableLook { Val = "04A0", FirstRow = OnOffValue.FromBoolean(true), LastRow = OnOffValue.FromBoolean(false), FirstColumn = OnOffValue.FromBoolean(true), LastColumn = OnOffValue.FromBoolean(false), NoHorizontalBand = OnOffValue.FromBoolean(false), NoVerticalBand = OnOffValue.FromBoolean(true) }
            );
            table.AppendChild(tblProperties);

            // Définir la grille de la table
            TableGrid tg = new TableGrid();
            foreach (var width in columnWidths)
            {
                tg.Append(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(tg);

            // Set row height to auto
            TableRowProperties defaultRowProperties = new TableRowProperties(
                new TableRowHeight() { HeightType = HeightRuleValues.Auto }
            );

            // Ajouter l'en-tête si présent
            if (headerCells != null && headerCells.Count > 0)
            {
                TableRow headerRow = new TableRow();

                
                headerRow.Append(defaultRowProperties);

                foreach (var cellData in headerCells)
                {
                    TableCell cell = CreateTableCell(cellData);
                    headerRow.Append(cell);
                }

                table.Append(headerRow);
            }

            // Ajouter les lignes
            foreach (var rowCells in rows)
            {
                TableRow row = new TableRow();

                row.Append(defaultRowProperties);

                foreach (var cellData in rowCells)
                {
                    TableCell cell = CreateTableCell(cellData);
                    row.Append(cell);
                }

                table.Append(row);
            }

            // Ajouter la table au corps du document
            body.Append(table);
        }

        // Cell creation
        private static TableCell CreateTableCell(TableCellData cellData)
        {
            TableCell cell = new TableCell();

            // Gérer la fusion de cellules
            if (cellData.RowSpan > 1)
            {
                cellData.CellProperties.Append(new VerticalMerge { Val = MergedCellValues.Restart });
            }
            else if (cellData.RowSpan == 0)
            {
                cellData.CellProperties.Append(new VerticalMerge());
            }

            if (cellData.ColSpan > 1)
            {
                cellData.CellProperties.Append(new GridSpan { Val = cellData.ColSpan });
            }

            // Définir la largeur de la cellule si nécessaire
            // cellData.CellProperties.Append(new TableCellWidth { Width = "..." });

            // Créer le paragraphe
            Paragraph paragraph = new Paragraph();

            // Appliquer les propriétés du paragraphe
            if (cellData.ParagraphProperties != null)
            {
                paragraph.Append(cellData.ParagraphProperties.CloneNode(true));
            }

            // Créer le run avec le texte
            Run run = new Run();

            // Appliquer les propriétés du run
            if (cellData.RunProperties != null)
            {
                run.Append(cellData.RunProperties.CloneNode(true));
            }

            run.Append(new Text(cellData.Text) { Space = SpaceProcessingModeValues.Preserve });

            paragraph.Append(run);

            cell.Append(cellData.CellProperties.CloneNode(true));
            cell.Append(paragraph);

            return cell;
        }



        /*STYLING METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // Set general Styling for the Pages
        public static void SetPageSettings(Body body)
        {
            // Convert inches to Twips (1 inch = 1440 Twips)
            Int32Value topMargin = InchesToTwipsInt32(0.51);
            Int32Value bottomMargin = InchesToTwipsInt32(1.38);
            UInt32Value insideMargin = InchesToTwipsUInt32(1.58); // Left margin for mirror margins
            UInt32Value outsideMargin = InchesToTwipsUInt32(0.39); // Right margin for mirror margins
            UInt32Value gutterMargin = InchesToTwipsUInt32(0.39);

            // Header and footer margins
            UInt32Value headerMargin = InchesToTwipsUInt32(0.39);
            UInt32Value footerMargin = InchesToTwipsUInt32(0.39);

            // Define page margins
            PageMargin pageMargin = new PageMargin
            {
                Top = topMargin,
                Bottom = bottomMargin,
                Left = insideMargin,
                Right = outsideMargin,
                Header = headerMargin,
                Footer = footerMargin,
                Gutter = gutterMargin
            };

            // Set page size to A4 in Twips (1 point = 20 Twips)
            PageSize pageSize = new PageSize
            {
                Width = PointsToTwipsUInt32(595.3),
                Height = PointsToTwipsUInt32(841.9),
                Orient = PageOrientationValues.Portrait
            };

            // Enable mirror margins
            MirrorMargins mirrorMargins = new MirrorMargins();

            // Start section on a new page
            SectionType sectionType = new SectionType { Val = SectionMarkValues.NextPage };

            // Create section properties
            SectionProperties sectionProperties = new SectionProperties(
                pageMargin,
                pageSize,
                mirrorMargins,
                sectionType
            );

            // Append section properties to the body
            body.Append(sectionProperties);
        }

        // Set the default styles to use throughout the doc
        public static void SetDefaultStyles(MainDocumentPart mainPart)
        {
            // Créer le StyleDefinitionsPart s'il n'existe pas
            StyleDefinitionsPart stylePart;
            if (mainPart.StyleDefinitionsPart == null)
            {
                stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylePart.Styles = new Styles();
            }
            else
            {
                stylePart = mainPart.StyleDefinitionsPart;
            }

            Styles styles;
            if (stylePart.Styles != null)
            {
                styles = stylePart.Styles;
            }
            else
            {
                styles = new Styles();
            }

            // Définir le style par défaut du paragraphe
            Style defaultParagraphStyle = new Style
            {
                Type = StyleValues.Paragraph,
                StyleId = "DefaultParagraphStyle",
                Default = true
            };

            StyleName styleName = new StyleName { Val = "Normal" };
            defaultParagraphStyle.Append(styleName);

            // Définir les propriétés du style
            StyleRunProperties styleRunProperties = new StyleRunProperties();
            RunFonts runFonts = new RunFonts { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" };
            FontSize fontSize = new FontSize { Val = "18" }; // Taille de police 9 pt (9 * 2 = 18 en demi-points)
            FontSizeComplexScript fontSizeComplex = new FontSizeComplexScript { Val = "18" };

            styleRunProperties.Append(runFonts);
            styleRunProperties.Append(fontSize);
            styleRunProperties.Append(fontSizeComplex);

            defaultParagraphStyle.Append(styleRunProperties);

            // Ajouter le style aux styles du document
            styles.Append(defaultParagraphStyle);
        }

        /*UTILITY METHODS----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // Enum Helper
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();

                return attribute == null ? value.ToString() : attribute.Description;
            }
            else
            {
                return string.Empty;
            }
        }

        // Convert Inches into Int32
        public static Int32Value InchesToTwipsInt32(double inches)
        {
            return new Int32Value((int)(inches * 1440));
        }

        // Convert Points into UInt32
        public static UInt32Value PointsToTwipsUInt32(double points)
        {
            return new UInt32Value((uint)(points * 20));
        }

        //Convert Inches into UInt32
        public static UInt32Value InchesToTwipsUInt32(double inches)
        {
            return new UInt32Value((uint)(inches * 1440));
        }

        private static void AddHeader(MainDocumentPart mainPart)
        {
            // Ajouter la partie de l'en-tête
            HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
            string headerPartId = mainPart.GetIdOfPart(headerPart);

            // Créer l'en-tête
            Header header = new Header();

            // Créer un paragraphe avec le numéro de page aligné à droite
            Paragraph paragraph = new Paragraph();

            // Définir les propriétés du paragraphe (alignement à droite)
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Justification = new Justification { Val = JustificationValues.Right };
            paragraph.Append(paragraphProperties);

            // Créer un champ de numéro de page
            Run runPageNumber = new Run();
            RunProperties runProperties = new RunProperties();
            Color color = new Color { Val = "808080" }; // Couleur gris
            runProperties.Append(color);
            runPageNumber.Append(runProperties);

            // Ajouter le champ de numéro de page
            //FieldCode fieldCode = new FieldCode(" PAGE   \\* MERGEFORMAT ");
            runPageNumber.Append(new SimpleField() { Instruction = "PAGE" });

            // Ajouter le run au paragraphe
            paragraph.Append(runPageNumber);

            // Ajouter le paragraphe à l'en-tête
            header.Append(paragraph);

            // Enregistrer l'en-tête
            headerPart.Header = header;

            // Associer l'en-tête à la section du document
            SectionProperties sectionProperties;
            if (mainPart.Document.Body != null)
            {
                if (mainPart.Document.Body.Elements<SectionProperties>().Any())
                {
                    sectionProperties = mainPart.Document.Body.Elements<SectionProperties>().Last();
                }
                else
                {
                    sectionProperties = new SectionProperties();
                    mainPart.Document.Body.Append(sectionProperties);
                }
            }
            else
            {
                sectionProperties = new SectionProperties();
            }
            
            // Créer une référence à l'en-tête
            HeaderReference headerReference = new HeaderReference
            {
                Type = HeaderFooterValues.Default,
                Id = headerPartId
            };
            sectionProperties.Append(headerReference);
        }




        //Tests, might delete
        public static Paragraph CreateCustomHeading2(string text, int outlineLevel)
        {
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                OutlineLevel = new OutlineLevel { Val = outlineLevel - 1 }, // Les niveaux vont de 0 à 8
                Justification = new Justification { Val = JustificationValues.Left }
            };

            RunProperties runProperties = new RunProperties
            {
                Bold = new Bold()
            };

            Run run = new Run(runProperties, new Text(text));

            Paragraph paragraph = new Paragraph(paragraphProperties, run);

            return paragraph;
        }

        public static void AddComment(MainDocumentPart mainPart, string commentText)
        {
            if (mainPart.WordprocessingCommentsPart == null)
            {
                mainPart.AddNewPart<WordprocessingCommentsPart>();
            }

            Comments comments;
            if (mainPart.WordprocessingCommentsPart != null)
            {
                mainPart.WordprocessingCommentsPart.Comments = new Comments();
                comments = mainPart.WordprocessingCommentsPart.Comments;
            }
            else
            {
                comments = new Comments();
            }
            

            // Créer un identifiant unique pour le commentaire
            string commentId = "0"; // Vous devrez gérer l'incrémentation si vous avez plusieurs commentaires

            // Créer le commentaire
            Comment comment = new Comment
            {
                Id = commentId,
                Author = "Auteur",
                Date = DateTime.Now
            };

            comment.AppendChild(new Paragraph(new Run(new Text(commentText))));
            comments.Append(comment);
            comments.Save();

            // Ajouter le texte du document avec une référence au commentaire
            Paragraph paragraph = new Paragraph();

            Run run = new Run();
            CommentRangeStart commentRangeStart = new CommentRangeStart { Id = commentId };
            CommentRangeEnd commentRangeEnd = new CommentRangeEnd { Id = commentId };
            Run commentReferenceRun = new Run(new CommentReference { Id = commentId });

            run.Append(commentRangeStart);
            run.Append(new Text("Texte annoté"));
            run.Append(commentRangeEnd);
            run.Append(commentReferenceRun);

            paragraph.Append(run);
            if (mainPart.Document.Body != null)
            {
                mainPart.Document.Body.Append(paragraph);
            }
        }

        public static void AddBookmark(Body body, string bookmarkName, string bookmarkText)
        {
            // Créer un identifiant unique pour le signet
            UInt32 bookmarkId = 1; // Vous devrez gérer l'incrémentation si vous avez plusieurs signets

            // Créer le signet de début
            BookmarkStart bookmarkStart = new BookmarkStart
            {
                Name = bookmarkName,
                Id = bookmarkId.ToString()
            };

            // Créer le run avec le texte du signet
            Run run = new Run(new Text(bookmarkText));

            // Créer le signet de fin
            BookmarkEnd bookmarkEnd = new BookmarkEnd
            {
                Id = bookmarkId.ToString()
            };

            // Créer le paragraphe et ajouter les éléments
            Paragraph paragraph = new Paragraph();
            paragraph.Append(bookmarkStart);
            paragraph.Append(run);
            paragraph.Append(bookmarkEnd);

            // Ajouter le paragraphe au corps du document
            body.AppendChild(paragraph);
        }
    }
}
