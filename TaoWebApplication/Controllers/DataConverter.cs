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
    }
}