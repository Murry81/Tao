using System;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Models;

namespace TaoWebApplication.Controllers
{
    public static class ControllerHelper
    {
        public static ModelBase FillModel(ModelBase model, IDataService service, PageDto page, Guid sessionId, int? customerId)
        {
            model.CurrentPage = page;
            model.Pages = service.GetAllPage(page.DocumentTypeId);
            model.PageDescriptors = service.GetPageDescriptor(page.Id).OrderBy(p => p.SectionGroup).ThenBy(p => p.Order).ToList();
            model.Fields = service.GetPageFields(page.Id, sessionId).OrderBy(p => p.SectionGroup).ThenBy(p => p.Order).ToList();
            model.IpaKapcsolt = service.GetFieldById(40, sessionId)?.BoolFieldValue;
            model.IsEnergiaellato = service.GetFieldById(61, sessionId).BoolFieldValue;
            if (customerId.HasValue)
                model.Customer = service.GetCustomer(customerId.Value);
            return model;
        }
    }
}