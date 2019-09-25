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

        List<BaseFieldDescriptorDto> GetTableFields(int tableId);

        void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, Guid sessionId);

        List<FieldDescriptorDto> GetFieldsByFieldIdList(List<int> fieldIds, Guid sessionId);

        List<SessionDto> GetCustomerSessions(int customerId);

        List<TableDescriptorDto> GetTableData(int pageId, Guid session);

        List<TaxesByCitiesDto> GetCityTaxes();
    }
}