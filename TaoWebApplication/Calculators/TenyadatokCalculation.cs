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
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 62: // Korrigált anyagköltség 
                        {
                            field.DecimalValue = Calculate62(fields);
                            break;
                        }
                    case 63: // KKV státusz
                        {
                            field.BoolFieldValue = Calculate63(fields, service, sessionId);
                            break;
                        }
                    case 64: // A cég innovációs járulékra kötelezett
                        {
                            field.BoolFieldValue = Calculate64(fields);
                            break;
                        }
                   
                }
            }
        }

        private static bool Calculate64(List<FieldDescriptorDto> fields)
        {
            if (fields.FirstOrDefault(f => f.Id == 63).BoolFieldValue)
                return true;

            if (fields.FirstOrDefault(f => f.Id == 58).BoolFieldValue)
                return true;

            if (fields.FirstOrDefault(f => f.Id == 59).BoolFieldValue)
                return true;

            if (fields.FirstOrDefault(f => f.Id == 60).BoolFieldValue)
                return true;

            return false;
        }

        private static bool Calculate63(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            if (fields.FirstOrDefault(f => f.Id == 56).DecimalValue < 50)
                return true;

            var elozoEviArfolyam = service.GetFieldsByFieldIdList(new List<int> { 34 }, sessionId).First();
            if(elozoEviArfolyam != null && elozoEviArfolyam.DecimalValue.HasValue && elozoEviArfolyam.DecimalValue.Value != 0)
            {
                var value = fields.FirstOrDefault(f => f.Id == 11).DecimalValue / elozoEviArfolyam.DecimalValue;
                if (value < 10000000)
                    return true;

                value = fields.FirstOrDefault(f => f.Id == 12).DecimalValue / elozoEviArfolyam.DecimalValue;
                if (value < 10000000)
                    return true;
            }
            return false;
        }

        private static decimal? Calculate62(List<FieldDescriptorDto> fields)
        {
            var anyagkoltseg = fields.FirstOrDefault(f => f.Id == 26).DecimalValue;
            var notAnyagkoltseg = fields.FirstOrDefault(f => f.Id == 10).DecimalValue;
            if (anyagkoltseg.HasValue && notAnyagkoltseg.HasValue)
                return anyagkoltseg - notAnyagkoltseg;

            return null;
        }
    }
}