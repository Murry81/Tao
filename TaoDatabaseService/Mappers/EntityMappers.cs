using System.Collections.Generic;
using System.Linq;
using TaoDatabaseService.Contracts;

namespace TaoDatabaseService.Mappers
{
    public static class EntityMappers
    {
        public static PageDto ToPage(this Page page)
        {
            return new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Name = page.Name,
                CaptionText = page.CaptionText,
                Number = page.Number
            };
        }

        public static PageDescriptorDto ToPageDescriptor(this PageDescriptor pageDescriptor)
        {
            return new PageDescriptorDto
            {
                Description = pageDescriptor.Description,
                FieldId = pageDescriptor.FieldId,
                Id = pageDescriptor.Id,
                PageId = pageDescriptor.PageId,
                Order = pageDescriptor.Order,
                OrderCharacter = pageDescriptor.OrderCharacter,
                SectionGroup = pageDescriptor.SectionGroup,

            };
        }

        public static FieldDescriptorDto ToFieldDescriptorDto(this FieldDescriptor fieldDescriptor, List<FieldValue> values)
        {
            var result = new FieldDescriptorDto
            {
                Caption = fieldDescriptor.Caption,
                Id = fieldDescriptor.Id,
                IsCaculated = fieldDescriptor.IsCalculated,
                IsEditable = fieldDescriptor.IsEditable,
                IsMandatory = fieldDescriptor.IsMandatory,
                Title = fieldDescriptor.Title,
                TypeName = fieldDescriptor.TypeName,
                TypeOptions = fieldDescriptor.TypeOptions
            };

            var currentValue = values.FirstOrDefault(v => v.FieldDescriptorId == fieldDescriptor.Id);
            if(currentValue != null)
            {
                result.FieldValueId = currentValue.Id;
                result.FieldValue = fieldDescriptor.TypeName == "numeric" ? (object)currentValue.DecimalValue : currentValue.StringValue;
            }
            else
            {
                result.FieldValueId = null;
            }
            return result;
        }
    }
}
