using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaoDatabaseService.Contracts;
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
            model.CustomerId = "MySweetCustomer";
            model.SessionId = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult Tartalomjegyzek(string id)
        {
            var hasSession = int.TryParse(Request.Form["SessionId"], out var sessionId);
            if(!hasSession)
            {
                return null;
            }
            var fields = service.GetFieldsByFieldIdList(Request.Form.AllKeys.Where(k=> k != "CustomerId" && k!= "SessionId").Select( k=> int.Parse(k, CultureInfo.InvariantCulture)).ToList(), sessionId);

            foreach(var field in fields)
            {
                if(Request.Form.AllKeys.Contains(field.Id.ToString()))
                {
                    field.FieldValue = Request.Form[field.Id.ToString()];
                }
            }

            service.UpdateFieldValues(fields, sessionId);

            var model = new Models.TartalomjegyzekModel();
            var currentpage = service.GetPage("Tartalomjegyzek");
            model.Pages = service.GetAllPage();
            model.PageDescriptors = service.GetPageDescriptor(currentpage.Id);
            model.Fields = service.GetPageFields(currentpage.Id, 1);
            model.CustomerId = "MySweetCustomer";
            model.SessionId = 1;
            return View(model);
        }
    }
}