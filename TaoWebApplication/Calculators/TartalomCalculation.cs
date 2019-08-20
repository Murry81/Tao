using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TartalomCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {

            foreach (var field in fields.OrderBy(s => s.Id))
            {

                if(field.Id == 5)
                {
                    var jellegField = fields.FirstOrDefault(f => f.Id == 29);
                    if(jellegField != null && jellegField.StringValue == "Végleges kalkuláció")
                    {
                        field.StringValue = "12";
                    }
                    continue;
                }

                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 35: // Eltérő üzleti év jelölő
                        {
                            field.BoolFieldValue = Calculate35(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 36: // Előlegfizetési időszak kezdete
                        {
                            field.DateValue = Calculate36(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 37: // Előlegfizetési időszak vége
                        {
                            field.DateValue = Calculate37(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 38: // Első előlegrészlet esedékessége
                        {
                            field.DateValue = Calculate38(fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                    case 39: // Második előlegrészlet esedékessége
                        {
                            field.DateValue = Calculate39(fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                }
            }
        }

        private static bool Calculate35(FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null)
                return false;

            return uzletiEvVegefield.DateValue?.Month != 12 || uzletiEvVegefield.DateValue?.Day != 31;
        }

        private static DateTimeOffset? Calculate36(FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null || !uzletiEvVegefield.DateValue.HasValue)
                return null;

            var result = uzletiEvVegefield.DateValue.Value.AddMonths(7);
            return new DateTimeOffset(result.Year, result.Month, 1, 0, 0, 0, TimeSpan.Zero);
        }

        private static DateTimeOffset? Calculate37(FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null || !uzletiEvVegefield.DateValue.HasValue)
                return null;

            var result = uzletiEvVegefield.DateValue.Value.AddMonths(13);
            return new DateTimeOffset(result.Year, result.Month, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1);
        }

        private static DateTimeOffset? Calculate38(FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null || !fieldFrom.DateValue.HasValue)
                return null;

            var result = fieldFrom.DateValue.Value.AddMonths(2);
            return new DateTimeOffset(result.Year, result.Month, 15, 0, 0, 0, TimeSpan.Zero);
        }

        private static DateTimeOffset? Calculate39(FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null || !fieldFrom.DateValue.HasValue)
                return null;

            var result = fieldFrom.DateValue.Value.AddMonths(8);
            return new DateTimeOffset(result.Year, result.Month, 15, 0, 0, 0, TimeSpan.Zero);
        }
    }
}