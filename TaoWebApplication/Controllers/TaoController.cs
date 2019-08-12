using Newtonsoft.Json;
using System;
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

        private readonly IDataService _service;
       
        public TaoController(IDataService service)
        {
            _service = service;
        }

        // GET: Tao
        public ActionResult Tartalomjegyzek()
        {
            Session["DatabaseName"] = "CustomerNameTao";
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            model = ControllerHelper.FillModel(model, _service, currentpage) as Models.TartalomjegyzekModel;
            var companyName = "MySweetCustomer";
            Session["SessionId"] = 1;
            Session["CustomerId"] = companyName;

            return View(model);
        }

        [HttpPost]
        public ActionResult Tartalomjegyzek(string id)
        {
            SaveValues();

            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            model = ControllerHelper.FillModel(model, _service, currentpage) as Models.TartalomjegyzekModel;
            return View(model);
        }

        private void SaveValues()
        {
            if (int.TryParse(System.Web.HttpContext.Current.Session["SessionId"]?.ToString(), out var sessionId))
            {

                string customerId = System.Web.HttpContext.Current.Session["CustomerId"].ToString();

                var fields = _service.GetFieldsByFieldIdList(Request.Form.AllKeys.Select(k => int.Parse(k, CultureInfo.InvariantCulture)).ToList(), sessionId);

                foreach (var field in fields)
                {
                    if (Request.Form.AllKeys.Contains(field.Id.ToString()))
                    {
                        field.FieldValue = Request.Form[field.Id.ToString()];
                    }
                }

                _service.UpdateFieldValues(fields, sessionId);
            }
        }

        public ActionResult Tenyadatok()
        {
            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            ControllerHelper.FillModel(model, _service, currentpage);
            var companyName = "MySweetCustomer";
            Session["SessionId"] = 1;
            Session["CustomerId"] = companyName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Tenyadatok(string id)
        {
            SaveValues();

            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            model = ControllerHelper.FillModel(model, _service, currentpage) as Models.TenyadatokModel;
            return View(model);
        }
    }
}