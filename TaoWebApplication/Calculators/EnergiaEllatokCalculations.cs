using Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class EnergiaEllatokCalculations
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Calculate 1530 and 1531 first
            var f1530 = fields.FirstOrDefault(f => f.Id == 1530);
            f1530.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 1500, 1501, 1502, 1503, 1504, 1505, 1506 });
            var f1531 = fields.FirstOrDefault(f => f.Id == 1531);
            f1531.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 1515, 1516, 1517, 1518, 1519, 1520, 1521 });

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated || field.Id == 1530 || field.Id == 1531)
                    continue;

                switch (field.Id)
                {
                    case 1523: // Adózás előtti eredmény
                        {
                            field.DecimalValue = Calculate1523(service, sessionId);
                            break;
                        }
                    case 1522: // 2019.01-YTD
                        {
                            field.DecimalValue = Calculate1522(service, sessionId);
                            break;
                        }
                    case 1524: // Várható
                        {
                            field.DecimalValue = Calculate1524(fields, service, sessionId);
                            break;
                        }
                    case 1525: // Számított adóalap
                        {
                            field.DecimalValue = Calculate1525(fields);
                            break;
                        }
                    case 1526: // Számított adó
                        {
                            field.DecimalValue = Calculate1526(fields);
                            break;
                        }
                    case 1527: // 2019.12.20-i feltöltési kötelezettség/adókülönbözet
                        {
                            field.DecimalValue = Calculate1527(fields);
                            break;
                        }
                    case 1528: // Pénzügyileg rendezendő
                        {
                            field.DecimalValue = Calculate1528(fields);
                            break;
                        }
                }
            }

            Calculate1532(fields, service, sessionId);
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(15, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static void Calculate1532(List<FieldDescriptorDto> pageFields, IDataService service, Guid sessionId)
        {
            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 1532 }, sessionId);

            var f1526 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 1526).DecimalValue);
            var f1519 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 1519).DecimalValue);
            var f1520 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 1520).DecimalValue);

            // (1420) f1526 – f1519 – f1520
            var f1532 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1532);
            if (f1532 != null)
            {
                f1532.DecimalValue = f1526 - f1519 - f1520;
            }
            else
            {
                f1532 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = f1526 - f1519 - f1520,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1532,
                    SessionId = sessionId
                };
            }

            service.UpdateFieldValues(new List<FieldValueDto> { f1532 }, sessionId);
        }

        private static decimal? Calculate1528(List<FieldDescriptorDto> fields)
        {
            // 2019.12.20-feltöltési kötelezettség + Az adóévre bevallott, de be nem fizetett előleg - Folyószámlán fennálló túlfizetés
            // f1527 + f1520 - f1521

            var f1520 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1520).DecimalValue);
            var f1521 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1521).DecimalValue);
            var f1527 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1527).DecimalValue);

            return f1527 + f1520 - f1521;
        }

        private static decimal? Calculate1527(List<FieldDescriptorDto> fields)
        {
            // Ha Az adóévre bevallott, de nem fizetett előleg < (Számított adó - Az adóévre bevallott és megfizetett előleg),
            // akkor Számított adó - Az adóévre bevallott és megfizetett előleg - Az adóévre bevallott, de be nem fizetett előleg, egyébként 0
            // if f1520 < f1526 - f1519 => f1526 - f1519 - f1520 else 0

            var f1520 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1520).DecimalValue);
            var f1526 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1526).DecimalValue);
            var f1519 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1519).DecimalValue);

            if (f1520 < f1526 - f1519)
                return f1526 - f1519 - f1520;

            return 0;
        }

        private static decimal? Calculate1526(List<FieldDescriptorDto> fields)
        {
            // Számított adóalap * 0,31
            // f1525 * 0.31

            var f1525 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1525).DecimalValue);

            return f1525 * (decimal)0.31;
        }

        private static decimal? Calculate1525(List<FieldDescriptorDto> fields)
        {
            // Adózás előtti eredmény + Szum (Növelő) - Szum (Csökkentő)
            // f1523 + f1530 - f1531
            var f1523 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1523).DecimalValue);
            var f1530 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1530).DecimalValue);
            var f1531 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1531).DecimalValue);

            return f1523 + f1530 - f1531;
        }

        private static decimal? Calculate1524(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 2019.01-YTD / 0.Figyelembe vett hónapok száma * 12
            // f1522 / f5 * 12
           return GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1522).DecimalValue);
        }

        private static decimal? Calculate1522(IDataService service, Guid sessionId)
        {
            // 3.1.Figyelembe vett hónapok adózás előtti eredménye
            // f800

            return GenericCalculations.GetValue(service.GetFieldById(800, sessionId).DecimalValue);
        }

        private static decimal? Calculate1523(IDataService service, Guid sessionId)
        {
            // f2205
            return GenericCalculations.GetValue(service.GetFieldById(2205, sessionId).DecimalValue);
        }
    }
}