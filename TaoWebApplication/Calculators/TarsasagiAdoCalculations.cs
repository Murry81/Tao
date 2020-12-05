using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TarsasagiAdoCalculations
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 1407: // Adózás előtti eredmény
                        {
                            field.DecimalValue = Calculate1407(service, sessionId);
                            break;
                        }
                    case 1408: // Évesre átszámított nyereség
                        {
                            field.DecimalValue = Calculate1408(fields, service, sessionId);
                            break;
                        }
                    case 1409: // Várható korrekció
                        {
                            field.DecimalValue = Calculate1409(service, sessionId);
                            break;
                        }
                    case 1410: // Adóalap csökkentő/növelő tételek
                        {
                            field.DecimalValue = Calculate1410(service, sessionId);
                            break;
                        }
                    case 1411: // Adóalap
                        {
                            field.DecimalValue = Calculate1411(fields);
                            break;
                        }
                    case 1412: // Számított adó
                        {
                            field.DecimalValue = Calculate1412(fields);
                            break;
                        }
                    case 1413: // Társasági adókedvezmények
                        {
                            field.DecimalValue = Calculate1413(service, sessionId);
                            break;
                        }
                    case 1414: // Tárgyévi társasági adó
                        {
                            field.DecimalValue = Calculate1414(fields);
                            break;
                        }
                    case 1415: // Tárgyévi társasági adó
                        {
                            field.DecimalValue = Calculate1415(fields);
                            break;
                        }
                    case 1416: // Pénzügyileg rendezendő
                        {
                            field.DecimalValue = Calculate1416(fields);
                            break;
                        }
                    case 1417: // Feltöltéskor még felajánlható
                        {
                            field.DecimalValue = Calculate1417(fields, service, sessionId);
                            break;
                        }
                    case 1418: // Adóbevalláskor még felajánlható
                        {
                            field.DecimalValue = Calculate1418(fields, service, sessionId);
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(14, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate1418(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha a kalkuláció jellege végleges, akkor Számított adó * 0,8 
            // - Adóelőleg terhére felajánlott összeg - Feltöltéskor felajánlott összeg
            // ha f29 == "Végleges kalkuláció" => f1412 * 0.8 - f1404 - f1405

            var f1412 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1412).DecimalValue);
            var f1404 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1404).DecimalValue);
            var f1405 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1405).DecimalValue);
            var f29 = service.GetFieldById(29, sessionId).StringValue;

            if (f29 == "Végleges kalkuláció")
                return f1412 * (decimal)0.8 - f1404 - f1405;

            return 0;
        }

        private static decimal? Calculate1417(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha a kalkuláció jellege feltöltés, akkor Számított adó * 0,8 
            // - Adóelőleg terhére felajánlott összeg, különben 0
            // f29 == Feltöltés => f1412 * 0.8 - f1404

            var f1412 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1412).DecimalValue);
            var f1404 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1404).DecimalValue);
            var f29 = service.GetFieldById(29, sessionId).StringValue;

            if (f29 == "Feltöltés")
                return f1412 * (decimal)0.8 - f1404;

            return 0;
        }

        private static decimal? Calculate1416(List<FieldDescriptorDto> fields)
        {
            // TAO előírás szerinti kötelezettség (12.20) + Be nem fizetett, korábban előírt előleg 
            // + 12.20-i feltöltési kötelezettség 
            // - Folyószámlán fennálló túlfizetés, ha ez negatív, akkor 0
            // f1400 + f1402 + f1405 - f1403 ha > 0

            var f1400 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1400).DecimalValue);
            var f1402 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1402).DecimalValue);
            var f1405 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1405).DecimalValue);
            var f1403 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1403).DecimalValue);

            if (f1400 + f1402 + f1405 - f1403 > 0)
                return f1400 + f1402 + f1405 - f1403;

            return 0;
        }

        private static decimal? Calculate1415(List<FieldDescriptorDto> fields)
        {
            // Ha (TAO előírás szerinti kötelezettség (12.20) + Be nem fizetett, korábban előírt előleg) 
            // < (Tárgyévi társasági adó - Már befizetett előleg), akkor Tárgyévi társasági adó -
            // TAO előírás szerinti kötelezettség (12.20) - Már befizetett előleg - 
            // Be nem fizetett, korábban előírt előleg, egyébként 0

            // f1400 + f1402  < f1414 - f1401 => f1414 - f1400 - f1401 - f1402
            var f1400 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1400).DecimalValue);
            var f1402 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1402).DecimalValue);
            var f1414 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1414).DecimalValue);
            var f1401 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1401).DecimalValue);

            if (f1400 + f1402 < f1414 - f1401)
                return f1414 - f1400 - f1401 - f1402;

            return 0;
        }

        private static decimal? Calculate1414(List<FieldDescriptorDto> fields)
        {
            // Számított adó - Társasági adókedvezmények
            // f1412 - f1413

            var f1412 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1412).DecimalValue);
            var f1413 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1413).DecimalValue);
            return f1412 - f1413;
        }

        private static decimal? Calculate1413(IDataService service, Guid sessionId)
        {
            // 3.6.Adókedvezmények összesen
            // f1312
            return GenericCalculations.GetValue(service.GetFieldById(1312, sessionId).DecimalValue);
        }

        private static decimal? Calculate1412(List<FieldDescriptorDto> fields)
        {
            // Adóalap * 0,09
            // f1411 * 0.09

            var f1411 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1411).DecimalValue);

            return f1411 * (decimal)0.09;
        }

        private static decimal? Calculate1411(List<FieldDescriptorDto> fields)
        {
            // Évesre átszámított nyereség + Várható korrekció + Adóalap csökkentő/növelő tételek
            // f1408 + f1409 + f1410

            return GenericCalculations.SumList(fields, new List<int> { 1408, 1409, 1410 });
        }

        private static decimal? Calculate1410(IDataService service, Guid sessionId)
        {
            // Szum (3.4.Növelő) - Szum (3.4.Csökkentő)
            // f1172 - f1171

            var f1172 = GenericCalculations.GetValue(service.GetFieldById(1172, sessionId).DecimalValue);
            var f1171 = GenericCalculations.GetValue(service.GetFieldById(1171, sessionId).DecimalValue);

            return f1172 - f1171;
        }

        private static decimal? Calculate1409(IDataService service, Guid sessionId)
        {
            // 1.1.Összes korrekció
            // f390

            var f390 = GenericCalculations.GetValue(service.GetFieldById(390, sessionId).DecimalValue);
            return f390;
        }

        private static decimal? Calculate1408(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Adózás előtti eredmény / 0.Figyelembe vett hónapok száma * 12
            // f1407 / f5 * 12
            var f5 = service.GetFieldById(5, sessionId).StringValue;
            var f1407 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1407).DecimalValue);

            return f1407 / Convert.ToInt32(f5) * 12;
        }

        private static decimal? Calculate1407(IDataService service, Guid sessionId)
        {
            // 3.1.Figyelembe vett hónapok adózás előtti eredménye
            // f800

            return service.GetFieldById(800, sessionId).DecimalValue; 
        }
    }
}