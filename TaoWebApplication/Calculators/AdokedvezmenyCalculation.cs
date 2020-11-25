using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class AdokedvezmenyCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
           foreach (var field in fields)
            {
                switch (field.Id)
                {
                    case 1307: // Figyelembe vehető maximum adókedvezmény
                        {
                            field.DecimalValue = Calculate1307(service, sessionId);
                            break;
                        }
                    case 1320: // Még felhasználható 80%
                        {
                            field.DecimalValue = Calculate1320(fields, service, sessionId);
                            break;
                        }
                    case 1309: // 80%-ban igénybe vehető kedvezmény
                        {
                            field.DecimalValue = fields.FirstOrDefault(f => f.Id == 1300).DecimalValue;
                            break;
                        }
                    case 1311: // 70%-ban igénybe vehető kedvezmény
                        {
                            field.DecimalValue = Calculate1311(fields);
                            break;
                        }
                    case 1312: // Adókedvezmények összesen
                        {
                            field.DecimalValue = Calculate1312(fields);
                            break;
                        }
                    case 1321: // Még felhasználható 70% -> Filmgyártást támogató kedvezmény
                        {
                            field.DecimalValue = Calculate1321(fields);
                            break;
                        }
                    case 1322:
                    case 1323:
                    case 1324:
                    case 1325:
                    case 1326: // Még felhasználható 70% -> Látvány-csapatsport kedvezmény
                        {
                            field.DecimalValue = CalculateMegFelhasznalhato70(fields, field);
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(13, sessionId);
            CalculateValues(fields, service, sessionId);

            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? CalculateMegFelhasznalhato70(List<FieldDescriptorDto> fields, FieldDescriptorDto currentField)
        {
            // Figyelembe vehető maximum adókedvezmény - Még felhasználható 70 % az előző rekordban, ha nagyobb, mint 0, különben 0
            // ha currentfield.id-1 - currentfield.id-20 > 0 ? currentfield.id-1 - currentfield.id-20 : 0;
            var curretnValue = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == currentField.Id - 20).DecimalValue);
            var previousValue = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == currentField.Id - 1).DecimalValue);

            return previousValue - curretnValue > 0 ? previousValue - curretnValue: 0;
        }

        private static decimal? Calculate1321(List<FieldDescriptorDto> fields)
        {
            // Figyelembe vehető maximum adókedvezmény - Még felhasználható 80%, ha nagyobb mint 0, különben 0
            // ha f1320 - f1301 > 0 ? f1320 - f1301 : 0;
            var f1301 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1301).DecimalValue);
            var f1320 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1320).DecimalValue);

            return f1320 - f1301 > 0 ? f1320 - f1301 : 0;
        }

        private static decimal? Calculate1312(List<FieldDescriptorDto> fields)
        {
            // 80%-ban igénybe vehető kedvezmény + 70%-ban igénybe vehető kedvezmény
            // f1309 + f1311
            var f1309 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1309).DecimalValue);
            var f1311 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1311).DecimalValue);

            return f1309 + f1311;
        }

        private static decimal? Calculate1311(List<FieldDescriptorDto> fields)
        {
            // Filmgyártást támogató kedvezmény + Látvány-csapatsport kedvezmény + Szövetkezeti kedvezmény + Hitelszerződés alapján fizetett kamat + Energiahatékonysági beruházás + Élőzenei szolgáltatás
            // f1301 +f1302 + f1303 + f1304 + ff1305 + f1306

            return GenericCalculations.SumList(fields, new List<int> { 1301, 1302, 1303, 1304, 1305, 1306 });
        }

        private static decimal? Calculate1320(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Figyelembe vehető maximum adókedvezmény > 0, akkor Figyelembe vehető maximum adókedvezmény - Fejlesztési adókedvezmény
            // ha f1307 > 0 => f1307 - f1300

            var f1307 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1307).DecimalValue);
            if (f1307 <= 0)
                return 0;

            var f1300 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1300).DecimalValue);

            return f1307 - f1300;
        }

        private static decimal? Calculate1307(IDataService service, Guid sessionId)
        {
            // 4.Számított adó
            // f1412

            return service.GetFieldById(1412, sessionId).DecimalValue;
        }
    }
}