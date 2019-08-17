using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;

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
                Number = page.Number,
                DocumentTypeId = page.DocumentTypeId
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

        public static FieldDescriptorDto ToFieldDescriptorDto(this FieldDescriptor fieldDescriptor, List<FieldValue> values, PageDescriptor pageDescriptor)
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
                TypeOptions = fieldDescriptor.TypeOptions,
                Description = pageDescriptor?.Description,
                OrderCharacter = pageDescriptor?.OrderCharacter,
                IsSpecial = fieldDescriptor.IsSpecial
            };

            var currentValue = values.FirstOrDefault(v => v.FieldDescriptorId == fieldDescriptor.Id);
            if(currentValue != null)
            {
                result.FieldValueId = currentValue.Id;
                switch(fieldDescriptor.TypeName)
                {
                    case "numeric":
                        result.DecimalValue = currentValue.DecimalValue;
                        break;
                    case "bool":
                        result.BoolFieldValue = currentValue.BoolValue.HasValue ? currentValue.BoolValue.Value : false;
                        break;
                    case "date":
                        result.DateValue = currentValue.DateValue;
                        break;
                    default:
                        result.StringValue = currentValue.StringValue;
                        break;
                }
            }
            else
            {
                result.FieldValueId = null;
            }
            return result;
        }

        public static CurrencyDto ToCurrency(this Currency currency)
        {
            return new CurrencyDto
            {
                Egyseg = currency.Egyseg,
                Id = currency.Id,
                ISO = currency.ISO,
                Megnevezes = currency.Megnevezes
            };
        }

        public static CustomerDto ToCustomer(this Customer customer)
        {
            var addresses = new List<AddressDto>();
            foreach (var address in customer.Address)
            {
                addresses.Add(new AddressDto
                {
                    City = address.City,
                    Country = address.Country,
                    County = address.County,
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    PostCode = address.PostCode
                });
            }

            return new CustomerDto
            {
                Address = addresses,
                Adoszam = customer.Adoszam,
                BeszamoloPenzneme = customer.Currency.ToCurrency(),
                Cegjegyzekszam = customer.Cegjegyzekszam,
                ContactDetailId = customer.ContactDetailId,
                Email = customer.ContactDetail.Email,
                Email2 = customer.ContactDetail.Email2,
                Id = customer.Id,
                KonyvelesPenzneme = customer.Currency1.ToCurrency(),
                KSHSzam = customer.KSHSzam,
                Nev = customer.Nev,
                Phone = customer.ContactDetail.Phone,
                Phone2 = customer.ContactDetail.Phone2,
                SharePointId = customer.SharePointId,
                WLSzam = customer.WLSzam
            };
        }

        public static SessionDto ToSession(this Session session)
        {
            return new SessionDto
            {
                Creator = session.Creator,
                CustomerId = session.CustomerId,
                DocumentState = session.DocumentState.State,
                DocumentType = session.DocumentType.ToDocument(),
                Id = session.Id,
                LastModifer = session.LastModifer,
                LastModifyDate = session.LastModifyDate,
                StartDateTime = session.StartDateTime
            };
        }

        public static DocumentDto ToDocument(this DocumentType docType)
        {
            return new DocumentDto
            {
                Id = docType.Id,
                DocumentName = docType.DocumentName
            };
        }
    }
}
