using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;

namespace TaoWebApplication.Validators
{
    public class IpaKapcsoltStatusValidator : IValidator
    {
        public static Dictionary<int, string> Validate(List<FieldDescriptorDto> fields)
        {
            var rows = fields.Select(f => f.RowIndex).Distinct();
            var result = new Dictionary<int, string>();
            foreach (var rowIndex in rows)
            {            
                var f601 = fields.FirstOrDefault(f => f.Id == 601 && f.RowIndex == rowIndex);

                if (f601 == null || !f601.DateValue.HasValue)
                {
                    result.Add(31, "A kezdő időpontnak léteznie kell.");
                }

                if (f601.DateValue < DateTime.UtcNow.AddYears(-2))
                {
                    result.Add(31, "A kezdő időpont nem leeht 2 évnél régebbi.");
                }

                var f602 = fields.FirstOrDefault(f => f.Id == 602 && f.RowIndex == rowIndex);

                if (f602 == null || !f602.DateValue.HasValue)
                {
                    result.Add(32, "A záró időpontnak léteznie kell.");
                }

                if (f601 != null && f601.DateValue.HasValue && f602 != null && f602.DateValue.HasValue && f602.DateValue < f601.DateValue )
                {
                    result.Add(602, "A kezdő időpontnak korábbinak kell lennie mint a zárónak.");
                }
                
            }
            return result;
        }
    }
}