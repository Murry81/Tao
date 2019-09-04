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
            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 608: // Eltérő üzleti év jelölő
                        {
                            field.DecimalValue = Calculate608(field, fields);
                            break;
                        }
                    case 609: // Előlegfizetési időszak kezdete
                        {
                            field.DecimalValue = 1;
                            break;
                        }
                    case 610: // Előlegfizetési időszak vége
                        {
                            field.DecimalValue = 2;
                            break;
                        }
                    case 611: // Első előlegrészlet esedékessége
                        {
                            field.DecimalValue = 3;
                            break;
                        }
                    case 612: // Második előlegrészlet esedékessége
                        {
                            field.DecimalValue = 4;
                            break;
                        }
                    case 613: // Második előlegrészlet esedékessége
                        {
                            field.DecimalValue = 5;
                            break;
                        }
                    case 614: // Második előlegrészlet esedékessége
                        {
                            field.DecimalValue = 6;
                            break;
                        }
                }
            }
        }

        private static decimal? Calculate608(FieldDescriptorDto currentField, List<FieldDescriptorDto> fields)
        {
            //602-601
            var f601 = fields.FirstOrDefault(f => f.Id == 601 && f.RowIndex == currentField.RowIndex);
            var f602 = fields.FirstOrDefault(f => f.Id == 602 && f.RowIndex == currentField.RowIndex);

            if (f601 == null || f602 == null || !f601.DateValue.HasValue || !f602.DateValue.HasValue)
                return null;

            return (decimal) Math.Ceiling((f602.DateValue - f601.DateValue).Value.TotalDays);
        }
    }
}