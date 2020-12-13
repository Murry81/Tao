using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;

namespace TaoWebApplication.Controllers
{
    public static class DataConverter
    {

        public static void GetTypedValue(FieldDescriptorDto field, List<FieldDescriptorDto> allFields)
        {
            var currentField = allFields.FirstOrDefault(f => f.Id == field.Id);

            switch (field.TypeName)
            {
                case "numeric":
                    field.DecimalValue = currentField.DecimalValue;
                    break;
                case "bool":
                    field.BoolFieldValue = currentField.BoolFieldValue;
                    break;
                case "date":
                    field.DateValue = currentField.DateValue;
                    break;
                default:
                    field.StringValue = currentField.StringValue;
                    break;
            }
        }

        public static string GetTypedValue(FieldDescriptorDto field)
        {
            switch (field.TypeName)
            {
                case "numeric":
                    return field.DecimalValue.HasValue ? field.DecimalValue.Value.ToString("0") : string.Empty;
                case "bool":
                    return field.BoolFieldValue ? "1" : "0";
                case "date":
                    return field.DateValue.HasValue ? field.DateValue.Value.ToString(string.IsNullOrEmpty(field.ExportFormat) ? "yyyyMMdd" : field.ExportFormat ) : string.Empty;
                default:
                    return field.StringValue;
            }
        }
    }
}