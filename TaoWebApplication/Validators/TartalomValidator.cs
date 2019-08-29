using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;

namespace TaoWebApplication.Validators
{
    public class TartalomValidator : IValidator
    {
        public static Dictionary<int, string> Validate(List<FieldDescriptorDto> fields)
        {
            var result = new Dictionary<int, string>();
            var f31 = fields.FirstOrDefault(f => f.Id == 31);

            if(f31 == null || !f31.DateValue.HasValue)
            {
                result.Add(31, "A kezdő időpontnak léteznie kell.");
            }

            if(f31.DateValue < DateTime.UtcNow.AddYears(-2))
            {
                result.Add(31, "A kezdő időpont nem leeht 2 évnél régebbi.");
            }

            var f32 = fields.FirstOrDefault(f => f.Id == 32);

            if (f32 == null || !f32.DateValue.HasValue)
            {
                result.Add(32, "A kezdő időpontnak léteznie kell.");
            }

            if (f31 != null && f31.DateValue.HasValue && f32 != null && f32.DateValue.HasValue && f32.DateValue - f31.DateValue > TimeSpan.FromDays(365))
            {
                result.Add(32, "Az intervallum maximum 1 év lehet.");
            }
            return result;
        }
    }
}