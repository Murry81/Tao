using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    internal static class GenericCalculations
    {
        internal static decimal GetValue(decimal? value)
        {
            return value.HasValue ? value.Value : 0;
        }

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

        internal static decimal? SumList(List<FieldDescriptorDto> fields)
        {
            decimal result = 0;
            foreach (var field in fields)
            {
                if (field != null && field.DecimalValue.HasValue)
                    result += field.DecimalValue.Value;
            }
            return result;
        }

        internal static Dictionary<int, decimal?> GetValuesById(List<int> fields, IDataService service, Guid sessionId)
        {
            var result = new Dictionary<int, decimal?>();
            var fieldValuList = service.GetFieldsByFieldIdList(fields, sessionId);

            foreach (var field in fieldValuList)
            {
                if (!field.DecimalValue.HasValue)
                    result.Add(field.Id, 0);
                else
                    result.Add(field.Id, field.DecimalValue);
            }

            return result;
        }
    }
}