using System.Collections.Generic;
using System.Linq;
using TaoDatabaseService.Contracts;
using TaoDatabaseService.Interfaces;
using TaoDatabaseService.Mappers;

namespace TaoDatabaseService.Services
{
    public class DataService : IDataService
    {
        CustomerNameTaoEntities entities;

        public DataService()
        {
            entities = new CustomerNameTaoEntities();
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

            foreach(var field in fieldDescriptor)
            {
                result.Add(field.ToFieldDescriptorDto(fieldValues.ToList()));
            }
            return result;
        }

        public void UpdateFieldValues(List<FieldDescriptorDto> updatedFields, int sessionId)
        {

        }
    }
}
