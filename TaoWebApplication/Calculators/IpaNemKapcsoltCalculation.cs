using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class IpaNemKapcsoltCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 413: // Értékesítés nettó árbevétele
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 414: // ELÁBÉ
                        {
                            field.DecimalValue = Calculate414(fields, service, sessionId);
                            break;
                        }
                    case 415: // Közvetített szolgáltatások
                        {
                            field.DecimalValue = Calculate415(fields, service, sessionId);
                            break;
                        }
                    case 416: // Nettó árbevétel csökkentő összeg
                        {
                            field.DecimalValue = Calculate416(fields, service, sessionId);
                            break;
                        }
                    case 417: // Korrigált anyagköltség
                        {
                            field.DecimalValue = Calculate417(fields, service, sessionId);
                            break;
                        }
                    case 418: // Alvállalkozói teljesítések értéke
                        {
                            field.DecimalValue = Calculate418(fields, service, sessionId);
                            break;
                        }
                    case 419: // Árbevétel 500 millióig  
                        {
                            field.DecimalValue = Calculate419(fields, service, sessionId);
                            break;
                        }
                    case 420: //  Árbevétel 500 millió és 20 milliárd között
                        {
                            field.DecimalValue = Calculate420(fields, service, sessionId);
                            break;
                        }
                    case 421: // Árbevétel 20 és 80 milliárd között
                        {
                            field.DecimalValue = Calculate421(fields, service, sessionId);
                            break;
                        }
                    case 422: // Árbevétel 80 milliárd felett 
                        {
                            field.DecimalValue = Calculate422(fields, service, sessionId);
                            break;
                        }
                    case 447: // Összes árbevétel
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 419, 420, 421, 422 });
                            break;
                        }
                    case 423: // Bevételi sávra jutó ELÁBÉ: árbevétel 500 millióig 
                        {
                            field.DecimalValue = Calculate423(fields, service, sessionId);
                            break;
                        }
                    case 424: // Bevételi sávra jutó ELÁBÉ: árbevétel 500 millió és 20 milliárd között 
                        {
                            field.DecimalValue = Calculate424(fields, service, sessionId);
                            break;
                        }
                    case 425: // Bevételi sávra jutó ELÁBÉ: árbevétel 20 és 80 milliárd között  
                        {
                            field.DecimalValue = Calculate425(fields, service, sessionId);
                            break;
                        }
                    case 426: // Bevételi sávra jutó ELÁBÉ: árbevétel 80 milliárd felett 
                        {
                            field.DecimalValue = Calculate426(fields, service, sessionId);
                            break;
                        }
                    case 448:
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 423, 424, 425, 426 });
                            break;
                        }





                    case 427: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 428: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 429: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 430: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 431: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 432: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 433: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 434: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 435: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 436: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 437: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 438: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 439: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 440: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 441: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 442: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 443: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 444: // KKV státusz
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 445: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }
                    case 446: // A cég innovációs járulékra kötelezett
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId);
                            break;
                        }

                }
            }
        }

        private static decimal? GetFigyelembeVettHonapk(IDataService service, Guid sessionId)
        {
            var requiredFields = service.GetFieldsByFieldIdList(new List<int> { 5 }, sessionId);
            var f5 = requiredFields.FirstOrDefault(f => f.Id == 5)?.StringValue;
            if(string.IsNullOrEmpty(f5))
            {
                return null;
            }
            if(int.TryParse(f5, out var result))
            {
                return (decimal)result;
            }
            return null;
        }

        private static Dictionary<int, decimal?> GetBaseValues(List<int> fields, IDataService service, Guid sessionId)
        {
            var result = new Dictionary<int, decimal?>();
            var requiredFields = service.GetFieldsByFieldIdList(fields, sessionId);

            foreach(var field in requiredFields)
            {
                if (!field.DecimalValue.HasValue)
                    return null;

                 result.Add(field.Id, field.DecimalValue);
            }

            return result;
        }

        private static decimal? Calculate413(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //  (1.Értékesítés nettó árbevétele - 1.Jogdíjból származó árbevétel - 1.Árbevételként elszámolt jövedéki, energia- és regisztrációs adó)
            //  / 0.Figyelembe vett hónapok száma * 12 + 1.1.Értékesítés nettó árbevétele)
            // (f13 - f54 - f55) / f5 * 12 + f13
            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GetBaseValues(new List<int> { 13, 54, 55 }, service, sessionId);
            if (values == null)
                return null;

            return (values[13] - values[54] - values[55]) / f5 * 12 + values[13];
        }

        private static decimal? Calculate414(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //1.ELÁBÉ / 0.Figyelembe vett hónapok száma * 12 + 1.1.ELÁBÉ
            // f9 / f5 * 12 + f325

            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GetBaseValues(new List<int> { 9, 325 }, service, sessionId);
            if (values == null)
                return null;

            return values[9] / f5 * 12 + values[325];
        }

        private static decimal? Calculate415(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 1.Közvetített szolgáltatások / 0.Figyelembe vett hónapok száma * 12 + 1.1.Közvetített szolgáltatások
            // f24 / f5 *12 + f326

            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GetBaseValues(new List<int> { 24, 326 }, service, sessionId);
            if (values == null)
                return null;

            return values[24] / f5 * 12 + values[326];
        }

        private static decimal? Calculate416(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // ELÁBÉ + Közvetített szolgáltatások
            // f414 + f415

            var f414 = fields.First(f => f.Id == 414).DecimalValue;
            var f415 = fields.First(f => f.Id == 415).DecimalValue;

            if (!f414.HasValue || !f415.HasValue)
            {
                return null;
            }

            return f414 + f415;
        }

        private static decimal? Calculate417(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 1.Korrigált anyagköltség / 0.Figyelembe vett hónapok száma * 12 + 1.1.Korrigált anyagköltség
            // f62 / f5 * 12 + f329

            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GetBaseValues(new List<int> { 62, 329 }, service, sessionId);
            if (values == null)
                return null;

            return values[62] / f5 * 12 + values[329];
        }

        private static decimal? Calculate418(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 1.Alvállalkozói teljesítmények / 0.Figyelembe vett hónapok száma * 12 + 1.1.Alvállalkozó teljesítmények
            // f25 / f5 * 12 + f327

            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GetBaseValues(new List<int> { 25, 327 }, service, sessionId);
            if (values == null)
                return null;

            return values[25] / f5 * 12 + values[327];
        }

        private static decimal? Calculate419(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Értékesítés nettó árbevétele >= 500.000.000, akkor 500.000.000, egyébként Értékesítés nettó árbevétele

            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f413.HasValue)
            {
                return null;
            }

            if (f413 >= 500000000)
                return 500000000;

            return f413;
        }

        private static decimal? Calculate420(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //  Ha Értékesítés nettó árbevétele >= 20.000.000.000, akkor 19.500.000.000, egyébként Értékesítés nettó árbevétele - 500.000.000

            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f413.HasValue)
            {
                return null;
            }

            if (500000000 > f413)
                return 0;

            if (f413 >= 20000000000)
                return 19500000000;

            return f413 - 500000000;
        }

        private static decimal? Calculate421(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //  Ha Értékesítés nettó árbevétele  >= 80.000.000.000, akkor 60.000.000.000, egyébként Értékesítés nettó árbevétele - 20.000.000.000


            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f413.HasValue)
            {
                return null;
            }

            if (20000000000 > f413)
                return 0;

            if (f413 >= 80000000000)
                return 60000000000;

            return f413 - 20000000000;
        }

        private static decimal? Calculate422(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Értékesítés nettó árbevétele > 80.000.000.000, akkor Értékesítés nettó árbevétele -80.000.000.000, egyébként 0

            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f413.HasValue)
            {
                return null;
            }

            if (f413 >= 80000000000)
                return f413 - 80000000000;

            return 0;
        }

        private static decimal? Calculate423(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 500 millióig <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 500 millióig / Értékesítés nettó árbevétele
            // (f416 - f401) * f419 / f413

            var f416 = fields.First(f => f.Id == 416).DecimalValue;
            var f401 = fields.First(f => f.Id == 401).DecimalValue;
            var f419 = fields.First(f => f.Id == 419).DecimalValue;
            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f401.HasValue || !f416.HasValue || !f419.HasValue || !f413.HasValue)
            {
                return null;
            }

            if (f419 <= 0)
                return 0;

            return (f416 - f401) * f419 / f413;
        }

        private static decimal? Calculate424(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 500 millió és 20 milliárd között <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 500 millió és 20 milliárd között / Értékesítés nettó árbevétele
            // (f416 - f401) * f420 / f413
            var f416 = fields.First(f => f.Id == 416).DecimalValue;
            var f401 = fields.First(f => f.Id == 401).DecimalValue;
            var f420 = fields.First(f => f.Id == 420).DecimalValue;
            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f401.HasValue || !f416.HasValue || !f420.HasValue || !f413.HasValue)
            {
                return null;
            }

            if (f420 <= 0)
                return 0;

            return (f416 - f401) * f420 / f413;
        }

        private static decimal? Calculate425(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 20 és 80 milliárd között <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 20 és 80 milliárd között / Értékesítés nettó árbevétele
            // (f416 - f401) * f421 / f413
            var f416 = fields.First(f => f.Id == 416).DecimalValue;
            var f401 = fields.First(f => f.Id == 401).DecimalValue;
            var f421 = fields.First(f => f.Id == 421).DecimalValue;
            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f401.HasValue || !f416.HasValue || !f421.HasValue || !f413.HasValue)
            {
                return null;
            }

            if (f421 <= 0)
                return 0;

            return (f416 - f401) * f421 / f413;
        }

        private static decimal? Calculate426(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 80 milliárd felett <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 80 milliárd felett / Értékesítés nettó árbevétele
            // (f416 - f401) * f422 / f413
            var f416 = fields.First(f => f.Id == 416).DecimalValue;
            var f401 = fields.First(f => f.Id == 401).DecimalValue;
            var f422 = fields.First(f => f.Id == 422).DecimalValue;
            var f413 = fields.First(f => f.Id == 413).DecimalValue;

            if (!f401.HasValue || !f416.HasValue || !f422.HasValue || !f413.HasValue)
            {
                return null;
            }

            if (f422 <= 0)
                return 0;

            return (f416 - f401) * f422 / f413;
        }
    }
}