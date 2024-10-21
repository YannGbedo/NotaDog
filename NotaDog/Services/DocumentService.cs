using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using NotaDog.Controls;
using NotaDog.Windows;

namespace NotaDog.Services
{
    public static class DocumentService
    {
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

        public static void CreateDocument(string filePath, string documentType, NotaryInfo notaryInfo, PromesseDeVenteData? documentData)
        {
            // Create the Word document
            using WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
            // Add main document part
            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            Body body = mainPart.Document.AppendChild(new Body());

            // Add custom properties
            AddCustomProperty(wordDoc, "GeneratedByNotaDog", "True");
            AddCustomProperty(wordDoc, "DocumentType", documentType);

            // Build the document content
            BuildDocumentContent(body, notaryInfo, documentData);
        }

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

        private static void BuildDocumentContent(Body body, NotaryInfo notaryInfo, PromesseDeVenteData? documentData)
        {
            // Add Notary information
            body.AppendChild(new Paragraph(new Run(new Text("Notary Information:"))) { ParagraphProperties = new ParagraphProperties(new Bold()) });

            body.AppendChild(CreateParagraph($"First Name: {notaryInfo.FirstName}"));
            body.AppendChild(CreateParagraph($"Last Name: {notaryInfo.LastName}"));
            body.AppendChild(CreateParagraph($"City: {notaryInfo.City}"));
            body.AppendChild(CreateParagraph($"Country: {notaryInfo.Country}"));
            body.AppendChild(CreateParagraph($"Office Name: {notaryInfo.OfficeName}"));
            body.AppendChild(CreateParagraph($"Office Address: {notaryInfo.OfficeAddress}"));
            body.AppendChild(CreateParagraph($"Authority City: {notaryInfo.AuthorityCity}"));
            body.AppendChild(CreateParagraph($"Authority Country: {notaryInfo.AuthorityCountry}"));

            // Add a separator
            body.AppendChild(new Paragraph(new Run(new Text(" "))));

            // Add Document-specific information
            body.AppendChild(new Paragraph(new Run(new Text("Document Information:"))) { ParagraphProperties = new ParagraphProperties(new Bold()) });

            if (documentData != null)
            {
                //body.AppendChild(CreateParagraph($"Property Address: {documentData.PropertyAddress}"));
                // Add other fields as needed
            }
        }
        public static Paragraph CreateParagraph(string text)
        {
            return new Paragraph(new Run(new Text(text)));
        }
    }
}
