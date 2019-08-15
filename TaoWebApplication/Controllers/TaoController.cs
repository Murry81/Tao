using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Calculators;

namespace TaoWebApplication.Controllers
{
    public class TaoController : Controller
    {

        private readonly IDataService _service;
       
        public TaoController(IDataService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult TartalomjegyzekStart(string id)
        {
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            int sessionId = int.Parse(Request.Form["Continue"]);
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId) as Models.TartalomjegyzekModel;
            
            Session["SessionId"] = sessionId.ToString();
            Session["CustomerId"] = Request.Form["SelectedCustomer.Id"];

            return View("~/Views/Tao/Tartalomjegyzek.cshtml", model);
        }

        [HttpPost]
        public ActionResult Tartalomjegyzek(string id)
        {
            SaveValues(TenyadatokCalculation.CalculateValues, 1);
       
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            model = ControllerHelper.FillModel(model, _service, currentpage, int.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TartalomjegyzekModel;
 
            return View(model);
        }

        public ActionResult Tenyadatok()
        {
            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            ControllerHelper.FillModel(model, _service, currentpage, int.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()));
            return View(model);
        }

        [HttpPost]
        public ActionResult Tenyadatok(string id)
        {
            SaveValues(null, 2);

            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            model = ControllerHelper.FillModel(model, _service, currentpage, int.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TenyadatokModel;
            return View(model);
        }

        private void SaveValues(Action<List<FieldDescriptorDto>, IDataService> calulator, int pageId)
        {
            if (int.TryParse(System.Web.HttpContext.Current.Session["SessionId"]?.ToString(), out var sessionId))
            {
                var fields = _service.GetPageFields(pageId, sessionId);

                foreach (var field in fields)
                {
                    if (Request.Form.AllKeys.Contains(field.Id.ToString()))
                    {
                        if (!field.IsCaculated && field.IsEditable)
                        {
                            DataConverter.GetTypedValue(field,  Request.Form[field.Id.ToString()]);
                        }
                    }
                }

                if (calulator != null)
                {
                    calulator(fields, _service);
                }

                _service.UpdateFieldValues(fields, sessionId);
            }
        }
    }
}