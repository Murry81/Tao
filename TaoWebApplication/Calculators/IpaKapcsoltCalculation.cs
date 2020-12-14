using Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class IpaKapcsoltCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var customer = service.GetCustomer(int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString()));
            decimal arfolyam = GetArfolyamSzorzo(service, sessionId, customer);

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 413: // Értékesítés nettó árbevétele
                        {
                            field.DecimalValue = Calculate413(fields, service, sessionId, arfolyam);
                            break;
                        }
                    case 414: // ELÁBÉ
                        {
                            field.DecimalValue = Calculate414(fields, service, sessionId, arfolyam);
                            break;
                        }
                    case 415: // Közvetített szolgáltatások
                        {
                            field.DecimalValue = Calculate415(fields, service, sessionId, arfolyam);
                            break;
                        }
                    case 416: // Nettó árbevétel csökkentő összeg
                        {
                            field.DecimalValue = Calculate416(fields, service, sessionId);
                            break;
                        }
                    case 417: // Korrigált anyagköltség
                        {
                            field.DecimalValue = Calculate417(fields, service, sessionId, arfolyam);
                            break;
                        }
                    case 418: // Alvállalkozói teljesítések értéke
                        {
                            field.DecimalValue = Calculate418(fields, service, sessionId, arfolyam);
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
                    case 448: // Beviteli sávra jutó szumma
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 423, 424, 425, 426 });
                            break;
                        }
                    case 427: // Korlátos ELÁBÉ: árbevétel 500 millióig
                        {
                            field.DecimalValue = Calculate427(fields, service, sessionId);
                            break;
                        }
                    case 428: // Korlátos ELÁBÉ: árbevétel 500 millió és 20 milliárd között  
                        {
                            field.DecimalValue = Calculate428(fields, service, sessionId);
                            break;
                        }
                    case 429: // Korlátos ELÁBÉ: árbevétel 20 és 80 milliárd között 
                        {
                            field.DecimalValue = Calculate429(fields, service, sessionId);
                            break;
                        }
                    case 430: // Korlátos ELÁBÉ: árbevétel 80 milliárd felett 
                        {
                            field.DecimalValue = Calculate430(fields, service, sessionId);
                            break;
                        }
                    case 449: //  Korlátos ELÁBÉ  szumma
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 427, 428, 429, 430 });
                            break;
                        }
                    case 431: // Figyelembe vehető ELÁBÉ: árbevétel 500 millióig  
                        {
                            field.DecimalValue = Calculate431(fields, service, sessionId);
                            break;
                        }
                    case 432: // Figyelembe vehető ELÁBÉ: árbevétel 500 millió és 20 milliárd között 
                        {
                            field.DecimalValue = Calculate432(fields, service, sessionId);
                            break;
                        }
                    case 433: // Figyelembe vehető ELÁBÉ: árbevétel 20 és 80 milliárd között 
                        {
                            field.DecimalValue = Calculate433(fields, service, sessionId);
                            break;
                        }
                    case 434: // Figyelembe vehető ELÁBÉ: árbevétel 80 milliárd felett  
                        {
                            field.DecimalValue = Calculate434(fields, service, sessionId);
                            break;
                        }
                    case 435: // Összes figyelembe vehető ELÁBÉ
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 431, 432, 433, 434 });
                            break;
                        }
                }
            }

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {

                    case 436: // Iparűzési adó alapja
                        {
                            field.DecimalValue = Calculate436(fields, service, sessionId);
                            break;
                        }
                    case 450: // Adóalanyra jutó adóalap
                        {
                            field.DecimalValue = Calculate450(fields, service, sessionId, arfolyam);
                            break;
                        }
                    case 451: //Kapcsolt vállalkozásokra jutó adóalap
                        {
                            field.DecimalValue = Calculate451(fields, service, sessionId);
                            break;
                        }
                    case 437: // Iparűzési adó mértéke
                        {
                            field.DecimalValue = Calculate437(fields, service, sessionId);
                            break;
                        }
                    case 438: // Iparűzési adó összege
                        {
                            field.DecimalValue = Calculate438(fields, service, sessionId);
                            break;
                        }
                    case 439: // Megfizetett útdíj 7,5%-a
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 409, 410, 411 });
                            break;
                        }
                    case 440: // Iparűzési adó csökkentett összege
                        {
                            field.DecimalValue = Calculate440(fields, service, sessionId);
                            break;
                        }
                    case 441: // Összes be nem fizetett, korábban előírt előleg
                        {
                            field.DecimalValue = Calculate441(fields, service, sessionId);
                            break;
                        }
                    case 442: // Összes befizetett előleg
                        {
                            field.DecimalValue = Calculate442(fields, service, sessionId);
                            break;
                        }
                    case 443: // Összes folyószámlán fennálló túlfizetés
                        {
                            field.DecimalValue = Calculate443(fields, service, sessionId);
                            break;
                        }
                    case 444: // Összes feltöltési kötelezettség/Adókülönbözet
                        {
                            field.DecimalValue = Calculate444(fields, service, sessionId);
                            break;
                        }
                    case 445: // Összes pénzügyileg rendezendő 
                        {
                            field.DecimalValue = Calculate445(fields, service, sessionId);
                            break;
                        }
                    case 446: // Alapkutatás, alkalmazott kutatás, kísérleti fejlesztés 10%-ából az adóalapcsökkentőként figyelembe vett összeg
                        {
                            field.DecimalValue = Calculate446(fields, service, sessionId);
                            break;
                        }
                }
            }

            CalculateExtraFields(service, sessionId, fields);
        }

        private static void CalculateExtraFields(IDataService service, Guid sessionId, List<FieldDescriptorDto> fields)
        {
            // (460) Új mező:  436 - 400 - 405
            // (461) Uj mező 2: (9 + 24 + 13 * árfolyam) =< 500.000.000 : 401 + 435
            // (462) Új mező 3: 13 * árfolyam > 500000000 : 435

            var calculatedFields = service.GetFieldValuesByFieldIdList(new List<int> { 460, 461, 462}, sessionId);

            var f436Value = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 436).DecimalValue);
            var f400Value = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 400).DecimalValue);
            var f405Value = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 405).DecimalValue);
            var f401Value = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 401).DecimalValue);
            var f435Value = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 435).DecimalValue);

            var f9Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 9 }, sessionId).FirstOrDefault().DecimalValue);
            var f24Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 24 }, sessionId).FirstOrDefault().DecimalValue);
            var f13Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 13 }, sessionId).FirstOrDefault().DecimalValue);

            var customer = service.GetCustomer(int.Parse(HttpContext.Current.Session["CustomerId"].ToString()));
            decimal arfolyam = IpaKapcsoltCalculation.GetArfolyamSzorzo(service, sessionId, customer);

            var f460 = calculatedFields.FirstOrDefault(f => f.FieldDescriptorId == 460);
            if (f460 != null)
            {
                f460.DecimalValue = f436Value - f400Value - f405Value;
            }
            else
            {
                f460 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = f436Value - f400Value - f405Value,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 460,
                    SessionId = sessionId
                };
            }

            var value = (f9Value + f24Value + f13Value) * arfolyam <= 500000000 ? f401Value + f435Value : 0;
            var f461 = calculatedFields.FirstOrDefault(f => f.FieldDescriptorId == 461);
            if (f461 != null)
            {
                f461.DecimalValue = value;
            }
            else
            {
                f461 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = value,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 461,
                    SessionId = sessionId
                };
            }

            value = f13Value * arfolyam > 500000000 ? f435Value : 0;
            var f462 = calculatedFields.FirstOrDefault(f => f.FieldDescriptorId == 462);
            if (f462 != null)
            {
                f462.DecimalValue = value;
            }
            else
            {
                f462 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = value,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 462,
                    SessionId = sessionId
                };
            }

            service.UpdateFieldValues(new List<FieldValueDto> { f460, f461, f462 }, sessionId);
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(5, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate446(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum (2.4.Kutatási kedvezmény)
            // sum(f1821)

            return GenericCalculations.SumList(service.GetFieldValuesById(1821, sessionId).ToList());
        }

        private static decimal? Calculate445(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum (2.4.Összes pénzügyileg rendezendő)
            // sum(f1827)

            return GenericCalculations.SumList(service.GetFieldValuesById(1827, sessionId).ToList());
        }

        private static decimal? Calculate444(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum (2.4.Feltöltési kötelezettség/Adókülönbözet)
            // sum(f1826)

            return GenericCalculations.SumList(service.GetFieldValuesById(1826, sessionId).ToList());
        }

        private static decimal? Calculate443(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum (2.4.Folyószámlán fennálló túlfizetés)
            // sum(f1825)

            return GenericCalculations.SumList(service.GetFieldValuesById(1825, sessionId).ToList());
        }

        private static decimal? Calculate442(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //	Szum (2.4.Befizetett előleg)
            // sum(1824)
            return GenericCalculations.SumList(service.GetFieldValuesById(1824, sessionId).ToList());
        }

        private static decimal? Calculate441(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum (2.4.Be nem fizetett, korábban előírt előleg)
            // szum(1823)

            return GenericCalculations.SumList(service.GetFieldValuesById(1823, sessionId).ToList());
        }

        private static decimal? Calculate440(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 	Szum (2.4.Települési adó összege)
            // Sum(f1822)

            return GenericCalculations.SumList(service.GetFieldValuesById(1822, sessionId).ToList());
        }

        private static decimal? Calculate451(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Szum(2.2.1.Kapcsolt vállalkozásra jutó adóalap)
            // sum(f614)

            return GenericCalculations.SumList(service.GetFieldValuesById(614, sessionId).ToList());
        }

        private static decimal? Calculate438(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // sum Adó kedvezmények nélkül
            // sum(1812)

            return GenericCalculations.SumList(service.GetFieldValuesById(1812, sessionId).ToList());
        }

        private static decimal? Calculate437(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //	Ha csak egy település van rögzítve a 2.4 lapon, akkor az ahhoz tartozó adómérték, egyébként inaktív és üres
            var adomertek = service.GetFieldValuesById(1802, sessionId).ToList();
            if (adomertek.Count == 1)
                return adomertek.FirstOrDefault().DecimalValue;

            return null;
        }

        internal static decimal GetArfolyamSzorzo(IDataService service, Guid sessionId, CustomerDto customer)
        {
            if (customer.KonyvelesPenzneme.ISO != "HUF")
            {
                var field = service.GetFieldsByFieldIdList(new List<int> { 34 }, sessionId).First();
                if (field.DecimalValue.HasValue)
                    return field.DecimalValue.Value;

                return 1;
            }
            return 1;
        }

        private static decimal? GetFigyelembeVettHonapk(IDataService service, Guid sessionId)
        {
            var requiredFields = service.GetFieldsByFieldIdList(new List<int> { 5 }, sessionId);
            var f5 = requiredFields.FirstOrDefault(f => f.Id == 5)?.StringValue;
            if (string.IsNullOrEmpty(f5))
            {
                return null;
            }
            if (int.TryParse(f5, out var result))
            {
                return (decimal)result;
            }
            return null;
        }



        private static decimal? Calculate413(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            //  (1.Értékesítés nettó árbevétele - 1.Jogdíjból származó árbevétel - 1.Árbevételként elszámolt jövedéki, energia- és regisztrációs adó) *arfolyam
            //  / 0.Figyelembe vett hónapok száma * 12 + 1.1.Értékesítés nettó árbevétele *arfolyam
            // (f13 - f54 - f55) *arfolyam / f5 * 12 + f13 *arfolyam
            var f5 = GetFigyelembeVettHonapk(service, sessionId);

            var values = GenericCalculations.GetValuesById(new List<int> { 13, 54, 55 }, service, sessionId);

            return (values[13] - values[54] - values[55]) * arfolyam / f5 * 12 + values[13] * arfolyam;
        }

        private static decimal? Calculate414(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            //1.ELÁBÉ / 0.Figyelembe vett hónapok száma * 12 + 1.1.ELÁBÉ
            // f9 * arfolyam / f5 * 12 + f325 * arfolyam

            var f5 = GetFigyelembeVettHonapk(service, sessionId);
            var values = GenericCalculations.GetValuesById(new List<int> { 9, 325 }, service, sessionId);
            return values[9] * arfolyam / f5 * 12 + values[325] * arfolyam;
        }

        private static decimal? Calculate415(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            // 1.Közvetített szolgáltatások * arfolyam / 0.Figyelembe vett hónapok száma * 12 + 1.1.Közvetített szolgáltatások * arfolyam
            // f24 / f5 *12 + f326 

            var f5 = GetFigyelembeVettHonapk(service, sessionId);
            var values = GenericCalculations.GetValuesById(new List<int> { 24, 326 }, service, sessionId);
            return values[24] * arfolyam / f5 * 12 + values[326] * arfolyam;
        }

        private static decimal? Calculate416(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // ELÁBÉ + Közvetített szolgáltatások
            // f414 + f415

            var f414 = GenericCalculations.GetValue(fields.First(f => f.Id == 414).DecimalValue);
            var f415 = GenericCalculations.GetValue(fields.First(f => f.Id == 415).DecimalValue);

            if (f414 == 0 && f415 == 0)
            {
                return null;
            }

            return f414 + f415;
        }

        private static decimal? Calculate417(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            // 1.Korrigált anyagköltség  * arfolyam / 0.Figyelembe vett hónapok száma * 12 + 1.1.Korrigált anyagköltség * arfolyam
            // f62 * arfolyam / f5 * 12 + f329 * arfolyam

            var f5 = GetFigyelembeVettHonapk(service, sessionId);
            var values = GenericCalculations.GetValuesById(new List<int> { 62, 329 }, service, sessionId);
            return values[62] * arfolyam / f5 * 12 + values[329] * arfolyam;
        }

        private static decimal? Calculate418(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            // 1.Alvállalkozói teljesítmények * arfolyam / 0.Figyelembe vett hónapok száma * 12 + 1.1.Alvállalkozó teljesítmények * arfolyam
            // f25 * arfolyam / f5 * 12 + f327 * arfolyam

            var f5 = GetFigyelembeVettHonapk(service, sessionId);
            var values = GenericCalculations.GetValuesById(new List<int> { 25, 327 }, service, sessionId);
            return values[25] * arfolyam / f5 * 12 + values[327] * arfolyam;
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
                return null;

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
                return null;

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

            return null;
        }

        private static decimal? Calculate423(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 500 millióig <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 500 millióig / Értékesítés nettó árbevétele
            // (f416 - f401) * f419 / f413

            var f416 = GenericCalculations.GetValue(fields.First(f => f.Id == 416).DecimalValue);
            var f401 = GenericCalculations.GetValue(fields.First(f => f.Id == 401).DecimalValue);
            var f419 = GenericCalculations.GetValue(fields.First(f => f.Id == 419).DecimalValue);
            var f413 = GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue);

            if (f413 == 0 || (f401 == 0 && f416 == 0 && f419 == 0))
            {
                return null;
            }

            if (f419 <= 0)
                return null;

            return (f416 - f401) * f419 / f413;
        }

        private static decimal? Calculate424(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 500 millió és 20 milliárd között <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 500 millió és 20 milliárd között / Értékesítés nettó árbevétele
            // (f416 - f401) * f420 / f413
            var f416 = GenericCalculations.GetValue(fields.First(f => f.Id == 416).DecimalValue);
            var f401 = GenericCalculations.GetValue(fields.First(f => f.Id == 401).DecimalValue);
            var f420 = GenericCalculations.GetValue(fields.First(f => f.Id == 420).DecimalValue);
            var f413 = GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue);

            if (f413 == 0 || (f401 == 0 && f416 == 0 && f420 == 0))
            {
                return null;
            }

            if (f420 <= 0)
                return null;

            return (f416 - f401) * f420 / f413;
        }

        private static decimal? Calculate425(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 20 és 80 milliárd között <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 20 és 80 milliárd között / Értékesítés nettó árbevétele
            // (f416 - f401) * f421 / f413
            var f416 = GenericCalculations.GetValue(fields.First(f => f.Id == 416).DecimalValue);
            var f401 = GenericCalculations.GetValue(fields.First(f => f.Id == 401).DecimalValue);
            var f421 = GenericCalculations.GetValue(fields.First(f => f.Id == 421).DecimalValue);
            var f413 = GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue);

            if (f413 == 0 || (f401 == 0 && f416 == 0 && f421 == 0))
            {
                return null;
            }

            if (f421 <= 0)
                return null;

            return (f416 - f401) * f421 / f413;
        }

        private static decimal? Calculate426(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Árbevétel 80 milliárd felett <= 0, akkor 0, egyébként 
            // (Nettó árbevétel csökkentő összeg - Export árbevételhez kapcsolódó ELÁBÉ és közvetített szolgáltatás) * Árbevétel 80 milliárd felett / Értékesítés nettó árbevétele
            // (f416 - f401) * f422 / f413
            var f416 = GenericCalculations.GetValue(fields.First(f => f.Id == 416).DecimalValue);
            var f401 = GenericCalculations.GetValue(fields.First(f => f.Id == 401).DecimalValue);
            var f422 = GenericCalculations.GetValue(fields.First(f => f.Id == 422).DecimalValue);
            var f413 = GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue);

            if (f413 == 0 || (f401 == 0 && f416 == 0 && f422 == 0))
            {
                return null;
            }

            if (f422 <= 0)
                return null;

            return (f416 - f401) * f422 / f413;
        }

        private static decimal? Calculate427(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Bevételi sávra jutó ELÁBÉ: árbevétel 500 millióig
            // (f419)

            return fields.First(f => f.Id == 419).DecimalValue;
        }

        private static decimal? Calculate428(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Árbevétel 500 millió és 20 milliárd között * 0,85
            // (f420) * 0.85

            return fields.First(f => f.Id == 420).DecimalValue.HasValue ? fields.First(f => f.Id == 420).DecimalValue.Value * (decimal)0.85 : (decimal?)null;
        }

        private static decimal? Calculate429(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Árbevétel 20 és 80 milliárd között * 0,75
            // (f421) * 0.75

            return fields.First(f => f.Id == 421).DecimalValue.HasValue ? fields.First(f => f.Id == 421).DecimalValue.Value * (decimal)0.85 : (decimal?)null;
        }

        private static decimal? Calculate430(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Árbevétel 20 és 80 milliárd között * 0,75
            // (f422) * 0.7

            return fields.First(f => f.Id == 422).DecimalValue.HasValue ? fields.First(f => f.Id == 422).DecimalValue.Value * (decimal)0.7 : (decimal?)null;
        }


        private static decimal? Calculate431(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Min(Bevételi sávra jutó ELÁBÉ: árbevétel 500 millióig, Korlátos ELÁBÉ: árbevétel 500 millióig)
            // Min(f423, f427)

            var firstValue = fields.First(f => f.Id == 423).DecimalValue;
            var secondValue = fields.First(f => f.Id == 427).DecimalValue;

            if (!firstValue.HasValue || !secondValue.HasValue)
                return null;

            return Math.Min(GenericCalculations.GetValue(firstValue), GenericCalculations.GetValue(secondValue));
        }

        private static decimal? Calculate432(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Min(Bevételi sávra jutó ELÁBÉ: árbevétel 500 millió és 20 milliárd között, Korlátos ELÁBÉ: árbevétel 500 millió és 20 milliárd között)
            // Min(f424, f428)

            var firstValue = fields.First(f => f.Id == 424).DecimalValue;
            var secondValue = fields.First(f => f.Id == 428).DecimalValue;

            if (!firstValue.HasValue || !secondValue.HasValue)
                return null;

            return Math.Min(GenericCalculations.GetValue(firstValue), GenericCalculations.GetValue(secondValue));
        }

        private static decimal? Calculate433(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Min(Bevételi sávra jutó ELÁBÉ: árbevétel 20 és 80 milliárd között, Korlátos ELÁBÉ: árbevétel 20 és 80 milliárd között)
            // Min(f425, f429)

            var firstValue = fields.First(f => f.Id == 425).DecimalValue;
            var secondValue = fields.First(f => f.Id == 429).DecimalValue;

            if (!firstValue.HasValue || !secondValue.HasValue)
                return null;

            return Math.Min(GenericCalculations.GetValue(firstValue), GenericCalculations.GetValue(secondValue));
        }

        private static decimal? Calculate434(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Min(Bevételi sávra jutó ELÁBÉ: árbevétel 80 milliárd felett, Korlátos ELÁBÉ: árbevétel 80 milliárd felett)
            // Min(f426, f430)

            var firstValue = fields.First(f => f.Id == 426).DecimalValue;
            var secondValue = fields.First(f => f.Id == 430).DecimalValue;

            if (!firstValue.HasValue || !secondValue.HasValue)
                return null;

            return Math.Min(GenericCalculations.GetValue(firstValue), GenericCalculations.GetValue(secondValue));
        }


        private static decimal? Calculate436(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Értékesítés nettó árbevétele + Transzferár korrekció + IFRS korrekció - Összes figyelembe vehető ELÁBÉ - Korrigált anyagköltség - Alvállalkozói teljesítések értéke, 
            // ha ez negatív, akkor 0
            // Max(0, f413 + f400 + f405 - f435 - f417 - f418 -f403 -f407 + f406)

            return Math.Max(0,
                GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue) +
                GenericCalculations.GetValue(fields.First(f => f.Id == 400).DecimalValue) +
                GenericCalculations.GetValue(fields.First(f => f.Id == 405).DecimalValue) -
                GenericCalculations.GetValue(fields.First(f => f.Id == 435).DecimalValue) -
                GenericCalculations.GetValue(fields.First(f => f.Id == 417).DecimalValue) -
                GenericCalculations.GetValue(fields.First(f => f.Id == 418).DecimalValue) -
                GenericCalculations.GetValue(fields.First(f => f.Id == 403).DecimalValue) -
                GenericCalculations.GetValue(fields.First(f => f.Id == 407).DecimalValue) +
                GenericCalculations.GetValue(fields.First(f => f.Id == 406).DecimalValue));
        }


        private static decimal? Calculate450(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId, decimal arfolyam)
        {
            // (1.Értékesítés nettó árbevétele - 1.Jogdíjból származó árbevétel - 1.Árbevételként elszámolt jövedéki, energia- és regisztrációs adó) *arfolyam / 
            // 0.Figyelembe vett hónapok száma * 12 + 1.1.Értékesítés nettó árbevétele * arfolyam / Értékesítés nettó árbevétele * Iparűzési adó együttes alapja
            // (f13 - f54 - f55) *arfolyam / f5 * 12 + f13 *arfolyam / f413 * f436
                        
            var f413 = GenericCalculations.GetValue(fields.First(f => f.Id == 413).DecimalValue);
            if (f413 == 0)
                return null;

            var f436 = GenericCalculations.GetValue(fields.First(f => f.Id == 436).DecimalValue);
            var f5 = GetFigyelembeVettHonapk(service, sessionId);
            var values = GenericCalculations.GetValuesById(new List<int> { 13, 54, 55 }, service, sessionId);

            return (values[13] - values[54] - values[55]) * arfolyam / f5 * 12 + values[13] * arfolyam / f413 * f436;
        }
    }
}