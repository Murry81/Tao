using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoDatabaseService.Mappers;

namespace TaoDatabaseService.Services
{
    public class DataService : IDataService
    {
        TaoDatabaseService.CustomerNameTaoEntities entities;

        public DataService(IDataServiceConfiguration configuration)
        {
            entities = new CustomerNameTaoEntities(configuration.DataBaseName);
        }

        public List<PageDto> GetAllPage()
        {
            List<PageDto> result = new List<PageDto>();
            foreach(var page in entities.Page)
            {
                result.Add(page.ToPage());
            }
            return result;
        }

        public PageDto GetPage(string name)
        {
            return entities.Page.FirstOrDefault(p => p.Name == name).ToPage();
        }

        public CustomerDto GetCustomer(int id)
        {
            return entities.Customer.FirstOrDefault(t => t.Id == id)?.ToCustomer();
        }

        public List<CustomerDto> GetCustomers()
        {
            return entities.Customer.ToList().Select(t => t.ToCustomer()).ToList();
        }

        public List<PageDescriptorDto> GetPageDescriptor(int pageId)
        {
            List<PageDescriptorDto> result = new List<PageDescriptorDto>();
            foreach (var page in entities.PageDescriptor.Where(p => p.PageId == pageId))
            {
                result.Add(page.ToPageDescriptor());
            }
            return result;
        }

        public List<FieldDescriptorDto> GetPageFields(int pageId, int sessionId)
        {
            var query = entities.Page.FirstOrDefault(p => p.Id == pageId).PageDescriptor;

            var fieldDescriptor = query.Select(p => p.FieldDescriptor);
            var fieldValues = entities.FieldValue.Where(fv => fv.SessionId == sessionId).ToList(); 

            var result = new List<FieldDescriptorDto>();
            var pageDescriptor = query.FirstOrDefault(p => p.PageId == pageId);

            foreach (var field in fieldDescriptor)
            {
                result.Add(field.ToFieldDescriptorDto(fieldValues.ToList(), pageDescriptor));
            }
            return result;
        }

        public List<FieldDescriptorDto> GetFieldsByFieldIdList(List<int> fieldIds, int sessionId)
        {
            var fieldDescriptor = entities.FieldDescriptor.Where(f => fieldIds.Contains(f.Id));
            var fieldValues = entities.FieldValue.Where(fv => fv.SessionId == sessionId).ToList();

            var result = new List<FieldDescriptorDto>();

            foreach (var field in fieldDescriptor)
            {
                result.Add(field.ToFieldDescriptorDto(fieldValues.ToList(), null));
            }
            return result;
        }

        public List<SessionDto> GetCustomerSessions(int customerId)
        {
            var sessions = entities.Session.Where(s => s.CustomerId == customerId);
            if (sessions == null || sessions.Count() == 0)
            {
                return null;
            }

            return sessions.ToList().Select(s => s.ToSession()).ToList();
        }

        public List<DocumentDto> GetAllDocumentType()
        {
            return entities.DocumentType.ToList().Select(d => d.ToDocument()).ToList();
        }

        public void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, int sessionId)
        {
            foreach(var field in updatedFields)
            {
                var myEntity = new FieldValue
                {
                    FieldDescriptorId = field.Id,
                    Id = field.FieldValueId.HasValue ? field.FieldValueId.Value : Guid.NewGuid(),
                SessionId = sessionId
                };


                switch(field.TypeName)
                {
                    case "numeric":
                        myEntity.DecimalValue = field.DecimalValue;
                        break;
                    case "bool":
                        myEntity.BoolValue = field.BoolFieldValue;
                        break;
                    case "date":
                        myEntity.DateValue = field.DateValue;
                        break;
                    default:
                        myEntity.StringValue = field.StringValue;
                        break;
                }

                if (myEntity.StringValue == null && myEntity.DecimalValue == null && myEntity.DateValue == null && myEntity.BoolValue == null)
                {
                    if (field.FieldValueId.HasValue)
                    {
                        var result = entities.FieldValue.FirstOrDefault(f => f.Id == field.FieldValueId.Value);
                        if(result != null)
                        {
                            entities.FieldValue.Remove(result);
                        }
                    }
                }
                else
                {
                    entities.FieldValue.AddOrUpdate(myEntity);
                }
            }
            entities.SaveChanges();
        }
    }
}
