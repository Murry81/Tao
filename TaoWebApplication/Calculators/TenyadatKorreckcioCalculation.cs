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

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 390: // Összes korrekció
                        {
                            field.DecimalValue = Calculate390(fields);
                            break;
                        }
                    case 380: // Bevételt módosító tényezők
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 301, 302, 307 });
                            break;
                        }
                    case 302: // Egyéb bevételek
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 303, 304, 305, 306 });
                            break;
                        }
                    case 307: // Pénzügyi műveletek bevétele
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 308, 309, 310, 311 });
                            break;
                        }
                    case 381: // Költséget, ráfordítást, módosító tényezők
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 312, 321, 325, 326, 327, 328, 329, 332, 333, 334, 335 });
                            break;
                        }
                    case 312: // Egyéb ráfordítás
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 313, 314, 315, 316, 317, 318, 319, 320 });
                            break;
                        }
                    case 321: // Pénzügyi műveletek ráfordítása
                        {
                            field.DecimalValue = GenericCalculations.SumList(fields, new List<int> { 322, 323, 324 });
                            break;
                        }
                    case 329: // Egyéb bevételek
                        {
                            field.DecimalValue = Calculate329(fields);
                            break;
                        }

                }
            }
        }

        private static decimal? Calculate329(List<FieldDescriptorDto> fields)
        {
            var anyagkoltseg = fields.FirstOrDefault(f => f.Id == 330);
            if (anyagkoltseg != null && anyagkoltseg.DecimalValue.HasValue)
            {
                var notkoltseg = fields.FirstOrDefault(f => f.Id == 331);
                if (notkoltseg != null && notkoltseg.DecimalValue.HasValue)
                {
                    return anyagkoltseg.DecimalValue - notkoltseg.DecimalValue;
                }
                return anyagkoltseg.DecimalValue;
            }
            return null;
        }

        private static decimal? Calculate390(List<FieldDescriptorDto> fields)
        {
            var bevetel = fields.FirstOrDefault(f => f.Id == 380);
            if (bevetel != null && bevetel.DecimalValue.HasValue)
            {
                var koltseg = fields.FirstOrDefault(f => f.Id == 381);
                if(koltseg != null && koltseg.DecimalValue.HasValue)
                {
                    return bevetel.DecimalValue - koltseg.DecimalValue;
                }
                return bevetel.DecimalValue;
            }
            return null;
        }
    }
}