using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.CustomProperties;
using System.Linq;

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
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
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
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var customProps = wordDoc.CustomFilePropertiesPart;
                    if (customProps != null)
                    {
                        var props = customProps.Properties;
                        var generatedByProp = props.Elements<CustomDocumentProperty>().FirstOrDefault(p => p.Name?.Value == "GeneratedByNotaDog");
                        if (generatedByProp != null && generatedByProp.VTLPWSTR  != null && generatedByProp.VTLPWSTR.Text == "True")
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
            }
            catch
            {
                // Handle exceptions if needed
            }
            return null;
        }

        public static void CreateDocument(string filePath, string documentType)
        {
            // Create the Word document
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // Add main document part
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();

                // Add custom properties
                AddCustomProperty(wordDoc, "GeneratedByNotaDog", "True");
                AddCustomProperty(wordDoc, "DocumentType", documentType);

                // TODO: Add content to the document
            }
        }

        private static void AddCustomProperty(WordprocessingDocument doc, string propName, string propValue)
        {
            var customProps = doc.CustomFilePropertiesPart;
            if (customProps == null)
            {
                customProps = doc.AddCustomFilePropertiesPart();
                customProps.Properties = new DocumentFormat.OpenXml.CustomProperties.Properties();
            }

            var props = customProps.Properties;
            var prop = new CustomDocumentProperty
            {
                Name = propName,
                FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}", // Text format
                VTLPWSTR = new DocumentFormat.OpenXml.VariantTypes.VTLPWSTR(propValue)
            };

            // Remove existing property with the same name if it exists
            var existingProp = props.Elements<CustomDocumentProperty>().FirstOrDefault(p => p.Name?.Value == propName);
            if (existingProp != null)
            {
                existingProp.Remove();
            }

            props.AppendChild(prop);
            props.Save();
        }
    }
}
