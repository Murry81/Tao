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

        public List<PageDto> GetAllPage(int documentTypeId)
        {
            List<PageDto> result = new List<PageDto>();
            foreach(var page in entities.Page.Where(p => p.DocumentTypeId == documentTypeId))
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

        public List<FieldDescriptorDto> GetPageFields(int pageId, Guid sessionId)
        {
            var query = entities.Page.FirstOrDefault(p => p.Id == pageId).PageDescriptor;

            var fieldDescriptor = query.Select(p => p.FieldDescriptor);
            var fieldValues = entities.FieldValue.Where(fv => fv.SessionId == sessionId).ToList(); 

            var result = new List<FieldDescriptorDto>();
            var pageDescriptor = query.Where(p => p.PageId == pageId).ToList();

            foreach (var field in fieldDescriptor)
            {
                result.Add(field.ToFieldDescriptorDto(fieldValues, pageDescriptor));
            }
            return result;
        }


        public List<BaseFieldDescriptorDto> GetTableFields(int tableId)
        {
            var fieldDescriptor = entities.TableDescriptor.Where(t => t.TableId == tableId).Select(t => t.FieldDescriptor).ToList();

            var result = new List<BaseFieldDescriptorDto>();
            foreach (var field in fieldDescriptor)
            {
                result.Add(field.ToBaseFieldDescriptorDto());
            }
            return result;
        }

        public List<FieldDescriptorDto> GetFieldsByFieldIdList(List<int> fieldIds, Guid sessionId)
        {
            var fieldDescriptor = entities.FieldDescriptor.Where(f => fieldIds.Contains(f.Id)).ToList();
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
            var sessions = entities.Session.Where(s => s.CustomerId == customerId).ToList();
            if (sessions == null || sessions.Count() == 0)
            {
                return null;
            }

            return sessions.Select(s => s.ToSession()).ToList();
        }

        public List<DocumentDto> GetAllDocumentType()
        {
            return entities.DocumentType.ToList().Select(d => d.ToDocument()).ToList();
        }

        public void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, Guid sessionId)
        {
            foreach(var field in updatedFields)
            {
                var myEntity = new FieldValue
                {
                    FieldDescriptorId = field.Id,
                    Id = field.FieldValueId.HasValue ? field.FieldValueId.Value : Guid.NewGuid(),
                    SessionId = sessionId,
                    RowIndex = field.RowIndex
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

                if (string.IsNullOrEmpty(myEntity.StringValue) && myEntity.DecimalValue == null && myEntity.DateValue == null && myEntity.BoolValue == null)
                {
                    if (field.FieldValueId.HasValue)
                    {
                        var result = entities.FieldValue.FirstOrDefault(f => f.Id == field.FieldValueId.Value && f.RowIndex == field.RowIndex);
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

            var sessionEntity = entities.Session.First(e => e.Id == sessionId);
            sessionEntity.LastModifyDate = DateTime.UtcNow;
            entities.SaveChanges();
        }

        public SessionDto CreateSession(SessionDto session)
        {
            var sessionCreated = new Session
            {
                CustomerId = session.CustomerId,
                DocumentStateId = 1,
                DocumentState = entities.DocumentState.First(ds => ds.Id == 1),
                DocumentTypeId = session.DocumentType.Id,
                StartDateTime = DateTime.UtcNow,
                LastModifyDate = DateTime.UtcNow,
                Id = Guid.NewGuid()
            };

            entities.Session.Add(sessionCreated);
            entities.SaveChanges();

            return sessionCreated.ToSession();
        }

        public List<TableDescriptorDto> GetTableData(int pageId, Guid session)
        {
            var tableIds = entities.TableDescriptor.Where(t => t.PageId == pageId).Select(t=> t.TableId).Distinct().ToList();

            var result = new List<TableDescriptorDto>();
            foreach (var tableId in tableIds)
            {
                var tableElement = new TableDescriptorDto
                {
                    TableId = tableId
                };

                var tableDescriptors = entities.TableDescriptor.Where(t => t.TableId == tableId).ToList();
                if (tableDescriptors == null || tableDescriptors.Count() == 0)
                {
                    return null;
                }

                // Set up ids
                var fieldDescriptors = tableDescriptors.OrderBy(t => t.ColumnOrder).Select(t => t.FieldDescriptor).ToList();
                tableElement.FieldDescriptorIds = fieldDescriptors.Select(t => t.Id).ToList();

                // Set up captions
                tableElement.Captions = new Dictionary<int, string>();
                foreach (var t in tableDescriptors.OrderBy(t => t.ColumnOrder))
                {
                    tableElement.Captions.Add(t.FieldDescriptorId, t.Caption);
                }

                // Set up saved values
                tableElement.FieldValues = new List<List<FieldDescriptorDto>>();
                var savedFields = entities.FieldValue.Where(f => f.SessionId == session && tableElement.FieldDescriptorIds.Contains(f.FieldDescriptorId)).ToList();

                int rowIndex = 0;
                List<FieldDescriptorDto> currentList = null;
                var rowNumbers = savedFields.Select(t => t.RowIndex).Distinct().OrderBy(t=> t.Value).ToList();

                foreach(int row in rowNumbers)
                {
                    currentList = new List<FieldDescriptorDto>();
                    tableElement.FieldValues.Add(currentList);
                    foreach (var field in fieldDescriptors)
                    {
                        var savedField = savedFields.FirstOrDefault(t => t.RowIndex == row && t.FieldDescriptorId == field.Id);
                        currentList.Add(field.ToFieldDescriptorDto(savedField, row));
                    }
                    rowIndex++;
                }

                // Add an empty row
                currentList = new List<FieldDescriptorDto>();
                rowIndex++;
                foreach (var field in fieldDescriptors)
                {
                    currentList.Add(field.ToFieldDescriptorDto(null, rowIndex));
                }

                tableElement.FieldValues.Add(currentList);
                result.Add(tableElement);
            }
            return result;
        }

        public List<TaxesByCitiesDto> GetCityTaxes()
        {
            var cities = entities.TaxesByCities.ToList();
            return cities.Select(c => new TaxesByCitiesDto
            {
                Id = c.Id,
                County = c.Megye,
                City = c.City,
                IparuzesiAdo = c.HelyiIparuzesiAdo
            }).ToList();
        }
    }
}
