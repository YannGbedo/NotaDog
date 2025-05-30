using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace NotaDog.Interfaces
{
    public interface IDocumentBuilder
    {
        void BuildDocumentPart(MainDocumentPart mainPart);
    }
}
