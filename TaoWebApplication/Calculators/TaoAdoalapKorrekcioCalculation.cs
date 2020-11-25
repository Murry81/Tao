using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TaoAdoalapKorrekcioCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            foreach (var field in fields)
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 1171: // Összes csökkentő
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields.Where(f => f.SectionGroup == 1 && f.Id != 1171).ToList());
                            break;
                        }
                    case 1172: // Öszes növelő
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields.Where(f => f.SectionGroup == 2 && f.Id != 1172).ToList());
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(11, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }
    }
}