using Contracts.Contracts;
using System;
using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoDatabaseService.Interfaces
{
    public interface IDataService
    {
        List<CustomerDto> GetCustomers();

        CustomerDto GetCustomer(int id);

        List<PageDto> GetAllPage(int documentTypeId);

        List<DocumentDto> GetAllDocumentType();

        PageDto GetPage(string name);

        SessionDto CreateSession(SessionDto session);
        
        List<PageDescriptorDto> GetPageDescriptor(int pageId);

        List<FieldDescriptorDto> GetPageFields(int pageId, Guid sessionId);

        void SaveValues(List<FieldValue> fields, Guid sessionId);

        List<BaseFieldDescriptorDto> GetTableFields(int tableId);

        void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, Guid sessionId);

        string GetDocumentIdentifier(int documentId);

        List<FieldDescriptorDto> GetFieldsByFieldIdList(List<int> fieldIds, Guid sessionId);

        FieldDescriptorDto GetFieldById(int fieldId, Guid sessionId);

        List<FieldDescriptorDto> GetFieldValuesById(int fieldId, Guid sessionId);

        List<FieldValue> GetAllPageFieldValues(int pageId, Guid sessionId);

        List<SessionDto> GetCustomerSessions(int customerId);

        List<TableDescriptorDto> GetTableData(int pageId, Guid session);

        List<TaxesByCitiesDto> GetCityTaxes();

        ExportReportDto GetExportReportData(Guid sessionId, int reportId);

        CustomerDto GetCustomerBySessionId(Guid sessionId);

        List<FieldDescriptorDto> GetTableFieldsByFieldIdList(int fieldId, Guid sessionId);

        List<FieldValueDto> GetFieldValuesByFieldIdList(List<int> lists, Guid sessionId);

        void UpdateFieldValues(List<FieldValueDto> fields, Guid sessionId);

        string GetOnkormanyzat(string cityName);

        void DeleteFieldValue(List<int> fieldIds, Guid sessionId);
    }
}