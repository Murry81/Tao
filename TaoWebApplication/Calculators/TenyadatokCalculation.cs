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
                   
                }
            }

            CalculateExtraFields(service, sessionId);
        }

        private static void CalculateExtraFields(IDataService service, Guid sessionId)
        {

            // (70) Passz(13) - (54) - (55) * árfolyam
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