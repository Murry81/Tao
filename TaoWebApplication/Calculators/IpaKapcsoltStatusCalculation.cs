using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class IpaKapcsoltStatusCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var requierdFields = service.GetFieldsByFieldIdList(new List<int> { 5, 413, 436 }, sessionId);
            Decimal.TryParse(requierdFields.FirstOrDefault(f => f.Id == 5).StringValue, out var numberOfMouth);
            var f413 = requierdFields.FirstOrDefault(f => f.Id == 413).DecimalValue;
            var f436 = requierdFields.FirstOrDefault(f => f.Id == 436).DecimalValue;

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 608: // Kapcsolt státusz hossza
                        {
                            field.DecimalValue = Calculate608(field, fields);
                            break;
                        }
                    case 609: // Arányosított értékesítés nettó árbevétele
                        {
                            field.DecimalValue = Calculate609(field, fields, numberOfMouth);
                            break;
                        }
                    case 610: // Arányosított ELÁBÉ
                        {
                            field.DecimalValue = Calculate610(field, fields, numberOfMouth);
                            break;
                        }
                    case 611: // Arányosított közvetített szolgáltatások
                        {
                            field.DecimalValue = Calculate611(field, fields, numberOfMouth);
                            break;
                        }
                    case 612: // Arányosított korrigált anyagköltség
                        {
                            field.DecimalValue = Calculate612(field, fields, numberOfMouth);
                            break;
                        }
                    case 613: // Arányosított alvállalkozói teljesítések értéke
                        {
                            field.DecimalValue = Calculate613(field, fields, numberOfMouth);
                            break;
                        }
                    case 614: // Kapcsolt vállalkozásra jutó adóalap
                        {
                            field.DecimalValue = Calculate614(field, fields, f413, f436);
                            break;
                        }
                }
            }
        }

        private static decimal? Calculate608(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields)
        {
            // Kapcsolt státusz vége - Kapcsolt státusz kezdete
            //602-601
            var f601 = fields.FirstOrDefault(f => f.Id == 601 && f.RowIndex == currentField.RowIndex);
            var f602 = fields.FirstOrDefault(f => f.Id == 602 && f.RowIndex == currentField.RowIndex);

            if (f601 == null || f602 == null || !f601.DateValue.HasValue || !f602.DateValue.HasValue)
                return null;

            return (decimal) Math.Ceiling((f602.DateValue - f601.DateValue).Value.TotalDays);
        }

        private static decimal? Calculate609(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? numberOfMouth)
        {
            // Értékesítés nettó árbevétele / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f603 / f5 * 12 / 365 * f608

            if(!numberOfMouth.HasValue)
            {
                return null;
            }

            var f603 = fields.FirstOrDefault(f => f.Id == 603 && f.RowIndex == currentField.RowIndex);
            var f608 = fields.FirstOrDefault(f => f.Id == 608 && f.RowIndex == currentField.RowIndex);

            if (f603 == null || f608 == null || !f603.DecimalValue.HasValue || !f608.DecimalValue.HasValue)
                return null;

            return f603.DecimalValue / numberOfMouth * 12 / 365 * f608.DecimalValue;
        }

        private static decimal? Calculate610(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? numberOfMouth)
        {
            // ELÁBÉ / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f604 / f5 * 12 / 365 * f608

            if (!numberOfMouth.HasValue)
            {
                return null;
            }

            var f604 = fields.FirstOrDefault(f => f.Id == 604 && f.RowIndex == currentField.RowIndex);
            var f608 = fields.FirstOrDefault(f => f.Id == 608 && f.RowIndex == currentField.RowIndex);

            if (f604 == null || f608 == null || !f604.DecimalValue.HasValue || !f608.DecimalValue.HasValue)
                return null;

            return f604.DecimalValue / numberOfMouth * 12 / 365 * f608.DecimalValue;
        }

        private static decimal? Calculate611(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? numberOfMouth)
        {
            // Közvetített szolgáltatások / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f611 / f5 * 12 / 365 * f608

            if (!numberOfMouth.HasValue)
            {
                return null;
            }

            var f605 = fields.FirstOrDefault(f => f.Id == 605 && f.RowIndex == currentField.RowIndex);
            var f608 = fields.FirstOrDefault(f => f.Id == 608 && f.RowIndex == currentField.RowIndex);

            if (f605 == null || f608 == null || !f605.DecimalValue.HasValue || !f608.DecimalValue.HasValue)
                return null;

            return f605.DecimalValue / numberOfMouth * 12 / 365 * f608.DecimalValue;
        }

        private static decimal? Calculate612(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? numberOfMouth)
        {
            // Korrigált anyagköltség / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f606 / f5 * 12 / 365 * f608

            if (!numberOfMouth.HasValue)
            {
                return null;
            }

            var f606 = fields.FirstOrDefault(f => f.Id == 606 && f.RowIndex == currentField.RowIndex);
            var f608 = fields.FirstOrDefault(f => f.Id == 608 && f.RowIndex == currentField.RowIndex);

            if (f606 == null || f608 == null || !f606.DecimalValue.HasValue || !f608.DecimalValue.HasValue)
                return null;

            return f606.DecimalValue / numberOfMouth * 12 / 365 * f608.DecimalValue;
        }

        private static decimal? Calculate613(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? numberOfMouth)
        {
            // Alvállalkozói teljesítések értéke / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f607 / f5 * 12 / 365 * f608

            if (!numberOfMouth.HasValue)
            {
                return null;
            }

            var f607 = fields.FirstOrDefault(f => f.Id == 607 && f.RowIndex == currentField.RowIndex);
            var f608 = fields.FirstOrDefault(f => f.Id == 608 && f.RowIndex == currentField.RowIndex);

            if (f607 == null || f608 == null || !f607.DecimalValue.HasValue || !f608.DecimalValue.HasValue)
                return null;

            return f607.DecimalValue / numberOfMouth * 12 / 365 * f608.DecimalValue;
        }

        private static decimal? Calculate614(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields, decimal? f413, decimal? f436)
        {
            // Korrigált anyagköltség / 0.Figyelembe vett hónapok száma * 12 / 365 * Kapcsolt státusz hossza
            // f607 / f413 * f36

            var f607 = fields.FirstOrDefault(f => f.Id == 607 && f.RowIndex == currentField.RowIndex);
            if (!(f413.HasValue && f436.HasValue && f607 != null && f607.DecimalValue.HasValue && f607.DecimalValue.Value != 0))
            {
                return null;
            }

            return f607.DecimalValue / f413 * f436;
        }
    }
}