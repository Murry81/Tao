using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TenyadatKorreckcioCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {

            //foreach (var field in fields.OrderBy(s => s.Id))
            //{
            //    if (!field.IsCaculated)
            //        continue;

            //    switch (field.Id)
            //    {
            //        case 62: // Korrigált anyagköltség 
            //            {
            //                field.DecimalValue = Calculate62(field, fields);
            //                break;
            //            }
            //        case 63: // KKV státusz
            //            {
            //                field.BoolFieldValue = Calculate63(field, fields, service, sessionId);
            //                break;
            //            }
            //        case 64: // A cég innovációs járulékra kötelezett
            //            {
            //                field.BoolFieldValue = Calculate64(field, fields);
            //                break;
            //            }

            //    }
            //}
        }
    }
}