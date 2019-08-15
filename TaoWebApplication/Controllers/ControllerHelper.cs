using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Models;

namespace TaoWebApplication.Controllers
{
    public static class ControllerHelper
    {
        public static ModelBase FillModel(ModelBase model, IDataService service, PageDto page, int sessionId)
        {
            model.CurrentPage = page;
            model.Pages = service.GetAllPage();
            model.PageDescriptors = service.GetPageDescriptor(page.Id).OrderBy(p => p.SectionGroup).ThenBy(p => p.Order).ToList();
            model.Fields = service.GetPageFields(page.Id, sessionId);
            return model;
        }
    }
}