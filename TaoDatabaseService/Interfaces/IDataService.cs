using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoDatabaseService.Interfaces
{
    public interface IDataService
    {
        List<CustomerDto> GetCustomers();

        CustomerDto GetCustomer(int id);

        List<PageDto> GetAllPage();

        List<DocumentDto> GetAllDocumentType();

        PageDto GetPage(string name);

        List<PageDescriptorDto> GetPageDescriptor(int pageId);

        List<FieldDescriptorDto> GetPageFields(int pageId, int sessionId);

        void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, int sessionId);

        List<FieldDescriptorDto> GetFieldsByFieldIdList(List<int> fieldIds, int sessionId);

        List<SessionDto> GetCustomerSessions(int customerId);
    }
}