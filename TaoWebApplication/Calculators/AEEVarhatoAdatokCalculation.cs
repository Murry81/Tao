using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class AEEVarhatoAdatokCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var customer = service.GetCustomer(int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString()));
            var fieldIds = new List<int> { 801, 802, 800, 803, 804, 805, 806, 807, 808};

            // Get FigyelembeVettHónapokSzáma
            var f5 = decimal.Parse(service.GetFieldById(5, sessionId).StringValue);

            foreach (var id in fieldIds)
            {
                var field = fields.FirstOrDefault(f => f.Id == id);
                switch (field.Id)
                {
                    case 800: // Figyelembe vett hónapok adózás előtti eredménye
                        {
                            field.DecimalValue = Calculate800(fields, service, sessionId);
                            break;
                        }
                    case 801: // Figyelembe vett hónapokra vonatkozó IPA
                        {
                            field.DecimalValue = Calculate801(fields, service, sessionId, f5);
                            break;
                        }
                    case 802: // Figyelembe vett hónapokra vonatkozó innovációs járulék
                        {
                            field.DecimalValue = Calculate802(fields, service, sessionId, f5);
                            break;
                        }
                    case 803: // Értékesítés nettó árbevétele
                        {
                            field.DecimalValue = Calculate803(fields, service, sessionId, f5);
                            break;
                        }
                    case 804: // Egyéb bevételek
                        {
                            field.DecimalValue = Calculate804(fields, service, sessionId, f5);
                            break;
                        }
                    case 805: // Pénzügyi bevételek
                        {
                            field.DecimalValue = Calculate805(fields, service, sessionId, f5);
                            break;
                        }
                    case 806: // ELÁBÉ
                        {
                            field.DecimalValue = Calculate806(fields, service, sessionId, f5);
                            break;
                        }
                    case 807: // Közvetített szolgáltatások
                        {
                            field.DecimalValue = Calculate807(fields, service, sessionId, f5);
                            break;
                        }
                    case 808: // Alvállalkozói teljesítmény
                        {
                            field.DecimalValue = Calculate808(fields, service, sessionId, f5);
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(8, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate808(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.Alvállalkozói teljesítmények / 0.Figyelembe vett hónapok száma * 12 + 1.1.Alvállalkozó teljesítmények
            // f25 / figyelembeVettHonapokSzama * 12 + f327

            var f25 = GenericCalculations.GetValue(service.GetFieldById(25, sessionId).DecimalValue);
            var f327 = GenericCalculations.GetValue(service.GetFieldById(327, sessionId).DecimalValue);

            return Math.Round(f25 / figyelembeVettHonapokSzama * 12 + f327);
        }

        private static decimal? Calculate807(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.Közvetített szolgáltatások / 0.Figyelembe vett hónapok száma * 12 + 1.1.Közvetített szolgáltatások
            // f24 / figyelembeVettHonapokSzama * 12 + f326

            var f24 = GenericCalculations.GetValue(service.GetFieldById(24, sessionId).DecimalValue);
            var f326 = GenericCalculations.GetValue(service.GetFieldById(326, sessionId).DecimalValue);

            return Math.Round(f24 / figyelembeVettHonapokSzama * 12 + f326);
        }

        private static decimal? Calculate806(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.ELÁBÉ / 0.Figyelembe vett hónapok száma * 12 + 1.1.ELÁBÉ
            // f9 / figyelembeVettHonapokSzama * 12 + f325

            var f9 = GenericCalculations.GetValue(service.GetFieldById(9, sessionId).DecimalValue);
            var f325 = GenericCalculations.GetValue(service.GetFieldById(325, sessionId).DecimalValue);

            return Math.Round(f9 / figyelembeVettHonapokSzama * 12 + f325);
        }

        private static decimal? Calculate805(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.Pénzügyi bevételek / 0.Figyelembe vett hónapok száma * 12 + 1.1.Pénzügyi műveletek bevétele
            // f18 / figyelembeVettHonapokSzama * 12 + f307

            var f18 = GenericCalculations.GetValue(service.GetFieldById(18, sessionId).DecimalValue);
            var f307 = GenericCalculations.GetValue(service.GetFieldById(307, sessionId).DecimalValue);

            return Math.Round(f18 / figyelembeVettHonapokSzama * 12 + f307);
        }

        private static decimal? Calculate804(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.Egyéb bevételek / 0.Figyelembe vett hónapok száma * 12 + 1.1.Egyéb bevételek
            // f14 / figyelembeVettHonapokSzama * 12 + f302

            var f14 = GenericCalculations.GetValue(service.GetFieldById(14, sessionId).DecimalValue);
            var f302 = GenericCalculations.GetValue(service.GetFieldById(302, sessionId).DecimalValue);

            return Math.Round(f14 / figyelembeVettHonapokSzama * 12 + f302);
        }

        private static decimal? Calculate803(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 1.Értékesítés nettó árbevétele / 0.Figyelembe vett hónapok száma * 12 + 1.1.Értékesítés nettó árbevétele
            // f13 / figyelembeVettHonapokSzama *12 + f301

            var f13 = GenericCalculations.GetValue(service.GetFieldById(13, sessionId).DecimalValue);
            var f301 = GenericCalculations.GetValue(service.GetFieldById(301, sessionId).DecimalValue);

            return Math.Round(f13 / figyelembeVettHonapokSzama * 12 + f301);
        }

        private static decimal? Calculate802(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // 2.3.Innovációs járulék / 12 * 0.Figyelembe vett hónapok száma / 1000
            // f705 / 12 * figyelembeVettHonapokSzama / 1000

            var f705 = GenericCalculations.GetValue(service.GetFieldById(705, sessionId).DecimalValue);
            return Math.Round(f705 / 12 * figyelembeVettHonapokSzama / 1000);
        }

        private static decimal? Calculate801(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal figyelembeVettHonapokSzama)
        {
            // Ha 0.IPA kapcsolt státusz = igaz, akkor 2.1.Iparűzési adó csökkentett összege / 12 * 0.Figyelembe vett hónapok száma / 1000
            // Ha 0.IPA kapcsolt státusz = hamis, akkor 2.2.Iparűzési adó csökkentett összege / 12 * 0.Figyelembe vett hónapok száma / 1000
            // F440 / 12 * f5 / 1000
            var f440 = GenericCalculations.GetValue(service.GetFieldById(440, sessionId).DecimalValue);
           
            return Math.Round(f440 / 12 * figyelembeVettHonapokSzama / 1000);
        }

        private static decimal? Calculate800(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 1.Adózás előtti eredmény(IPA és innovációs járulék nélkül) / 1000 - Figyelembe vett hónapokra vonatkozó IPA -Figyelembe vett hónapokra vonatkozó innovációs járulék
            // f53 / 1000 - f801 - f802

            var f53 = GenericCalculations.GetValue(service.GetFieldById(53, sessionId).DecimalValue);
            var f801 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 801).DecimalValue);
            var f802 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 802).DecimalValue);

            return Math.Round(f53 / 1000 - f801 - f802);
        }
    }
}