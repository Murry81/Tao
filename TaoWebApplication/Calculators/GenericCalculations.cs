using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;

namespace TaoWebApplication.Calculators
{
    internal static class GenericCalculations
    {
        internal static decimal? SumList(List<FieldDescriptorDto> fields, List<int> fieldIds)
        {
            decimal result = 0;
            foreach (var fieldId in fieldIds)
            {
                var field = fields.FirstOrDefault(f => f.Id == fieldId);
                if (field != null && field.DecimalValue.HasValue)
                    result += field.DecimalValue.Value;
            }
            return result;
        }
    }
}