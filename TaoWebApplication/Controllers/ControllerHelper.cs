using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoDatabaseService.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Models;

namespace TaoWebApplication.Controllers
{
    public static class ControllerHelper
    {
        public static ModelBase FillModel(ModelBase model, IDataService service, PageDto page)
        {
            model.CurrentPage = page;
            model.Pages = service.GetAllPage();
            model.PageDescriptors = service.GetPageDescriptor(page.Id).OrderBy(p => p.SectionGroup).ThenBy(p => p.Order).ToList();
            model.Fields = service.GetPageFields(page.Id, 1);
            return model;
        }
    }
}