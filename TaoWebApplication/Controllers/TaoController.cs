using System.Web.Mvc;
using TaoDatabaseService.Interfaces;
using TaoDatabaseService.Services;

namespace TaoWebApplication.Controllers
{
    public class TaoController : Controller
    {
        private IDataService service;

        public TaoController()
        {
            service = new DataService();
        }
        // GET: Tao
        public ActionResult Tartalomjegyzek()
        {
            var model = new TaoWebApplication.Models.TartalomjegyzekModel();
            var currentpage = service.GetPage("Tartalomjegyzek");
            model.Pages = service.GetAllPage();
            model.PageDescriptors = service.GetPageDescriptor(currentpage.Id);
            model.Fields = service.GetPageFields(currentpage.Id, 1);
            return View(model);
        }
    }
}