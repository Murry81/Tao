using Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TartalomCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {

            foreach (var field in fields.OrderBy(s => s.Id))
            {

                if(field.Id == 5)
                {
                    var jellegField = fields.FirstOrDefault(f => f.Id == 29);
                    if(jellegField != null && jellegField.StringValue == "Végleges kalkuláció")
                    {
                        field.StringValue = "12";
                    }
                    continue;
                }

                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 35: // Eltérő üzleti év jelölő
                        {
                            field.BoolFieldValue = Calculate35(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 36: // Előlegfizetési időszak kezdete
                        {
                            field.DateValue = Calculate36(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 37: // Előlegfizetési időszak vége
                        {
                            field.DateValue = Calculate37(fields.FirstOrDefault(f => f.Id == 32));
                            break;
                        }
                    case 38: // Első előlegrészlet esedékessége
                        {
                            field.DateValue = Calculate38(fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                    case 39: // Második előlegrészlet esedékessége
                        {
                            field.DateValue = Calculate39(fields.FirstOrDefault(f => f.Id == 36));
                            break;
                        }
                }
            }

            CalculateExtraFields(service, sessionId);
        }

        private static void CalculateExtraFields(IDataService service, Guid sessionId)
        {
            CalculateCegadatok(service, sessionId);
        }

        private static void CalculateCegadatok(IDataService service, Guid sessionId)
        {
            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 71, 72, 73, 74, 75, 80 }, sessionId);

            var f80 = fields.FirstOrDefault(f => f.FieldDescriptorId == 80);
            if (f80 != null)
                return;

            var customer = service.GetCustomerBySessionId(sessionId);
            var result = new List<FieldValueDto>
            {
                new FieldValueDto
                {
                    SessionId = sessionId,
                    FieldDescriptorId = 80,
                    StringValue = "Budapest"
                },
                new FieldValueDto
                {
                    // cegnev
                    SessionId = sessionId,
                    FieldDescriptorId = 71,
                    StringValue = customer.Nev
                },
                new FieldValueDto
                {
                    // adoszám
                    SessionId = sessionId,
                    FieldDescriptorId = 72,
                    StringValue = customer.Adoszam
                },
                new FieldValueDto
                {
                    // iranyitoszám
                    SessionId = sessionId,
                    FieldDescriptorId = 73,
                    StringValue = customer.Address.FirstOrDefault()?.PostCode
                },
                new FieldValueDto
                {
                    // település
                    SessionId = sessionId,
                    FieldDescriptorId = 74,
                    StringValue = customer.Address.FirstOrDefault()?.City
                },
                new FieldValueDto
                {
                    // utca + hszm
                    SessionId = sessionId,
                    FieldDescriptorId = 75,
                    StringValue = $"{customer.Address.FirstOrDefault()?.Line1} {customer.Address.FirstOrDefault()?.Line2}"
                }
            };

            service.UpdateFieldValues(result, sessionId);
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(1, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static bool Calculate35(FieldDescriptorDto uzletiEvVegefield)
        {
            // Eltérő üzleti év jelölő
            // Ha az üzleti év vége nem december 31., akkor = igaz
            if (uzletiEvVegefield == null)
                return false;

            return uzletiEvVegefield.DateValue?.Month != 12 || uzletiEvVegefield.DateValue?.Day != 31;
        }

        private static DateTimeOffset? Calculate36(FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null || !uzletiEvVegefield.DateValue.HasValue)
                return null;

            var result = uzletiEvVegefield.DateValue.Value.AddMonths(7);
            return new DateTimeOffset(result.Year, result.Month, 1, 0, 0, 0, TimeSpan.Zero);
        }

        private static DateTimeOffset? Calculate37(FieldDescriptorDto uzletiEvVegefield)
        {
            if (uzletiEvVegefield == null || !uzletiEvVegefield.DateValue.HasValue)
                return null;

            var result = uzletiEvVegefield.DateValue.Value.AddMonths(19);
            return new DateTimeOffset(result.Year, result.Month, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1);
        }

        private static DateTimeOffset? Calculate38(FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null || !fieldFrom.DateValue.HasValue)
                return null;

            var result = fieldFrom.DateValue.Value.AddMonths(2);
            return new DateTimeOffset(result.Year, result.Month, 15, 0, 0, 0, TimeSpan.Zero);
        }

        private static DateTimeOffset? Calculate39(FieldDescriptorDto fieldFrom)
        {
            if (fieldFrom == null || !fieldFrom.DateValue.HasValue)
                return null;

            var result = fieldFrom.DateValue.Value.AddMonths(8);
            return new DateTimeOffset(result.Year, result.Month, 15, 0, 0, 0, TimeSpan.Zero);
        }
    }
}