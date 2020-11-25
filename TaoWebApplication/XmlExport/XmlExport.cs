using System;
using System.Xml;
using System.Xml.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Controllers;

namespace TaoWebApplication.XmlExport
{
    public class XmlExport
    {
        public static string GenerateDocument(int documentId, Guid sessionId, IDataService service)
        {
            var reportData = service.GetExportReportData(sessionId, documentId);
            XDocument document = new XDocument();

            var nyomtatvanyok = new XElement("nyomtatvanyok");
            document.Add(nyomtatvanyok);
            var nyomtatvany = new XElement("nyomtatvany");

            nyomtatvanyok.Add(nyomtatvany);

            var nyomtatvanyinformacio = new XElement("nyomtatvanyinformacio");
            nyomtatvanyinformacio.Add(new XElement("nyomtatvanyazonosito", "1929"));
            nyomtatvanyinformacio.Add(new XElement("nyomtatvanyverzio", "7.0"));

            nyomtatvany.Add(nyomtatvanyinformacio);

            var adozo = new XElement("adozo");
            adozo.Add(new XElement("nev", service.GetFieldById(1, sessionId).StringValue));
            adozo.Add(new XElement("adoszam", service.GetCustomerTaxNumberBySessionId(sessionId)));
            nyomtatvanyinformacio.Add(adozo);

            // 32/33 field
            var idoszak = new XElement("idoszak");
            idoszak.Add(new XElement("tol", service.GetFieldById(31, sessionId).DateValue.Value.ToString("yyyyMMdd")));
            idoszak.Add(new XElement("ig", service.GetFieldById(32, sessionId).DateValue.Value.ToString("yyyyMMdd")));
            nyomtatvanyinformacio.Add(idoszak);

            var mezok = new XElement("mezok");
            nyomtatvany.Add(mezok);

            foreach (var field in reportData.Fields)
            {
                mezok.Add(new XElement("mezo", new XAttribute("eazon", field.AnykId), DataConverter.GetTypedValue(field)));
            }


            return document.ToString();
        }
    }
}