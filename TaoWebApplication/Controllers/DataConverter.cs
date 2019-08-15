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

        public static void GetTypedValue(FieldDescriptorDto field, string value)
        {

            switch (field.TypeName)
            {
                case "numeric":
                    {
                        if (decimal.TryParse(value, out var decimalValue))
                        {
                            field.DecimalValue = decimalValue;
                        }
                        else
                        {
                            field.DecimalValue = null;
                        }
                        break;
                    }
                case "bool":
                    if (bool.TryParse(value, out var boolValue))
                    {
                        field.BoolFieldValue = boolValue;
                    }
                    else
                    {
                        field.BoolFieldValue = false;
                    }
                    break;
                case "date":
                    if (DateTimeOffset.TryParse(value, out var dateValue))
                    {
                        field.DateValue = dateValue;
                    }
                    else
                    {
                        field.DateValue = null;
                    }
                    break;
                default:
                    field.StringValue = value;
                    break;
            }
        }
    }
}