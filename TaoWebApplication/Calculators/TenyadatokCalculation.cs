using Contracts.Contracts;
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
                    case 3113: // Évesített árbevétel
                        {
                            field.DecimalValue = Calculate3113(service, sessionId, fields);
                            break;
                        }

                    case 3040: // Adózás előtti eredmény 
                        {
                            field.DecimalValue = Calculate3040(fields);
                            break;
                        }
                    case 3112: // Belföldi létszám
                        {
                            field.DecimalValue = Calculate3112(fields);
                            break;
                        }
                }
            }

            CalculateExtraFields(service, sessionId);
        }

        private static decimal? Calculate3112(List<FieldDescriptorDto> fields)
        {
            // f57-f3111
            var f57Value = fields.FirstOrDefault(f => f.Id == 57)?.DecimalValue;
            var f3111Value = fields.FirstOrDefault(f => f.Id == 3111)?.DecimalValue;

            return f57Value - f3111Value;
        }

        private static decimal? Calculate3040(List<FieldDescriptorDto> fields)
        {
            // 13+3013+14 -26-3015-3016-9-24-3019-3020-3021-3022-3023-3028-3029+18-3035-440-705
            var pos = GenericCalculations.SumList(fields.Where(f => new[] { 13, 3013, 14, 18 }.Contains(f.Id)).ToList());
            var minus = GenericCalculations.SumList(fields.Where(f => new[] { 26, 3015, 3016, 9, 24, 3019, 3020, 3021, 3022, 3023, 3028, 3029, 3035, 440, 705 }.Contains(f.Id)).ToList());

            return pos - minus;
        }

        private static decimal? Calculate3113(IDataService service, Guid sessionId, List<FieldDescriptorDto> pageFields)
        {
            // (31)Üzleti év kezdete
            // (32)Üzleti év vége
            // Ha az üzleti év nincs teljes év(365 vagy 366 nap), akkor = 13 / Napok száma * 365(366)

            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 31, 32 }, sessionId);
            var f13Value = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 13)?.DecimalValue);
            var f31Value = fields.FirstOrDefault(f => f.FieldDescriptorId == 31)?.DateValue;
            var f32Value = fields.FirstOrDefault(f => f.FieldDescriptorId == 32)?.DateValue;
            
            if (!f31Value.HasValue || !f32Value.HasValue)
                return null;

            var dayCount = GenericCalculations.CalculateDayCount(f32Value, f31Value) + 1;
            var effectiveDayCount = 365;

            effectiveDayCount = (f31Value.Value.Month < 3 && DateTime.IsLeapYear(f31Value.Value.Year)) || (f32Value.Value.Month > 2 && DateTime.IsLeapYear(f32Value.Value.Year)) ? 366 : 355;

            if(dayCount != effectiveDayCount)
            {
                return f13Value / dayCount * effectiveDayCount;
            }

            return f13Value;
        }

        private static void CalculateExtraFields(IDataService service, Guid sessionId)
        {

            // (70) (13) - (54) - (55) * árfolyam
            // (81) 13 * arfolyam
            // (82) 54 * arfolyam
            // (83) 55 * Netto arbevarfolyam

            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 70, 81, 82, 83 }, sessionId);

            var f13Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 13 }, sessionId).FirstOrDefault().DecimalValue);
            var f54Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 54 }, sessionId).FirstOrDefault().DecimalValue);
            var f55Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 55 }, sessionId).FirstOrDefault().DecimalValue);

            var customer = service.GetCustomer(int.Parse(HttpContext.Current.Session["CustomerId"].ToString()));
            decimal arfolyam = IpaKapcsoltCalculation.GetArfolyamSzorzo(service, sessionId, customer);

            var f70 = fields.FirstOrDefault(f => f.FieldDescriptorId == 70);
            if (f70 != null)
            {
                f70.DecimalValue = (f13Value - f54Value - f55Value) * arfolyam;
            }
            else
            {
                f70 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = (f13Value - f54Value - f55Value) * arfolyam,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 70,
                    SessionId = sessionId
                };
            }

            var f81 = fields.FirstOrDefault(f => f.FieldDescriptorId == 81);
            if (f81 != null)
            {
                f81.DecimalValue = f13Value * arfolyam;
            }
            else
            {
                f81 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = f13Value * arfolyam,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 81,
                    SessionId = sessionId
                };
            }

            var f82 = fields.FirstOrDefault(f => f.FieldDescriptorId == 82);
            if (f82 != null)
            {
                f82.DecimalValue = f54Value * arfolyam;
            }
            else
            {
                f82 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = f54Value * arfolyam,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 82,
                    SessionId = sessionId
                };
            }

            var f83 = fields.FirstOrDefault(f => f.FieldDescriptorId == 83);
            if (f83 != null)
            {
                f83.DecimalValue = f55Value * arfolyam;
            }
            else
            {
                f83 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = f55Value * arfolyam,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 83,
                    SessionId = sessionId
                };
            }

            service.UpdateFieldValues(new List<FieldValueDto> { f70, f81, f82, f83 } , sessionId);
        }
        

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(2, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
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
            // 	Anyagköltség - Anyagköltségként figyelembe nem vehető tételek
            // f26 - f10
            var anyagkoltseg = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 26).DecimalValue);
            var notAnyagkoltseg = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 10).DecimalValue);
            
            return anyagkoltseg - notAnyagkoltseg;
       }
    }
}