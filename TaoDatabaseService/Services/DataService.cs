﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using TaoDatabaseService.Contracts;
using TaoDatabaseService.Interfaces;
using TaoDatabaseService.Mappers;

namespace TaoDatabaseService.Services
{
    public class DataService : IDataService
    {
        TaoDatabaseService.CustomerNameTaoEntities entities;

        public DataService(string databaseName)
        {
            entities = new TaoDatabaseService.CustomerNameTaoEntities(databaseName);
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
            return entities.Customer.Select(t => t.ToCustomer()).ToList();
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

                //set up value
                if(field.TypeName != "numeric")
                {
                    myEntity.StringValue = field.FieldValue.ToString();
                }
                else
                {
                    if (decimal.TryParse(field.FieldValue.ToString(), out var decimalValue))
                    {
                        myEntity.DecimalValue = decimalValue;
                    }
                }

                if (myEntity.StringValue == null && myEntity.DecimalValue == null)
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
