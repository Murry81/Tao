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

        public static object GetTypedValue(FieldDescriptorDto field, string value)
        {
            switch (field.TypeName)
            {
                case "bool":
                    return value.Contains("true");
                case "numeric":
                    {
                        if (decimal.TryParse(value, out var decimalValue))
                        {
                            return decimalValue;
                        }
                        return 0;
                    }
                case "date":
                    {
                        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var dateValue))
                        {
                            return dateValue;
                        }
                        return null;
                    }
                default:
                    return value;
            }
        }
    }
}