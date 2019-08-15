using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TenyadatokCalculation
    {

        private static List<int> calculatedFields = new List<int> { 35, 36, 37, 38, 39 };

        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service)
        {

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 35: // Eltérő üzleti év jelölő
                        {
                            field.BoolFieldValue = Calculate35(field, fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 36: // Előlegfizetési időszak kezdete
                        {
                            field.StringValue = Calculate36(field, fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 37: // Előlegfizetési időszak vége
                        {
                            field.StringValue = Calculate37(field, fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 38: // Első előlegrészlet esedékessége
                        {
                            field.StringValue = Calculate38(field, fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                    case 39: // Második előlegrészlet esedékessége
                        {
                            field.StringValue = Calculate39(field, fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                }
            }
        }

        private static bool Calculate35(FieldDescriptorDto field, FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null)
                return false;

            return ((DateTime)uzletiEvVegefield.StringValue).Month != 12 || ((DateTime)uzletiEvVegefield.StringValue).Day != 31;
        }

        private static DateTime Calculate36(FieldDescriptorDto field, FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null)
                return DateTime.MinValue;

            var result = ((DateTime)uzletiEvVegefield.StringValue).AddMonths(7);
            return new DateTime(result.Year, result.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        private static DateTime Calculate37(FieldDescriptorDto field, FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null)
                return DateTime.MinValue;

            var result = ((DateTime)uzletiEvVegefield.StringValue).AddMonths(13);
            return new DateTime(result.Year, result.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-1);
        }

        private static DateTime Calculate38(FieldDescriptorDto field, FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null)
                return DateTime.MinValue;

            var result = ((DateTime)fieldFrom.StringValue).AddMonths(2);
            return new DateTime(result.Year, result.Month, 15, 0, 0, 0, DateTimeKind.Utc);
        }

        private static DateTime Calculate39(FieldDescriptorDto field, FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null)
                return DateTime.MinValue;

            var result = ((DateTime)fieldFrom.StringValue).AddMonths(8);
            return new DateTime(result.Year, result.Month, 15, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}