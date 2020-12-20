using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.XmlExport
{
    public class XmlExport
    {
        public static byte[] GenerateDocument(int documentId, Guid sessionId, IDataService service)
        {
            var reportData = service.GetExportReportData(sessionId, documentId);

            if (documentId == 2)
            {
                return GenerateIpa(documentId, sessionId, service, reportData);
            }
            if (documentId == 1)
            {
                return GenerateTarsasagiAdoDoc(documentId, sessionId, service, reportData);
            }
            return null;
        }

        private static byte[] GenerateTarsasagiAdoDoc(int documentId, Guid sessionId, IDataService service, Contracts.Contracts.ExportReportDto reportData)
        {
            return Encoding.UTF8.GetBytes(CreateDocument(documentId, sessionId, service, reportData).ToString());
        }

        private static byte[] GenerateIpa(int documentId, Guid sessionId, IDataService service, Contracts.Contracts.ExportReportDto reportData)
        {
            using (var ms = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    if (reportData.RowIds.Any())
                    {
                        foreach (int? rowIndex in reportData.Fields.Where(r => r.FieldId == 1834).Select(f => f.rowId).Distinct())
                        {
                            XDocument document = CreateDocument(documentId, sessionId, service, reportData, rowIndex);
                            var documentResult = document.ToString();

                            var onkormanyzat = reportData.Fields.FirstOrDefault(f => f.rowId == rowIndex && f.FieldId == 1834).FormattedValue;
                            var zipEntry = archive.CreateEntry($"{onkormanyzat}_IPA.xml", CompressionLevel.Fastest);
                            using (var zipStream = zipEntry.Open())
                            {
                                var bytes = Encoding.UTF8.GetBytes(documentResult);
                                zipStream.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    else
                    {
                        XDocument document = CreateDocument(documentId, sessionId, service, reportData);
                    }
                }
                return ms.ToArray();
            }
        }

        private static XDocument CreateDocument(int documentId, Guid sessionId, IDataService service, Contracts.Contracts.ExportReportDto reportData, int? rowIndex = null)
        {
            XDocument document = new XDocument();

            var nyomtatvanyok = new XElement("nyomtatvanyok");
            document.Add(nyomtatvanyok);
            var nyomtatvany = new XElement("nyomtatvany");

            nyomtatvanyok.Add(nyomtatvany);

            var nyomtatvanyinformacio = new XElement("nyomtatvanyinformacio");
            nyomtatvanyinformacio.Add(new XElement("nyomtatvanyazonosito", service.GetDocumentIdentifier(documentId)));
            nyomtatvanyinformacio.Add(new XElement("nyomtatvanyverzio", "7.0"));

            nyomtatvany.Add(nyomtatvanyinformacio);

            var customer = service.GetCustomerBySessionId(sessionId);
            var adozo = new XElement("adozo");
            adozo.Add(new XElement("nev", customer.Nev));
            adozo.Add(new XElement("adoszam", customer.Adoszam));
            nyomtatvanyinformacio.Add(adozo);

            // 31/32 field
            var idoszak = new XElement("idoszak");
            idoszak.Add(new XElement("tol", service.GetFieldById(31, sessionId).DateValue.Value.ToString("yyyyMMdd")));
            idoszak.Add(new XElement("ig", service.GetFieldById(32, sessionId).DateValue.Value.ToString("yyyyMMdd")));
            nyomtatvanyinformacio.Add(idoszak);

            var mezok = new XElement("mezok");
            nyomtatvany.Add(mezok);

            foreach (var field in reportData.Fields.Where(f => f.rowId == null || f.rowId == rowIndex))
            {
                var stringValue = field.FormattedValue;
                if (!string.IsNullOrEmpty(stringValue))
                {
                    mezok.Add(new XElement("mezo", new XAttribute("eazon", field.AnykId), stringValue));
                }
            }

            return document;
        }
    }
}